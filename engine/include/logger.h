#pragma once
#include "renderer.h"
#include <vector>
namespace fe_engine {
	class logger {
	public:
		struct message {
			std::string msg;
			renderer::color color;
		};
		static void print(const std::string& msg, renderer::color color = renderer::color::white);
		static std::vector<message> get_log();
	};
}