#pragma once
#include <cstdint>
namespace fe_engine {
	class random_number_generator {
	public:
		static void seed(uint32_t seed);
		static uint32_t generate();
		static uint32_t generate(uint32_t min, uint32_t max);
	};
}