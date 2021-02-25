#include "keyboard.h"
#include "buffer.h"
#include <cassert>
#include <memory>

#ifdef FEENGINE_WINDOWS
#include <Windows.h>
#endif

namespace fe_engine {
	keyboard::keyboard() {
	}

	void keyboard::update()
	{
		this->m_input.clear();
#ifdef FEENGINE_WINDOWS
		HANDLE stdinput = GetStdHandle(STD_INPUT_HANDLE);
		DWORD number_of_events, events_read;
		assert(GetNumberOfConsoleInputEvents(stdinput, &number_of_events));
		std::unique_ptr<util::buffer> buf(new util::buffer(number_of_events * sizeof(INPUT_RECORD)));
		assert(ReadConsoleInputA(stdinput, util::buffer_cast<INPUT_RECORD>(buf.get()), number_of_events, &events_read));
		for (int i = 0; i < events_read; i++) {
			INPUT_RECORD* ptr = util::buffer_cast<INPUT_RECORD>(buf.get());
			INPUT_RECORD event = ptr[i];
			if (event.EventType == KEY_EVENT) {
				if ((event.Event.KeyEvent.wRepeatCount == 1) && event.Event.KeyEvent.bKeyDown) {
					char c = event.Event.KeyEvent.uChar.AsciiChar;
					this->m_input.push_back(c);
				}
			}
		}
#endif
	}

	std::vector<char> keyboard::get_input()
	{
		return this->m_input;
	}
}