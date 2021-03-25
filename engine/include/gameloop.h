#pragma once
#include "player.h"
#include "renderer.h"
namespace fe_engine {
	void gameloop(reference<player> p, reference<renderer> r);
}