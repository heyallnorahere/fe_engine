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
		struct interaction_data {
			reference<cs_delegate> callback;
			std::string menu_text;
		};
		tile(const passing_properties& properties, renderer::background_color color = renderer::background_color::green);
		renderer::background_color get_color();
		void set_color(renderer::background_color color);
		passing_properties get_properties();
		void set_interaction_data(const interaction_data& data);
		interaction_data get_interaction_data();
	private:
		passing_properties m_properties;
		renderer::background_color m_color;
		interaction_data m_interaction_data;
	};
}