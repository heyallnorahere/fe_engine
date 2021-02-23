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
	json_parser(const std::string& json_path, fe_engine::reference<fe_engine::assembly> script_assembly);
	void load_items();
	nlohmann::json m_file;
	std::vector<fe_engine::reference<fe_engine::item>> m_items;
	fe_engine::reference<fe_engine::assembly> m_script_assembly;
	friend class fe_engine::reference<json_parser>;
};