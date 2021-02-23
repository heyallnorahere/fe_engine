#pragma once
// c++ std library
#include <string>
#include <vector>
// json library
#include <nlohmann/json.hpp>
// json parser class; pass in a path, load a file, and parse items/units
class json_parser : fe_engine::ref_counted {
public:
	size_t get_unit_count();
	fe_engine::reference<fe_engine::unit> make_unit_from_index(size_t index);
private:
	json_parser(const std::string& json_path, std::vector<fe_engine::reference<fe_engine::assembly>> script_assemblies, fe_engine::reference<fe_engine::assembly> core_assembly, fe_engine::reference<fe_engine::map> map);
	void load_items();
	fe_engine::reference<fe_engine::cs_class> find_class(const std::string& namespace_name, const std::string& class_name);
	nlohmann::json m_file;
	std::vector<fe_engine::reference<fe_engine::item>> m_items;
	fe_engine::reference<fe_engine::assembly> m_core;
	std::vector<fe_engine::reference<fe_engine::assembly>> m_assemblies;
	fe_engine::reference<fe_engine::map> m_map;
	friend class fe_engine::reference<json_parser>;
};