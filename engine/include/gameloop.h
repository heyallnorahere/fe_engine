#pragma once
#include "player.h"
#include "renderer.h"
#include "discord_app.h"
namespace fe_engine {
	void gameloop(reference<player> p, reference<renderer> r, reference<discord_app> da = reference<discord_app>());
}