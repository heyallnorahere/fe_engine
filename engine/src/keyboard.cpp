#include "keyboard.h"
#include "buffer.h"
#include <cassert>
#include <memory>
#include <sstream>
#include "logger.h"
#include <chrono>
#include <future>
#include <iostream>

#ifdef FEENGINE_WINDOWS
#include <Windows.h>
#else
#include <termios.h>
#include <unistd.h>
static std::string get_from_cin() {
	char c;
	std::cin.get(c);
	return std::string(&c, 1);
}
static std::future<std::string> future;
static void make_new_future() {
	future = std::async(std::launch::async, get_from_cin);
}
#endif
namespace fe_engine {
	keyboard::keyboard() {
#ifndef FEENGINE_WINDOWS
		// todo: oh god please do not use system()
		system("/bin/stty raw");
		termios tios;
		tcgetattr(STDIN_FILENO, &tios);
		tios.c_oflag |= OPOST;
		tcsetattr(STDIN_FILENO, TCSAFLUSH, &tios);
		make_new_future();
#endif
	}
	keyboard::~keyboard() {
#ifndef FEENGINE_WINDOWS
		// see constructor
		system("/bin/stty cooked");
#endif
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
				if (event.Event.KeyEvent.bKeyDown) {
					int repeats = event.Event.KeyEvent.wRepeatCount;
					char c = event.Event.KeyEvent.uChar.AsciiChar;
					while (repeats > 0) {
						this->m_input.push_back(c);
						repeats--;
					}
				}
			}
		}
#else
		while (future.wait_for(std::chrono::milliseconds(1)) == std::future_status::ready) {
			std::string line = future.get();
			make_new_future();
			for (char c : line) {
				this->m_input.push_back(c);
			}
		}
#endif
	}

	std::vector<char> keyboard::get_input()
	{
		return this->m_input;
	}
}