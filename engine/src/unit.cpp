#include "unit.h"
namespace fe_engine {
	unit::unit(const unit_stats& stats, u8vec2 pos) {
		this->m_stats = stats;
		this->m_pos = pos;
		this->m_hp = this->m_stats.max_hp;
	}
	unit::~unit() {
		// todo: delete
	}
	const unit::unit_stats& unit::get_stats() const {
		return this->m_stats;
	}
	u8vec2 unit::get_pos() const {
		return this->m_pos;
	}
	unit::unit_stats::stat_type unit::get_current_hp() const {
		return this->m_hp;
	}
	void unit::update() {
		// todo: update
	}
}