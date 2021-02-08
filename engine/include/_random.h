#pragma once
namespace fe_engine {
	class random_number_generator {
	public:
		static void seed(unsigned int seed);
		static unsigned int generate();
		static unsigned int generate(unsigned int min, unsigned int max);
	};
}