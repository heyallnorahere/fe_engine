#include "script_engine.h"
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/attrdefs.h>
#include <fstream>
#include <vector>
#include <string>
#include <iostream>
#include "script_wrappers.h"
#include "buffer.h"
#ifdef FEENGINE_WINDOWS
#include <Windows.h>
#endif
namespace fe_engine {
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
	MonoObject* call_method(MonoObject* object, MonoMethod* method, void** params = NULL) {
		MonoObject* exception = NULL;
		return mono_runtime_invoke(method, object, params, &exception);
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
	script_engine::script_engine(const std::string& core_assembly_path, reference<map> m) {
		this->init_engine(core_assembly_path, m);
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
		mono_set_assemblies_path("mono/lib");
		MonoDomain* domain = mono_jit_init("FEEngine");
		char* name = (char*)"FEEngineRuntime";
		this->m_domain = mono_domain_create_appdomain(name, NULL);
	}
	void script_engine::shutdown_mono() {
		mono_jit_cleanup(this->m_domain);
	}
	static void register_wrappers() {
		// unit class
		mono_add_internal_call("FEEngine.Unit::GetPosition_Native", (void*)script_wrappers::FEEngine_Unit_GetPosition);
		mono_add_internal_call("FEEngine.Unit::SetPosition_Native", (void*)script_wrappers::FEEngine_Unit_SetPosition);
		mono_add_internal_call("FEEngine.Unit::GetHP_Native", (void*)script_wrappers::FEEngine_Unit_GetHP);
		mono_add_internal_call("FEEngine.Unit::SetHP_Native", (void*)script_wrappers::FEEngine_Unit_SetHP);
		mono_add_internal_call("FEEngine.Unit::GetCurrentMovement_Native",  (void*)script_wrappers::FEEngine_Unit_GetCurrentMovement);
		mono_add_internal_call("FEEngine.Unit::SetCurrentMovement_Native", (void*)script_wrappers::FEEngine_Unit_SetCurrentMovement);
		mono_add_internal_call("FEEngine.Unit::GetInventorySize_Native", (void*)script_wrappers::FEEngine_Unit_GetInventorySize);
		mono_add_internal_call("FEEngine.Unit::GetStats_Native", (void*)script_wrappers::FEEngine_Unit_GetStats);
		mono_add_internal_call("FEEngine.Unit::SetStats_Native", (void*)script_wrappers::FEEngine_Unit_SetStats);
		mono_add_internal_call("FEEngine.Unit::Move_Native", (void*)script_wrappers::FEEngine_Unit_Move);
		mono_add_internal_call("FEEngine.Unit::GetUnitAt_Native", (void*)script_wrappers::FEEngine_Unit_GetUnitAt);
		mono_add_internal_call("FEEngine.Map::GetUnitCount_Native", (void*)script_wrappers::FEEngine_Map_GetUnitCount);
		mono_add_internal_call("FEEngine.Map::GetSize_Native", (void*)script_wrappers::FEEngine_Map_GetSize);
		mono_add_internal_call("FEEngine.Renderer::RenderCharAt_Native", (void*)script_wrappers::FEEngine_Renderer_RenderCharAt);
		mono_add_internal_call("FEEngine.Renderer::RenderStringAt_Native", (void*)script_wrappers::FEEngine_Renderer_RenderStringAt);
		mono_add_internal_call("FEEngine.Renderer::GetBufferSize_Native", (void*)script_wrappers::FEEngine_Renderer_GetBufferSize);
		mono_add_internal_call("FEEngine.Item::GetName_Native", (void*)script_wrappers::FEEngine_Item_GetName);
		mono_add_internal_call("FEEngine.Item::SetName_Native", (void*)script_wrappers::FEEngine_Item_SetName);
		mono_add_internal_call("FEEngine.Item::Use_Native", (void*)script_wrappers::FEEngine_Item_Use);
	}
	void script_engine::init_engine(const std::string& core_assembly_path, reference<map> m) {
		this->init_mono();
		MonoDomain* domain = NULL;
		bool cleanup = false;
		if (this->m_domain) {
			domain = mono_domain_create_appdomain("FEEngine Runtime", NULL);
			mono_domain_set(domain, false);
			cleanup = true;
		}
		this->m_core = load_assembly_from_file(core_assembly_path.c_str());
		script_wrappers::set_map(m);
		script_wrappers::set_domain(cleanup ? domain : this->m_domain);
		register_wrappers();
		if (cleanup) {
			mono_domain_unload(this->m_domain);
			this->m_domain = domain;
		}
	}
	void script_engine::shutdown_engine() {
		this->shutdown_mono();
	}
}
