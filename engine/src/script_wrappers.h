#pragma once
#include "_math.h"
#include "map.h"
#include "unit.h"
#include "renderer.h"
#include "weapon.h"
#include "input_mapper.h"
#include "script_engine.h"
extern "C" {
	typedef struct _MonoObject MonoObject;
	typedef struct _MonoString MonoString;
	typedef struct _MonoDomain MonoDomain;
	typedef struct _MonoImage MonoImage;
	typedef struct _MonoReflectionType MonoReflectionType;
}
namespace fe_engine {
	namespace script_wrappers {
		namespace cs_structs {
			struct menu_description_struct {
				MonoString* name;
				MonoObject* menu;
			};
			struct menu_item {
				MonoString* name;
				int type;
				MonoObject* action;
				uint64_t submenu_index;
			};
		}
		void init_wrappers(MonoDomain* domain, MonoImage* image, reference<script_engine> engine);
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
		void FEEngine_Unit_Wait(uint64_t unit_index);
		void FEEngine_Unit_Equip(uint64_t unit_index, uint64_t item_index);
		uint64_t FEEngine_Unit_GetEquippedWeapon(uint64_t unit_index);
		bool FEEngine_Unit_HasWeaponEquipped(uint64_t unit_index);
		// map class
		uint64_t FEEngine_Map_GetUnitCount(uint64_t index);
		s32vec2 FEEngine_Map_GetSize(uint64_t index);
		uint64_t FEEngine_Map_GetUnit(uint64_t index, uint64_t unit_index);
		uint64_t FEEngine_Map_GetUnitAt(uint64_t index, s32vec2 position);
		bool FEEngine_Map_IsTileOccupied(uint64_t index, s32vec2 position);
		// renderer class
		void FEEngine_Renderer_RenderCharAt(renderer* address, s32vec2 position, char character, int color, int background);
		void FEEngine_Renderer_RenderStringAt(renderer* address, s32vec2 position, MonoString* text, int color, int background);
		s32vec2 FEEngine_Renderer_GetBufferSize(renderer* address);
		// item class
		MonoString* FEEngine_Item_GetName(uint64_t item_index);
		void FEEngine_Item_SetName(uint64_t item_index, MonoString* name);
		void FEEngine_Item_Use(uint64_t unit_index, uint64_t item_index);
		bool FEEngine_Item_IsWeapon(uint64_t item_index);
		// weapon class
		weapon::weapon_stats FEEngine_Weapon_GetStats(uint64_t unit, uint64_t index);
		void FEEngine_Weapon_SetStats(uint64_t unit, uint64_t index, weapon::weapon_stats stats);
		weapon::type FEEngine_Weapon_GetType(uint64_t unit, uint64_t index);
		// logger class
		void FEEngine_Logger_Print(MonoString* message, int32_t color);
		// inputmapper class
		input_mapper::commands FEEngine_InputMapper_GetState(uint64_t index);
		// tile class
		tile::passing_properties FEEngine_Tile_GetPassingProperties(uint64_t map_index, s32vec2 tile_position);
		int32_t FEEngine_Tile_GetColor(uint64_t map_index, s32vec2 tile_position);
		// objectregistry/objectregister class
		bool FEEngine_Util_ObjectRegistry_RegisterExists(MonoReflectionType* type);
		uint64_t FEEngine_Util_ObjectRegister_GetCount(MonoReflectionType* type);
		// uicontroller class
		uint64_t FEEngine_UI_UIController_GetUnitMenuTarget(uint64_t index);
		bool FEEngine_UI_UIController_HasUnitSelected(uint64_t index);
		uint64_t FEEngine_UI_UIController_GetUserMenuCount(uint64_t index);
		cs_structs::menu_description_struct FEEngine_UI_UIController_GetUserMenu(uint64_t index, uint64_t menu_index);
		void FEEngine_UI_UIController_AddUserMenu(uint64_t index, cs_structs::menu_description_struct menu);
		void FEEngine_UI_UIController_ExitUnitMenu(uint64_t index);
		// menu class
		uint64_t FEEngine_UI_Menu_MakeNew();
		uint64_t FEEngine_UI_Menu_GetMenuItemCount(uint64_t index);
		cs_structs::menu_item FEEngine_UI_Menu_GetMenuItem(uint64_t index, uint64_t item_index);
		void FEEngine_UI_Menu_AddMenuItem(uint64_t index, cs_structs::menu_item item);
	}
}