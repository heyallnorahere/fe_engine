#include "ui_controller.h"
#include <sstream>
#include <cassert>
namespace fe_engine {
	ui_controller::ui_controller(reference<renderer> r, reference<map> m, reference<controller> c) {
		this->m_renderer = r;
		this->m_map = m;
		this->m_controller = c;
	}
	void ui_controller::set_info_panel_target(reference<unit> u) {
		this->m_info_panel_target = u;
	}
	void ui_controller::set_unit_menu_target(reference<unit> u) {
		this->m_unit_menu_target = u;
		this->m_unit_menu_index = 0;
		this->m_menu_items.clear();
		this->m_menu_items.push_back({ "Item", [](reference<ui_controller> controller) { controller->m_unit_menu_state.page = menu_page::item; controller->m_unit_menu_index = 0; } });
		this->m_menu_items.push_back({ "Wait", [](reference<ui_controller> controller) { controller->close_unit_menu(); } });
		this->m_unit_menu_state.page = menu_page::base;
		this->m_unit_menu_state.selected_item.reset();
	}
	void ui_controller::update() {
		if (this->m_unit_menu_target) {
			controller::buttons buttons = this->m_controller->get_state();
			if (buttons.up.down && this->m_unit_menu_index > 0) {
				this->m_unit_menu_index--;
			}
			switch (this->m_unit_menu_state.page) {
			case menu_page::base:
				if (buttons.down.down && this->m_unit_menu_index < this->m_menu_items.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (buttons.a.down && this->m_can_close) {
					this->m_menu_items[this->m_unit_menu_index].on_select(reference<ui_controller>(this));
				}
				break;
			case menu_page::item:
			{
				std::list<reference<item>> inventory = this->m_unit_menu_target->get_inventory();
				if (buttons.down.down && this->m_unit_menu_index < inventory.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (buttons.a.down && this->m_can_close && inventory.size() > 0) {
					auto it = inventory.begin();
					std::advance(it, this->m_unit_menu_index);
					this->m_unit_menu_state.selected_item = *it;
					this->m_unit_menu_state.page = menu_page::select;
					this->m_unit_menu_index = 0;
				}
				if (buttons.b.down && this->m_can_close) {
					this->m_unit_menu_state.page = menu_page::base;
					this->m_unit_menu_index = 0;
				}
			}
				break;
			case menu_page::select:
			{
				std::vector<unit_menu_item> menu_items = this->generate_menu_items(this->m_unit_menu_state.selected_item);
				if (buttons.down.down && this->m_unit_menu_index < menu_items.size() - 1) {
					this->m_unit_menu_index++;
				}
				if (buttons.a.down && this->m_can_close) {
					menu_items[this->m_unit_menu_index].on_select(reference<ui_controller>(this));
				}
				if (buttons.b.down && this->m_can_close) {
					this->m_unit_menu_state.page = menu_page::item;
					this->m_unit_menu_state.selected_item.reset();
					this->m_unit_menu_index = 0;
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
		size_t unit_menu_width = 20;
		size_t info_panel_width = viewport_width - (map_width + unit_menu_width + 3);
		this->render_frame(info_panel_width, unit_menu_width, map_width, map_height);
		this->render_info_panel(map_width + 1, viewport_height - map_height, info_panel_width, map_height);
		this->render_unit_menu(map_width + info_panel_width + 2, viewport_height - map_height, unit_menu_width, map_height);
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
	void ui_controller::render_frame(size_t info_panel_width, size_t unit_menu_width, size_t map_width, size_t map_height) {
		size_t viewport_width, viewport_height;
		this->m_renderer->get_buffer_size(viewport_width, viewport_height);
		for (size_t y = viewport_height - map_height; y < viewport_height; y++) {
			this->m_renderer->render_char_at(map_width, y, '#', renderer::color::white);
			this->m_renderer->render_char_at(map_width + info_panel_width + 1, y, '#', renderer::color::white);
			this->m_renderer->render_char_at(map_width + info_panel_width + unit_menu_width + 2, y, '#', renderer::color::white);
		}
		for (size_t x = 0; x < map_width + info_panel_width + unit_menu_width + 3; x++) {
			size_t y = viewport_height - (map_height + 1);
			this->m_renderer->render_char_at(x, y, '#', renderer::color::white);
		}
	}
	void ui_controller::render_info_panel(size_t origin_x, size_t origin_y, size_t width, size_t height) {
		if (!this->m_info_panel_target) {
			this->m_renderer->render_string_at(origin_x, origin_y + height - 1, "Empty tile", renderer::color::white);
		} else {
			std::string affiliation = "Unknown";
			switch (this->m_info_panel_target->get_affiliation()) {
			case unit_affiliation::player:
				affiliation = "Player";
				break;
			case unit_affiliation::ally:
				affiliation = "Ally";
				break;
			case unit_affiliation::enemy:
				affiliation = "Enemy";
				break;
			case unit_affiliation::separate_enemy:
				affiliation = "Third Army";
				break;
			}
			std::stringstream hp_string;
			hp_string << (unsigned int)this->m_info_panel_target->get_current_hp() << "/" << (unsigned int)this->m_info_panel_target->get_stats().max_hp;
			this->m_renderer->render_string_at(origin_x, origin_y + height - 1, affiliation + " unit     " + hp_string.str(), renderer::color::white);
		}
	}
	void ui_controller::render_unit_menu(size_t origin_x, size_t origin_y, size_t width, size_t height) {
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
				std::list<reference<item>> inventory = this->m_unit_menu_target->get_inventory();
				for (size_t i = 0; i < inventory.size(); i++) {
					bool selected = (this->m_unit_menu_index == i);
					size_t y = origin_y + height - (1 + (i * 2));
					if (selected) {
						this->m_renderer->render_char_at(origin_x, y, '>', renderer::color::red);
					}
					auto it = inventory.begin();
					std::advance(it, i);
					this->m_renderer->render_string_at(origin_x + 2, y, (*it)->get_name(), selected ? renderer::color::red : renderer::color::white);
				}
				if (inventory.size() == 0) {
					this->m_renderer->render_string_at(origin_x, origin_y + height - 1, "No items available", renderer::color::white);
				}
			}
				break;
			case menu_page::select:
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
			}
		}
	}
	std::vector<ui_controller::unit_menu_item> ui_controller::generate_menu_items(reference<item> i) {
		std::vector<unit_menu_item> items;
		if (i->get_item_flags() & item::usable) {
			items.push_back({ "Use", [](reference<ui_controller> controller) {
				item::on_use_proc proc = controller->m_unit_menu_state.selected_item->get_on_use_proc();
				assert(proc);
				proc(controller->m_unit_menu_target.get());
				controller->m_unit_menu_target->get_inventory().remove(controller->m_unit_menu_state.selected_item);
				controller->close_unit_menu();
			} });
		}
		if (i->get_item_flags() & item::equipable) {
			items.push_back({ "Equip", [](reference<ui_controller> controller) {
				reference<item> equipped = controller->m_unit_menu_target->get_equipped_weapon();
				controller->m_unit_menu_target->get_inventory().remove_if([&](reference<item> i) { return controller->m_unit_menu_state.selected_item.get() == i.get(); });
				controller->m_unit_menu_target->set_equipped_weapon(controller->m_unit_menu_state.selected_item);
				controller->m_unit_menu_target->get_inventory().push_back(equipped);
				controller->m_unit_menu_state.page = menu_page::item;
				controller->m_unit_menu_index = 0;
				controller->m_unit_menu_state.selected_item.reset();
			} });
		}
		// todo: add more
		items.push_back({ "Cancel", [](reference<ui_controller> controller) { controller->m_unit_menu_state.page = menu_page::item; controller->m_unit_menu_state.selected_item.reset(); controller->m_unit_menu_index = 0; } });
		return items;
	}
}