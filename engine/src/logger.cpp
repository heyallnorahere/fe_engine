#include "logger.h"
namespace fe_engine {
	std::vector<logger::message> log;
	void logger::print(const std::string& msg, renderer::color color) {
		log.push_back({ msg, color });
	}
	std::vector<logger::message> logger::get_log() {
		return log;
	}
}