// fe_engine header
#include <engine.h>
// c++ std library
#include <iostream>
#include <vector>
#include <string>
// recently added header (c++17 i think?)
#include <filesystem>
// json file parser
#include "json_parser.h"
// this function cycles through all of the files in a directory and returns the paths relative to cwd
static std::vector<std::string> get_file_entries(const std::string& directory, const std::string& exclude = "") {
	std::vector<std::string> filenames;
	for (const auto& n : std::filesystem::directory_iterator(directory)) {
		if (n.path().string() == exclude && !exclude.empty()) {
			continue;
		}
		filenames.push_back(n.path().string());
	}
	return filenames;
}
// entrypoint
int main() {
	// all of the loaded 
	std::vector<fe_engine::reference<fe_engine::assembly>> script_assemblies;
	// width and height of the map
	constexpr size_t width = 20;
	constexpr size_t height = 10;
	// create a map object
	fe_engine::reference<fe_engine::map> map = fe_engine::reference<fe_engine::map>::create(width, height);
	// create a renderer
	fe_engine::reference<fe_engine::renderer> renderer = fe_engine::reference<fe_engine::renderer>::create();
	// resize the buffer
	renderer->set_buffer_size((size_t)((double)width * 4.5), height + 5);
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
	// load all assemblies in the "script-assemblies" directory, excluding the core assembly
	std::string directory = "script-assemblies/";
	std::vector<std::string> script_assembly_names = get_file_entries(directory, directory + "scriptcore.dll");
	for (auto filename : script_assembly_names) {
		script_assemblies.push_back(script_engine->load_assembly(filename));
	}
	// load a test behavior
	fe_engine::reference<fe_engine::cs_class> enemy_script;
	for (auto assembly : script_assemblies) {
		fe_engine::reference<fe_engine::cs_class> cls = assembly->get_class("Scripts", "Enemy");
		if (cls->raw()) {
			enemy_script = cls;
			break;
		}
	}
	// load json files
	fe_engine::reference<json_parser> parser = fe_engine::reference<json_parser>::create("data/map.json", script_assemblies[0]);
	for (size_t i = 0; i < parser->get_unit_count(); i++) {
		auto unit = parser->make_unit_from_index(i);
		map->add_unit(unit);
	}
	// add placeholder units of each affiliation
	{
		fe_engine::reference<fe_engine::unit> u = fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 1, 1 }, fe_engine::unit_affiliation::player);
		u->get_inventory().push_back(fe_engine::reference<fe_engine::item>::create("reserve", fe_engine::item::usable, [](fe_engine::unit* unit) {
			unit->move({ 0, 1 });
		}));
		u->set_equipped_weapon(fe_engine::reference<fe_engine::weapon>::create(fe_engine::weapon::type::sword, fe_engine::weapon::weapon_stats{ 5, 100, 0, 2, { 1, 1 } }));
		u->get_inventory().push_back(fe_engine::reference<fe_engine::weapon>::create(fe_engine::weapon::type::axe));
		map->add_unit(u);
		u = fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 18, 8 }, fe_engine::unit_affiliation::enemy);
		u->set_equipped_weapon(fe_engine::reference<fe_engine::weapon>::create(fe_engine::weapon::type::darkmagic));
		u->attach_behavior(fe_engine::reference<fe_engine::behavior>::create(enemy_script, core), map->get_unit_count());
		map->add_unit(u);
		u = fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 1, 8 }, fe_engine::unit_affiliation::ally);
		u->set_equipped_weapon(fe_engine::reference<fe_engine::weapon>::create(fe_engine::weapon::type::whitemagic));
		map->add_unit(u);
		u = fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 18, 1 }, fe_engine::unit_affiliation::separate_enemy);
		u->set_equipped_weapon(fe_engine::reference<fe_engine::weapon>::create(fe_engine::weapon::type::lance));
		map->add_unit(u);
	}
	// start the loop
	while (true) {
		// update the engine state
		map->update();
		map->update_units(phase_manager->get_current_phase());
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
	// free all of the allocated memory (via fe_engine::reference) and terminate the program
	return 0;
}
