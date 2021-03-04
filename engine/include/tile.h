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
		tile(const passing_properties& properties, renderer::background_color color = renderer::background_color::green);
		renderer::background_color get_color();
		passing_properties get_properties();
	private:
		passing_properties m_properties;
		renderer::background_color m_color;
	};
}