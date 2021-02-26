#include "map.h"
#include <iostream>
#include "renderer.h"
#ifdef max
#undef max
#endif
#include <limits>
#include <cassert>
namespace fe_engine {
	static void update_next_units(std::list<reference<unit>>& units, reference<unit> u) {
		size_t index = std::numeric_limits<size_t>::max();
		size_t i = 0;
		for (auto _u : units) {
			if (_u.get() == u.get()) {
				index = i;
				break;
			}
			i++;
		}
		assert(index != std::numeric_limits<size_t>::max());
		i = 0;
		for (auto _u : units) {
			if (i > index) {
				_u->update_index();
			}
			i++;
		}
	}
	map::map(size_t width, size_t height) {
		this->m_width = width;
		this->m_height = height;
	}
	void map::add_unit(const reference<unit>& unit) {
		this->m_units.push_back(unit);
	}
	void map::update() {
		this->m_units.remove_if([&](reference<unit> u) {
			bool remove = u->get_current_hp() <= 0;
			if (remove) {
				u->attach_behavior(reference<behavior>(), 0);
				update_next_units(this->m_units, u);
			}
			return remove;
		});
		for (auto& u : this->m_units) {
			if (!u->initialized()) {
				u->init();
			}
			u->update();
		}
	}
	void map::update_units(unit_affiliation affiliation, reference<input_mapper> im) {
		for (auto& u : this->m_units) {
			if (u->get_affiliation() == affiliation) {
				u->unit_update(im);
			}
		}
	}
	void map::render(reference<renderer> r) {
		for (auto& u : this->m_units) {
			renderer::color color = renderer::color::white;
			switch (u->get_affiliation()) {
			case unit_affiliation::enemy:
				color = renderer::color::red;
				break;
			case unit_affiliation::ally:
				color = renderer::color::green;
				break;
			case unit_affiliation::player:
				color = renderer::color::blue;
				break;
			case unit_affiliation::separate_enemy:
				color = renderer::color::yellow;
				break;
			}
			weapon::type type = weapon::type::fists;
			if (u->get_equipped_weapon()) {
				type = u->get_equipped_weapon()->get_type();
			}
			size_t width, height;
			r->get_buffer_size(width, height);
			r->render_char_at(u->get_pos().x, u->get_pos().y + (height - this->m_height), weapon::get_char_from_type(type), color);
			reference<behavior> b = u->get_behavior();
			if (b) b->on_render(r);
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
	std::vector<reference<unit>> map::get_all_units_of_affiliation(unit_affiliation affiliation) {
		std::vector<reference<unit>> units;
		for (reference<unit> u : this->m_units) {
			if (u->get_affiliation() == affiliation) {
				units.push_back(u);
			}
		}
		return units;
	}
	size_t map::get_width() const {
		return this->m_width;
	}
	size_t map::get_height() const {
		return this->m_height;
	}
}