#pragma once
#include "reference.h"
#include "renderer.h"
#include <list>
#include <vector>
#include "unit.h"
#include "map.h"
#include "input_mapper.h"
namespace fe_engine {
	class map : public ref_counted {
	public:
		map(size_t width, size_t height);
		void add_unit(const reference<unit>& unit);
		void update();
		void update_units(unit_affiliation affiliation, reference<input_mapper> im);
		void render(reference<renderer> r);
		size_t get_unit_count() const;
		reference<unit> get_unit(size_t index) const;
		reference<unit> get_unit_at(s8vec2 pos) const;
		std::vector<reference<unit>> get_all_units_of_affiliation(unit_affiliation affiliation);
		size_t get_width() const;
		size_t get_height() const;
	private:
		std::list<reference<unit>> m_units;
		size_t m_width, m_height;
	};
}