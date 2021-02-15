#include "script_wrappers.h"
#ifdef max
#undef max
#endif
#include <limits>
static fe_engine::reference<fe_engine::map> script_wrapper_map;
namespace fe_engine::script_wrappers {
	void set_map(reference<map> m) {
		script_wrapper_map = m;
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
		return script_wrapper_map->get_unit(unit_index)->get_current_hp();
	}
	void FEEngine_Unit_SetHP(uint64_t unit_index, uint32_t hp) {
		script_wrapper_map->get_unit(unit_index)->set_current_hp(hp);
	}
	uint64_t FEEngine_Unit_GetUnitAt(s32vec2 position) {
		for (uint64_t i = 0; i < script_wrapper_map->get_unit_count(); i++) {
			if (script_wrapper_map->get_unit(i)->get_pos() == (s8vec2)position) {
				return i;
			}
		}
		return std::numeric_limits<uint64_t>::max();
	}
}