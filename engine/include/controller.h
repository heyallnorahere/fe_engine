#pragma once
#include "reference.h"
namespace fe_engine {
	class controller : public ref_counted {
	public:
		struct button {
			bool held, down, up;
		};
		struct buttons {
			button a, b, x, y, lb, rb, ls, rs, up, down, left, right;
		};
		controller(size_t index);
		void update();
		buttons get_state() const;
		bool connected() const;
	private:
		size_t m_index;
		buttons m_last, m_current;
	};
}