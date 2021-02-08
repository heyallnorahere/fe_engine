#pragma once
#include "reference.h"
#include "_math.h"
#include "controller.h"
#include "weapon.h"
#include <list>
namespace fe_engine {
	enum class unit_affiliation {
		player,
		ally,
		enemy,
		separate_enemy,
	};
	class unit : public ref_counted {
	public:
		struct unit_stats {
			using stat_type = unsigned char;
			stat_type level;
			stat_type max_hp;
			stat_type strength;
			stat_type magic;
			stat_type dexterity;
			stat_type speed;
			stat_type luck;
			stat_type defense;
			stat_type resilience;
			stat_type charm;
			stat_type move;
		};
		unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation);
		~unit();
		const unit_stats& get_stats() const;
		s8vec2 get_pos() const;
		unit_stats::stat_type get_current_hp() const;
		unit_affiliation get_affiliation() const;
		void update();
		void move(s8vec2 offset);
		reference<weapon> get_equipped_weapon() const;
		void set_equipped_weapon(const reference<weapon>& w);
		const std::list<reference<item>>& get_inventory() const;
		std::list<reference<item>>& get_inventory();
	private:
		unit_stats m_stats;
		s8vec2 m_pos;
		unit_stats::stat_type m_hp;
		unit_affiliation m_affiliation;
		reference<weapon> m_equipped_weapon;
		std::list<reference<item>> m_inventory;
	};
}