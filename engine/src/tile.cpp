#include "tile.h"
namespace fe_engine {
	tile::tile(const passing_properties& properties, renderer::background_color color) {
		this->m_properties = properties;
		this->m_color = color;
	}
	renderer::background_color tile::get_color() {
		return this->m_color;
	}
	void tile::set_color(renderer::background_color color) {
		this->m_color = color;
	}
	tile::passing_properties tile::get_properties() {
		return this->m_properties;
	}
	void tile::set_interaction_data(const interaction_data& data) {
		this->m_interaction_data = data;
	}
	tile::interaction_data tile::get_interaction_data() {
		return this->m_interaction_data;
	}
}