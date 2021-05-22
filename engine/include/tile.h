#pragma once
#include "reference.h"
#include "renderer.h"
#include "_math.h"
#include "mono_classes.h"
namespace fe_engine {
	class tile : public ref_counted {
	public:
		struct passing_properties {
			bool foot;
		};
		tile(const passing_properties& properties, renderer::background_color color = renderer::background_color::green);
		renderer::background_color get_color();
		void set_color(renderer::background_color color);
		passing_properties get_properties();
		void set_interact_behavior(reference<cs_delegate> behavior);
		reference<cs_delegate> get_interact_behavior();
	private:
		passing_properties m_properties;
		renderer::background_color m_color;
		reference<cs_delegate> m_interact_behavior;
	};
}