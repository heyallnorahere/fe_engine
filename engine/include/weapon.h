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
			uint32_t attack;
			uint32_t hit_rate;
			uint32_t critical_rate;
			uint32_t durability;
			s32vec2 range; // signed integer because of c#
		};
		weapon(type weapon_type, weapon_stats stats = { 1, 100, 0, 50, { 1, 1 } }, const std::string& name = "");
		type get_type() const;
		weapon_stats& get_stats();
		int32_t get_current_durability() const;
		void consume_durability(int32_t uses = 1);
	private:
		type m_type;
		weapon_stats m_stats;
		int32_t m_durability;
	};
}