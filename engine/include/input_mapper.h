#pragma once

#include "reference.h"
#include "controller.h"
#include "keyboard.h"
#include <unordered_map>
#include <string>

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
		void load_mappings_from_json(const std::string& path);

		reference<controller> m_controller;
		reference<keyboard> m_keyboard;
		std::unordered_map<std::string, std::string> m_controller_mappings;
		std::unordered_map<std::string, char> m_keyboard_mappings;
		commands m_current;
	};
}