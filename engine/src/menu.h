#pragma once
#include "reference.h"
#include <vector>
#include <string>
#include <functional>
#include "ui_controller.h"
#include "mono_classes.h"
namespace fe_engine {
	namespace internal {
		enum class menu_item_type {
			submenu,
			action,
			back,
			no_action,
		};
		struct menu_item {
			std::string name;
			menu_item_type type;
			reference<cs_delegate> action;
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