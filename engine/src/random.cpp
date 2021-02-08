#include "_random.h"
#include <random>
#include "..\include\_random.h"
namespace fe_engine {
	void random_number_generator::seed(unsigned int seed) {
		std::srand(seed);
	}
	unsigned int random_number_generator::generate() {
		return std::rand();
	}
	unsigned int random_number_generator::generate(unsigned int min, unsigned int max) {
		return (generate() % (max - min + 1)) + min;
	}
}