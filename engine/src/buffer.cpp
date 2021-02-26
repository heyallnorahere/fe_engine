#include "buffer.h"
#include <cstdlib>
#include <memory>
#include <cstring>
namespace fe_engine {
	namespace util {
		buffer::buffer(size_t size) {
			this->m_buffer = malloc(size);
			this->m_size = size;
		}
		buffer::~buffer() {
			free(this->m_buffer);
		}
		void buffer::resize(size_t size) {
			this->m_buffer = realloc(this->m_buffer, size);
			this->m_size = size;
		}
		void* buffer::get() {
			return this->m_buffer;
		}
		size_t buffer::get_size() {
			return this->m_size;
		}
		void buffer::zero() {
			memset(this->m_buffer, 0, this->m_size);
		}
	}
}