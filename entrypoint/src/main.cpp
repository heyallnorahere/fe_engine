// fe_engine header
#include <engine.h>
// c++ std library
#include <iostream>
#include <vector>
#include <string>
// recently added header (c++17 i think?)
#ifdef FEENGINE_LINUX
#include <experimental/filesystem>
namespace fs = std::experimental::filesystem;
#else
#include <filesystem>
namespace fs = std::filesystem;
#endif
// json file parser
#include "json_parser.h"
// this function cycles through all of the files in a directory and returns the paths relative to cwd
static std::vector<std::string> get_file_entries(const std::string& directory, const std::string& exclude = "", const std::string& extension = "") {
	std::vector<std::string> filenames;
	for (const auto& n : fs::directory_iterator(directory)) {
		std::string path = n.path().string();
		bool keep = true;
		if (!extension.empty()) {
			size_t spos = path.find_last_of('/');
			size_t pos = path.find_last_of('.');
			if (pos == std::string::npos || (pos < spos && spos != std::string::npos)) {
				keep = false;
			}
			else {
				std::string substr = path.substr(pos + 1);
				keep = (substr == extension);
			}
		}
		if (path == exclude && !exclude.empty()) {
			continue;
		}
		if (keep) filenames.push_back(n.path().string());
	}
	return filenames;
}
static void add_registers() {
	using namespace fe_engine;
	object_registry::add_register<unit>();
	object_registry::add_register<item>();
}
// entrypoint
int main() {
	fe_engine::logger::print("Initializing...", fe_engine::renderer::color::green);
	// add registers
	add_registers();
	// all of the loaded assemblies
	std::vector<fe_engine::reference<fe_engine::assembly>> script_assemblies;
	// width and height of the map
	constexpr size_t width = 20;
	constexpr size_t height = 10;
	// create a map object
	fe_engine::reference<fe_engine::map> map = fe_engine::reference<fe_engine::map>::create(width, height);
	// create a renderer
	fe_engine::reference<fe_engine::renderer> renderer = fe_engine::reference<fe_engine::renderer>::create();
	// resize the buffer
	renderer->set_buffer_size((size_t)((double)width * 4.5), (size_t)((double)height * 2.75));
	// set stats for placeholder units
	fe_engine::unit::unit_stats stats;
	memset(&stats, 0, sizeof(fe_engine::unit::unit_stats));
	stats.level = 1;
	stats.max_hp = 30;
	stats.movement = 5;
	// make a controller object
	fe_engine::reference<fe_engine::controller> controller = fe_engine::reference<fe_engine::controller>::create(0);
	// make the input mapper object
	fe_engine::reference<fe_engine::input_mapper> imapper = fe_engine::reference<fe_engine::input_mapper>::create(controller);
	// make a ui controller
	fe_engine::reference<fe_engine::ui_controller> ui_controller = fe_engine::reference<fe_engine::ui_controller>::create(renderer, map, imapper);
	// make a phase manager
	fe_engine::reference<fe_engine::phase_manager> phase_manager = fe_engine::reference<fe_engine::phase_manager>::create();
	// make the player (cursor, etc.)
	fe_engine::reference<fe_engine::player> player = fe_engine::reference<fe_engine::player>::create(imapper, map, ui_controller, phase_manager);
	// instantiate the script engine and load the core assembly
	fe_engine::reference<fe_engine::script_engine> script_engine = fe_engine::reference<fe_engine::script_engine>::create("script-assemblies/scriptcore.dll", map);
	fe_engine::reference<fe_engine::assembly> core = script_engine->get_core();
	fe_engine::reference<fe_engine::cs_class> test_class = core->get_class("FEEngine", "Test");
	// load a test item script
	fe_engine::reference<fe_engine::cs_class> test_item = core->get_class("FEEngine", "TestItem");
	// load all assemblies in the "script-assemblies" directory, excluding the core assembly
	std::string directory = "script-assemblies/";
	std::vector<std::string> script_assembly_names = get_file_entries(directory, directory + "scriptcore.dll", "dll");
	for (auto filename : script_assembly_names) {
		script_assemblies.push_back(script_engine->load_assembly(filename));
	}
	// load json map file
	fe_engine::reference<json_parser> parser = fe_engine::reference<json_parser>::create("data/map.json", script_assemblies, core, map.get());
	for (size_t i = 0; i < parser->get_unit_count(); i++) {
		auto unit = parser->make_unit_from_index(i);
		map->add_unit(unit);
	}
	fe_engine::logger::print("Initialized! Starting main loop...", fe_engine::renderer::color::green);
	phase_manager->log_phase();
	// start the loop
	while (true) {
		// update the engine state
		imapper->update();
		map->update();
		map->update_units(phase_manager->get_current_phase(), imapper);
		player->update();
		ui_controller->update();
		if (imapper->get_state().exit) break;
		// render the map and ui
		renderer->clear();
		map->render(renderer);
		player->render_cursor(renderer);
		ui_controller->render();
		// print the characters to the console
		renderer->present();
	}
	fe_engine::logger::print("Shutting down. Goodbye!", fe_engine::renderer::color::green);
	// free all of the allocated memory (via fe_engine::reference) and terminate the program
	return 0;
}
