#include "phase_manager.h"
#include "logger.h"
namespace fe_engine {
	phase_manager::phase_manager() {
		this->m_current_phase = unit_affiliation::player;
	}
	unit_affiliation phase_manager::get_current_phase() const {
		return this->m_current_phase;
	}
	void phase_manager::cycle_phase(reference<map> m) {
		this->cycle_enum();
		for (size_t i = 0; i < m->get_unit_count(); i++) {
			reference<unit> u = m->get_unit(i);
			if (u->get_affiliation() == this->m_current_phase) {
				u->refresh_movement();
			}
		}
		this->log_phase();
	}
	void phase_manager::log_phase() {
		std::string phase;
		renderer::color color;
		switch (this->m_current_phase) {
		case unit_affiliation::player:
			phase = "Player";
			color = renderer::color::blue;
			break;
		case unit_affiliation::enemy:
			phase = "Enemy";
			color = renderer::color::red;
			break;
		case unit_affiliation::separate_enemy:
			phase = "Enemy";
			color = renderer::color::yellow;
			break;
		case unit_affiliation::ally:
			phase = "Ally";
			color = renderer::color::green;
			break;
		default:
			return;
			break;
		}
		logger::print(phase + " phase", color);
	}
	void phase_manager::cycle_enum() {
		this->m_current_phase = (unit_affiliation)(((int)this->m_current_phase + 1) % 4);
	}
}