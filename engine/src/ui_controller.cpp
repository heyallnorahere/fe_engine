#include "ui_controller.h"
#include <sstream>
namespace fe_engine {
	ui_controller::ui_controller(reference<renderer> r, reference<map> m, reference<controller> c) {
		this->m_renderer = r;
		this->m_map = m;
		this->m_controller = c;
	}
	void ui_controller::set_info_panel_target(reference<unit> u) {
		this->m_info_panel_target = u;
	}
	void ui_controller::set_unit_menu_target(reference<unit> u) {
		this->m_unit_menu_target = u;
		this->m_unit_menu_index = 0;
		this->m_menu_items.clear();
		this->m_menu_items.push_back({ "Wait", [](reference<ui_controller> controller) {} });
	}
	void ui_controller::update() {
		if (this->m_unit_menu_target) {
			controller::buttons buttons = this->m_controller->get_state();
			if (buttons.down.down && this->m_unit_menu_index < this->m_menu_items.size() - 1) {
				this->m_unit_menu_index++;
			}
			if (buttons.up.down && this->m_unit_menu_index > 0) {
				this->m_unit_menu_index--;
			}
			if (buttons.a.down && this->m_can_close) {
				this->m_menu_items[this->m_unit_menu_index].on_select(reference<ui_controller>(this));
				this->close_unit_menu();
			}
		}
	}
	void ui_controller::render() {
		size_t map_width = this->m_map->get_width();
		size_t map_height = this->m_map->get_height();
		size_t viewport_width, viewport_height;
		this->m_renderer->get_buffer_size(viewport_width, viewport_height);
		size_t unit_menu_width = 20;
		size_t info_panel_width = viewport_width - (map_width + unit_menu_width + 3);
		this->render_frame(info_panel_width, unit_menu_width, map_width, map_height);
		this->render_info_panel(map_width + 1, viewport_height - map_height, info_panel_width, map_height);
		this->render_unit_menu(map_width + info_panel_width + 2, viewport_height - map_height, unit_menu_width, map_height);
	}
	reference<unit> ui_controller::get_unit_menu_target() const {
		return this->m_unit_menu_target;
	}
	void ui_controller::set_can_close(bool c) {
		this->m_can_close = c;
	}
	void ui_controller::close_unit_menu() {
		this->m_unit_menu_target.reset();
	}
	void ui_controller::render_frame(size_t info_panel_width, size_t unit_menu_width, size_t map_width, size_t map_height) {
		size_t viewport_width, viewport_height;
		this->m_renderer->get_buffer_size(viewport_width, viewport_height);
		for (size_t y = viewport_height - map_height; y < viewport_height; y++) {
			this->m_renderer->render_char_at(map_width, y, '#', renderer::color::white);
			this->m_renderer->render_char_at(map_width + info_panel_width + 1, y, '#', renderer::color::white);
			this->m_renderer->render_char_at(map_width + info_panel_width + unit_menu_width + 2, y, '#', renderer::color::white);
		}
		for (size_t x = 0; x < map_width + info_panel_width + unit_menu_width + 3; x++) {
			size_t y = viewport_height - (map_height + 1);
			this->m_renderer->render_char_at(x, y, '#', renderer::color::white);
		}
	}
	void ui_controller::render_info_panel(size_t origin_x, size_t origin_y, size_t width, size_t height) {
		if (!this->m_info_panel_target) {
			this->m_renderer->render_string_at(origin_x, origin_y + height - 1, "Empty tile", renderer::color::white);
		} else {
			std::string affiliation = "Unknown";
			switch (this->m_info_panel_target->get_affiliation()) {
			case unit_affiliation::player:
				affiliation = "Player";
				break;
			case unit_affiliation::ally:
				affiliation = "Ally";
				break;
			case unit_affiliation::enemy:
				affiliation = "Enemy";
				break;
			case unit_affiliation::separate_enemy:
				affiliation = "Third Army";
				break;
			}
			std::stringstream hp_string;
			hp_string << (unsigned int)this->m_info_panel_target->get_current_hp() << "/" << (unsigned int)this->m_info_panel_target->get_stats().max_hp;
			this->m_renderer->render_string_at(origin_x, origin_y + height - 1, affiliation + " unit     " + hp_string.str(), renderer::color::white);
		}
	}
	void ui_controller::render_unit_menu(size_t origin_x, size_t origin_y, size_t width, size_t height) {
		if (this->m_unit_menu_target) {
			for (size_t i = 0; i < this->m_menu_items.size(); i++) {
				bool selected = (this->m_unit_menu_index == i);
				size_t y = origin_y + height - (1 + (i * 2));
				if (selected) {
					this->m_renderer->render_char_at(origin_x, y, '>', renderer::color::red);
				}
				this->m_renderer->render_string_at(origin_x + 2, y, this->m_menu_items[i].text, selected ? renderer::color::red : renderer::color::white);
			}
		}
	}
}