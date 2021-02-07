#pragma once
#include "reference.h"
#include "controller.h"
#include "_math.h"
#include "map.h"
#include "renderer.h"
#include "unit.h"
namespace fe_engine {
	class player : public ref_counted {
	public:
		player(reference<controller> c, reference<map> m);
		void update();
		void render_cursor(const reference<renderer>& r);
	private:
		reference<controller> m_controller;
		reference<map> m_map;
		reference<unit> m_selected;
		s8vec2 m_cursor_pos;
	};
}