#include "controller.h"
#include <memory>
#include <cstring>
extern bool is_controller_connected(size_t index);
extern fe_engine::controller::buttons get_controller_state(size_t index);
namespace fe_engine {
	controller::controller(size_t index) {
		this->m_index = index;
		memset(&this->m_current, 0, sizeof(buttons));
		memset(&this->m_last, 0, sizeof(buttons));
	}
	static bool calc_down(bool current, bool last) {
		return current && (!last);
	}
	static bool calc_up(bool current, bool last) {
		return (!current) && last;
	}
	static void calculate_difference(controller::button& current, controller::button last) {
		current.down = calc_down(current.held, last.held);
		current.up = calc_up(current.held, last.held);
	}
	static void calculate_difference(controller::buttons& current, const controller::buttons& last) {
		calculate_difference(current.a, last.a);
		calculate_difference(current.b, last.b);
		calculate_difference(current.x, last.x);
		calculate_difference(current.y, last.y);
		calculate_difference(current.lb, last.lb);
		calculate_difference(current.rb, last.rb);
		calculate_difference(current.ls, last.ls);
		calculate_difference(current.rs, last.rs);
		calculate_difference(current.up, last.up);
		calculate_difference(current.down, last.down);
		calculate_difference(current.left, last.left);
		calculate_difference(current.right, last.right);
		calculate_difference(current.start, last.start);
		calculate_difference(current.select, last.select);
	}
	void controller::update() {
		this->m_last = this->m_current;
		if (this->connected()) {
			this->m_current = get_controller_state(this->m_index);
			calculate_difference(this->m_current, this->m_last);
		}
	}
	controller::buttons controller::get_state() const {
		return this->m_current;
	}
	bool controller::connected() const {
		return is_controller_connected(this->m_index);
	}
}