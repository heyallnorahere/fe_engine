#include "player.h"
namespace fe_engine {
	player::player(reference<controller> c, reference<map> m, reference<ui_controller> uc) {
		this->m_controller = c;
		this->m_map = m;
		this->m_ui_controller = uc;
		memset(&this->m_cursor_pos, 0, sizeof(u8vec2));
	}
	void player::update() {
		this->m_controller->update();
		this->m_ui_controller->set_can_close(true);
		if (!this->m_ui_controller->get_unit_menu_target()) {
			controller::buttons buttons = this->m_controller->get_state();
			bool update_tile_selection = false;
			if (buttons.right.down && (size_t)this->m_cursor_pos.x < this->m_map->get_width() - 1) {
				this->m_cursor_pos.x++;
				update_tile_selection = true;
			}
			if (buttons.left.down && this->m_cursor_pos.x > 0) {
				this->m_cursor_pos.x--;
				update_tile_selection = true;
			}
			if (buttons.up.down && (size_t)this->m_cursor_pos.y < this->m_map->get_height() - 1) {
				this->m_cursor_pos.y++;
				update_tile_selection = true;
			}
			if (buttons.down.down && this->m_cursor_pos.y > 0) {
				this->m_cursor_pos.y--;
				update_tile_selection = true;
			}
			if (update_tile_selection) {
				this->m_ui_controller->set_info_panel_target(this->m_map->get_unit_at(this->m_cursor_pos));
			}
			if (buttons.a.down) {
				if (this->m_selected) {
					this->m_selected->move(this->m_cursor_pos - this->m_selected->get_pos());
					this->m_ui_controller->set_unit_menu_target(this->m_selected);
					this->m_ui_controller->set_can_close(false);
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
	}
	void player::render_cursor(const reference<renderer>& r) {
		if (!this->m_ui_controller->get_unit_menu_target()) {
			size_t width, height;
			r->get_buffer_size(width, height);
			if (this->m_cursor_pos.y < height - 1) {
				r->render_char_at(this->m_cursor_pos.x, this->m_cursor_pos.y + 1 + (height - this->m_map->get_height()), 'v', renderer::color::white);
			}
		}
	}
}