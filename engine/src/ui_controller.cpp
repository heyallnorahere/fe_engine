#include "ui_controller.h"
#include <sstream>
#include <cassert>
#include "logger.h"
#include "object_register.h"
#include "menu.h"
namespace fe_engine {
	template<typename T> static size_t get_register_index(reference<T> element) {
		auto obj_register = object_registry::get_register<T>();
		for (size_t i = 0; i < obj_register->size(); i++) {
			if (obj_register->get(i).get() == element.get()) {
				return i;
			}
		}
		return (size_t)-1;
	}
	ui_controller::ui_controller(reference<renderer> r, reference<map> m, reference<input_mapper> im) {
		this->m_renderer = r;
		this->m_map = m;
		this->m_imapper = im;
		if (!object_registry::register_exists<internal::menu>()) {
			object_registry::add_register<internal::menu>();
		}
	}
	void ui_controller::set_info_panel_target(reference<unit> u) {
		this->m_info_panel_target = u;
	}
	void ui_controller::set_unit_menu_target(reference<unit> u, s8vec2 original_position) {
		this->m_unit_menu_target = u;
		this->m_unit_menu_index = 0;
		this->refresh_base_menu_items();
		this->m_unit_menu_state.page = menu_page::base;
		this->m_unit_menu_state.selected_item.reset();
		this->m_unit_menu_state.original_position = original_position;
	}
	void ui_controller::set_core_assembly(reference<assembly> core) {
		this->m_core = core;
	}
	static std::vector<internal::menu_item> get_menu_items(size_t index) {
		auto menu_register = object_registry::get_register<internal::menu>();
		reference<internal::menu> menu = menu_register->get(index);
		return menu->get_items();
	}
	void ui_controller::update() {
		if (this->m_script_data.can_init()) {
			reference<cs_class> uc_class = this->m_core->get_class("FEEngine.UI", "UIController");
			reference<cs_method> creation_method = uc_class->get_method("FEEngine.UI.UIController:MakeFromRegisterIndex(ulong)");
			uint64_t index = (size_t)-1;
			reference<object_register<ui_controller>> uc_register = object_registry::get_register<ui_controller>();
			for (size_t i = 0; i < uc_register->size(); i++) {
				if (uc_register->get(i).get() == this) {
					index = i;
					break;
				}
			}
			assert(index != (size_t)-1);
			void* addr = &index;
			reference<cs_object> uc_instance = cs_method::call_function(creation_method, &addr);
			addr = uc_instance->raw();
			this->m_script_data.instance->call_method(this->m_script_data.init_method, &addr);
			this->m_script_data.initialized = true;
		}
		auto item_register = object_registry::get_register<item>();
		if (this->m_unit_menu_target) {
			auto input = this->m_imapper->get_state();
			if (input.up && this->m_unit_menu_index > 0) {
				this->m_unit_menu_index--;
			}
			switch (this->m_unit_menu_state.page) {
			case menu_page::base:
				if (input.down && this->m_unit_menu_index < this->m_menu_items.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (input.ok && this->m_can_close) {
					this->m_menu_items[this->m_unit_menu_index].on_select(reference<ui_controller>(this));
				}
				if (input.back && this->m_can_close) {
					this->m_unit_menu_target->move(this->m_unit_menu_state.original_position - this->m_unit_menu_target->get_pos(), -1);
					this->close_unit_menu();
					this->m_info_panel_target.reset();
				}
				break;
			case menu_page::item:
			{
				std::list<size_t> inventory = this->m_unit_menu_target->get_inventory();
				if (input.down && this->m_unit_menu_index < inventory.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (input.ok && this->m_can_close && inventory.size() > 0) {
					auto it = inventory.begin();
					std::advance(it, this->m_unit_menu_index);
					this->m_unit_menu_state.selected_item = item_register->get(*it);
					this->m_unit_menu_state.page = menu_page::item_select;
					this->m_unit_menu_index = 0;
				}
				if (input.back && this->m_can_close) {
					this->m_unit_menu_state.page = menu_page::base;
					this->m_unit_menu_index = 0;
				}
			}
				break;
			case menu_page::item_select:
			{
				std::vector<unit_menu_item> menu_items = this->generate_menu_items(this->m_unit_menu_state.selected_item);
				if (input.down && this->m_unit_menu_index < menu_items.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (input.ok && this->m_can_close) {
					menu_items[this->m_unit_menu_index].on_select(reference<ui_controller>(this));
				}
				if (input.back && this->m_can_close) {
					this->m_unit_menu_state.page = menu_page::item;
					this->m_unit_menu_state.selected_item.reset();
					this->m_unit_menu_index = 0;
				}
			}
				break;
			case menu_page::enemy_select:
			{
				std::vector<reference<unit>> units = this->get_attackable_units(this->m_unit_menu_target);
				if (input.down && this->m_unit_menu_index < units.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (input.ok && this->m_can_close) {
					this->m_unit_menu_target->attack(get_register_index(units[this->m_unit_menu_index]));
					this->close_unit_menu();
				}
				if (input.back && this->m_can_close) {
					this->m_unit_menu_state.page = menu_page::base;
					this->m_unit_menu_index = 0;
				}
			}
				break;
			case menu_page::user_menu:
			{
				auto go_back = [&]() {
					this->m_user_menu_queue.pop_front();
					if (this->m_user_menu_queue.size() == 0) {
						this->m_unit_menu_state.page = menu_page::base;
					}
					this->m_unit_menu_index = 0;
				};
				std::vector<internal::menu_item> items = get_menu_items(this->m_user_menu_queue[0]);
				if (input.down && this->m_unit_menu_index < items.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (input.ok && this->m_can_close) {
					internal::menu_item _item = items[this->m_unit_menu_index];
					switch (_item.type) {
					case internal::menu_item_type::action:
					{
						reference<cs_class> ui_controller_class = this->m_core->get_class("FEEngine.UI", "UIController");
						reference<cs_method> creation_method = ui_controller_class->get_method("FEEngine.UI.UIController:MakeFromRegisterIndex(ulong)");
						uint64_t index = (size_t)-1;
						reference<object_register<ui_controller>> uc_register = object_registry::get_register<ui_controller>();
						for (size_t i = 0; i < uc_register->size(); i++) {
							if (uc_register->get(i).get() == this) {
								index = i;
								break;
							}
						}
						assert(index != (size_t)-1);
						void* addr = &index;
						reference<cs_object> ui_controller_object = cs_method::call_function(creation_method, &addr);
						addr = ui_controller_object->raw();
						_item.action->invoke(&addr);
					}
						break;
					case internal::menu_item_type::submenu:
						this->m_user_menu_queue.push_front(_item.submenu);
						this->m_unit_menu_index = 0;
						break;
					case internal::menu_item_type::back:
						go_back();
						break;
					}
				}
				if (input.back && this->m_can_close) {
					go_back();
				}
			}
				break;
			}
		}
	}
	void ui_controller::render() {
		size_t map_width = this->m_map->get_width();
		size_t map_height = this->m_map->get_height();
		size_t viewport_width, viewport_height;
		this->m_renderer->get_buffer_size(viewport_width, viewport_height);
		size_t unit_menu_width = 30;
		size_t info_panel_width = viewport_width - (map_width + unit_menu_width + 3);
		size_t log_height = viewport_height - (map_height + 2);
		this->render_frame(info_panel_width, unit_menu_width, map_width, map_height, log_height);
		this->render_info_panel(map_width + 1, viewport_height - map_height, info_panel_width, map_height);
		this->render_unit_menu(map_width + info_panel_width + 2, viewport_height - map_height, unit_menu_width, map_height);
		this->render_log(0, 1, viewport_width - 1, log_height);
	}
	reference<unit> ui_controller::get_unit_menu_target() const {
		return this->m_unit_menu_target;
	}
	void ui_controller::set_can_close(bool c) {
		this->m_can_close = c;
	}
	void ui_controller::close_unit_menu() {
		this->m_unit_menu_target.reset();
	}
	void ui_controller::add_user_menu(const user_menu& menu) {
		this->m_user_menus.push_back(menu);
	}
	const std::vector<ui_controller::user_menu>& ui_controller::get_user_menus() const {
		return this->m_user_menus;
	}
	void ui_controller::set_ui_script(reference<cs_class> _class) {
		this->m_script_data.initialized = false;
		this->m_script_data._class = _class;
		if (_class) {
			this->m_script_data.instance = _class->instantiate();
			this->m_script_data.init_method = _class->get_method(_class->get_full_name() + ":Initialize(UIController)");
		}
		else {
			this->m_script_data.instance.reset();
			this->m_script_data.init_method.reset();
		}
	}
	void ui_controller::render_frame(size_t info_panel_width, size_t unit_menu_width, size_t map_width, size_t map_height, size_t log_height) {
		size_t viewport_width, viewport_height;
		this->m_renderer->get_buffer_size(viewport_width, viewport_height);
		for (size_t y = viewport_height - map_height; y < viewport_height; y++) {
			this->m_renderer->render_char_at(map_width, y, '#', renderer::color::white);
			this->m_renderer->render_char_at(map_width + info_panel_width + 1, y, '#', renderer::color::white);
		}
		for (size_t y = 0; y < viewport_height; y++) {
			this->m_renderer->render_char_at(map_width + info_panel_width + unit_menu_width + 2, y, '#', renderer::color::white);
		}
		for (size_t x = 0; x < map_width + info_panel_width + unit_menu_width + 3; x++) {
			size_t y = viewport_height - (map_height + 1);
			this->m_renderer->render_char_at(x, y, '#', renderer::color::white);
			this->m_renderer->render_char_at(x, 0, '#', renderer::color::white);
		}
	}
	void ui_controller::render_info_panel(size_t origin_x, size_t origin_y, size_t width, size_t height) {
		auto item_register = object_registry::get_register<item>();
		if (!this->m_info_panel_target) {
			this->m_renderer->render_string_at(origin_x, origin_y + height - 1, "Empty tile", renderer::color::white);
		} else {
			std::stringstream hp_string, mv_string, ew_string;
			hp_string << "HP: " << (uint32_t)this->m_info_panel_target->get_current_hp() << "/" << (uint32_t)this->m_info_panel_target->get_stats().max_hp;
			mv_string << "Movement: " << (uint32_t)this->m_info_panel_target->get_available_movement() << "/" << (uint32_t)this->m_info_panel_target->get_stats().movement;
			ew_string << "E: ";
			if (this->m_info_panel_target->get_equipped_weapon() != (size_t)-1) {
				reference<weapon> equipped = item_register->get(this->m_info_panel_target->get_equipped_weapon());
				ew_string << equipped->get_name() << " (" << equipped->get_current_durability() << "/" << equipped->get_stats().durability << ")";
			}
			else {
				ew_string << "None";
			}
			this->m_renderer->render_string_at(origin_x, origin_y + height - 1, this->m_info_panel_target->get_name(), renderer::color::white);
			this->m_renderer->render_string_at(origin_x, origin_y + height - 2, hp_string.str(), renderer::color::white);
			this->m_renderer->render_string_at(origin_x, origin_y + height - 3, mv_string.str(), renderer::color::white);
			this->m_renderer->render_string_at(origin_x, origin_y + height - 4, ew_string.str(), renderer::color::white);
		}
	}
	void ui_controller::render_unit_menu(size_t origin_x, size_t origin_y, size_t width, size_t height) {
		auto item_register = object_registry::get_register<item>();
		if (this->m_unit_menu_target) {
			switch (this->m_unit_menu_state.page) {
			case menu_page::base:
				for (size_t i = 0; i < this->m_menu_items.size(); i++) {
					bool selected = (this->m_unit_menu_index == i);
					size_t y = origin_y + height - (1 + (i * 2));
					if (selected) {
						this->m_renderer->render_char_at(origin_x, y, '>', renderer::color::red);
					}
					this->m_renderer->render_string_at(origin_x + 2, y, this->m_menu_items[i].text, selected ? renderer::color::red : renderer::color::white);
				}
				break;
			case menu_page::item:
			{
				std::list<size_t> inventory = this->m_unit_menu_target->get_inventory();
				for (size_t i = 0; i < inventory.size(); i++) {
					bool selected = (this->m_unit_menu_index == i);
					size_t y = origin_y + height - (1 + (i * 2));
					if (selected) {
						this->m_renderer->render_char_at(origin_x, y, '>', renderer::color::red);
					}
					auto it = inventory.begin();
					std::advance(it, i);
					this->m_renderer->render_string_at(origin_x + 2, y, item_register->get(*it)->get_name(), selected ? renderer::color::red : renderer::color::white);
				}
				if (inventory.size() == 0) {
					this->m_renderer->render_string_at(origin_x, origin_y + height - 1, "No items available", renderer::color::white);
				}
			}
				break;
			case menu_page::item_select:
			{
				std::vector<unit_menu_item> menu_items = this->generate_menu_items(this->m_unit_menu_state.selected_item);
				for (size_t i = 0; i < menu_items.size(); i++) {
					bool selected = (this->m_unit_menu_index == i);
					size_t y = origin_y + height - (1 + (i * 2));
					if (selected) {
						this->m_renderer->render_char_at(origin_x, y, '>', renderer::color::red);
					}
					this->m_renderer->render_string_at(origin_x + 2, y, menu_items[i].text, selected ? renderer::color::red : renderer::color::white);
				}
			}
				break;
			case menu_page::enemy_select:
			{
				std::vector<reference<unit>> units = this->get_attackable_units(this->m_unit_menu_target);
				for (size_t i = 0; i < units.size(); i++) {
					bool selected = (this->m_unit_menu_index == i);
					size_t y = origin_y + height - (1 + (i * 2));
					if (selected) {
						this->m_renderer->render_char_at(origin_x, y, '>', renderer::color::red);
					}
					std::string text = units[i]->get_name() + " (" + std::to_string((uint32_t)units[i]->get_current_hp()) + "/" + std::to_string((uint32_t)units[i]->get_stats().max_hp) + ")";
					this->m_renderer->render_string_at(origin_x + 2, y, text, selected ? renderer::color::red : renderer::color::white);
				}
			}
				break;
			case menu_page::user_menu:
			{
				std::vector<internal::menu_item> items = get_menu_items(this->m_user_menu_queue[0]);
				for (size_t i = 0; i < items.size(); i++) {
					bool selected = (this->m_unit_menu_index == i);
					size_t y = origin_y + height - (1 + (i * 2));
					if (selected) {
						this->m_renderer->render_char_at(origin_x, y, '>', renderer::color::red);
					}
					this->m_renderer->render_string_at(origin_x + 2, y, items[i].name, selected ? renderer::color::red : renderer::color::white);
				}
			}
				break;
			}
		}
	}
	void ui_controller::render_log(size_t origin_x, size_t origin_y, size_t width, size_t height) {
		std::vector<logger::message> log = logger::get_log();
		size_t log_start = ((log.size() < height) ? 0 : log.size() - height);
		for (size_t i = log_start; i < log.size(); i++) {
			logger::message message = log[i];
			size_t y = origin_y + height - 1;
			this->m_renderer->render_string_at(origin_x, y - (i - log_start), message.msg, message.color);
		}
	}
	std::vector<ui_controller::unit_menu_item> ui_controller::generate_menu_items(reference<item> i) {
		std::vector<unit_menu_item> items;
		if (i->get_item_flags() & item::usable) {
			items.push_back({ "Use", [](reference<ui_controller> controller) {
				assert(!controller->m_unit_menu_state.selected_item->used());
				controller->m_unit_menu_state.selected_item->set_used(true);
				reference<item_behavior> behavior = controller->m_unit_menu_state.selected_item->get_behavior();
				if (behavior) behavior->on_use();
				controller->m_unit_menu_target->get_inventory().remove_if([&](size_t index) { return controller->m_unit_menu_state.selected_item.get() == object_registry::get_register<item>()->get(index).get(); });
				controller->m_unit_menu_target->wait();
				controller->close_unit_menu();
			} });
		}
		if ((i->get_item_flags() & item::equipable) && (i->get_item_flags() & item::weapon)) {
			items.push_back({ "Equip", [](reference<ui_controller> controller) {
				controller->m_unit_menu_target->equip(controller->m_unit_menu_state.selected_item);
				controller->m_unit_menu_state.page = menu_page::item;
				controller->m_unit_menu_index = 0;
				controller->m_unit_menu_state.selected_item.reset();
				controller->refresh_base_menu_items();
			} });
		}
		// todo: add more
		items.push_back({ "Cancel", [](reference<ui_controller> controller) { controller->m_unit_menu_state.page = menu_page::item; controller->m_unit_menu_state.selected_item.reset(); controller->m_unit_menu_index = 0; } });
		return items;
	}
	std::vector<reference<unit>> ui_controller::get_attackable_units(reference<unit> u) {
		auto item_register = object_registry::get_register<item>();
		std::vector<reference<unit>> units;
		if (u->get_equipped_weapon() == (size_t)-1) return units;
		u8vec2 range = reference<weapon>(item_register->get(u->get_equipped_weapon()))->get_stats().range;
		for (size_t i = 0; i < this->m_map->get_unit_count(); i++) {
			reference<unit> _u = this->m_map->get_unit(i);
			s8vec2 delta = u->get_pos() - _u->get_pos();
			delta.x = abs(delta.x);
			delta.y = abs(delta.y);
			uint8_t delta_total = (uint8_t)delta.x + (uint8_t)delta.y;
			if (delta_total >= range.x && delta_total <= range.y) {
				if (_u->get_affiliation() == unit_affiliation::enemy || _u->get_affiliation() == unit_affiliation::separate_enemy) {
					units.push_back(_u);
				}
			}
		}
		return units;
	}
	static reference<cs_object> make_cs_object(reference<unit> u, reference<assembly> core) {
		auto unit_class = core->get_class("FEEngine", "Unit");
		auto instance = unit_class->instantiate();
		auto index_property = unit_class->get_property("Index");
		// using uint64_t for 32-bit systems
		uint64_t index = (uint64_t)-1;
		auto unit_register = object_registry::get_register<unit>();
		for (size_t i = 0; i < unit_register->size(); i++) {
			if (unit_register->get(i).get() == u.get()) {
				index = (uint64_t)i;
				break;
			}
		}
		assert(index != (uint64_t)-1);
		instance->set_property(index_property, &index);
		return instance;
	}
	static reference<cs_object> make_cs_object(s8vec2 pos, reference<assembly> core) {
		auto tile_class = core->get_class("FEEngine", "Tile");
		auto instance = tile_class->instantiate();
		auto position_property = tile_class->get_property("Position");
		instance->set_property(position_property, &pos);
		return instance;
	}
	void ui_controller::add_tile_menu_items() {
		for (int8_t x = 0; x < (int8_t)this->m_map->get_width(); x++) {
			for (int8_t y = 0; y < (int8_t)this->m_map->get_height(); y++) {
				s8vec2 pos = { x, y };
				s8vec2 unit_pos = this->m_unit_menu_target->get_pos();
				if ((pos - unit_pos).taxicab() != 1) {
					continue;
				}
				reference<tile> t = this->m_map->get_tile(pos);
				auto id = t->get_interaction_data();
				if (!id.callback) {
					continue;
				}
				bool found_identical_item = false;
				for (const auto& item : this->m_menu_items) {
					if (item.text == id.menu_text) {
						found_identical_item = true;
						break;
					}
				}
				if (found_identical_item) continue; // todo: implement other menu
				this->m_menu_items.push_back({ id.menu_text, [id, pos](reference<ui_controller> controller) {
					auto cs_tile = make_cs_object(pos, controller->m_core);
					auto cs_unit = make_cs_object(controller->m_unit_menu_target, controller->m_core);
					s32vec2 temp_pos = (s32vec2)pos;
					std::vector<void*> args;
					args.push_back(cs_unit->raw());
					args.push_back(cs_tile->raw());
					args.push_back(&temp_pos);
					id.callback->invoke(args.data());
					controller->close_unit_menu();
				} });
			}
		}
	}
	void ui_controller::refresh_base_menu_items() {
		this->m_menu_items.clear();
		// if there is an enemy unit nearby, add the option to attack it
		{
			std::vector<reference<unit>> units = this->get_attackable_units(this->m_unit_menu_target);
			if (units.size() > 0) {
				this->m_menu_items.push_back({ "Attack", [](reference<ui_controller> controller) {
					controller->m_unit_menu_state.page = menu_page::enemy_select;
					controller->m_unit_menu_index = 0;
				} });
			}
		}
		// add item menu
		this->m_menu_items.push_back({ "Item", [](reference<ui_controller> controller) { controller->m_unit_menu_state.page = menu_page::item; controller->m_unit_menu_index = 0; } });
		// add menu items for nearby, interactable tiles
		this->add_tile_menu_items();
		// add user menus
		for (auto menu : this->m_user_menus) {
			size_t index = menu.register_index;
			this->m_menu_items.push_back({ menu.menu_item_name, [index](reference<ui_controller> controller) {
				controller->m_unit_menu_state.page = menu_page::user_menu;
				controller->m_user_menu_queue.push_front(index);
				controller->m_unit_menu_index = 0;
			} });
		}
		// add wait option
		this->m_menu_items.push_back({ "Wait", [](reference<ui_controller> controller) { controller->close_unit_menu(); } });
	}
}