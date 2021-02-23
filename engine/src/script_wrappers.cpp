#include "script_wrappers.h"
#ifdef max
#undef max
#endif
#include <limits>
#include <cassert>
#include <mono/jit/jit.h>
static fe_engine::reference<fe_engine::map> script_wrapper_map;
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
		}
	}
	static std::string from_mono(MonoString* str) {
		return std::string(mono_string_to_utf8(str));
	}
	static MonoString* to_mono(const std::string& str) {
		return mono_string_new(domain, str.c_str());
	}
	namespace script_wrappers {
		void set_map(reference<map> m) {
			script_wrapper_map = m;
		}
		void set_domain(MonoDomain* domain) {
			::domain = domain;
		}
		void FEEngine_Unit_GetPosition(uint64_t unit_index, s32vec2* out_position) {
			*out_position = script_wrapper_map->get_unit(unit_index)->get_pos();
		}
		void FEEngine_Unit_SetPosition(uint64_t unit_index, s32vec2 in_position) {
			reference<unit> u = script_wrapper_map->get_unit(unit_index);
			s8vec2 pos = u->get_pos();
			u->move(static_cast<s8vec2>(in_position) - pos, 0);
		}
		uint32_t FEEngine_Unit_GetHP(uint64_t unit_index) {
			return (uint32_t)script_wrapper_map->get_unit(unit_index)->get_current_hp();
		}
		void FEEngine_Unit_SetHP(uint64_t unit_index, uint32_t hp) {
			script_wrapper_map->get_unit(unit_index)->set_current_hp((int32_t)hp);
		}
		uint32_t FEEngine_Unit_GetCurrentMovement(uint64_t unit_index) {
			return (uint32_t)script_wrapper_map->get_unit(unit_index)->get_available_movement();
		}
		void FEEngine_Unit_SetCurrentMovement(uint64_t unit_index, uint32_t mv) {
			script_wrapper_map->get_unit(unit_index)->set_available_movement((int32_t)mv);
		}
		uint64_t FEEngine_Unit_GetInventorySize(uint64_t unit_index) {
			return script_wrapper_map->get_unit(unit_index)->get_inventory().size();
		}
		unit::unit_stats FEEngine_Unit_GetStats(uint64_t unit_index) {
			return script_wrapper_map->get_unit(unit_index)->get_stats();
		}
		void FEEngine_Unit_SetStats(uint64_t unit_index, unit::unit_stats stats) {
			script_wrapper_map->get_unit(unit_index)->get_stats() = stats;
		}
		void FEEngine_Unit_Move(uint64_t unit_index, s32vec2 offset) {
			script_wrapper_map->get_unit(unit_index)->move(offset);
		}
		bool FEEngine_Unit_HasWeaponEquipped(uint64_t unit_index) {
			return script_wrapper_map->get_unit(unit_index)->get_equipped_weapon();
		}
		uint64_t FEEngine_Unit_GetUnitAt(s32vec2 position) {
			for (uint64_t i = 0; i < script_wrapper_map->get_unit_count(); i++) {
				if (script_wrapper_map->get_unit(i)->get_pos() == (s8vec2)position) {
					return i;
				}
			}
			return std::numeric_limits<uint64_t>::max();
		}
		uint64_t FEEngine_Map_GetUnitCount() {
			return script_wrapper_map->get_unit_count();
		}
		s32vec2 FEEngine_Map_GetSize() {
			return { (int32_t)script_wrapper_map->get_width(), (int32_t)script_wrapper_map->get_height() };
		}
		void FEEngine_Renderer_RenderCharAt(renderer* address, s32vec2 position, char character, int color) {
			renderer::color c = parse_cs_color_enum(color);
			reference<renderer> r = reference<renderer>(address);
			r->render_char_at(position.x, position.y, character, c);
		}
		void FEEngine_Renderer_RenderStringAt(renderer* address, s32vec2 position, MonoString* text, int color) {
			std::string str = from_mono(text);
			renderer::color c = parse_cs_color_enum(color);
			reference<renderer> r = reference<renderer>(address);
			r->render_string_at(position.x, position.y, str, c);
		}
		s32vec2 FEEngine_Renderer_GetBufferSize(renderer* address) {
			reference<renderer> r = reference<renderer>(address);
			size_t width, height;
			r->get_buffer_size(width, height);
			return { (int32_t)width, (int32_t)height };
		}
		MonoString* FEEngine_Item_GetName(uint64_t unit_index, uint64_t item_index) {
			std::list<reference<item>>& inventory = script_wrapper_map->get_unit(unit_index)->get_inventory();
			reference<item> i;
			if (item_index != inventory.size()) {
				std::list<reference<item>>::iterator it = inventory.begin();
				std::advance(it, item_index);
				i = *it;
			}
			else {
				i = script_wrapper_map->get_unit(unit_index)->get_equipped_weapon();
			}
			return to_mono(i->get_name());
		}
		void FEEngine_Item_SetName(uint64_t unit_index, uint64_t item_index, MonoString* name) {
			std::list<reference<item>>& inventory = script_wrapper_map->get_unit(unit_index)->get_inventory();
			reference<item> i;
			if (item_index != inventory.size()) {
				std::list<reference<item>>::iterator it = inventory.begin();
				std::advance(it, item_index);
				i = *it;
			}
			else {
				i = script_wrapper_map->get_unit(unit_index)->get_equipped_weapon();
			}
			i->set_name(from_mono(name));
		}
		void FEEngine_Item_Use(uint64_t unit_index, uint64_t item_index) {
			reference<unit> u = script_wrapper_map->get_unit(unit_index);
			std::list<reference<item>>& inventory = u->get_inventory();
			std::list<reference<item>>::iterator it = inventory.begin();
			std::advance(it, item_index);
			reference<item> i = *it;
			assert(!i->used());
			i->set_used(true);
			reference<item_behavior> ib = i->get_behavior();
			if (ib) {
				ib->on_use();
			}
			inventory.remove_if([&](reference<item> _i) { return _i.get() == i.get(); });
		}
		bool FEEngine_Item_IsWeapon(uint64_t unit_index, uint64_t item_index) {
			reference<unit> u = script_wrapper_map->get_unit(unit_index);
			std::list<reference<item>>& inventory = u->get_inventory();
			std::list<reference<item>>::iterator it = inventory.begin();
			std::advance(it, item_index);
			reference<item> i = *it;
			return i->get_item_flags() & item::weapon;
		}
		weapon::weapon_stats FEEngine_Weapon_GetStats(uint64_t unit, uint64_t index) {
			reference<::fe_engine::unit> u = script_wrapper_map->get_unit(unit);
			std::list<reference<item>>& inventory = u->get_inventory();
			reference<weapon> w;
			if (index != inventory.size()) {
				std::list<reference<item>>::iterator it = inventory.begin();
				std::advance(it, index);
				w = *it;
			}
			else {
				w = u->get_equipped_weapon();
			}
			assert(w);
			return w->get_stats();
		}
		void FEEngine_Weapon_SetStats(uint64_t unit, uint64_t index, weapon::weapon_stats stats) {
			reference<::fe_engine::unit> u = script_wrapper_map->get_unit(unit);
			std::list<reference<item>>& inventory = u->get_inventory();
			reference<weapon> w;
			if (index != inventory.size()) {
				std::list<reference<item>>::iterator it = inventory.begin();
				std::advance(it, index);
				w = *it;
			}
			else {
				w = u->get_equipped_weapon();
			}
			assert(w);
			w->get_stats() = stats;
		}
	}
}