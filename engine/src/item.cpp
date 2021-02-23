#include "item.h"
namespace fe_engine {
	item::item(const std::string& name, uint32_t flags, reference<item_behavior> itembehavior) {
		this->m_name = name;
		this->m_flags = flags;
		this->m_behavior = itembehavior;
		this->m_initialized = false;
		this->m_used = false;
	}
	std::string item::get_name() const {
		return this->m_name;
	}
	void item::set_name(const std::string& name) {
		this->m_name = name;
	}
	uint32_t item::get_item_flags() const {
		return this->m_flags;
	}
	bool item::used() {
		return this->m_used;
	}
	void item::set_used(bool used) {
		this->m_used = used;
	}
	bool item::initialized() {
		return this->m_initialized;
	}
	void item::init(uint64_t index, uint64_t parent_index) {
		if (this->m_behavior) {
			this->m_behavior->on_attach(parent_index, index);
		}
		this->m_initialized = true;
	}
	reference<item_behavior> item::get_behavior() {
		return this->m_behavior;
	}
}