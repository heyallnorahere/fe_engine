#include "tile.h"
namespace fe_engine {
	tile::tile(const passing_properties& properties, s32vec2 pos, renderer::background_color color) {
		this->m_properties = properties;
		this->m_pos = pos;
		this->m_color = color;
	}
	renderer::background_color tile::get_color() {
		return this->m_color;
	}
	s32vec2 tile::get_pos() {
		return this->m_pos;
	}
	tile::passing_properties tile::get_properties() {
		return this->m_properties;
	}
}