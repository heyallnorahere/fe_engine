#pragma once
#include "reference.h"
#include <string>
extern "C" {
	typedef struct _MonoAssembly MonoAssembly;
	typedef struct _MonoDomain MonoDomain;
}
namespace fe_engine {
	class cs_method : public ref_counted {
	private:
		cs_method(void* method);
		void* m_method;
		friend class cs_class;
		friend class cs_object;
	};
	class cs_object : public ref_counted {
	public:
		reference<cs_object> call_method(reference<cs_method> method, void** params = NULL);
	private:
		cs_object(uint32_t object);
		uint32_t m_object;
		friend class cs_class;
	};
	class cs_class : public ref_counted {
	public:
		reference<cs_object> instantiate();
		reference<cs_method> get_method(const std::string& desc);
	private:
		cs_class(void* _class, MonoDomain* domain, void* image);
		void* m_class;
		MonoDomain* m_domain;
		void* m_image;
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