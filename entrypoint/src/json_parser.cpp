// fe_engine header
#include <engine.h>
using namespace fe_engine;
// class header
#include "json_parser.h"
// c++ std library
#include <fstream>
#include <vector>
#include <cassert>
namespace fe_engine {
	void from_json(const nlohmann::json& j, unit::unit_stats& s) {
		j["lvl"].get_to(s.level);
		j["hp"].get_to(s.max_hp);
		j["str"].get_to(s.strength);
		j["mag"].get_to(s.magic);
		j["dex"].get_to(s.dexterity);
		j["spd"].get_to(s.speed);
		j["lck"].get_to(s.luck);
		j["def"].get_to(s.defense);
		j["res"].get_to(s.resistance);
		j["cha"].get_to(s.charm);
		j["mv"].get_to(s.movement);
	}
	void from_json(const nlohmann::json& j, s8vec2& v) {
		j["x"].get_to(v.x);
		j["y"].get_to(v.y);
	}
	void from_json(const nlohmann::json& j, tile::passing_properties& pp) {
		j["foot"].get_to(pp.foot);
	}
}
namespace json_types {
	struct unit_data {
		s8vec2 pos;
		unit::unit_stats stats;
		unit_affiliation affiliation;
		std::vector<size_t> inventory;
		size_t equipped_weapon;
		bool has_weapon, has_behavior, has_name;
		std::string behavior_name, unit_name;
	};
	struct item_data;
	struct item_subdata : public ref_counted {
		item_data* pointer;
	};
	struct weapon_data : public item_subdata {
		int32_t attack;
		int32_t hit;
		int32_t crit;
		int32_t durability;
		s8vec2 range;
		weapon::type type;
	};
	struct consumable_data : public item_subdata {
		bool has_behavior;
		std::string behavior_name;
	};
	struct item_data {
		std::string name;
		bool is_weapon;
		reference<item_subdata> data;
	};
	struct tile_data {
		s8vec2 pos;
		renderer::background_color color;
		tile::passing_properties properties;
		std::string interact_behavior_name;
	};
	void from_json(const nlohmann::json& j, unit_data& ud) {
		ud.has_weapon = j.find("equipped") != j.end();
		if (ud.has_weapon) j["equipped"].get_to(ud.equipped_weapon);
		j["inventory"].get_to(ud.inventory);
		j["pos"].get_to(ud.pos);
		j["stats"].get_to(ud.stats);
		j["affiliation"].get_to(ud.affiliation);
		ud.has_behavior = j.find("behavior") != j.end();
		if (ud.has_behavior) j["behavior"].get_to(ud.behavior_name);
		ud.has_name = j.find("name") != j.end();
		if (ud.has_name) j["name"].get_to(ud.unit_name);
	}
	void from_json(const nlohmann::json& j, reference<item_subdata>& isd);
	void from_json(const nlohmann::json& j, item_data& id) {
		j["name"].get_to(id.name);
		j["is weapon"].get_to(id.is_weapon);
		if (id.is_weapon) {
			id.data = reference<weapon_data>::create();
		}
		else {
			id.data = reference<consumable_data>::create();
		}
		id.data->pointer = &id;
		j["data"].get_to(id.data);
	}
	void from_json(const nlohmann::json& j, reference<item_subdata>& isd) {
		if (isd->pointer->is_weapon) {
			reference<weapon_data> wd = isd;
			j["attack"].get_to(wd->attack);
			j["hit"].get_to(wd->hit);
			j["crit"].get_to(wd->crit);
			j["durability"].get_to(wd->durability);
			j["range"].get_to(wd->range);
			j["type"].get_to(wd->type);
		}
		else {
			reference<consumable_data> cd = isd;
			cd->has_behavior = j.find("behavior") != j.end();
			if (cd->has_behavior) j["behavior"].get_to(cd->behavior_name);
		}
	}
	void from_json(const nlohmann::json& j, tile_data& td) {
		j["pos"].get_to(td.pos);
		int bg_color_id = j["color"].get<int>();
		switch (bg_color_id) {
		case 0:
			td.color = renderer::background_color::red;
			break;
		case 1:
			td.color = renderer::background_color::green;
			break;
		case 2:
			td.color = renderer::background_color::blue;
			break;
		case 3:
			td.color = renderer::background_color::yellow;
			break;
		case 4:
			td.color = renderer::background_color::white;
			break;
		case 5:
			td.color = renderer::background_color::black;
			break;
		default:
			td.color = renderer::background_color::none;
			break;
		}
		j["passing_properties"].get_to(td.properties);
		if (!j["interact_behavior"].is_null()) {
			j["interact_behavior"].get_to(td.interact_behavior_name);
		}
	}
}
void parse_cs_classname(const std::string& full_name, std::string& namespace_name, std::string& class_name) {
	size_t period = full_name.find_last_of('.');
	if (period != std::string::npos) {
		namespace_name = full_name.substr(0, period);
		class_name = full_name.substr(period + 1);
	}
	else {
		namespace_name = "";
		class_name = full_name;
	}
}
size_t json_parser::get_unit_count() {
	std::vector<nlohmann::json> units;
	this->m_file["units"].get_to(units);
	return units.size();
}
reference<unit> json_parser::make_unit_from_index(size_t index) {
	std::vector<json_types::unit_data> data;
	this->m_file["units"].get_to(data);
	json_types::unit_data current_unit = data[index];
	std::string unit_name;
	if (current_unit.has_name) {
		unit_name = current_unit.unit_name;
	}
	reference<unit> unit_object = reference<unit>::create(current_unit.stats, current_unit.pos, current_unit.affiliation, this->m_map.get(), unit_name);
	if (current_unit.has_behavior) {
		std::string namespace_name, class_name;
		parse_cs_classname(current_unit.behavior_name, namespace_name, class_name);
		reference<cs_class> script = this->find_class(namespace_name, class_name);
		assert(script);
		unit_object->attach_behavior(reference<behavior>::create(script, this->m_core), object_registry::get_register<unit>()->size());
	}
	if (current_unit.has_weapon) unit_object->set_equipped_weapon(current_unit.equipped_weapon);
	for (size_t index : current_unit.inventory) {
		unit_object->get_inventory().push_back(index);
	}
	return unit_object;
}
void json_parser::update_tiles() {
	if (this->m_file.find("tiles") == this->m_file.end()) {
		return;
	}
	if (this->m_file["tiles"].is_null()) {
		return;
	}
	std::vector<json_types::tile_data> data;
	this->m_file["tiles"].get_to(data);
	for (const auto& entry : data) {
		reference<tile> t = reference<tile>::create(entry.properties, entry.color);
		if (!entry.interact_behavior_name.empty()) {
			std::string ns, cls;
			parse_cs_classname(entry.interact_behavior_name, ns, cls);
			auto _class = this->find_class(ns, cls);
			if (_class) {
				auto get_delegate = _class->get_method(entry.interact_behavior_name + ":GetBehavior()");
				reference<cs_object> return_value = cs_method::call_function(get_delegate);
				t->set_interact_behavior(reference<cs_delegate>::create(return_value));
			}
		}
		this->m_map->set_tile(entry.pos, t);
	}
}
json_parser::ui_data json_parser::parse_ui_data(const std::string& filepath) {
	nlohmann::json j;
	{
		std::ifstream file(filepath);
		file >> j;
		file.close();
	}
	ui_data data;
	if (j.find("script") != j.end()) {
		std::string ns, cls, full;
		j["script"].get_to(full);
		parse_cs_classname(full, ns, cls);
		data.ui_script = this->find_class(ns, cls);
	}
	return data;
}
json_parser::json_parser(const std::string& json_path, std::vector<fe_engine::reference<fe_engine::assembly>> script_assemblies, fe_engine::reference<fe_engine::assembly> core_assembly, fe_engine::reference<fe_engine::map> map) {
	std::ifstream file(json_path);
	file >> this->m_file;
	file.close();
	this->m_assemblies = script_assemblies;
	this->m_core = core_assembly;
	this->m_map = map;
	this->load_items();
}
void json_parser::load_items() {
	assert(object_registry::register_exists<item>());
	auto item_register = object_registry::get_register<item>();
	std::vector<json_types::item_data> data;
	this->m_file["items"].get_to(data);
	for (auto& id : data) {
		reference<item> i;
		if (id.is_weapon) {
			reference<json_types::weapon_data> wd = id.data;
			weapon::weapon_stats stats;
			stats.attack = wd->attack;
			stats.hit_rate = wd->hit;
			stats.critical_rate = wd->crit;
			stats.durability = wd->durability;
			stats.range = wd->range;
			i = reference<weapon>::create(wd->type, stats, id.name);
		} else {
			reference<json_types::consumable_data> cd = id.data;
			reference<item_behavior> itembehavior;
			if (cd->has_behavior) {
				std::string namespace_name, class_name;
				parse_cs_classname(cd->behavior_name, namespace_name, class_name);
				reference<cs_class> script = this->find_class(namespace_name, class_name);
				assert(script);
				itembehavior = reference<item_behavior>::create(script, this->m_core);
			}
			i = reference<item>::create(id.name, item::usable, itembehavior);
		}
		item_register->add(i);
	}
}
reference<cs_class> json_parser::find_class(const std::string& namespace_name, const std::string& class_name) {
	reference<cs_class> _class;
	std::vector<reference<assembly>> assemblies;
	assemblies.push_back(this->m_core);
	for (auto assembly : this->m_assemblies) {
		assemblies.push_back(assembly);
	}
	for (auto assembly : assemblies) {
		reference<cs_class> cls = assembly->get_class(namespace_name, class_name);
		if (cls->raw()) {
			_class = cls;
			break;
		}
	}
	return _class;
}