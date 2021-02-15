#pragma once
#include "reference.h"
#include <string>
#include "map.h"
#include "mono_classes.h"
extern "C" {
	typedef struct _MonoAssembly MonoAssembly;
	typedef struct _MonoDomain MonoDomain;
}
namespace fe_engine {
	class script_engine : public ref_counted {
	public:
		script_engine(const std::string& core_assembly_path, reference<map> m);
		~script_engine();
		reference<assembly> load_assembly(const std::string& assembly_path);
		reference<assembly> get_core();
	private:
		void init_mono();
		void shutdown_mono();
		void init_engine(const std::string& core_assembly_path, reference<map> m);
		void shutdown_engine();
		MonoAssembly* m_core;
		MonoDomain* m_domain;
	};
}