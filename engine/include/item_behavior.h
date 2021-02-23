#pragma once
#include "mono_classes.h"
#include <unordered_map>
namespace fe_engine {
	class item_behavior : public ref_counted {
	public:
		item_behavior(reference<cs_class> _class, reference<assembly> core_assembly);
		void on_attach(uint64_t unit_index, uint64_t item_index);
		void on_use();
	private:
		reference<cs_class> m_class;
		reference<cs_object> m_instance;
		reference<assembly> m_core;
		std::unordered_map<std::string, reference<cs_method>> m_methods;
		void register_methods();
	};
}