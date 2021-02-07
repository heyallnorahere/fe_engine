#include "unit.h"
namespace fe_engine {
	unit::unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation) {
		this->m_stats = stats;
		this->m_pos = pos;
		this->m_hp = this->m_stats.max_hp;
		this->m_affiliation = affiliation;
	}
	unit::~unit() {
		// todo: delete
	}
	const unit::unit_stats& unit::get_stats() const {
		return this->m_stats;
	}
	s8vec2 unit::get_pos() const {
		return this->m_pos;
	}
	unit::unit_stats::stat_type unit::get_current_hp() const {
		return this->m_hp;
	}
	unit_affiliation unit::get_affiliation() const {
		return this->m_affiliation;
	}
	void unit::update() {
		// todo: update
	}
	void unit::move(s8vec2 offset) {
		s8vec2 to_move = offset;
		if ((unit_stats::stat_type)to_move.length() > this->m_stats.move) {
			to_move = to_move.normalize() * this->m_stats.move;
		}
		this->m_pos += to_move;
	}
	reference<weapon> unit::get_equipped_weapon() const {
		return this->m_equipped_weapon;
	}
	void unit::set_equipped_weapon(const reference<weapon>& w) {
		this->m_equipped_weapon = w;
	}
}