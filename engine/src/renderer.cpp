#include "renderer.h"
#include "buffer.h"
#include <iostream>
#include <sstream>
#include <string>
#ifdef _WIN32
#include <Windows.h>
#endif
static void set_cursor_pos(int x, int y) {
#ifdef _WIN32
	SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), { (short)x, (short)y });
#endif
}
static void print_char(char c, fe_engine::renderer::color _c) {
#ifdef _WIN32
	HANDLE console = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(console, (WORD)_c);
	WriteConsole(console, &c, 1, NULL, NULL);
#endif
}
static void disable_console_cursor() {
#ifdef _WIN32
	CONSOLE_CURSOR_INFO info;
	info.dwSize = 20;
	info.bVisible = false;
	SetConsoleCursorInfo(GetStdHandle(STD_OUTPUT_HANDLE), &info);
#endif
}
namespace fe_engine {
	renderer::renderer() {
		this->m_buffer = new util::buffer(sizeof(char));
		this->m_color_buffer = new util::buffer(sizeof(color));
		this->m_width = this->m_height = 1;
		disable_console_cursor();
	}
	renderer::~renderer() {
		delete this->m_color_buffer;
		delete this->m_buffer;
	}
	void renderer::clear() {
		for (size_t y = 0; y < this->m_height; y++) {
			for (size_t x = 0; x < this->m_width; x++) {
				this->render_char_at(x, y, ' ', color::white);
			}
		}
		set_cursor_pos(0, 0);
	}
	void renderer::set_buffer_size(size_t width, size_t height) {
		this->m_buffer->resize(width * height * sizeof(char));
		this->m_color_buffer->resize(width * height * sizeof(color));
		this->m_width = width;
		this->m_height = height;
	}
	void renderer::get_buffer_size(size_t& width, size_t& height) const {
		width = this->m_width;
		height = this->m_height;
	}
	void renderer::present() {
		for (int y = (int)this->m_height - 1; y >= 0; y--) {
			for (size_t x = 0; x < this->m_width; x++) {
				char* buffer = util::buffer_cast<char>(this->m_buffer);
				// this is assuming that the buffers size is m_width * m_height * sizeof(char)
				int8_t c = buffer[this->get_index_for_pos(x, (size_t)y)];
				color* color_buffer = util::buffer_cast<color>(this->m_color_buffer);
				color _c = color_buffer[this->get_index_for_pos(x, (size_t)y)];
				print_char(c, _c);
			}
			print_char('\n', color::white);
		}
	}
	void renderer::render_char_at(size_t x, size_t y, char c, color _c) const {
		char* buffer = util::buffer_cast<char>(this->m_buffer);
		char& ref = buffer[this->get_index_for_pos(x, y)];
		ref = c;
		color* color_buffer = util::buffer_cast<color>(this->m_color_buffer);
		color& color_ref = color_buffer[this->get_index_for_pos(x, y)];
		color_ref = _c;
	}
	size_t renderer::get_index_for_pos(size_t x, size_t y) const {
		return (y * this->m_width) + x;
	}
	void renderer::render_string_at(size_t x, size_t y, const std::string& text, color c) const {
		for (char _c : text) {
			this->render_char_at(x++, y, _c, c);
		}
	}
}