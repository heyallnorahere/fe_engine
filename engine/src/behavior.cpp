#include "behavior.h"
namespace fe_engine {
	behavior::behavior(reference<cs_class> _class, reference<assembly> core_assembly) {
		this->m_class = _class;
		this->m_core = core_assembly;
		this->register_methods();
		this->m_instance = this->m_class->instantiate();
	}
	void behavior::on_attach(uint64_t index) {
		reference<cs_class> unit_class = this->m_core->get_class("FEEngine", "Unit");
		reference<cs_method> unit_creation_method = unit_class->get_method("FEEngine.Unit:MakeFromIndex(ulong)");
		void* args[1];
		args[0] = &index;
		reference<cs_object> unit_object = reference<cs_object>(cs_method::call_function(unit_creation_method, args));
		reference<cs_field> parent_field = this->m_class->get_field("parent");
		this->m_instance->set_field(parent_field, unit_object->raw());
		this->m_instance->call_method(this->m_methods["on_attach"]);
	}
	void behavior::on_unit_update() {
		this->m_instance->call_method(this->m_methods["on_update"]);
	}
	void behavior::on_detach() {
		this->m_instance->call_method(this->m_methods["on_detach"]);
	}
	void behavior::register_methods() {
		std::string prefix = this->m_class->get_full_name();
		this->m_methods["on_attach"] = this->m_class->get_method(prefix + ":OnAttach()");
		this->m_methods["on_update"] = this->m_class->get_method(prefix + ":OnUpdate()");
		this->m_methods["on_detach"] = this->m_class->get_method(prefix + ":OnDetach()");
	}
}