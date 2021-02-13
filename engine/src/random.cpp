#include "_random.h"
#include <random>
namespace fe_engine {
	void random_number_generator::seed(uint32_t seed) {
		std::srand(seed);
	}
	uint32_t random_number_generator::generate() {
		return std::rand();
	}
	uint32_t random_number_generator::generate(uint32_t min, uint32_t max) {
		return (generate() % (max - min + 1)) + min;
	}
}