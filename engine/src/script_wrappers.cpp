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
static MonoDomain* domain;
static MonoImage* image;
namespace fe_engine {
	static renderer::color parse_cs_color_enum(int color) {
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
	static renderer::background_color parse_cs_bg_color_enum(int color) {
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
	std::string from_mono(MonoString* str) {
		return std::string(mono_string_to_utf8(str));
	}
	static MonoString* to_mono(const std::string& str) {
		return mono_string_new(domain, str.c_str());
	}
	namespace script_wrappers {
		static reference<object_register<unit>> unit_register;
		static reference<object_register<item>> item_register;
		static reference<object_register<map>> map_register;
		static reference<object_register<input_mapper>> im_register;
		static reference<object_register<ui_controller>> uc_register;
		static std::unordered_map<MonoType*, std::function<bool()>> registerexists_map;
		static std::unordered_map<MonoType*, std::function<uint64_t()>> size_map;
		template<typename T> static uint64_t get_register_size() {
			return object_registry::get_register<T>()->size();
		}
		template<typename T> void register_registeredobject_type(const std::string& class_name) {
			MonoType* type = mono_reflection_type_from_name((char*)class_name.c_str(), image);
			assert(type);
			registerexists_map[type] = object_registry::register_exists<T>;
			size_map[type] = get_register_size<T>;
		}
		void init_registeredobject_types() {
			register_registeredobject_type<unit>("FEEngine.Unit");
			register_registeredobject_type<item>("FEEngine.Item");
			register_registeredobject_type<map>("FEEngine.Map");
			register_registeredobject_type<input_mapper>("FEEngine.InputMapper");
			register_registeredobject_type<ui_controller>("FEEngine.UI.UIController");
		}
		template<typename T> void get_register(reference<object_register<T>>& ref) {
			assert(object_registry::register_exists<T>());
			ref = object_registry::get_register<T>();
		}
		void init_wrappers(MonoDomain* domain, MonoImage* image) {
			::domain = domain;
			::image = image;
			init_registeredobject_types();
			get_register(unit_register);
			get_register(item_register);
			get_register(map_register);
			get_register(im_register);
			get_register(uc_register);
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
		void FEEngine_Logger_Print(MonoString* message, int color) {
			logger::print(from_mono(message), parse_cs_color_enum(color));
		}
		input_mapper::commands FEEngine_InputMapper_GetState(uint64_t index) {
			reference<input_mapper> im = im_register->get(index);
			return im->get_state();
		}
		bool FEEngine_Util_ObjectRegistry_RegisterExists(MonoReflectionType* type) {
			MonoType* _type = mono_reflection_type_get_type(type);
			return registerexists_map[_type]();
		}
		uint64_t FEEngine_Util_ObjectRegister_GetCount(MonoReflectionType* type) {
			MonoType* _type = mono_reflection_type_get_type(type);
			return size_map[_type]();
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
	}
}