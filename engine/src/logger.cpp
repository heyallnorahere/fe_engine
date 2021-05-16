#include "logger.h"
namespace fe_engine {
	std::vector<logger::message> log;
	void logger::print(const std::string& msg, renderer::color color) {
		size_t pos = msg.find("\n");
		std::string to_print, to_print_next;
		if (pos != std::string::npos) {
			to_print = msg.substr(0, pos);
			to_print_next = msg.substr(pos + 1);
		} else {
			to_print = msg;
		}
		log.push_back({ to_print, color });
		if (!to_print_next.empty()) {
			print(to_print_next, color);
		}
	}
	std::vector<logger::message> logger::get_log() {
		return log;
	}
}