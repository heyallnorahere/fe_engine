#include "unit.h"
namespace fe_engine {
	unit::unit(const unit_stats& stats, u8vec2 pos, reference<controller> c) {
		this->m_stats = stats;
		this->m_pos = pos;
		this->m_hp = this->m_stats.max_hp;
		this->m_controller = c;
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
		if (this->m_controller) {
			controller::buttons b = this->m_controller->get_state();
			if (b.right.down) {
				this->m_pos.x++;
			}
			if (b.left.down) {
				this->m_pos.x--;
			}
			if (b.up.down) {
				this->m_pos.y++;
			}
			if (b.down.down) {
				this->m_pos.y--;
			}
		}
	}
}