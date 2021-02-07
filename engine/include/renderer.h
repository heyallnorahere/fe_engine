#pragma once
#include "reference.h"
namespace fe_engine {
	namespace util {
		class buffer;
	}
	class renderer : public ref_counted {
	public:
		renderer();
		~renderer();
		void clear();
		void set_buffer_size(size_t width, size_t height);
		void get_buffer_size(size_t& width, size_t& height) const;
		void present();
		void render_char_at(size_t x, size_t y, char c) const;
	private:
		size_t m_width, m_height;
		util::buffer* m_buffer;
		size_t get_index_for_pos(size_t x, size_t y) const;
	};
}