#pragma once
#include "player.h"
#include "renderer.h"
#include "phase_manager.h"
namespace fe_engine {
	namespace internal {
		float get_current_time();
		struct pm_struct {
			reference<phase_manager> m;
		};
	}
}