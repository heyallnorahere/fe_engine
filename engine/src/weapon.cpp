#include "weapon.h"
namespace fe_engine {
	std::string weapon_type_to_string(weapon::type type) {
		switch (type) {
		case weapon::type::axe:
			return "Axe";
			break;
		case weapon::type::blackmagic:
			return "Black magic";
			break;
		case weapon::type::bow:
			return "Bow";
			break;
		case weapon::type::darkmagic:
			return "Dark magic";
			break;
		case weapon::type::fists:
			return "Fists";
			break;
		case weapon::type::lance:
			return "Lance";
			break;
		case weapon::type::sword:
			return "Sword";
			break;
		case weapon::type::whitemagic:
			return "White magic";
			break;
		default:
			return "Other weapon";
			break;
		}
	}
	weapon::weapon(type weapon_type, weapon_stats stats, const std::string& name) : item(name.empty() ? weapon_type_to_string(weapon_type) : name, item::equipable | item::weapon) {
		this->m_type = weapon_type;
		this->m_stats = stats;
		this->m_durability = this->m_stats.durability;
	}
	weapon::type weapon::get_type() const {
		return this->m_type;
	}
	weapon::weapon_stats weapon::get_stats() const {
		return this->m_stats;
	}
	int16_t weapon::get_current_durability() const {
		return this->m_durability;
	}
	void weapon::consume_durability(int16_t uses) {
		this->m_durability -= uses;
	}
}
