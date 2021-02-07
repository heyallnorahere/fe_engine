#include "weapon.h"
#include "..\include\weapon.h"
namespace fe_engine {
	weapon::weapon(type weapon_type, weapon_stats stats) {
		this->m_type = weapon_type;
		this->m_stats = stats;
	}
	weapon::type weapon::get_type() const {
		return this->m_type;
	}
}