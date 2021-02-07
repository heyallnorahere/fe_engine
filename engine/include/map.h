#pragma once
#include "reference.h"
#include "renderer.h"
#include <list>
#include "unit.h"
#include "map.h"
namespace fe_engine {
	class map : public ref_counted {
	public:
		map(size_t width, size_t height);
		void add_unit(const reference<unit>& unit);
		void update();
		void render(const reference<renderer>& r);
		size_t get_unit_count() const;
		reference<unit> get_unit(size_t index) const;
		reference<unit> get_unit_at(s8vec2 pos) const;
	private:
		std::list<reference<unit>> m_units;
		size_t m_width, m_height;
	};
}