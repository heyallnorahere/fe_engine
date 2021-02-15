#pragma once
#include "_math.h"
#include "map.h"
namespace fe_engine {
	namespace script_wrappers {
		void set_map(reference<map> m);
		// unit class
		void FEEngine_Unit_GetPosition(uint64_t unit_index, s32vec2* out_position);
		void FEEngine_Unit_SetPosition(uint64_t unit_index, s32vec2 in_position);
		uint32_t FEEngine_Unit_GetHP(uint64_t unit_index);
		void FEEngine_Unit_SetHP(uint64_t unit_index, uint32_t hp);
		uint32_t FEEngine_Unit_GetCurrentMovement(uint64_t unit_index);
		void FEEngine_Unit_SetCurrentMovement(uint64_t unit_index, uint32_t mv);
		void FEEngine_Unit_Move(uint64_t unit_index, s32vec2 offset);
		uint64_t FEEngine_Unit_GetUnitAt(s32vec2 position);
		// map class
		uint64_t FEEngine_Map_GetUnitCount();
	}
}