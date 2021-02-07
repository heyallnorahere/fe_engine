#include "map.h"
#include <iostream>
#include "renderer.h"
namespace fe_engine {
	map::map(size_t width, size_t height) {
		this->m_width = width;
		this->m_height = height;
	}
	void map::add_unit(const reference<unit>& unit) {
		this->m_units.push_back(unit);
	}
	void map::update() {
		for (auto& u : this->m_units) {
			u->update();
		}
		this->m_units.remove_if([](const reference<unit>& u) { return u->get_current_hp() == 0; });
	}
	void map::render(const reference<renderer>& r) {
		for (const auto& u : this->m_units) {
			r->render_char_at(u->get_pos().x, u->get_pos().y, 'u');
		}
	}
	size_t map::get_unit_count() const {
		return this->m_units.size();
	}
	reference<unit> map::get_unit(size_t index) const {
		auto it = this->m_units.begin();
		std::advance(it, index);
		return *it;
	}
	reference<unit> map::get_unit_at(s8vec2 pos) const {
		for (const auto& u : this->m_units) {
			if (u->get_pos() == pos) {
				return u;
			}
		}
		return reference<unit>();
	}
}