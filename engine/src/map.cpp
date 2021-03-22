#include "map.h"
#include <iostream>
#include "renderer.h"
#ifdef max
#undef max
#endif
#include <limits>
#include <cassert>
#include "object_register.h"
namespace fe_engine {
	map::map(size_t width, size_t height) {
		this->m_width = width;
		this->m_height = height;
	}
	void map::add_unit(const reference<unit>& unit) {
		auto unit_register = object_registry::get_register<::fe_engine::unit>();
		this->m_units.push_back(unit_register->add(unit));
	}
	void map::update() {
		if (this->m_tiles.size() != this->m_width * this->m_height) {
			for (int8_t x = 0; (int8_t)x < this->m_width; x++) {
				for (int8_t y = 0; (int8_t)y < this->m_height; y++) {
					if (this->m_tiles.find({ x, y }) == this->m_tiles.end()) {
						this->m_tiles[{ x, y }] = reference<tile>::create(tile::passing_properties{ true });
					}
				}
			}
		}
		auto unit_register = object_registry::get_register<unit>();
		this->m_units.remove_if([&](size_t index) {
			auto u = unit_register->get(index);
			bool remove = u->get_current_hp() <= 0;
			if (remove) {
				u->attach_behavior(reference<behavior>(), 0);
			}
			return remove;
		});
		for (size_t index : this->m_units) {
			auto u = unit_register->get(index);
			if (!u->initialized()) {
				u->init();
			}
			u->update();
		}
	}
	void map::update_units(unit_affiliation affiliation, reference<input_mapper> im) {
		auto unit_register = object_registry::get_register<unit>();
		for (size_t index : this->m_units) {
			auto u = unit_register->get(index);
			if (u->get_affiliation() == affiliation) {
				u->unit_update(im);
			}
		}
	}
	void map::render(reference<renderer> r) {
		size_t width, height;
		r->get_buffer_size(width, height);
		for (size_t x = 0; x < this->m_width; x++) {
			for (size_t y = 0; y < this->m_height; y++) {
				reference<tile> tile = this->m_tiles[{ (int8_t)x, (int8_t)y }];
				r->render_char_at(x, y + (height - this->m_height), ' ', renderer::color::none, tile->get_color());
			}
		}
		auto unit_register = object_registry::get_register<unit>();
		auto item_register = object_registry::get_register<item>();
		for (size_t index : this->m_units) {
			auto u = unit_register->get(index);
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
			if (u->get_equipped_weapon() != (size_t)-1) {
				type = reference<weapon>(item_register->get(u->get_equipped_weapon()))->get_type();
			}
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
		auto unit_register = object_registry::get_register<unit>();
		return unit_register->get(*it);
	}
	size_t map::get_unit_register_index(size_t index) const {
		auto it = this->m_units.begin();
		std::advance(it, index);
		return *it;
	}
	reference<unit> map::get_unit_at(s8vec2 pos) const {
		auto unit_register = object_registry::get_register<unit>();
		for (size_t index : this->m_units) {
			auto u = unit_register->get(index);
			if (u->get_pos() == pos) {
				return u;
			}
		}
		return reference<unit>();
	}
	reference<tile> map::get_tile(s8vec2 pos) {
		return this->m_tiles[pos];
	}
	void map::set_tile(s8vec2 pos, reference<tile> tile) {
		this->m_tiles[pos] = tile;
	}
	std::vector<reference<unit>> map::get_all_units_of_affiliation(unit_affiliation affiliation) {
		std::vector<reference<unit>> units;
		auto unit_register = object_registry::get_register<unit>();
		for (size_t index : this->m_units) {
			auto u = unit_register->get(index);
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