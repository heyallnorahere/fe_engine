#include "behavior.h"
#include <vector>
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
		reference<cs_method> method = this->m_methods["on_attach"];
		if (method->raw()) {
			this->m_instance->call_method(method);
		}
	}
	void behavior::on_unit_update() {
		reference<cs_method> method = this->m_methods["on_update"];
		if (method->raw()) {
			this->m_instance->call_method(method);
		}
	}
	void behavior::on_render(reference<renderer> r) {
		reference<cs_method> method = this->m_methods["on_render"];
		if (method->raw()) {
			std::vector<void*> args(1);
			uint64_t renderer_address = (uint64_t)r.get();
			args[0] = &renderer_address;
			reference<cs_object> r = cs_method::call_function(this->m_renderer_creation_method, args.data());
			args[0] = r->raw();
			this->m_instance->call_method(method, args.data());
		}
	}
	void behavior::register_methods() {
		std::string prefix = this->m_class->get_full_name();
		this->m_methods["on_attach"] = this->m_class->get_method(prefix + ":OnAttach()");
		this->m_methods["on_update"] = this->m_class->get_method(prefix + ":OnUpdate()");
		this->m_methods["on_render"] = this->m_class->get_method(prefix + ":OnRender(Renderer)");
		reference<cs_class> renderer_class = this->m_core->get_class("FEEngine", "Renderer");
		this->m_renderer_creation_method = renderer_class->get_method("FEEngine.Renderer:MakeFromMemoryAddress(ulong)");
	}
}