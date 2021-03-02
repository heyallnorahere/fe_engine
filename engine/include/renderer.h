#pragma once
#include "reference.h"
#ifdef _WIN32
#include <Windows.h>
#endif
#ifdef max
#undef max
#endif
#include <limits>
#include <string>
namespace fe_engine {
	namespace util {
		class buffer;
	}
	class renderer : public ref_counted {
	public:
		enum class color {
			red
#ifdef _WIN32
			= FOREGROUND_RED,
#else
			= 0,
#endif
			green
#ifdef _WIN32
			= FOREGROUND_GREEN,
#else
			= 1,
#endif
			blue
#ifdef _WIN32
			= FOREGROUND_BLUE,
#else
			= 2,
#endif
			yellow
#ifdef _WIN32
			= FOREGROUND_RED | FOREGROUND_GREEN,
#else
			= 3,
#endif
			white
#ifdef _WIN32
			= FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE,
#else
			= 4,
#endif
			black
#ifdef _WIN32
			= 0,
#else
			= 5,
#endif
			none
#ifdef _WIN32
			= std::numeric_limits<int>::max(),
#else
			= 6,
#endif
		};
		enum class background_color {
			red
#ifdef _WIN32
			= BACKGROUND_RED,
#else
			= 0,
#endif
			green
#ifdef _WIN32
			= BACKGROUND_GREEN,
#else
			= 1,
#endif
			blue
#ifdef _WIN32
			= BACKGROUND_BLUE,
#else
			= 2,
#endif
			yellow
#ifdef _WIN32
			= BACKGROUND_RED | BACKGROUND_GREEN,
#else
			= 3,
#endif
			white
#ifdef _WIN32
			= BACKGROUND_RED | BACKGROUND_GREEN | BACKGROUND_BLUE,
#else
			= 4,
#endif
			black
#ifdef _WIN32
			= 0,
#else
			= 5,
#endif
			none
#ifdef _WIN32
			= std::numeric_limits<int>::max(),
#else
			= 6,
#endif
		};
		renderer();
		~renderer();
		void clear();
		void set_buffer_size(size_t width, size_t height);
		void get_buffer_size(size_t& width, size_t& height) const;
		void present();
		void render_char_at(size_t x, size_t y, char c, color _c = color::none, background_color bg = background_color::none) const;
		void render_string_at(size_t x, size_t y, const std::string& text, color c = color::none, background_color bg = background_color::none) const;
	private:
		size_t m_width, m_height;
		util::buffer* m_buffer, *m_color_buffer, *m_background_color_buffer;
		size_t get_index_for_pos(size_t x, size_t y) const;
	};
}