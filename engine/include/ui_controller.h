#pragma once
#include "reference.h"
#include "unit.h"
#include "renderer.h"
#include "map.h"
#include "controller.h"
namespace fe_engine {
	class ui_controller : public ref_counted {
	public:
		ui_controller(reference<renderer> r, reference<map> m, reference<controller> c);
		void set_info_panel_target(reference<unit> u);
		void set_unit_menu_target(reference<unit> u);
		void update();
		void render();
		reference<unit> get_unit_menu_target() const;
	private:
		reference<renderer> m_renderer;
		reference<map> m_map;
		reference<controller> m_controller;
		reference<unit> m_info_panel_target, m_unit_menu_target;
		size_t m_unit_menu_index;
		void close_unit_menu();
		void render_frame(size_t info_panel_width, size_t unit_menu_width, size_t map_width, size_t map_height);
		void render_info_panel(size_t origin_x, size_t origin_y, size_t width, size_t height);
		void render_unit_menu(size_t origin_x, size_t origin_y, size_t width, size_t height);
	};
}