#pragma once
#include "reference.h"
#include "_math.h"
#include "controller.h"
#include "weapon.h"
#include <list>
#include <vector>
namespace fe_engine {
	enum class unit_affiliation {
		player,
		enemy,
		separate_enemy,
		ally,
	};
	class unit : public ref_counted {
	public:
		struct unit_stats {
			using stat_type = uint8_t;
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
			stat_type movement;
		};
		unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation);
		~unit();
		const unit_stats& get_stats() const;
		s8vec2 get_pos() const;
		unit_stats::stat_type get_current_hp() const;
		unit_affiliation get_affiliation() const;
		void update();
		void move(s8vec2 offset, char consumption_multiplier = 1);
		reference<weapon> get_equipped_weapon() const;
		void set_equipped_weapon(const reference<weapon>& w);
		const std::list<reference<item>>& get_inventory() const;
		std::list<reference<item>>& get_inventory();
		void attack(reference<unit> to_attack);
		unit_stats::stat_type get_available_movement() const;
		void refresh_movement();
		bool can_move() const;
	private:
		struct attack_packet {
			uint8_t might, hit, crit;
		};
		unit_stats m_stats;
		s8vec2 m_pos;
		unit_stats::stat_type m_hp, m_movement;
		unit_affiliation m_affiliation;
		reference<weapon> m_equipped_weapon;
		std::list<reference<item>> m_inventory;
		bool m_can_move;
		attack_packet generate_attack_packet(reference<unit> other);
		void receive_attack_packet(attack_packet packet, reference<unit> sender);
	};
}