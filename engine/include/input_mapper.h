#pragma once

#include "reference.h"
#include "controller.h"

namespace fe_engine {
	class input_mapper : public ref_counted {
	public:
		struct commands {
			bool up, down, left, right, ok, back, exit;
		};

		input_mapper(reference<controller> controller);
		void update();
		commands get_state() const { return m_current; }

	private:
		reference<controller> m_controller;
		commands m_current;
	};
}