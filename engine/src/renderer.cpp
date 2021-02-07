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
namespace fe_engine {
	renderer::renderer() {
		this->m_buffer = new util::buffer(sizeof(char));
		this->m_width = this->m_height = 1;
	}
	renderer::~renderer() {
		delete this->m_buffer;
	}
	void renderer::clear() {
		for (size_t y = 0; y < this->m_height; y++) {
			for (size_t x = 0; x < this->m_width; x++) {
				this->render_char_at(x, y, ' ');
			}
		}
		set_cursor_pos(0, 0);
	}
	void renderer::set_buffer_size(size_t width, size_t height) {
		this->m_buffer->resize(width * height * sizeof(char));
		this->m_width = width;
		this->m_height = height;
	}
	void renderer::get_buffer_size(size_t& width, size_t& height) const {
		width = this->m_width;
		height = this->m_height;
	}
	void renderer::present() {
		std::stringstream ss;
		for (int y = (int)this->m_height - 1; y >= 0; y--) {
			for (size_t x = 0; x < this->m_width; x++) {
				char* buffer = util::buffer_cast<char>(this->m_buffer);
				// this is assuming that the buffers size is m_width * m_height * sizeof(char)
				char c = buffer[this->get_index_for_pos(x, (size_t)y)];
				ss << c;
			}
			ss << '\n';
		}
		std::cout << ss.str() << std::flush;
	}
	void renderer::render_char_at(size_t x, size_t y, char c) const {
		char* buffer = util::buffer_cast<char>(this->m_buffer);
		char& ref = buffer[this->get_index_for_pos(x, y)];
		ref = c;
	}
	size_t renderer::get_index_for_pos(size_t x, size_t y) const {
		return (y * this->m_width) + x;
	}
}