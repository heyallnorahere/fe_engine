#include <iostream>
#include <engine.h>
int main() {
	constexpr size_t width = 20;
	constexpr size_t height = 10;
	fe_engine::reference<fe_engine::map> map = fe_engine::reference<fe_engine::map>::create(width, height);
	fe_engine::reference<fe_engine::renderer> renderer = fe_engine::reference<fe_engine::renderer>::create();
	renderer->set_buffer_size(width, height);
	fe_engine::unit::unit_stats stats;
	memset(&stats, 0, sizeof(fe_engine::unit::unit_stats));
	stats.level = 1;
	stats.max_hp = 1;
	stats.move = 5;
	fe_engine::reference<fe_engine::controller> controller = fe_engine::reference<fe_engine::controller>::create(0);
	fe_engine::reference<fe_engine::player> player = fe_engine::reference<fe_engine::player>::create(controller, map);
	map->add_unit(fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 1, 1 }, fe_engine::unit_affiliation::player));
	map->add_unit(fe_engine::reference<fe_engine::unit>::create(stats, fe_engine::u8vec2{ 18, 8 }, fe_engine::unit_affiliation::enemy));
	while (true) {
		map->update();
		player->update();
		renderer->clear();
		map->render(renderer);
		player->render_cursor(renderer);
		renderer->present();
	}
	return 0;
}
