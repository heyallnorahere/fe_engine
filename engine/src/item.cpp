#include "item.h"
namespace fe_engine {
	item::item(const std::string& name, uint32_t flags, on_use_proc on_use) {
		this->m_name = name;
		this->m_flags = flags;
		this->m_on_use = on_use;
	}
	std::string item::get_name() const {
		return this->m_name;
	}
	uint32_t item::get_item_flags() const {
		return this->m_flags;
	}
	item::on_use_proc item::get_on_use_proc() const {
		return this->m_on_use;
	}
}