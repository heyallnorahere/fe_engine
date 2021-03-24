#include "menu.h"
namespace fe_engine {
	namespace internal {
		const std::vector<menu_item>& menu::get_items() const {
			return this->m_items;
		}
		void menu::add_item(const menu_item& item) {
			this->m_items.push_back(item);
		}
	}
}