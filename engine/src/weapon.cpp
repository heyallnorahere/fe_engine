#include "weapon.h"
#include "..\include\weapon.h"
namespace fe_engine {
	weapon::weapon(type weapon_type) {
		this->m_type = weapon_type;
	}
	weapon::type weapon::get_type() const {
		return this->m_type;
	}
}