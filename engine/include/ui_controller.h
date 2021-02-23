#pragma once
#include "reference.h"
#include "unit.h"
#include "renderer.h"
#include "map.h"
#include "controller.h"
#include "input_mapper.h"
#include <string>
#include <vector>
#include <functional>
namespace fe_engine {
	class ui_controller : public ref_counted {
	public:
		ui_controller(reference<renderer> r, reference<map> m, reference<input_mapper> im);
		void set_info_panel_target(reference<unit> u);
		void set_unit_menu_target(reference<unit> u, s8vec2 original_position);
		void update();
		void render();
		reference<unit> get_unit_menu_target() const;
		void set_can_close(bool c);
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
		std::vector<unit_menu_item> m_menu_items;
		menu_state m_unit_menu_state;
		bool m_can_close;
		size_t m_unit_menu_index;
		void close_unit_menu();
		void render_frame(size_t info_panel_width, size_t unit_menu_width, size_t map_width, size_t map_height);
		void render_info_panel(size_t origin_x, size_t origin_y, size_t width, size_t height);
		void render_unit_menu(size_t origin_x, size_t origin_y, size_t width, size_t height);
		std::vector<unit_menu_item> generate_menu_items(reference<item> i);
		std::vector<reference<unit>> get_attackable_units(reference<unit> u);
		void refresh_base_menu_items();
	};
}