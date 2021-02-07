#pragma once
#include "reference.h"
namespace fe_engine {
	class weapon : public ref_counted {
	public:
		enum class type {
			fists = 'F',
			sword = 'S',
			lance = 'L',
			axe = 'A',
			bow = 'B',
			blackmagic = 'Y',
			darkmagic = 'X',
			whitemagic = 'W',
		};
		weapon(type weapon_type);
		type get_type() const;
	private:
		type m_type;
	};
}