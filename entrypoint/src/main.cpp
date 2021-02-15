#include <iostream>
#include <engine.h>
int main() {
	constexpr size_t width = 20;
	constexpr size_t height = 10;
	fe_engine::reference<fe_engine::map> map = fe_engine::reference<fe_engine::map>::create(width, height);
	fe_engine::reference<fe_engine::renderer> renderer = fe_engine::reference<fe_engine::renderer>::create();
	renderer->set_buffer_size((size_t)((double)width * 4.5), height + 1);
	fe_engine::unit::unit_stats stats;
	memset(&stats, 0, sizeof(fe_engine::unit::unit_stats));
	stats.level = 1;
	stats.max_hp = 30;
	stats.movement = 5;
	fe_engine::reference<fe_engine::controller> controller = fe_engine::reference<fe_engine::controller>::create(0);
	fe_engine::reference<fe_engine::ui_controller> ui_controller = fe_engine::reference<fe_engine::ui_controller>::create(renderer, map, controller);
	fe_engine::reference<fe_engine::phase_manager> phase_manager = fe_engine::reference<fe_engine::phase_manager>::create();
	fe_engine::reference<fe_engine::player> player = fe_engine::reference<fe_engine::player>::create(controller, map, ui_controller, phase_manager);
	fe_engine::reference<fe_engine::script_engine> script_engine = fe_engine::reference<fe_engine::script_engine>::create("script-assemblies/scriptcore.dll", map);
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
		map->add_unit(u);
		u = fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 1, 8 }, fe_engine::unit_affiliation::ally);
		u->set_equipped_weapon(fe_engine::reference<fe_engine::weapon>::create(fe_engine::weapon::type::whitemagic));
		map->add_unit(u);
		u = fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 18, 1 }, fe_engine::unit_affiliation::separate_enemy);
		u->set_equipped_weapon(fe_engine::reference<fe_engine::weapon>::create(fe_engine::weapon::type::lance));
		map->add_unit(u);
	}
	while (true) {
		map->update();
		player->update();
		ui_controller->update();
		if (controller->get_state().start.down) break;
		renderer->clear();
		map->render(renderer);
		player->render_cursor(renderer);
		ui_controller->render();
		renderer->present();
	}
	return 0;
}
