#pragma once
class managed_interface {
public:
    virtual void* raw() const = 0;
    virtual ~managed_interface();
};
class managed_field : public managed_interface {
public:
    virtual void* raw() const override;
private:
    managed_field(MonoClassField* field);
    MonoClassField* m;
    friend class managed_class;
    friend class managed_object;
};
class managed_property : public managed_interface {
public:
    virtual void* raw() const override;
private:
    managed_property(MonoProperty* property);
    MonoProperty* m;
    friend class managed_class;
    friend class managed_object;
};
class managed_object;
class managed_method : public managed_interface {
public:
    static managed_object* call_function(std::shared_ptr<managed_method> method, void** params = NULL);
    virtual void* raw() const override;
private:
    managed_method(MonoMethod* method, MonoDomain* domain);
    MonoMethod* m;
    MonoDomain* m_domain;
    friend class managed_class;
    friend class managed_object;
};
class managed_object : public managed_interface {
public:
    static std::shared_ptr<managed_object> make_null();
    std::shared_ptr<managed_object> call_method(std::shared_ptr<managed_method> method, void** params = NULL);
    std::shared_ptr<managed_object> get_property(std::shared_ptr<managed_property> property) const;
    void set_property(std::shared_ptr<managed_property> property, void* value);
    std::shared_ptr<managed_object> get_field(std::shared_ptr<managed_field> field) const;
    void set_field(std::shared_ptr<managed_field> field, void* value);
    virtual void* raw() const override;
    void* unbox() const;
    virtual ~managed_object() override;
private:
    managed_object(uint32_t object, MonoDomain* domain);
    uint32_t m;
    MonoDomain* m_domain;
    friend class managed_class;
    friend class managed_method;
    friend class managed_delegate;
};
class managed_delegate : public managed_interface {
public:
    managed_delegate(std::shared_ptr<managed_object> object);
    managed_delegate(MonoObject* object, MonoDomain* domain);
    ~managed_delegate();
    virtual void* raw() const override;
    std::shared_ptr<managed_object> invoke(void** params = NULL) const;
private:
    uint32_t m_object;
    MonoDomain* m_domain;
};
class managed_class : public managed_interface {
public:
    // todo: add better instantiate() function
    std::shared_ptr<managed_object> instantiate() const;
    std::shared_ptr<managed_method> get_method(const std::string& desc) const;
    std::shared_ptr<managed_field> get_field(const std::string& name) const;
    std::shared_ptr<managed_property> get_property(const std::string& name) const;
    std::string get_full_name() const;
    std::string get_namespace() const;
    std::string get_class_name() const;
    virtual void* raw() const override;
private:
    managed_class(MonoClass* _class, MonoDomain* domain, MonoImage* image, const std::string& namespace_name, const std::string& class_name);
    MonoClass* m;
    MonoDomain* m_domain;
    MonoImage* m_image;
    std::string m_namespace_name, m_class_name;
    friend class managed_assembly;
};
class managed_assembly : public managed_interface {
public:
    std::shared_ptr<managed_class> get_class(const std::string& name) const;
    virtual void* raw() const override;
    MonoImage* get_image() const;
    static std::shared_ptr<managed_assembly> get_corlib(MonoDomain* domain);
private:
    managed_assembly(MonoAssembly* assembly, MonoDomain* domain);
    MonoAssembly* m;
    MonoImage* m_image;
    MonoDomain* m_domain;
    friend class scripthost;
};