#include "pch.h"
#include "managed_objects.h"
// im lazy
#define IMPLEMENT_RAW_FUNCTION_IMPL(type, expression) void* type::raw() const { return expression; }
#define IMPLEMENT_RAW_FUNCTION(type) IMPLEMENT_RAW_FUNCTION_IMPL(type, this->m)
IMPLEMENT_RAW_FUNCTION(managed_field);
IMPLEMENT_RAW_FUNCTION(managed_property);
IMPLEMENT_RAW_FUNCTION(managed_method);
IMPLEMENT_RAW_FUNCTION_IMPL(managed_object, mono_gchandle_get_target(this->m));
IMPLEMENT_RAW_FUNCTION_IMPL(managed_delegate, mono_gchandle_get_target(this->m_object));
IMPLEMENT_RAW_FUNCTION(managed_class);
IMPLEMENT_RAW_FUNCTION(managed_assembly);
// ACTUAL code
managed_interface::~managed_interface() { }
managed_field::managed_field(MonoClassField* field) {
    this->m = field;
}
managed_property::managed_property(MonoProperty* property) {
    this->m = property;
}
static MonoObject* call_method(MonoObject* object, MonoMethod* method, void** params = NULL) {
    MonoObject* exception = NULL;
    MonoObject* return_value = mono_runtime_invoke(method, object, params, &exception);
    // todo: check exception object
    return return_value;
}
managed_object* managed_method::call_function(std::shared_ptr<managed_method> method, void** params) {
    auto return_value = call_method(NULL, method->m, params);
    uint32_t handle = mono_gchandle_new(return_value, false);
    return new managed_object(handle, method->m_domain);
}
managed_method::managed_method(MonoMethod* method, MonoDomain* domain) {
    this->m = method;
    this->m_domain = domain;
}
std::shared_ptr<managed_object> managed_object::make_null() {
    return std::shared_ptr<managed_object>(new managed_object(0, NULL));
}
std::shared_ptr<managed_object> managed_object::call_method(std::shared_ptr<managed_method> method, void** params) {
    auto return_value = ::call_method(mono_gchandle_get_target(this->m), method->m, params);
    uint32_t handle = mono_gchandle_new(return_value, false);
    return std::shared_ptr<managed_object>(new managed_object(handle, this->m_domain));
}
std::shared_ptr<managed_object> managed_object::get_property(std::shared_ptr<managed_property> property) const {
    MonoObject* value = mono_property_get_value(property->m, mono_gchandle_get_target(this->m), NULL, NULL);
    uint32_t handle = mono_gchandle_new(value, false);
    return std::shared_ptr<managed_object>(new managed_object(handle, this->m_domain));
}
void managed_object::set_property(std::shared_ptr<managed_property> property, void* value) {
    mono_property_set_value(property->m, mono_gchandle_get_target(this->m), &value, NULL);
}
std::shared_ptr<managed_object> managed_object::get_field(std::shared_ptr<managed_field> field) const {
    MonoObject* value = mono_field_get_value_object(this->m_domain, field->m, mono_gchandle_get_target(this->m));
    uint32_t handle = mono_gchandle_new(value, false);
    return std::shared_ptr<managed_object>(new managed_object(handle, this->m_domain));
}
void managed_object::set_field(std::shared_ptr<managed_field> field, void* value) {
    mono_field_set_value(mono_gchandle_get_target(this->m), field->m, value);
}
void* managed_object::unbox() const {
    return mono_object_unbox(mono_gchandle_get_target(this->m));
}
managed_object::~managed_object() {
    mono_gchandle_free(this->m);
}
managed_object::managed_object(uint32_t object, MonoDomain* domain) {
    this->m = object;
    this->m_domain = domain;
}
managed_delegate::managed_delegate(std::shared_ptr<managed_object> object) {
    this->m_object = object->m;
    this->m_domain = object->m_domain;
}
managed_delegate::managed_delegate(MonoObject* object, MonoDomain* domain) {
    this->m_object = mono_gchandle_new(object, false);
    this->m_domain = domain;
}
managed_delegate::~managed_delegate() {
    mono_gchandle_free(this->m_object);
}
std::shared_ptr<managed_object> managed_delegate::invoke(void** params) const {
    MonoObject* exception = NULL;
    MonoObject* return_value = mono_runtime_delegate_invoke(mono_gchandle_get_target(this->m_object), params, &exception);
    // todo: check exception object
    uint32_t handle = mono_gchandle_new(return_value, false);
    return std::shared_ptr<managed_object>(new managed_object(handle, this->m_domain));
}
std::shared_ptr<managed_object> managed_class::instantiate() const {
    MonoObject* instance = mono_object_new(this->m_domain, this->m);
    mono_runtime_object_init(instance);
    uint32_t handle = mono_gchandle_new(instance, false);
    return std::shared_ptr<managed_object>(new managed_object(handle, this->m_domain));
}
std::shared_ptr<managed_method> managed_class::get_method(const std::string& desc) const {
    MonoMethodDesc* methoddesc = mono_method_desc_new(desc.c_str(), false);
    MonoMethod* method = mono_method_desc_search_in_class(methoddesc, this->m);
    return std::shared_ptr<managed_method>(new managed_method(method, this->m_domain));
}
std::shared_ptr<managed_field> managed_class::get_field(const std::string& name) const {
    MonoClassField* field = mono_class_get_field_from_name(this->m, name.c_str());
    return std::shared_ptr<managed_field>(new managed_field(field));
}
std::shared_ptr<managed_property> managed_class::get_property(const std::string& name) const {
    MonoProperty* property = mono_class_get_property_from_name(this->m, name.c_str());
    return std::shared_ptr<managed_property>(new managed_property(property));
}
std::string managed_class::get_full_name() const {
    return this->m_namespace_name + "." + this->m_class_name;
}
std::string managed_class::get_namespace() const {
    return this->m_namespace_name;
}
std::string managed_class::get_class_name() const {
    return this->m_class_name;
}
managed_class::managed_class(MonoClass* _class, MonoDomain* domain, MonoImage* image, const std::string& namespace_name, const std::string& class_name) {
    this->m = _class;
    this->m_domain = domain;
    this->m_image = image;
    this->m_namespace_name = namespace_name;
    this->m_class_name = class_name;
}
static void parse_cs_classname(const std::string& fullname, std::string& ns_name, std::string& cls_name) {
    size_t pos = fullname.find_last_of('.');
    if (pos == std::string::npos) {
        cls_name = fullname;
    } else {
        ns_name = fullname.substr(0, pos);
        cls_name = fullname.substr(pos + 1);
    }
}
std::shared_ptr<managed_class> managed_assembly::get_class(const std::string& name) const {
    std::string namespace_name, class_name;
    parse_cs_classname(name, namespace_name, class_name);
    MonoClass* _class = mono_class_from_name(this->m_image, namespace_name.c_str(), class_name.c_str());
    return std::shared_ptr<managed_class>(new managed_class(_class, this->m_domain, this->m_image, namespace_name, class_name));
}
MonoImage* managed_assembly::get_image() const {
    return this->m_image;
}
std::shared_ptr<managed_assembly> managed_assembly::get_corlib(MonoDomain* domain) {
    auto assembly = new managed_assembly(NULL, domain);
    assembly->m_image = mono_get_corlib();
    return std::shared_ptr<managed_assembly>(assembly);
}
managed_assembly::managed_assembly(MonoAssembly* assembly, MonoDomain* domain) {
    this->m = assembly;
    this->m_domain = domain;
    if (this->m) {
        this->m_image = mono_assembly_get_image(this->m);
    }
}