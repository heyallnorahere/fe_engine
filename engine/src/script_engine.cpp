#include "script_engine.h"
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/attrdefs.h>
#include <mono/metadata/environment.h>
#include <nlohmann/json.hpp>
#include <fstream>
#include <vector>
#include <string>
#include <sstream>
#include <iostream>
#include <cassert>
#include <cstring>
#include "script_wrappers.h"
#include "buffer.h"
#include "logger.h"
#ifdef FEENGINE_WINDOWS
#include <Windows.h>
#endif
namespace fe_engine {
	template<typename T> bool config_has_property(const std::string& name, T& value) {
		std::ifstream file("scriptconfig.json");
		if (!file.is_open()) {
			return false;
		}
		nlohmann::json json_data;
		file >> json_data;
		file.close();
		if (json_data.find(name) != json_data.end()) {
			json_data[name].get_to(value);
			return true;
		}
		return false;
	}
	util::buffer* read_file(const char* filepath) {
#ifdef FEENGINE_WINDOWS
		HANDLE file = CreateFileA(filepath, FILE_READ_ACCESS, NULL, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		if (file == INVALID_HANDLE_VALUE) {
			return NULL;
		}
		size_t file_size = GetFileSize(file, NULL);
		if (file_size == INVALID_FILE_SIZE) {
			return NULL;
		}
		void* file_data = malloc(file_size);
		if (!file_data) {
			CloseHandle(file);
			return NULL;
		}
		DWORD read = 0;
		ReadFile(file, file_data, file_size, &read, NULL);
		if (file_size != read) {
			free(file_data);
			CloseHandle(file);
			return NULL;
		}
		CloseHandle(file);
#else
		FILE* f = fopen(filepath, "rb");
		fseek(f, 0, SEEK_END);
		size_t file_size = ftell(f);
		rewind(f);
		void* file_data = malloc(file_size * sizeof(char));
		memset(file_data, 0, file_size * sizeof(char));
		size_t read = fread(file_data, sizeof(char), file_size, f);
		fclose(f);
#endif
		util::buffer* buf = new util::buffer(file_size * sizeof(char));
		memcpy(buf->get(), file_data, buf->get_size());
		free(file_data);
		return buf;
	}
	MonoAssembly* load_assembly_from_file(const char* filepath) {
        util::buffer* buf = read_file(filepath);
		MonoImageOpenStatus status;
		MonoImage* image = mono_image_open_from_data_full(util::buffer_cast<char>(buf), buf->get_size(), true, &status, false);
		if (status != MONO_IMAGE_OK) {
			return NULL;
		}
		MonoAssembly* assembly = mono_assembly_load_from_full(image, filepath, &status, false);
		mono_image_close(image);
		delete buf;
		return assembly;
	}
	MonoClass* get_class(MonoImage* image, const std::string& namespace_name, const std::string& class_name) {
		return mono_class_from_name(image, namespace_name.c_str(), class_name.c_str());
	}
	uint32_t instantiate(MonoDomain* domain, MonoClass* _class) {
		MonoObject* instance = mono_object_new(domain, _class);
		mono_runtime_object_init(instance);
		return mono_gchandle_new(instance, false);
	}
	MonoMethod* get_method(MonoImage* image, const std::string& method_desc) {
		MonoMethodDesc* desc = mono_method_desc_new(method_desc.c_str(), false);
		return mono_method_desc_search_in_image(desc, image);
	}
	std::string from_mono(MonoString* str);
	MonoProperty* get_property_id(MonoClass* _class, const std::string& name);
	MonoObject* get_property(MonoProperty* property, MonoObject* object);
	static MonoObject* get_property(MonoObject* object, MonoClass* _class, const std::string& name) {
		MonoProperty* id = get_property_id(_class, name);
		return get_property(id, object);
	}
	void check_exception(MonoObject* exception) {
		if (exception) {
			MonoClass* exception_class = mono_get_exception_class();
			MonoDomain* domain = mono_object_get_domain(exception);
			std::string message = from_mono((MonoString*)get_property(exception, exception_class, "Message"));
			std::string stacktrace = from_mono((MonoString*)get_property(exception, exception_class, "StackTrace"));
			logger::print("Exception occurred!", renderer::color::red);
			logger::print("Message: " + message);
			logger::print("Stacktrace: " + stacktrace);
		}
	}
	MonoObject* call_method(MonoObject* object, MonoMethod* method, void** params = NULL) {
		MonoObject* exception = NULL;
		MonoObject* return_value = mono_runtime_invoke(method, object, params, &exception);
		check_exception(exception);
		return return_value;
	}
	void* unbox_object(MonoObject* object) {
		return mono_object_unbox(object);
	}
	MonoClassField* get_field_id(MonoClass* _class, const std::string& name) {
		return mono_class_get_field_from_name(_class, name.c_str());
	}
	MonoObject* get_field(MonoClassField* field, MonoObject* object, MonoDomain* domain) {
		return mono_field_get_value_object(domain, field, object);
	}
	void set_field(MonoObject* object, MonoClassField* field, void* value) {
		mono_field_set_value(object, field, value);
	}
	MonoProperty* get_property_id(MonoClass* _class, const std::string& name) {
		return mono_class_get_property_from_name(_class, name.c_str());
	}
	MonoObject* get_property(MonoProperty* property, MonoObject* object) {
		return mono_property_get_value(property, object, NULL, NULL);
	}
	void set_property(MonoProperty* property, MonoObject* object, void* value) {
		mono_property_set_value(property, object, &value, NULL);
	}
	script_engine::script_engine(const std::string& core_assembly_path) {
		this->init_engine(core_assembly_path);
	}
	script_engine::~script_engine() {
		this->shutdown_engine();
	}
	reference<assembly> script_engine::load_assembly(const std::string& assembly_path) {
		return reference<assembly>(new assembly(load_assembly_from_file(assembly_path.c_str()), this->m_domain));
	}
	reference<assembly> script_engine::get_core() {
		return reference<assembly>(new assembly(this->m_core, this->m_domain));
	}
	void script_engine::init_mono() {
		std::string path;
		bool has_path = config_has_property("cslibrarypath", path);
		mono_set_assemblies_path(has_path ? path.c_str() : "mono/lib");
		MonoDomain* domain = mono_jit_init("FEEngine");
		char* name = (char*)"FEEngineRuntime";
		this->m_domain = mono_domain_create_appdomain(name, NULL);
	}
	void script_engine::shutdown_mono() {
		mono_jit_cleanup(this->m_domain);
	}
	static void register_wrappers() {
#define register(name, address) mono_add_internal_call(name, (void*)script_wrappers:: address)
		register("FEEngine.Unit::GetName_Native", FEEngine_Unit_GetName);
		register("FEEngine.Unit::SetName_Native", FEEngine_Unit_SetName);
		register("FEEngine.Unit::GetPosition_Native", FEEngine_Unit_GetPosition);
		register("FEEngine.Unit::SetPosition_Native", FEEngine_Unit_SetPosition);
		register("FEEngine.Unit::GetHP_Native", FEEngine_Unit_GetHP);
		register("FEEngine.Unit::SetHP_Native", FEEngine_Unit_SetHP);
		register("FEEngine.Unit::GetCurrentMovement_Native",  FEEngine_Unit_GetCurrentMovement);
		register("FEEngine.Unit::SetCurrentMovement_Native", FEEngine_Unit_SetCurrentMovement);
		register("FEEngine.Unit::GetInventorySize_Native", FEEngine_Unit_GetInventorySize);
		register("FEEngine.Unit::GetItemIndex_Native", FEEngine_Unit_GetItemIndex);
		register("FEEngine.Unit::AddItem_Native", FEEngine_Unit_AddItem);
		register("FEEngine.Unit::GetAffiliation_Native", FEEngine_Unit_GetAffiliation);
		register("FEEngine.Unit::GetStats_Native", FEEngine_Unit_GetStats);
		register("FEEngine.Unit::SetStats_Native", FEEngine_Unit_SetStats);
		register("FEEngine.Unit::Move_Native", FEEngine_Unit_Move);
		register("FEEngine.Unit::Attack_Native", FEEngine_Unit_Attack);
		register("FEEngine.Unit::Wait_Native", FEEngine_Unit_Wait);
		register("FEEngine.Unit::Equip_Native", FEEngine_Unit_Equip);
		register("FEEngine.Unit::GetEquippedWeapon_Native", FEEngine_Unit_GetEquippedWeapon);
		register("FEEngine.Unit::HasWeaponEquipped_Native", FEEngine_Unit_HasWeaponEquipped);
		register("FEEngine.Map::GetUnit_Native", FEEngine_Map_GetUnit);
		register("FEEngine.Map::GetUnitCount_Native", FEEngine_Map_GetUnitCount);
		register("FEEngine.Map::GetSize_Native", FEEngine_Map_GetSize);
		register("FEEngine.Map::GetUnitAt_Native", FEEngine_Map_GetUnitAt);
		register("FEEngine.Map::IsTileOccupied_Native", FEEngine_Map_IsTileOccupied);
		register("FEEngine.Renderer::RenderCharAt_Native", FEEngine_Renderer_RenderCharAt);
		register("FEEngine.Renderer::RenderStringAt_Native", FEEngine_Renderer_RenderStringAt);
		register("FEEngine.Renderer::GetBufferSize_Native", FEEngine_Renderer_GetBufferSize);
		register("FEEngine.Item::GetName_Native", FEEngine_Item_GetName);
		register("FEEngine.Item::SetName_Native", FEEngine_Item_SetName);
		register("FEEngine.Item::Use_Native", FEEngine_Item_Use);
		register("FEEngine.Item::IsWeapon_Native", FEEngine_Item_IsWeapon);
		register("FEEngine.Item::NewItem_Native", FEEngine_Item_NewItem);
		register("FEEngine.Weapon::GetStats_Native", FEEngine_Weapon_GetStats);
		register("FEEngine.Weapon::SetStats_Native", FEEngine_Weapon_SetStats);
		register("FEEngine.Weapon::GetType_Native", FEEngine_Weapon_GetType);
		register("FEEngine.Logger::Print_Native", FEEngine_Logger_Print);
		register("FEEngine.InputMapper::GetState_Native", FEEngine_InputMapper_GetState);
		register("FEEngine.Tile::GetPassingProperties_Native", FEEngine_Tile_GetPassingProperties);
		register("FEEngine.Tile::GetColor_Native", FEEngine_Tile_GetColor);
		register("FEEngine.Tile::SetColor_Native", FEEngine_Tile_SetColor);
		register("FEEngine.Util.ObjectRegistry::RegisterExists_Native", FEEngine_Util_ObjectRegistry_RegisterExists);
		register("FEEngine.Util.ObjectRegister`1::GetCount_Native", FEEngine_Util_ObjectRegister_GetCount);
		register("FEEngine.UI.UIController::GetUnitMenuTarget_Native", FEEngine_UI_UIController_GetUnitMenuTarget);
		register("FEEngine.UI.UIController::HasUnitSelected_Native", FEEngine_UI_UIController_HasUnitSelected);
		register("FEEngine.UI.UIController::GetUserMenuCount_Native", FEEngine_UI_UIController_GetUserMenuCount);
		register("FEEngine.UI.UIController::GetUserMenu_Native", FEEngine_UI_UIController_GetUserMenu);
		register("FEEngine.UI.UIController::AddUserMenu_Native", FEEngine_UI_UIController_AddUserMenu);
		register("FEEngine.UI.UIController::ExitUnitMenu_Native", FEEngine_UI_UIController_ExitUnitMenu);
		register("FEEngine.UI.Menu::MakeNew_Native", FEEngine_UI_Menu_MakeNew);
		register("FEEngine.UI.Menu::GetMenuItemCount_Native", FEEngine_UI_Menu_GetMenuItemCount);
		register("FEEngine.UI.Menu::GetMenuItem_Native", FEEngine_UI_Menu_GetMenuItem);
		register("FEEngine.UI.Menu::AddMenuItem_Native", FEEngine_UI_Menu_AddMenuItem);
#undef register
	}
	void script_engine::init_engine(const std::string& core_assembly_path) {
		this->init_mono();
		MonoDomain* domain = NULL;
		bool cleanup = false;
		if (this->m_domain) {
			domain = mono_domain_create_appdomain("FEEngine Runtime", NULL);
			mono_domain_set(domain, false);
			cleanup = true;
		}
		this->m_core = load_assembly_from_file(core_assembly_path.c_str());
		script_wrappers::init_wrappers(cleanup ? domain : this->m_domain, mono_assembly_get_image(this->m_core), this);
		register_wrappers();
		if (cleanup) {
#ifndef FEENGINE_LINUX
			mono_domain_unload(this->m_domain);
#endif
			this->m_domain = domain;
		}
	}
	void script_engine::shutdown_engine() {
		this->shutdown_mono();
	}
}
