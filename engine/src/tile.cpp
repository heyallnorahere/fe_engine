#include "tile.h"
namespace fe_engine {
	tile::tile(const passing_properties& properties, renderer::background_color color) {
		this->m_properties = properties;
		this->m_color = color;
	}
	renderer::background_color tile::get_color() {
		return this->m_color;
	}
	tile::passing_properties tile::get_properties() {
		return this->m_properties;
	}
}