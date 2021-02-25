#include "input_mapper.h"
#include <memory>
#include "script_wrappers.h"

namespace fe_engine {
	input_mapper::input_mapper(reference<controller> controller) {
		m_controller = controller;
		script_wrappers::set_imapper(reference<input_mapper>(this));
	}
	void input_mapper::update()
	{
		m_current = commands({ 0 });
		if (m_controller) {
			m_controller->update();
			auto buttons = m_controller->get_state();

			if (buttons.up.down) {
				m_current.up = true;
			}
			if (buttons.down.down) {
				m_current.down = true;
			}
			if (buttons.left.down) {
				m_current.left = true;
			}
			if (buttons.right.down) {
				m_current.right = true;
			}
			if (buttons.a.down) {
				m_current.ok = true;
			}
			if (buttons.b.down) {
				m_current.back = true;
			}
			if (buttons.start.down) {
				m_current.exit = true;
			}
		}
	}
}