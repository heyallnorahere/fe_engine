#include "input_mapper.h"
#include <memory>
#include <fstream>
#include "script_wrappers.h"
#include <nlohmann/json.hpp>
enum class controller_button {
	a,
	b,
	x,
	y,
	lb,
	rb,
	ls,
	rs,
	up,
	down,
	left,
	right,
	start,
	select,
};
using _button_map = std::unordered_map<std::string, controller_button>;
static _button_map gen_button_map() {
	_button_map button_map;
	button_map["a"] = controller_button::a;
	button_map["b"] = controller_button::b;
	button_map["x"] = controller_button::x;
	button_map["y"] = controller_button::y;
	button_map["lb"] = controller_button::lb;
	button_map["rb"] = controller_button::rb;
	button_map["ls"] = controller_button::ls;
	button_map["rs"] = controller_button::rs;
	button_map["up"] = controller_button::up;
	button_map["down"] = controller_button::down;
	button_map["left"] = controller_button::left;
	button_map["right"] = controller_button::right;
	button_map["start"] = controller_button::start;
	button_map["select"] = controller_button::select;
	return button_map;
}
static _button_map button_map = gen_button_map();
static bool get_value(const std::string& button_name, const fe_engine::controller::buttons& buttons) {
	switch (button_map[button_name]) {
	case controller_button::a:
		return buttons.a.down;
		break;
	case controller_button::b:
		return buttons.b.down;
		break;
	case controller_button::x:
		return buttons.x.down;
		break;
	case controller_button::y:
		return buttons.y.down;
		break;
	case controller_button::lb:
		return buttons.lb.down;
		break;
	case controller_button::rb:
		return buttons.rb.down;
		break;
	case controller_button::ls:
		return buttons.ls.down;
		break;
	case controller_button::rs:
		return buttons.rs.down;
		break;
	case controller_button::up:
		return buttons.up.down;
		break;
	case controller_button::down:
		return buttons.down.down;
		break;
	case controller_button::left:
		return buttons.left.down;
		break;
	case controller_button::right:
		return buttons.right.down;
		break;
	case controller_button::start:
		return buttons.start.down;
		break;
	case controller_button::select:
		return buttons.select.down;
		break;
	}
	return false;
}
template<typename T> static bool contains(const std::vector<T>& vector, const T& value) {
	for (const T& val : vector) {
		if (val == value) {
			return true;
		}
	}
	return false;
}
namespace fe_engine {
	input_mapper::input_mapper(reference<controller> controller) {
		this->m_controller = controller;
		this->m_keyboard = new keyboard();
		this->load_mappings_from_json("data/mappings.json");
		script_wrappers::set_imapper(reference<input_mapper>(this));
	}

	void input_mapper::update()
	{
		memset(&this->m_current, 0, sizeof(commands));
		if (this->m_controller) {
			this->m_controller->update();
			auto buttons = this->m_controller->get_state();

			if (get_value(this->m_controller_mappings["up"], buttons)) {
				this->m_current.up = true;
			}
			if (get_value(this->m_controller_mappings["down"], buttons)) {
				this->m_current.down = true;
			}
			if (get_value(this->m_controller_mappings["left"], buttons)) {
				this->m_current.left = true;
			}
			if (get_value(this->m_controller_mappings["right"], buttons)) {
				this->m_current.right = true;
			}
			if (get_value(this->m_controller_mappings["ok"], buttons)) {
				this->m_current.ok = true;
			}
			if (get_value(this->m_controller_mappings["back"], buttons)) {
				this->m_current.back = true;
			}
			if (get_value(this->m_controller_mappings["exit"], buttons)) {
				this->m_current.exit = true;
			}
		}

		if (this->m_keyboard) {
			this->m_keyboard->update();
			auto chars = this->m_keyboard->get_input();
			if (contains(chars, this->m_keyboard_mappings["up"])) {
				this->m_current.up = true;
			}
			if (contains(chars, this->m_keyboard_mappings["down"])) {
				this->m_current.down = true;
			}
			if (contains(chars, this->m_keyboard_mappings["left"])) {
				this->m_current.left = true;
			}
			if (contains(chars, this->m_keyboard_mappings["right"])) {
				this->m_current.right = true;
			}
			if (contains(chars, this->m_keyboard_mappings["ok"])) {
				this->m_current.ok = true;
			}
			if (contains(chars, this->m_keyboard_mappings["back"])) {
				this->m_current.back = true;
			}
			if (contains(chars, this->m_keyboard_mappings["exit"])) {
				this->m_current.exit = true;
			}
		}
	}
	
	void input_mapper::load_mappings_from_json(const std::string& path) {
		std::ifstream file(path);
		nlohmann::json json_data;
		file >> json_data;
		file.close();
		if (json_data.find("controller mappings") != json_data.end()) {
			json_data["controller mappings"].get_to(this->m_controller_mappings);
		}
		else {
			this->m_controller_mappings["up"] = "up";
			this->m_controller_mappings["down"] = "down";
			this->m_controller_mappings["left"] = "left";
			this->m_controller_mappings["right"] = "right";
			this->m_controller_mappings["ok"] = "a";
			this->m_controller_mappings["back"] = "b";
			this->m_controller_mappings["exit"] = "start";
		}
		if (json_data.find("keyboard mappings") != json_data.end()) {
			std::unordered_map<std::string, std::string> temp;
			json_data["keyboard mappings"].get_to(temp);
			for (const auto& m : temp) {
				this->m_keyboard_mappings[m.first] = m.second[0];
			}
		}
		else {
			this->m_keyboard_mappings["up"] = 'w';
			this->m_keyboard_mappings["down"] = 's';
			this->m_keyboard_mappings["left"] = 'a';
			this->m_keyboard_mappings["right"] = 'd';
			this->m_keyboard_mappings["ok"] = 'x';
			this->m_keyboard_mappings["back"] = 'z';
			this->m_keyboard_mappings["exit"] = 'q';
		}
	}
}