#include "renderer.h"
#include "buffer.h"
#include <iostream>
#include <sstream>
#include <string>
#include <unordered_map>
#ifdef FEENGINE_WINDOWS
#include <Windows.h>
#else
using color_map = std::unordered_map<fe_engine::renderer::color, std::string>;
using background_color_map = std::unordered_map<fe_engine::renderer::background_color, std::string>;
static color_map gen_color_map() {
	color_map cm;
	using color = fe_engine::renderer::color;
	cm[color::red] = "\033[31m";
	cm[color::green] = "\033[32m";
	cm[color::blue] = "\033[34m";
	cm[color::yellow] = "\033[33m";
	cm[color::white] = "\033[37m";
	cm[color::black] = "\033[30m";
	return cm;
}
static background_color_map gen_bg_color_map() {
	background_color_map bcm;
	using color = fe_engine::renderer::background_color;
	bcm[color::red] = "\033[41m";
	bcm[color::green] = "\033[42m";
	bcm[color::blue] = "\033[44m";
	bcm[color::yellow] = "\033[43m";
	bcm[color::white] = "\033[47m";
	bcm[color::black] = "\033[40m";
	return bcm;
}
color_map _color_map = gen_color_map();
background_color_map bg_color_map = gen_bg_color_map();
#endif
static void set_cursor_pos(int x, int y) {
#ifdef FEENGINE_WINDOWS
	SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), { (short)x, (short)y });
#else
	std::cout << "\033[" << x << ";" << y << "H" << std::flush;
#endif
}
static void print_char(char c, fe_engine::renderer::color _c, fe_engine::renderer::background_color bg) {
#ifdef FEENGINE_WINDOWS
	HANDLE console = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(console, (WORD)_c | (WORD)bg);
	WriteConsoleA(console, &c, 1, NULL, NULL);
#else
	if (_c != fe_engine::renderer::color::none) {
		std::cout << _color_map[_c];
	}
	if (bg != fe_engine::renderer::background_color::none) {
		std::cout << bg_color_map[bg];
	}
	std::cout << c << std::flush;
#endif
}
static void disable_console_cursor() {
#ifdef FEENGINE_WINDOWS
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
		this->m_background_color_buffer = new util::buffer(sizeof(background_color));
		this->m_width = this->m_height = 1;
		disable_console_cursor();
	}
	renderer::~renderer() {
		delete this->m_background_color_buffer;
		delete this->m_color_buffer;
		delete this->m_buffer;
	}
	void renderer::clear() {
		for (size_t y = 0; y < this->m_height; y++) {
			for (size_t x = 0; x < this->m_width; x++) {
				this->render_char_at(x, y, ' ', color::white, background_color::black);
			}
		}
		set_cursor_pos(0, 0);
	}
	void renderer::set_buffer_size(size_t width, size_t height) {
		this->m_buffer->resize(width * height * sizeof(char));
		this->m_color_buffer->resize(width * height * sizeof(color));
		this->m_background_color_buffer->resize(width * height * sizeof(background_color));
		this->m_width = width;
		this->m_height = height;
	}
	void renderer::get_buffer_size(size_t& width, size_t& height) const {
		width = this->m_width;
		height = this->m_height;
	}
	void renderer::present() {
		for (size_t y = 0; y < this->m_height; y++) {
			for (size_t x = 0; x < this->m_width; x++) {
				size_t _y = this->m_height - (y + 1);
				char* buffer = util::buffer_cast<char>(this->m_buffer);
				// this is assuming that the buffers size is m_width * m_height * sizeof(char)
				int8_t c = buffer[this->get_index_for_pos(x, _y)];
				color* color_buffer = util::buffer_cast<color>(this->m_color_buffer);
				color _c = color_buffer[this->get_index_for_pos(x, _y)];
				background_color* bg_color_buffer = util::buffer_cast<background_color>(this->m_background_color_buffer);
				background_color bg = bg_color_buffer[this->get_index_for_pos(x, _y)];
				print_char(c, _c, bg);
			}
			print_char('\n', color::white, background_color::black);
		}
	}
	void renderer::render_char_at(size_t x, size_t y, char c, color _c, background_color bg) const {
		char* buffer = util::buffer_cast<char>(this->m_buffer);
		char& ref = buffer[this->get_index_for_pos(x, y)];
		ref = c;
		if (_c != color::none) {
			color* color_buffer = util::buffer_cast<color>(this->m_color_buffer);
			color& color_ref = color_buffer[this->get_index_for_pos(x, y)];
			color_ref = _c;
		}
		if (bg != background_color::none) {
			background_color* bg_color_buffer = util::buffer_cast<background_color>(this->m_background_color_buffer);
			background_color& bg_color_ref = bg_color_buffer[this->get_index_for_pos(x, y)];
			bg_color_ref = bg;
		}
	}
	size_t renderer::get_index_for_pos(size_t x, size_t y) const {
		return (y * this->m_width) + x;
	}
	void renderer::render_string_at(size_t x, size_t y, const std::string& text, color c, background_color bg) const {
		for (char _c : text) {
			this->render_char_at(x++, y, _c, c, bg);
		}
	}
}