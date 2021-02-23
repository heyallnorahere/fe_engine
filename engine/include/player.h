#pragma once
#include "reference.h"
#include "controller.h"
#include "input_mapper.h"
#include "_math.h"
#include "map.h"
#include "renderer.h"
#include "unit.h"
#include "ui_controller.h"
#include "phase_manager.h"
namespace fe_engine {
	class player : public ref_counted {
	public:
		player(reference<input_mapper> im, reference<map> m, reference<ui_controller> uc, reference<phase_manager> pm);
		void update();
		void render_cursor(const reference<renderer>& r);
	private:
		reference<input_mapper> m_imapper;
		reference<map> m_map;
		reference<ui_controller> m_ui_controller;
		reference<unit> m_selected;
		reference<phase_manager> m_phase_manager;
		s8vec2 m_cursor_pos;
	};
}