#pragma once
namespace fe_engine {
	// basic vec2 struct, good enough for our purposes
	template<typename T> struct vec2t {
		T x, y;
	};
	using u8vec2 = vec2t<unsigned char>;
}