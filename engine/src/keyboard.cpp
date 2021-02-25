#include "keyboard.h"

#ifdef FEENGINE_WINDOWS
#include <conio.h>
#endif

namespace fe_engine {
	keyboard::keyboard() {
	}

	void keyboard::update()
	{
		m_input.clear();
#ifdef FEENGINE_WINDOWS
		// TODO: Switch to Peek/ReadConsoleInput on windows as we can 
		// do a lot more with those. This is simpler but deprecated.
		while (_kbhit()) {
			auto ch = _getch();
			this->m_input.push_back(ch);
		}
#endif
	}

	std::vector<char> keyboard::get_input()
	{
		return this->m_input;
	}
}