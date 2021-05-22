#include "script_wrappers.h"
#ifdef max
#undef max
#endif
#include <limits>
#include <cassert>
#include <functional>
#include <mono/jit/jit.h>
#include "logger.h"
#include "object_register.h"
#include "ui_controller.h"
#include "menu.h"
static MonoDomain* domain;
static MonoImage* image;
static fe_engine::reference<fe_engine::script_engine> engine;
namespace fe_engine {
	static renderer::color parse_cs_color_enum(int32_t color) {
		switch (color) {
		case 0: // red
			return renderer::color::red;
			break;
		case 1: // green
			return renderer::color::green;
			break;
		case 2: // blue
			return renderer::color::blue;
			break;
		case 3: // yellow
			return renderer::color::yellow;
			break;
		case 4: // white
			return renderer::color::white;
			break;
		case 5: // black
			return renderer::color::black;
			break;
		default:
			return renderer::color::none;
			break;
		}
	}
	static renderer::background_color parse_cs_bg_color_enum(int32_t color) {
		switch (color) {
		case 0: // red
			return renderer::background_color::red;
			break;
		case 1: // green
			return renderer::background_color::green;
			break;
		case 2: // blue
			return renderer::background_color::blue;
			break;
		case 3: // yellow
			return renderer::background_color::yellow;
			break;
		case 4: // white
			return renderer::background_color::white;
			break;
		case 5: // black
			return renderer::background_color::black;
			break;
		default:
			return renderer::background_color::none;
			break;
		}
	}
	static int32_t get_cs_bg_color_enum(renderer::background_color color) {
		switch (color) {
		case renderer::background_color::red:
			return 0;
			break;
		case renderer::background_color::green:
			return 1;
			break;
		case renderer::background_color::blue:
			return 2;
			break;
		case renderer::background_color::yellow:
			return 3;
			break;
		case renderer::background_color::white:
			return 4;
			break;
		case renderer::background_color::black:
			return 5;
			break;
		case renderer::background_color::none:
			return 6;
			break;
		}
	}
	std::string from_mono(MonoString* str) {
		return std::string(mono_string_to_utf8(str));
	}
	static MonoString* to_mono(const std::string& str) {
		return mono_string_new(domain, str.c_str());
	}
	namespace internal {
		class close_ui_controller_unit_menu {
		public:
			void operator()(reference<ui_controller> uc) {
				uc->close_unit_menu();
			}
		};
	}
	namespace script_wrappers {
		static reference<object_register<unit>> unit_register;
		static reference<object_register<item>> item_register;
		static reference<object_register<map>> map_register;
		static reference<object_register<input_mapper>> im_register;
		static reference<object_register<ui_controller>> uc_register;
		static reference<object_register<internal::menu>> menu_register;
		static reference<object_register<assembly>> assembly_register;
		struct register_function_struct {
			std::function<bool()> register_exists;
			std::function<uint64_t()> size;
		};
		static std::unordered_map<MonoType*, register_function_struct> register_function_map;
		template<typename T> static uint64_t get_register_size() {
			return object_registry::get_register<T>()->size();
		}
		template<typename T> void register_registeredobject_type(const std::string& class_name) {
			MonoType* type = mono_reflection_type_from_name((char*)class_name.c_str(), image);
			assert(type);
			register_function_struct rfs;
			rfs.register_exists = object_registry::register_exists<T>;
			rfs.size = get_register_size<T>;
			register_function_map[type] = rfs;
		}
		template<typename T> void get_register(reference<object_register<T>>& ref) {
			assert(object_registry::register_exists<T>());
			ref = object_registry::get_register<T>();
		}
		template<typename T> void init_register(reference<object_register<T>>& ref, const std::string& class_name) {
			get_register(ref);
			register_registeredobject_type<T>(class_name);
		}
		void init_registers() {
			init_register(unit_register, "FEEngine.Unit");
			init_register(item_register, "FEEngine.Item");
			init_register(map_register, "FEEngine.Map");
			init_register(im_register, "FEEngine.InputMapper");
			if (object_registry::register_exists<ui_controller>()) {
				init_register(uc_register, "FEEngine.UI.UIController");
				init_register(menu_register, "FEEngine.UI.Menu");
			}
			// for FEEngine.Item.NewItem<>()
			if (object_registry::register_exists<assembly>()) {
				get_register(assembly_register);
			}
		}
		void init_wrappers(MonoDomain* domain, MonoImage* image, reference<script_engine> engine) {
			::domain = domain;
			::image = image;
			::engine = engine;
			init_registers();
		}
		static MonoType* find_type(const std::string& name) {
			for (size_t i = 0; i < assembly_register->size(); i++) {
				auto _assembly = assembly_register->get(i);
				MonoImage* image = (MonoImage*)_assembly->get_image();
				MonoType* type = mono_reflection_type_from_name((char*)name.c_str(), image);
				if (type) {
					return type;
				}
			}
			return NULL;
		}
		static reference<cs_class> find_class(const std::string& name, reference<assembly>* assembly_out = NULL) {
			size_t pos = name.find_last_of('.');
			std::string ns_name, class_name;
			if (pos == std::string::npos) {
				class_name = name;
			}
			else {
				ns_name = name.substr(0, pos);
				class_name = name.substr(pos + 1);
			}
			reference<object_register<assembly>> assembly_register = object_registry::get_register<assembly>();
			for (size_t i = 0; i < assembly_register->size(); i++) {
				auto _assembly = assembly_register->get(i);
				auto _class = _assembly->get_class(ns_name, class_name);
				if (_class->raw()) {
					if (assembly_out) {
						*assembly_out = _assembly;
					}
					return _class;
				}
			}
			return reference<cs_class>();
		}
		MonoString* FEEngine_Unit_GetName(uint64_t unit_index) {
			return to_mono(unit_register->get(unit_index)->get_name());
		}
		void FEEngine_Unit_SetName(uint64_t unit_index, MonoString* name) {
			unit_register->get(unit_index)->set_name(from_mono(name));
		}
		void FEEngine_Unit_GetPosition(uint64_t unit_index, s32vec2* out_position) {
			*out_position = unit_register->get(unit_index)->get_pos();
		}
		void FEEngine_Unit_SetPosition(uint64_t unit_index, s32vec2 in_position) {
			reference<unit> u = unit_register->get(unit_index);
			s8vec2 pos = u->get_pos();
			u->move(static_cast<s8vec2>(in_position) - pos, 0);
		}
		uint32_t FEEngine_Unit_GetHP(uint64_t unit_index) {
			return (uint32_t)unit_register->get(unit_index)->get_current_hp();
		}
		void FEEngine_Unit_SetHP(uint64_t unit_index, uint32_t hp) {
			unit_register->get(unit_index)->set_current_hp((int32_t)hp);
		}
		uint32_t FEEngine_Unit_GetCurrentMovement(uint64_t unit_index) {
			return (uint32_t)unit_register->get(unit_index)->get_available_movement();
		}
		void FEEngine_Unit_SetCurrentMovement(uint64_t unit_index, uint32_t mv) {
			unit_register->get(unit_index)->set_available_movement((int32_t)mv);
		}
		uint64_t FEEngine_Unit_GetInventorySize(uint64_t unit_index) {
			return unit_register->get(unit_index)->get_inventory().size();
		}
		uint64_t FEEngine_Unit_GetItemIndex(uint64_t unit_index, uint64_t item_index) {
			auto& inventory = unit_register->get(unit_index)->get_inventory();
			auto it = inventory.begin();
			std::advance(it, item_index);
			return *it;
		}
		void FEEngine_Unit_AddItem(uint64_t unit_index, uint64_t item_registry_index) {
			auto& inventory = unit_register->get(unit_index)->get_inventory();
			inventory.push_back(item_registry_index);
		}
		unit_affiliation FEEngine_Unit_GetAffiliation(uint64_t unit_index) {
			return unit_register->get(unit_index)->get_affiliation();
		}
		unit::unit_stats FEEngine_Unit_GetStats(uint64_t unit_index) {
			return unit_register->get(unit_index)->get_stats();
		}
		void FEEngine_Unit_SetStats(uint64_t unit_index, unit::unit_stats stats) {
			unit_register->get(unit_index)->get_stats() = stats;
		}
		void FEEngine_Unit_Move(uint64_t unit_index, s32vec2 offset) {
			unit_register->get(unit_index)->move(offset);
		}
		void FEEngine_Unit_Attack(uint64_t unit_index, uint64_t other_index) {
			unit_register->get(unit_index)->attack(other_index);
		}
		void FEEngine_Unit_Wait(uint64_t unit_index) {
			unit_register->get(unit_index)->wait();
		}
		void FEEngine_Unit_Equip(uint64_t unit_index, uint64_t item_index) {
			unit_register->get(unit_index)->equip(item_index);
		}
		uint64_t FEEngine_Unit_GetEquippedWeapon(uint64_t unit_index) {
			reference<unit> u = unit_register->get(unit_index);
			return u->get_equipped_weapon();
		}
		bool FEEngine_Unit_HasWeaponEquipped(uint64_t unit_index) {
			return unit_register->get(unit_index)->get_equipped_weapon() != (size_t)-1;
		}
		uint64_t FEEngine_Map_GetUnitCount(uint64_t index) {
			return map_register->get(index)->get_unit_count();
		}
		s32vec2 FEEngine_Map_GetSize(uint64_t index) {
			return { (int32_t)map_register->get(index)->get_width(), (int32_t)map_register->get(index)->get_height() };
		}
		uint64_t FEEngine_Map_GetUnit(uint64_t index, uint64_t unit_index) {
			return map_register->get(index)->get_unit_register_index(unit_index);
		}
		uint64_t FEEngine_Map_GetUnitAt(uint64_t index, s32vec2 position) {
			for (uint64_t i = 0; i < unit_register->size(); i++) {
				if (unit_register->get(i)->get_pos() == (s8vec2)position) {
					return i;
				}
			}
			assert(false);
			return std::numeric_limits<uint64_t>::max();
		}
		bool FEEngine_Map_IsTileOccupied(uint64_t index, s32vec2 position) {
			return map_register->get(index)->get_unit_at(position);
		}
		void FEEngine_Renderer_RenderCharAt(renderer* address, s32vec2 position, char character, int color, int background) {
			renderer::color c = parse_cs_color_enum(color);
			renderer::background_color bg = parse_cs_bg_color_enum(background);
			reference<renderer> r = reference<renderer>(address);
			r->render_char_at(position.x, position.y, character, c, bg);
		}
		void FEEngine_Renderer_RenderStringAt(renderer* address, s32vec2 position, MonoString* text, int color, int background) {
			std::string str = from_mono(text);
			renderer::color c = parse_cs_color_enum(color);
			renderer::background_color bg = parse_cs_bg_color_enum(background);
			reference<renderer> r = reference<renderer>(address);
			r->render_string_at(position.x, position.y, str, c, bg);
		}
		s32vec2 FEEngine_Renderer_GetBufferSize(renderer* address) {
			reference<renderer> r = reference<renderer>(address);
			size_t width, height;
			r->get_buffer_size(width, height);
			return { (int32_t)width, (int32_t)height };
		}
		MonoString* FEEngine_Item_GetName(uint64_t item_index) {
			reference<item> i = item_register->get(item_index);
			return to_mono(i->get_name());
		}
		void FEEngine_Item_SetName(uint64_t item_index, MonoString* name) {
			reference<item> i = item_register->get(item_index);
			i->set_name(from_mono(name));
		}
		void FEEngine_Item_Use(uint64_t unit_index, uint64_t item_index) {
			reference<unit> u = unit_register->get(unit_index);
			std::list<size_t>& inventory = u->get_inventory();
			reference<item> i = item_register->get(item_index);
			assert(!i->used());
			i->set_used(true);
			reference<item_behavior> ib = i->get_behavior();
			if (ib) {
				ib->on_use();
			}
			inventory.remove_if([&](size_t index) { return index == item_index; });
			if (u->can_move()) u->wait();
		}
		bool FEEngine_Item_IsWeapon(uint64_t item_index) {
			reference<item> i = item_register->get(item_index);
			return i->get_item_flags() & item::weapon;
		}
		uint64_t FEEngine_Item_NewItem(MonoString* managed_name, MonoReflectionType* behavior_type) {
			std::string name = from_mono(managed_name);
			MonoType* type = mono_reflection_type_get_type(behavior_type);
			std::string type_name = mono_type_get_name(type);
			reference<cs_class> cls = find_class(type_name);
			reference<item_behavior> behavior = reference<item_behavior>::create(cls, engine->get_core());
			reference<item> i = reference<item>::create(name, item::usable, behavior);
			return item_register->add(i);
		}
		weapon::weapon_stats FEEngine_Weapon_GetStats(uint64_t unit, uint64_t index) {
			auto item_register = object_registry::get_register<item>();
			reference<weapon> w = item_register->get(index);
			return w->get_stats();
		}
		void FEEngine_Weapon_SetStats(uint64_t unit, uint64_t index, weapon::weapon_stats stats) {
			auto item_register = object_registry::get_register<item>();
			reference<weapon> w = item_register->get(index);
			w->get_stats() = stats;
		}
		weapon::type FEEngine_Weapon_GetType(uint64_t unit, uint64_t index) {
			auto item_register = object_registry::get_register<item>();
			reference<weapon> w = item_register->get(index);
			return w->get_type();
		}
		void FEEngine_Logger_Print(MonoString* message, int32_t color) {
			logger::print(from_mono(message), parse_cs_color_enum(color));
		}
		input_mapper::commands FEEngine_InputMapper_GetState(uint64_t index) {
			reference<input_mapper> im = im_register->get(index);
			return im->get_state();
		}
		tile::passing_properties FEEngine_Tile_GetPassingProperties(uint64_t map_index, s32vec2 tile_position) {
			auto map_register = object_registry::get_register<map>();
			reference<map> m = map_register->get(map_index);
			return m->get_tile(tile_position)->get_properties();
		}
		int32_t FEEngine_Tile_GetColor(uint64_t map_index, s32vec2 tile_position) {
			auto map_register = object_registry::get_register<map>();
			reference<map> m = map_register->get(map_index);
			return get_cs_bg_color_enum(m->get_tile(tile_position)->get_color());
		}
		void FEEngine_Tile_SetColor(uint64_t map_index, s32vec2 tile_position, int32_t color) {
			auto map_register = object_registry::get_register<map>();
			reference<map> m = map_register->get(map_index);
			reference<tile> t = m->get_tile(tile_position);
			t->set_color(parse_cs_bg_color_enum(color));
		}
		bool FEEngine_Util_ObjectRegistry_RegisterExists(MonoReflectionType* type) {
			MonoType* _type = mono_reflection_type_get_type(type);
			return register_function_map[_type].register_exists();
		}
		uint64_t FEEngine_Util_ObjectRegister_GetCount(MonoReflectionType* type) {
			MonoType* _type = mono_reflection_type_get_type(type);
			return register_function_map[_type].size();
		}
		uint64_t FEEngine_UI_UIController_GetUnitMenuTarget(uint64_t index) {
			reference<ui_controller> uc = uc_register->get(index);
			reference<unit> selected = uc->get_unit_menu_target();
			for (size_t i = 0; i < unit_register->size(); i++) {
				if (unit_register->get(i).get() == selected.get()) {
					return i;
				}
			}
			return std::numeric_limits<uint64_t>::max();
		}
		bool FEEngine_UI_UIController_HasUnitSelected(uint64_t index) {
			return uc_register->get(index)->get_unit_menu_target();
		}
		uint64_t FEEngine_UI_UIController_GetUserMenuCount(uint64_t index) {
			return uc_register->get(index)->get_user_menus().size();
		}
		cs_structs::menu_description_struct FEEngine_UI_UIController_GetUserMenu(uint64_t index, uint64_t menu_index) {
			reference<ui_controller> uc = uc_register->get(index);
			ui_controller::user_menu user_menu = uc->get_user_menus()[menu_index];
			reference<cs_class> menu_class = engine->get_core()->get_class("FEEngine.UI", "Menu");
			reference<cs_method> construction_method = menu_class->get_method("FEEngine.UI.Menu:MakeFromRegisterIndex(ulong)");
			std::vector<void*> args;
			args.push_back(&menu_index);
			reference<cs_object> menu_object = cs_method::call_function(construction_method, args.data());
			MonoString* menu_name = to_mono(user_menu.menu_item_name);
			return { menu_name, (MonoObject*)menu_object->raw() };
		}
		void FEEngine_UI_UIController_AddUserMenu(uint64_t index, cs_structs::menu_description_struct menu) {
			reference<ui_controller> uc = uc_register->get(index);
			reference<cs_object> menu_object = reference<cs_object>::create(menu.menu, domain);
			reference<cs_class> menu_class = engine->get_core()->get_class("FEEngine.UI", "Menu");
			reference<cs_property> index_property = menu_class->get_property("Index");
			reference<cs_object> property_value = menu_object->get_property(index_property);
			uint64_t menu_index = *(uint64_t*)property_value->unbox();
			std::string menu_name = from_mono(menu.name);
			uc->add_user_menu({ menu_index, menu_name });
		}
		void FEEngine_UI_UIController_ExitUnitMenu(uint64_t index) {
			internal::close_ui_controller_unit_menu close;
			close(uc_register->get(index));
		}
		uint64_t FEEngine_UI_Menu_MakeNew() {
			uint64_t index = menu_register->size();
			menu_register->add(reference<internal::menu>::create());
			return index;
		}
		uint64_t FEEngine_UI_Menu_GetMenuItemCount(uint64_t index) {
			return menu_register->get(index)->get_items().size();
		}
		cs_structs::menu_item FEEngine_UI_Menu_GetMenuItem(uint64_t index, uint64_t item_index) {
			internal::menu_item item = menu_register->get(index)->get_items()[item_index];
			cs_structs::menu_item menu_item;
			if (item.action) {
				menu_item.action = (MonoObject*)item.action->raw();
			}
			menu_item.submenu_index = item.submenu;
			menu_item.name = to_mono(item.name);
			menu_item.type = (int)item.type;
			return menu_item;
		}
		void FEEngine_UI_Menu_AddMenuItem(uint64_t index, cs_structs::menu_item item) {
			internal::menu_item menu_item;
			menu_item.name = from_mono(item.name);
			if (item.action) {
				menu_item.action = reference<cs_delegate>::create(item.action, domain);
			}
			menu_item.type = (internal::menu_item_type)item.type;
			menu_item.submenu = item.submenu_index;
			reference<internal::menu> menu = menu_register->get(index);
			menu->add_item(menu_item);
		}
	}
}