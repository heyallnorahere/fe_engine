#pragma once
namespace fe_engine {
	namespace util {
		class buffer {
		public:
			buffer(size_t size);
			~buffer();
			void resize(size_t size);
			// DO NOT keep this pointer, it is freed on resize
			void* get();
			size_t get_size();
			void zero();
		private:
			void* m_buffer;
			size_t m_size;
		};
		template<typename T> inline T* buffer_cast(buffer* buf) {
			return (T*)buf->get();
		}
	}
}