#include "keyboard.h"

#include "conio.h"

namespace fe_engine {
	keyboard::keyboard() {
	}

	void keyboard::update()
	{
		m_input.clear();
		// TODO: Switch to Peek/ReadConsoleInput on windows as we can 
		// do a lot more with those. This is simpler but deprecated.
		while (_kbhit()) {
			auto ch = _getch();
			m_input.push_back(ch);
		}
	}

	std::vector<char> keyboard::get_input()
	{
		return m_input;
	}
}