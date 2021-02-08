#pragma once
#include "item.h"
namespace fe_engine {
	class weapon : public item {
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
		struct weapon_stats {
			using stat_type = unsigned char;
			stat_type attack;
			stat_type hit_rate;
			stat_type critical_rate;
			stat_type durability;
		};
		weapon(type weapon_type, weapon_stats stats = { 1, 100, 0, 50 });
		type get_type() const;
		weapon_stats get_stats() const;
	private:
		type m_type;
		weapon_stats m_stats;
	};
}