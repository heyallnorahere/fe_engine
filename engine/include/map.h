#pragma once
#include "reference.h"
#include "renderer.h"
#include <list>
#include "unit.h"
namespace fe_engine {
	class map : public ref_counted {
	public:
		map(size_t width, size_t height);
		void add_unit(const reference<unit>& unit);
		void update();
		void render(const reference<renderer>& r);
	private:
		std::list<reference<unit>> m_units;
		size_t m_width, m_height;
	};
}