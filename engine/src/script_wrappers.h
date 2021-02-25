#pragma once
#include "_math.h"
#include "map.h"
#include "unit.h"
#include "renderer.h"
#include "weapon.h"
extern "C" {
	typedef struct _MonoString MonoString;
	typedef struct _MonoDomain MonoDomain;
}
namespace fe_engine {
	namespace script_wrappers {
		void set_map(reference<map> m);
		void set_domain(MonoDomain* domain);
		// unit class
		MonoString* FEEngine_Unit_GetName(uint64_t unit_index);
		void FEEngine_Unit_SetName(uint64_t unit_index, MonoString* name);
		void FEEngine_Unit_GetPosition(uint64_t unit_index, s32vec2* out_position);
		void FEEngine_Unit_SetPosition(uint64_t unit_index, s32vec2 in_position);
		uint32_t FEEngine_Unit_GetHP(uint64_t unit_index);
		void FEEngine_Unit_SetHP(uint64_t unit_index, uint32_t hp);
		unit::unit_stats FEEngine_Unit_GetStats(uint64_t unit_index);
		void FEEngine_Unit_SetStats(uint64_t unit_index, unit::unit_stats stats);
		uint32_t FEEngine_Unit_GetCurrentMovement(uint64_t unit_index);
		void FEEngine_Unit_SetCurrentMovement(uint64_t unit_index, uint32_t mv);
		uint64_t FEEngine_Unit_GetInventorySize(uint64_t unit_index);
		unit_affiliation FEEngine_Unit_GetAffiliation(uint64_t unit_index);
		void FEEngine_Unit_Move(uint64_t unit_index, s32vec2 offset);
		void FEEngine_Unit_Attack(uint64_t unit_index, uint64_t other_index);
		bool FEEngine_Unit_HasWeaponEquipped(uint64_t unit_index);
		uint64_t FEEngine_Unit_GetUnitAt(s32vec2 position);
		// map class
		uint64_t FEEngine_Map_GetUnitCount();
		s32vec2 FEEngine_Map_GetSize();
		// renderer class
		void FEEngine_Renderer_RenderCharAt(renderer* address, s32vec2 position, char character, int color);
		void FEEngine_Renderer_RenderStringAt(renderer* address, s32vec2 position, MonoString* text, int color);
		s32vec2 FEEngine_Renderer_GetBufferSize(renderer* address);
		// item class
		MonoString* FEEngine_Item_GetName(uint64_t unit_index, uint64_t item_index);
		void FEEngine_Item_SetName(uint64_t unit_index, uint64_t item_index, MonoString* name);
		void FEEngine_Item_Use(uint64_t unit_index, uint64_t item_index);
		bool FEEngine_Item_IsWeapon(uint64_t unit_index, uint64_t item_index);
		// weapon class
		weapon::weapon_stats FEEngine_Weapon_GetStats(uint64_t unit, uint64_t index);
		void FEEngine_Weapon_SetStats(uint64_t unit, uint64_t index, weapon::weapon_stats stats);
		// logger class
		void FEEngine_Logger_Print(MonoString* message, int color);
	}
}