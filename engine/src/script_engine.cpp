#include "script_engine.h"
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/attrdefs.h>
#include <Windows.h>
#include "script_wrappers.h"
namespace fe_engine {
	MonoAssembly* load_assembly_from_file(const char* filepath) {
		if (!filepath) {
			return NULL;
		}
		HANDLE file = CreateFileA(filepath, FILE_READ_ACCESS, NULL, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		if (file == INVALID_HANDLE_VALUE) {
			return NULL;
		}
		DWORD file_size = GetFileSize(file, NULL);
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
		MonoImageOpenStatus status;
		MonoImage* image = mono_image_open_from_data_full(reinterpret_cast<char*>(file_data), file_size, true, &status, false);
		if (status != MONO_IMAGE_OK) {
			return NULL;
		}
		MonoAssembly* assembly = mono_assembly_load_from_full(image, filepath, &status, false);
		free(file_data);
		CloseHandle(file);
		mono_image_close(image);
		return assembly;
	}
	static MonoClass* get_class(MonoImage* image, const std::string& namespace_name, const std::string& class_name) {
		return mono_class_from_name(image, namespace_name.c_str(), class_name.c_str());
	}
	static uint32_t instantiate(MonoDomain* domain, MonoClass* _class) {
		MonoObject* instance = mono_object_new(domain, _class);
		mono_runtime_object_init(instance);
		return mono_gchandle_new(instance, false);
	}
	static MonoMethod* get_method(MonoImage* image, const std::string& method_desc) {
		MonoMethodDesc* desc = mono_method_desc_new(method_desc.c_str(), false);
		return mono_method_desc_search_in_image(desc, image);
	}
	static MonoObject* call_method(MonoObject* object, MonoMethod* method, void** params = NULL) {
		MonoObject* exception = NULL;
		return mono_runtime_invoke(method, object, params, &exception);
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
		mono_add_internal_call("FEEngine.Unit::GetPosition_Native", script_wrappers::FEEngine_Unit_GetPosition);
		mono_add_internal_call("FEEngine.Unit::SetPosition_Native", script_wrappers::FEEngine_Unit_SetPosition);
		mono_add_internal_call("FEEngine.Unit::GetHP_Native", script_wrappers::FEEngine_Unit_GetHP);
		mono_add_internal_call("FEEngine.Unit::SetHP_Native", script_wrappers::FEEngine_Unit_SetHP);
		mono_add_internal_call("FEEngine.Unit::GetUnitAt_Native", script_wrappers::FEEngine_Unit_GetUnitAt);
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
		register_wrappers();
		if (cleanup) {
			mono_domain_unload(this->m_domain);
			this->m_domain = domain;
		}
	}
	void script_engine::shutdown_engine() {
		this->shutdown_mono();
	}
	reference<cs_class> assembly::get_class(const std::string& namespace_name, const std::string& class_name) {
		MonoImage* image = mono_assembly_get_image(this->m_assembly);
		MonoClass* _class = ::fe_engine::get_class(image, namespace_name, class_name);
		return reference<cs_class>(new cs_class(_class, this->m_domain, image));
	}
	assembly::assembly(MonoAssembly* a, MonoDomain* domain) {
		this->m_assembly = a;
		this->m_domain = domain;
	}
	reference<cs_object> cs_class::instantiate() {
		return reference<cs_object>(new cs_object(::fe_engine::instantiate(this->m_domain, (MonoClass*)this->m_class)));
	}
	reference<cs_method> cs_class::get_method(const std::string& desc) {
		return reference<cs_method>(new cs_method(::fe_engine::get_method((MonoImage*)this->m_image, desc)));
	}
	cs_class::cs_class(void* _class, MonoDomain* domain, void* image) {
		this->m_class = _class;
		this->m_domain = domain;
		this->m_image = image;
	}
	reference<cs_object> cs_object::call_method(reference<cs_method> method, void** params) {
		MonoObject* object = ::fe_engine::call_method(mono_gchandle_get_target(this->m_object), (MonoMethod*)method->m_method, params);
		uint32_t handle = mono_gchandle_new(object, false);
		return reference<cs_object>(new cs_object(handle));
	}
	cs_object::cs_object(uint32_t object) {
		this->m_object = object;
	}
	cs_method::cs_method(void* method) {
		this->m_method = method;
	}
}