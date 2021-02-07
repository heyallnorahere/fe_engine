#include "player.h"
namespace fe_engine {
	player::player(reference<controller> c, reference<map> m) {
		this->m_controller = c;
		this->m_map = m;
		memset(&this->m_cursor_pos, 0, sizeof(u8vec2));
	}
	void player::update() {
		this->m_controller->update();
		controller::buttons buttons = this->m_controller->get_state();
		if (buttons.right.down) this->m_cursor_pos.x++;
		if (buttons.left.down) this->m_cursor_pos.x--;
		if (buttons.up.down) this->m_cursor_pos.y++;
		if (buttons.down.down) this->m_cursor_pos.y--;
		if (buttons.a.down) {
			if (this->m_selected) {
				this->m_selected->move(this->m_cursor_pos - this->m_selected->get_pos());
				this->m_selected.reset();
			}
			else {
				reference<unit> u = this->m_map->get_unit_at(this->m_cursor_pos);
				if (u) {
					if (u->get_affiliation() == unit_affiliation::player) {
						this->m_selected = u;
					}
				}
			}
		}
	}
	void player::render_cursor(const reference<renderer>& r) {
		size_t width, height;
		r->get_buffer_size(width, height);
		if (this->m_cursor_pos.y < height - 1) {
			r->render_char_at(this->m_cursor_pos.x, this->m_cursor_pos.y + 1, 'v', renderer::color::white);
		}
	}
}