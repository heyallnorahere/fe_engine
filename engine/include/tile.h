#pragma once
#include "reference.h"
#include "renderer.h"
#include "_math.h"
namespace fe_engine {
	class tile : public ref_counted {
	public:
		struct passing_properties {
			bool foot;
		};
		tile(const passing_properties& properties, s32vec2 pos, renderer::background_color color = renderer::background_color::green);
		renderer::background_color get_color();
		s32vec2 get_pos();
		passing_properties get_properties();
	private:
		passing_properties m_properties;
		s32vec2 m_pos;
		renderer::background_color m_color;
	};
}