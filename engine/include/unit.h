#pragma once
#include "reference.h"
#include "_math.h"
#include "controller.h"
#include "weapon.h"
#include "behavior.h"
#include <list>
#include <vector>
namespace fe_engine {
	enum class unit_affiliation {
		player,
		enemy,
		separate_enemy,
		ally,
	};
	class map;
	class unit : public ref_counted {
	public:
		struct unit_stats {
			int16_t level;
			int16_t max_hp;
			int16_t strength;
			int16_t magic;
			int16_t dexterity;
			int16_t speed;
			int16_t luck;
			int16_t defense;
			int16_t resistance;
			int16_t charm;
			int16_t movement;
		};
		unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation, map* m = NULL);
		~unit();
		const unit_stats& get_stats() const;
		unit_stats& get_stats();
		s8vec2 get_pos() const;
		int16_t get_current_hp() const;
		void set_current_hp(int16_t hp);
		unit_affiliation get_affiliation() const;
		void update();
		void unit_update();
		void move(s8vec2 offset, int8_t consumption_multiplier = 1);
		reference<weapon> get_equipped_weapon() const;
		void set_equipped_weapon(const reference<weapon>& w);
		const std::list<reference<item>>& get_inventory() const;
		std::list<reference<item>>& get_inventory();
		void attack(reference<unit> to_attack);
		int16_t get_available_movement() const;
		void set_available_movement(int16_t mv);
		void refresh_movement();
		bool can_move() const;
		void attach_behavior(reference<behavior> b, uint64_t map_index);
		reference<behavior> get_behavior();
	private:
		struct attack_packet {
			int16_t might, hit, crit;
		};
		unit_stats m_stats;
		s8vec2 m_pos;
		int16_t m_hp, m_movement;
		unit_affiliation m_affiliation;
		reference<weapon> m_equipped_weapon;
		std::list<reference<item>> m_inventory;
		reference<behavior> m_behavior;
		map* m_map = NULL;
		bool m_can_move;
		attack_packet generate_attack_packet(reference<unit> other);
		void receive_attack_packet(attack_packet packet, reference<unit> sender);
	};
}