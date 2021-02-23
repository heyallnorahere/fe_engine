// fe_engine header
#include <engine.h>
using namespace fe_engine;
// class header
#include "json_parser.h"
// c++ std library
#include <fstream>
#include <vector>
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
}
namespace json_types {
	struct unit_data {
		s8vec2 pos;
		unit::unit_stats stats;
		unit_affiliation affiliation;
		std::vector<size_t> inventory;
		size_t equipped_weapon;
		bool has_weapon;
	};
	struct item_data;
	struct item_subdata : public ref_counted {
		item_data* pointer;
	};
	struct weapon_data : public item_subdata {
		uint8_t attack;
		uint8_t hit;
		uint8_t crit;
		uint8_t durability;
		s8vec2 range;
		weapon::type type;
	};
	struct consumable_data : public item_subdata {
		// todo: implement
	};
	struct item_data {
		std::string name;
		bool is_weapon;
		reference<item_subdata> data;
	};
	void from_json(const nlohmann::json& j, unit_data& ud) {
		ud.has_weapon = j.find("equipped") != j.end();
		if (ud.has_weapon) j["equipped"].get_to(ud.equipped_weapon);
		j["inventory"].get_to(ud.inventory);
		j["pos"].get_to(ud.pos);
		j["stats"].get_to(ud.stats);
		j["affiliation"].get_to(ud.affiliation);
	}
	void from_json(const nlohmann::json& j, reference<item_subdata>& isd);
	void from_json(const nlohmann::json& j, item_data& id) {
		j["name"].get_to(id.name);
		j["is weapon"].get_to(id.is_weapon);
		if (id.is_weapon) {
			id.data = new weapon_data;
		}
		else {
			id.data = new consumable_data;
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
	reference<unit> unit_object = reference<unit>::create(current_unit.stats, current_unit.pos, current_unit.affiliation);
	if (current_unit.has_weapon) unit_object->set_equipped_weapon((reference<weapon>)this->m_items[current_unit.equipped_weapon]);
	for (size_t index : current_unit.inventory) {
		unit_object->get_inventory().push_back(this->m_items[index]);
	}
	return unit_object;
}
json_parser::json_parser(const std::string& json_path) {
	std::ifstream file(json_path);
	file >> this->m_file;
	file.close();
	this->load_items();
}
void json_parser::load_items() {
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
			i = reference<item>::create(id.name, item::usable);
		}
		this->m_items.push_back(i);
	}
}