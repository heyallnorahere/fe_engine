#include "util.h"
#include <chrono>
#include "object_register.h"
#include "map.h"
#include "ui_controller.h"
#include "gameloop.h"
namespace fe_engine {
	std::unordered_map<size_t, reference<ref_counted>> object_registry::m;
	namespace internal {
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
	void gameloop(reference<player> p, reference<renderer> r, reference<discord_app> da) {
		auto imapper = object_registry::get_register<input_mapper>()->get(0);
		auto m = object_registry::get_register<map>()->get(0);
		auto uc = object_registry::get_register<ui_controller>()->get(0);
		while (true) {
			// if a discord_app object was passed, update it
			if (da) {
				da->update();
			}
			// update the engine state
			imapper->update();
			m->update();
			m->update_units();
			p->update();
			uc->update();
			if (imapper->get_state().exit) break;
			// render the map and ui
			r->clear();
			m->render(r);
			p->render_cursor(r);
			uc->render();
			// print the characters to the console
			r->present();
		}
	}
}