#pragma once
#include "reference.h"
#include "_math.h"
#include "controller.h"
#include "weapon.h"
#include "behavior.h"
#include <list>
#include <vector>
#include "input_mapper.h"
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
			int32_t level;
			int32_t max_hp;
			int32_t strength;
			int32_t magic;
			int32_t dexterity;
			int32_t speed;
			int32_t luck;
			int32_t defense;
			int32_t resistance;
			int32_t charm;
			int32_t movement;
		};
		unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation, map* m = NULL, const std::string& name = "");
		~unit();
		const unit_stats& get_stats() const;
		unit_stats& get_stats();
		s8vec2 get_pos() const;
		int32_t get_current_hp() const;
		void set_current_hp(int32_t hp);
		unit_affiliation get_affiliation() const;
		void update();
		void unit_update(reference<input_mapper> im);
		void move(s8vec2 offset, int8_t consumption_multiplier = 1);
		reference<weapon> get_equipped_weapon() const;
		void set_equipped_weapon(const reference<weapon>& w);
		const std::list<reference<item>>& get_inventory() const;
		std::list<reference<item>>& get_inventory();
		void attack(reference<unit> to_attack);
		int32_t get_available_movement() const;
		void set_available_movement(int32_t mv);
		void refresh_movement();
		bool can_move() const;
		void wait();
		void attach_behavior(reference<behavior> b, uint64_t map_index);
		reference<behavior> get_behavior();
		void set_name(const std::string& name);
		std::string get_name();
		bool initialized();
		void init();
		void equip(reference<item> to_equip);
		void update_index();
	private:
		struct attack_packet {
			int32_t might, hit, crit;
		};
		std::string m_name;
		unit_stats m_stats;
		s8vec2 m_pos;
		int32_t m_hp, m_movement;
		uint64_t m_map_index;
		unit_affiliation m_affiliation;
		reference<weapon> m_equipped_weapon;
		std::list<reference<item>> m_inventory;
		reference<behavior> m_behavior;
		map* m_map = NULL;
		bool m_can_move, m_initialized;
		attack_packet generate_attack_packet(reference<unit> other);
		void receive_attack_packet(attack_packet packet, reference<unit> sender);
	};
}