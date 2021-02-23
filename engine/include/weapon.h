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
			int16_t attack;
			int16_t hit_rate;
			int16_t critical_rate;
			int16_t durability;
			vec2t<int16_t> range;
		};
		weapon(type weapon_type, weapon_stats stats = { 1, 100, 0, 50, { 1, 1 } }, const std::string& name = "");
		type get_type() const;
		weapon_stats get_stats() const;
		int16_t get_current_durability() const;
		void consume_durability(int16_t uses = 1);
	private:
		type m_type;
		weapon_stats m_stats;
		int16_t m_durability;
	};
}