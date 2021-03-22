#include "mono_classes.h"
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
namespace fe_engine {
	extern MonoClass* get_class(MonoImage* image, const std::string& namespace_name, const std::string& class_name);
	extern uint32_t instantiate(MonoDomain* domain, MonoClass* _class);
	extern MonoMethod* get_method(MonoImage* image, const std::string& method_desc);
	extern MonoObject* call_method(MonoObject* object, MonoMethod* method, void** params = NULL);
	extern MonoClassField* get_field_id(MonoClass* _class, const std::string& name);
	extern MonoObject* get_field(MonoClassField* field, MonoObject* object, MonoDomain* domain);
	extern void set_field(MonoObject* object, MonoClassField* field, void* value);
	extern MonoProperty* get_property_id(MonoClass* _class, const std::string& name);
	extern MonoObject* get_property(MonoProperty* property, MonoObject* object);
	extern void set_property(MonoProperty* property, MonoObject* object, void* value);
	extern void* unbox_object(MonoObject* object);
	extern void check_exception(MonoObject* exception);
	reference<cs_class> assembly::get_class(const std::string& namespace_name, const std::string& class_name) {
		MonoImage* image = mono_assembly_get_image(this->m_assembly);
		MonoClass* _class = ::fe_engine::get_class(image, namespace_name, class_name);
		return reference<cs_class>(new cs_class(_class, this->m_domain, image, namespace_name, class_name));
	}
	assembly::assembly(MonoAssembly* a, MonoDomain* domain) {
		this->m_assembly = a;
		this->m_domain = domain;
	}
	reference<cs_object> cs_class::instantiate() {
		return reference<cs_object>(new cs_object(::fe_engine::instantiate(this->m_domain, (MonoClass*)this->m_class), this->m_domain));
	}
	reference<cs_method> cs_class::get_method(const std::string& desc) {
		return reference<cs_method>(new cs_method(::fe_engine::get_method((MonoImage*)this->m_image, desc), this->m_domain));
	}
	reference<cs_field> cs_class::get_field(const std::string& name) {
		return reference<cs_field>(new cs_field(get_field_id((MonoClass*)this->m_class, name)));
	}
	reference<cs_property> cs_class::get_property(const std::string& name) {
		return reference<cs_property>(new cs_property(get_property_id((MonoClass*)this->m_class, name)));
	}
	std::string cs_class::get_full_name() {
		return this->m_namespace_name + (this->m_namespace_name.empty() ? "" : ".") + this->m_class_name;
	}
	std::string cs_class::get_namespace_name() {
		return this->m_namespace_name;
	}
	std::string cs_class::get_class_name() {
		return this->m_class_name;
	}
	void* cs_class::raw() {
		return this->m_class;
	}
	cs_class::cs_class(void* _class, MonoDomain* domain, void* image, const std::string& ns_name, const std::string& cls_name) {
		this->m_class = _class;
		this->m_domain = domain;
		this->m_image = image;
		this->m_namespace_name = ns_name;
		this->m_class_name = cls_name;
	}
	reference<cs_object> cs_object::call_method(reference<cs_method> method, void** params) {
		MonoObject* object = ::fe_engine::call_method(mono_gchandle_get_target(this->m_object), (MonoMethod*)method->m_method, params);
		uint32_t handle = mono_gchandle_new(object, false);
		return reference<cs_object>(new cs_object(handle, this->m_domain));
	}
	reference<cs_object> cs_object::get_property(reference<cs_property> property) {
		MonoObject* object = ::fe_engine::get_property((MonoProperty*)property->m_property, mono_gchandle_get_target(this->m_object));
		uint32_t handle = mono_gchandle_new(object, false);
		return reference<cs_object>(new cs_object(handle, this->m_domain));
	}
	void cs_object::set_property(reference<cs_property> property, void* value) {
		::fe_engine::set_property((MonoProperty*)property->m_property, mono_gchandle_get_target(this->m_object), value);
	}
	reference<cs_object> cs_object::get_field(reference<cs_field> field) {
		MonoObject* object = ::fe_engine::get_field((MonoClassField*)field->m_field, mono_gchandle_get_target(this->m_object), this->m_domain);
		uint32_t handle = mono_gchandle_new(object, false);
		return reference<cs_object>(new cs_object(handle, this->m_domain));
	}
	void cs_object::set_field(reference<cs_field> field, void* value) {
		::fe_engine::set_field(mono_gchandle_get_target(this->m_object), (MonoClassField*)field->m_field, value);
	}
	void* cs_object::raw() {
		return mono_gchandle_get_target(this->m_object);
	}
	void* cs_object::unbox() {
		return unbox_object((MonoObject*)this->raw());
	}
	cs_object::~cs_object() {
		if (this->m_object) mono_gchandle_free(this->m_object);
	}
	cs_object::cs_object(uint32_t object, MonoDomain* domain) {
		this->m_object = object;
		this->m_domain = domain;
	}
	reference<cs_object> cs_object::make_null() {
		return reference<cs_object>(new cs_object(0, (MonoDomain*)NULL));
	}
	cs_object* cs_method::call_function(reference<cs_method> method, void** params) {
		MonoObject* object = ::fe_engine::call_method(NULL, (MonoMethod*)method->m_method, params);
		uint32_t handle = mono_gchandle_new(object, false);
		return new cs_object(handle, method->m_domain);
	}
	void* cs_method::raw() {
		return this->m_method;
	}
	cs_method::cs_method(void* method, MonoDomain* domain) {
		this->m_method = method;
		this->m_domain = domain;
	}
	cs_property::cs_property(void* property) {
		this->m_property = property;
	}
	cs_field::cs_field(void* field) {
		this->m_field = field;
	}
	cs_delegate::cs_delegate(reference<cs_object> object) {
		this->m_object = mono_gchandle_new((MonoObject*)object->raw(), false);
		this->m_domain = object->m_domain;
	}
	cs_delegate::cs_delegate(void* object, void* domain) {
		this->m_object = mono_gchandle_new((MonoObject*)object, false);
		this->m_domain = (MonoDomain*)domain;
	}
	cs_delegate::~cs_delegate() {
		mono_gchandle_free(this->m_object);
	}
	void* cs_delegate::raw() {
		return mono_gchandle_get_target(this->m_object);
	}
	reference<cs_object> cs_delegate::invoke(void** params) {
		MonoObject* exception = NULL;
		MonoObject* object = mono_runtime_delegate_invoke((MonoObject*)this->raw(), params, &exception);
		check_exception(exception);
		uint32_t handle = mono_gchandle_new(object, false);
		return reference<cs_object>(new cs_object(handle, this->m_domain));
	}
}
