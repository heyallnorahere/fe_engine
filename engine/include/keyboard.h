#pragma once

#include "reference.h"

#include <vector>

namespace fe_engine {

	class keyboard : public ref_counted {
	public:
		keyboard();
		~keyboard();
		void update();
		std::vector<char> get_input();

	private:
		std::vector<char> m_input;
	};

}
