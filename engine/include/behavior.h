#pragma once
#include "mono_classes.h"
#include <unordered_map>
#include "renderer.h"
namespace fe_engine {
	class behavior : public ref_counted {
	public:
		behavior(reference<cs_class> _class, reference<assembly> core_assembly);
		void on_attach(uint64_t index);
		void on_unit_update();
		void on_render(reference<renderer> r);
	private:
		reference<cs_class> m_class;
		reference<cs_object> m_instance;
		reference<cs_method> m_renderer_creation_method;
		reference<assembly> m_core;
		std::unordered_map<std::string, reference<cs_method>> m_methods;
		void register_methods();
	};
}