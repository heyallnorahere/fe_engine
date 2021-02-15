#include "mono_classes.h"
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
namespace fe_engine {
	extern MonoClass* get_class(MonoImage* image, const std::string& namespace_name, const std::string& class_name);
	extern uint32_t instantiate(MonoDomain* domain, MonoClass* _class);
	extern MonoMethod* get_method(MonoImage* image, const std::string& method_desc);
	extern MonoObject* call_method(MonoObject* object, MonoMethod* method, void** params = NULL);
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