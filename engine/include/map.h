#pragma once
#include "reference.h"
#include "renderer.h"
#include <list>
#include <vector>
#include <unordered_map>
#include "unit.h"
#include "tile.h"
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
		reference<tile> get_tile(s8vec2 pos);
		void set_tile(s8vec2 pos, reference<tile> tile);
		std::vector<reference<unit>> get_all_units_of_affiliation(unit_affiliation affiliation);
		size_t get_width() const;
		size_t get_height() const;
	private:
		
		std::list<reference<unit>> m_units;
		std::unordered_map<s8vec2, reference<tile>, hash_vec2t<int8_t>> m_tiles;
		size_t m_width, m_height;
	};
}