#include "item_behavior.h"
#include <vector>
namespace fe_engine {
	item_behavior::item_behavior(reference<cs_class> _class, reference<assembly> core_assembly) {
		this->m_class = _class;
		this->m_core = core_assembly;
		this->register_methods();
		this->m_instance = this->m_class->instantiate();
	}
	void item_behavior::on_attach(uint64_t unit_index, uint64_t item_index) {
		reference<cs_class> item_class = this->m_core->get_class("FEEngine", "Item");
		reference<cs_method> item_creation_method = item_class->get_method("FEEngine.Item:MakeFromInventoryIndex(Unit,ulong)");
		reference<cs_class> unit_class = this->m_core->get_class("FEEngine", "Unit");
		reference<cs_method> unit_creation_method = unit_class->get_method("FEEngine.Unit:MakeFromIndex(ulong)");
		std::vector<void*> args(1);
		args[0] = &unit_index;
		reference<cs_object> unit_object = reference<cs_object>(cs_method::call_function(unit_creation_method, args.data()));
		args[0] = unit_object->raw();
		args.push_back(&item_index);
		reference<cs_object> item_object = reference<cs_object>(cs_method::call_function(item_creation_method, args.data()));
		reference<cs_field> parent_field = this->m_class->get_field("parent");
		this->m_instance->set_field(parent_field, item_object->raw());
		reference<cs_method> method = this->m_methods["on_attach"];
		if (method->raw()) {
			this->m_instance->call_method(method);
		}
	}
	void item_behavior::on_use() {
		reference<cs_method> method = this->m_methods["on_use"];
		if (method->raw()) {
			this->m_instance->call_method(method);
		}
	}
	void item_behavior::register_methods() {
		std::string prefix = this->m_class->get_full_name();
		this->m_methods["on_attach"] = this->m_class->get_method(prefix + ":OnAttach()");
		this->m_methods["on_use"] = this->m_class->get_method(prefix + ":OnUse()");
	}
}