#pragma once
#include "reference.h"
#include "unit.h"
#include "renderer.h"
#include "map.h"
#include "controller.h"
#include "input_mapper.h"
#include "mono_classes.h"
#include <string>
#include <vector>
#include <functional>
#include <deque>
namespace fe_engine {
	namespace internal {
		struct menu_spec;
		class close_ui_controller_unit_menu;
	}
	class ui_controller : public ref_counted {
	public:
		struct user_menu {
			size_t register_index;
			std::string menu_item_name;
		};
		ui_controller(reference<renderer> r, reference<map> m, reference<input_mapper> im);
		void set_info_panel_target(reference<unit> u);
		void set_unit_menu_target(reference<unit> u, s8vec2 original_position);
		void set_core_assembly(reference<assembly> core);
		void update();
		void render();
		reference<unit> get_unit_menu_target() const;
		void set_can_close(bool c);
		void add_user_menu(const user_menu& menu);
		const std::vector<user_menu>& get_user_menus() const;
	private:
		struct unit_menu_item {
			std::string text;
			std::function<void(reference<ui_controller>)> on_select;
		};
		enum class menu_page {
			base,
			item,
			item_select,
			enemy_select,
			user_menu,
		};
		struct menu_state {
			menu_page page;
			reference<item> selected_item;
			s8vec2 original_position;
		};
		reference<renderer> m_renderer;
		reference<map> m_map;
		reference<input_mapper> m_imapper;
		reference<unit> m_info_panel_target, m_unit_menu_target;
		reference<assembly> m_core;
		std::vector<unit_menu_item> m_menu_items;
		menu_state m_unit_menu_state;
		std::vector<user_menu> m_user_menus;
		std::deque<size_t> m_user_menu_queue;
		bool m_can_close;
		size_t m_unit_menu_index;
		void close_unit_menu();
		void render_frame(size_t info_panel_width, size_t unit_menu_width, size_t map_width, size_t map_height, size_t log_height);
		void render_info_panel(size_t origin_x, size_t origin_y, size_t width, size_t height);
		void render_unit_menu(size_t origin_x, size_t origin_y, size_t width, size_t height);
		void render_log(size_t origin_x, size_t origin_y, size_t width, size_t height);
		std::vector<unit_menu_item> generate_menu_items(reference<item> i);
		std::vector<reference<unit>> get_attackable_units(reference<unit> u);
		void refresh_base_menu_items();
		friend class internal::close_ui_controller_unit_menu;
	};
}