#include "item.h"
namespace fe_engine {
	item::item(const std::string& name) {
		this->m_name = name;
	}
	std::string item::get_name() const {
		return this->m_name;
	}
}