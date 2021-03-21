#include "util.h"
#include <chrono>
#include "object_register.h"
namespace fe_engine {
	std::unordered_map<size_t, reference<ref_counted>> object_registry::m;
	namespace util {
		double start_time = 0;
		bool time_initialized = false;
		float get_current_time() {
			double current_time = std::chrono::duration_cast<std::chrono::duration<double>>(std::chrono::system_clock::now().time_since_epoch()).count();
			if (!time_initialized) {
				start_time = current_time;
				time_initialized = true;
			}
			return static_cast<float>(current_time - start_time);
		}
	}
}