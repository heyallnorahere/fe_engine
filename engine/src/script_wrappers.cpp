#include "script_wrappers.h"
#ifdef max
#undef max
#endif
#include <limits>
#include <cassert>
#include <mono/jit/jit.h>
#include "logger.h"
#include "object_register.h"
static MonoDomain* domain;
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
	static reference<map> get_map() {
		auto map_register = object_registry::get_register<map>();
		assert(map_register->size() > 0);
		return map_register->get(0);
	}
	std::string from_mono(MonoString* str) {
		return std::string(mono_string_to_utf8(str));
	}
	static MonoString* to_mono(const std::string& str) {
		return mono_string_new(domain, str.c_str());
	}
	namespace script_wrappers {
		void set_domain(MonoDomain* domain) {
			::domain = domain;
		}
		MonoString* FEEngine_Unit_GetName(uint64_t unit_index) {
			return to_mono(get_map()->get_unit(unit_index)->get_name());
		}
		void FEEngine_Unit_SetName(uint64_t unit_index, MonoString* name) {
			get_map()->get_unit(unit_index)->set_name(from_mono(name));
		}
		void FEEngine_Unit_GetPosition(uint64_t unit_index, s32vec2* out_position) {
			*out_position = get_map()->get_unit(unit_index)->get_pos();
		}
		void FEEngine_Unit_SetPosition(uint64_t unit_index, s32vec2 in_position) {
			reference<unit> u = get_map()->get_unit(unit_index);
			s8vec2 pos = u->get_pos();
			u->move(static_cast<s8vec2>(in_position) - pos, 0);
		}
		uint32_t FEEngine_Unit_GetHP(uint64_t unit_index) {
			return (uint32_t)get_map()->get_unit(unit_index)->get_current_hp();
		}
		void FEEngine_Unit_SetHP(uint64_t unit_index, uint32_t hp) {
			get_map()->get_unit(unit_index)->set_current_hp((int32_t)hp);
		}
		uint32_t FEEngine_Unit_GetCurrentMovement(uint64_t unit_index) {
			return (uint32_t)get_map()->get_unit(unit_index)->get_available_movement();
		}
		void FEEngine_Unit_SetCurrentMovement(uint64_t unit_index, uint32_t mv) {
			get_map()->get_unit(unit_index)->set_available_movement((int32_t)mv);
		}
		uint64_t FEEngine_Unit_GetInventorySize(uint64_t unit_index) {
			return get_map()->get_unit(unit_index)->get_inventory().size();
		}
		unit_affiliation FEEngine_Unit_GetAffiliation(uint64_t unit_index) {
			return get_map()->get_unit(unit_index)->get_affiliation();
		}
		unit::unit_stats FEEngine_Unit_GetStats(uint64_t unit_index) {
			return get_map()->get_unit(unit_index)->get_stats();
		}
		void FEEngine_Unit_SetStats(uint64_t unit_index, unit::unit_stats stats) {
			get_map()->get_unit(unit_index)->get_stats() = stats;
		}
		void FEEngine_Unit_Move(uint64_t unit_index, s32vec2 offset) {
			get_map()->get_unit(unit_index)->move(offset);
		}
		void FEEngine_Unit_Attack(uint64_t unit_index, uint64_t other_index) {
			get_map()->get_unit(unit_index)->attack(get_map()->get_unit(other_index));
		}
		void FEEngine_Unit_Wait(uint64_t unit_index) {
			get_map()->get_unit(unit_index)->wait();
		}
		void FEEngine_Unit_Equip(uint64_t unit_index, uint64_t item_index) {
			std::list<size_t>& inventory = get_map()->get_unit(unit_index)->get_inventory();
			std::list<size_t>::iterator it = inventory.begin();
			std::advance(it, item_index);
			get_map()->get_unit(unit_index)->equip(*it);
		}
		bool FEEngine_Unit_HasWeaponEquipped(uint64_t unit_index) {
			return get_map()->get_unit(unit_index)->get_equipped_weapon();
		}
		uint64_t FEEngine_Map_GetUnitCount() {
			return get_map()->get_unit_count();
		}
		s32vec2 FEEngine_Map_GetSize() {
			return { (int32_t)get_map()->get_width(), (int32_t)get_map()->get_height() };
		}
		uint64_t FEEngine_Map_GetUnitAt(s32vec2 position) {
			for (uint64_t i = 0; i < get_map()->get_unit_count(); i++) {
				if (get_map()->get_unit(i)->get_pos() == (s8vec2)position) {
					return i;
				}
			}
			assert(false);
			return std::numeric_limits<uint64_t>::max();
		}
		bool FEEngine_Map_IsTileOccupied(s32vec2 position) {
			return get_map()->get_unit_at(position);
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
		MonoString* FEEngine_Item_GetName(uint64_t unit_index, uint64_t item_index) {
			auto item_register = object_registry::get_register<item>();
			std::list<size_t>& inventory = get_map()->get_unit(unit_index)->get_inventory();
			reference<item> i;
			if (item_index != inventory.size()) {
				std::list<size_t>::iterator it = inventory.begin();
				std::advance(it, item_index);
				i = item_register->get(*it);
			}
			else {
				i = item_register->get(get_map()->get_unit(unit_index)->get_equipped_weapon());
			}
			return to_mono(i->get_name());
		}
		void FEEngine_Item_SetName(uint64_t unit_index, uint64_t item_index, MonoString* name) {
			auto item_register = object_registry::get_register<item>();
			std::list<size_t>& inventory = get_map()->get_unit(unit_index)->get_inventory();
			reference<item> i;
			if (item_index != inventory.size()) {
				std::list<size_t>::iterator it = inventory.begin();
				std::advance(it, item_index);
				i = item_register->get(*it);
			}
			else {
				i = item_register->get(get_map()->get_unit(unit_index)->get_equipped_weapon());
			}
			i->set_name(from_mono(name));
		}
		void FEEngine_Item_Use(uint64_t unit_index, uint64_t item_index) {
			auto item_register = object_registry::get_register<item>();
			reference<unit> u = get_map()->get_unit(unit_index);
			std::list<size_t>& inventory = u->get_inventory();
			std::list<size_t>::iterator it = inventory.begin();
			std::advance(it, item_index);
			reference<item> i = item_register->get(*it);
			assert(!i->used());
			i->set_used(true);
			reference<item_behavior> ib = i->get_behavior();
			if (ib) {
				ib->on_use();
			}
			inventory.remove_if([&](size_t index) { return index == *it; });
		}
		bool FEEngine_Item_IsWeapon(uint64_t unit_index, uint64_t item_index) {
			auto item_register = object_registry::get_register<item>();
			reference<unit> u = get_map()->get_unit(unit_index);
			std::list<size_t>& inventory = u->get_inventory();
			std::list<size_t>::iterator it = inventory.begin();
			std::advance(it, item_index);
			reference<item> i = item_register->get(*it);
			return i->get_item_flags() & item::weapon;
		}
		weapon::weapon_stats FEEngine_Weapon_GetStats(uint64_t unit, uint64_t index) {
			auto item_register = object_registry::get_register<item>();
			reference<::fe_engine::unit> u = get_map()->get_unit(unit);
			std::list<size_t>& inventory = u->get_inventory();
			reference<weapon> w;
			if (index != inventory.size()) {
				std::list<size_t>::iterator it = inventory.begin();
				std::advance(it, index);
				w = item_register->get(*it);
			}
			else {
				w = item_register->get(u->get_equipped_weapon());
			}
			assert(w);
			return w->get_stats();
		}
		void FEEngine_Weapon_SetStats(uint64_t unit, uint64_t index, weapon::weapon_stats stats) {
			auto item_register = object_registry::get_register<item>();
			reference<::fe_engine::unit> u = get_map()->get_unit(unit);
			std::list<size_t>& inventory = u->get_inventory();
			reference<weapon> w;
			if (index != inventory.size()) {
				std::list<size_t>::iterator it = inventory.begin();
				std::advance(it, index);
				w = item_register->get(*it);
			}
			else {
				w = item_register->get(u->get_equipped_weapon());
			}
			assert(w);
			w->get_stats() = stats;
		}
		weapon::type FEEngine_Weapon_GetType(uint64_t unit, uint64_t index) {
			auto item_register = object_registry::get_register<item>();
			reference<::fe_engine::unit> u = get_map()->get_unit(unit);
			std::list<size_t>& inventory = u->get_inventory();
			reference<weapon> w;
			if (index != inventory.size()) {
				std::list<size_t>::iterator it = inventory.begin();
				std::advance(it, index);
				w = item_register->get(*it);
			}
			else {
				w = item_register->get(u->get_equipped_weapon());
			}
			assert(w);
			return w->get_type();
		}
		void FEEngine_Logger_Print(MonoString* message, int color) {
			logger::print(from_mono(message), parse_cs_color_enum(color));
		}
		input_mapper::commands FEEngine_InputMapper_GetState(input_mapper* address) {
			reference<input_mapper> im = address;
			return im->get_state();
		}
	}
}