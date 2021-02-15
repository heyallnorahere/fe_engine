#pragma once
#include "reference.h"
#include <string>
extern "C" {
	typedef struct _MonoAssembly MonoAssembly;
	typedef struct _MonoDomain MonoDomain;
}
namespace fe_engine {
	class cs_field : public ref_counted {
	private:
		cs_field(void* field);
		void* m_field;
		friend class cs_class;
		friend class cs_object;
	};
	class cs_property : public ref_counted {
	private:
		cs_property(void* property);
		void* m_property;
		friend class cs_class;
		friend class cs_object;
	};
	class cs_object;
	class cs_method : public ref_counted {
	public:
		static cs_object* call_function(reference<cs_method> method, void** params = NULL);
		void* raw();
	private:
		cs_method(void* method, MonoDomain* domain);
		void* m_method;
		MonoDomain* m_domain;
		friend class cs_class;
		friend class cs_object;
	};
	class cs_object : public ref_counted {
	public:
		reference<cs_object> call_method(reference<cs_method> method, void** params = NULL);
		reference<cs_object> get_property(reference<cs_property> property);
		void set_property(reference<cs_property> property, void* value);
		reference<cs_object> get_field(reference<cs_field> field);
		void set_field(reference<cs_field> field, void* value);
		void* raw();
	private:
		cs_object(uint32_t object, MonoDomain* domain);
		uint32_t m_object;
		MonoDomain* m_domain;
		friend class cs_class;
		friend class cs_method;
	};
	class cs_class : public ref_counted {
	public:
		reference<cs_object> instantiate();
		reference<cs_method> get_method(const std::string& desc);
		reference<cs_field> get_field(const std::string& name);
		reference<cs_property> get_property(const std::string& name);
		std::string get_full_name();
		std::string get_namespace_name();
		std::string get_class_name();
		void* raw();
	private:
		cs_class(void* _class, MonoDomain* domain, void* image, const std::string& ns_name, const std::string& cls_name);
		void* m_class;
		MonoDomain* m_domain;
		void* m_image;
		std::string m_namespace_name, m_class_name;
		friend class assembly;
	};
	class assembly : public ref_counted {
	public:
		reference<cs_class> get_class(const std::string& namespace_name, const std::string& class_name);
	private:
		assembly(MonoAssembly* a, MonoDomain* domain);
		MonoAssembly* m_assembly;
		MonoDomain* m_domain;
		friend class script_engine;
	};
}