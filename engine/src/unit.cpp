#include "unit.h"
#include "_random.h"
#include "map.h"
#ifdef max
#undef max
#endif
#include <limits>
#include <cassert>
namespace fe_engine {
	unit::unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation, map* m) {
		this->m_stats = stats;
		this->m_pos = pos;
		this->m_hp = this->m_stats.max_hp;
		this->m_affiliation = affiliation;
		this->m_map = m;
		this->refresh_movement();
	}
	unit::~unit() {
		// todo: delete
	}
	const unit::unit_stats& unit::get_stats() const {
		return this->m_stats;
	}
	unit::unit_stats& unit::get_stats() {
		return this->m_stats;
	}
	s8vec2 unit::get_pos() const {
		return this->m_pos;
	}
	int16_t unit::get_current_hp() const {
		return this->m_hp;
	}
	void unit::set_current_hp(int16_t hp) {
		this->m_hp = hp;
		if (this->m_hp > this->m_stats.max_hp) {
			this->m_hp = this->m_stats.max_hp;
		}
	}
	unit_affiliation unit::get_affiliation() const {
		return this->m_affiliation;
	}
	void unit::update() {
		this->m_inventory.remove_if([](reference<item> i) {
			if (i->get_item_flags() & item::weapon) {
				return reference<weapon>(i)->get_current_durability() <= 0;
			}
			return false;
		});
		for (size_t i = 0; i < this->m_inventory.size(); i++) {
			std::list<reference<item>>::iterator it = this->m_inventory.begin();
			std::advance(it, i);
			reference<item> _i = *it;
			if (!_i->initialized()) {
				size_t unit_index = std::numeric_limits<size_t>::max();
				for (size_t j = 0; j < this->m_map->get_unit_count(); j++) {
					if (this->m_map->get_unit(j).get() == this) {
						unit_index = j;
						break;
					}
				}
				assert(unit_index != std::numeric_limits<size_t>::max());
				_i->init(i, unit_index);
			}
		}
		if (this->m_equipped_weapon) {
			if (this->m_equipped_weapon->get_current_durability() <= 0) {
				this->m_equipped_weapon.reset();
			}
		}
	}
	void unit::unit_update() {
		if (this->m_affiliation != unit_affiliation::player) {
			if (!this->m_behavior) {
				this->m_can_move = false;
			}
			else {
				this->m_behavior->on_unit_update();
			}
		}
	}
	void unit::move(s8vec2 offset, int8_t consumption_multiplier) {
		this->m_pos += offset;
		this->m_movement -= (abs(offset.x) + abs(offset.y)) * consumption_multiplier;
		this->m_can_move = (consumption_multiplier <= 0);
	}
	reference<weapon> unit::get_equipped_weapon() const {
		return this->m_equipped_weapon;
	}
	void unit::set_equipped_weapon(const reference<weapon>& w) {
		this->m_equipped_weapon = w;
	}
	const std::list<reference<item>>& unit::get_inventory() const {
		return this->m_inventory;
	}
	std::list<reference<item>>& unit::get_inventory() {
		return this->m_inventory;
	}
	void unit::attack(reference<unit> to_attack) {
		to_attack->receive_attack_packet(this->generate_attack_packet(to_attack), reference<unit>(this));
	}
	unit::attack_packet unit::generate_attack_packet(reference<unit> other) {
		weapon::weapon_stats weapon_stats = this->m_equipped_weapon->get_stats();
		weapon::type weapon_type = this->m_equipped_weapon->get_type();
		attack_packet packet;
		bool magic = false;
		bool white_magic = false;
		if (weapon_type == weapon::type::darkmagic) magic = true;
		if (weapon_type == weapon::type::blackmagic) magic = true;
		if (weapon_type == weapon::type::whitemagic) {
			magic = true;
			white_magic = true;
		}
		int16_t defense = other->m_stats.defense;
		if (magic && !white_magic) defense = other->m_stats.resistance;
		packet.might = (int16_t)weapon_stats.attack + (magic ? this->m_stats.magic : this->m_stats.strength) - defense;
		packet.hit = (int16_t)weapon_stats.hit_rate + this->m_stats.dexterity - other->m_stats.dexterity;
		packet.crit = (int16_t)weapon_stats.critical_rate;
		this->m_equipped_weapon->consume_durability();
		return packet;
	}
	void unit::receive_attack_packet(attack_packet packet, reference<unit> sender) {
		int16_t offset = this->m_stats.luck - sender->m_stats.luck;
		int16_t hit = static_cast<int16_t>(random_number_generator::generate(0, 100)) + offset;
		int16_t crit = static_cast<int16_t>(random_number_generator::generate(0, 100)) + offset;
		if (hit <= packet.hit) {
			if (crit <= packet.crit) {
				packet.might *= 3;
			}
			this->m_hp -= packet.might;
		}
	}
	int16_t unit::get_available_movement() const {
		return this->m_movement;
	}
	void unit::set_available_movement(int16_t mv) {
		this->m_movement = mv;
	}
	void unit::refresh_movement() {
		this->m_movement = this->m_stats.movement;
		this->m_can_move = true;
	}
	bool unit::can_move() const {
		return this->m_can_move;
	}
	void unit::attach_behavior(reference<behavior> b, uint64_t map_index) {
		this->m_behavior = b;
		this->m_behavior->on_attach(map_index);
	}
	reference<behavior> unit::get_behavior() {
		return this->m_behavior;
	}
}