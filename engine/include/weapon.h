#pragma once
#include "item.h"
#include "_math.h"
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
			using stat_type = uint8_t;
			stat_type attack;
			stat_type hit_rate;
			stat_type critical_rate;
			stat_type durability;
			vec2t<stat_type> range;
		};
		weapon(type weapon_type, weapon_stats stats = { 1, 100, 0, 50, { 1, 1 } }, const std::string& name = "");
		type get_type() const;
		weapon_stats get_stats() const;
		weapon_stats::stat_type get_current_durability() const;
		void consume_durability(weapon_stats::stat_type uses = 1);
	private:
		type m_type;
		weapon_stats m_stats;
		weapon_stats::stat_type m_durability;
	};
}