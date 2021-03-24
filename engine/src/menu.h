#pragma once
#include "reference.h"
#include <vector>
#include <string>
#include <functional>
#include "ui_controller.h"
namespace fe_engine {
	namespace internal {
		enum class menu_item_type {
			submenu,
			action,
		};
		struct menu_item {
			std::string name;
			menu_item_type type;
			std::string action;
			size_t submenu;
		};
		class menu : public ref_counted {
		public:
			const std::vector<menu_item>& get_items() const;
			void add_item(const menu_item& item);
		private:
			std::vector<menu_item> m_items;
		};
	}
}