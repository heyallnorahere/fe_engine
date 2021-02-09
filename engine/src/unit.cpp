#include "unit.h"
#include "_random.h"
namespace fe_engine {
	unit::unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation) {
		this->m_stats = stats;
		this->m_pos = pos;
		this->m_hp = this->m_stats.max_hp;
		this->m_affiliation = affiliation;
	}
	unit::~unit() {
		// todo: delete
	}
	const unit::unit_stats& unit::get_stats() const {
		return this->m_stats;
	}
	s8vec2 unit::get_pos() const {
		return this->m_pos;
	}
	unit::unit_stats::stat_type unit::get_current_hp() const {
		return this->m_hp;
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
		if (this->m_equipped_weapon) {
			if (this->m_equipped_weapon->get_current_durability() <= 0) {
				this->m_equipped_weapon.reset();
			}
		}
	}
	void unit::move(s8vec2 offset) {
		this->m_pos += offset;
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
		unit_stats::stat_type defense = other->m_stats.defense;
		if (magic && !white_magic) defense *= -1;
		packet.might = weapon_stats.attack + (magic ? this->m_stats.magic : this->m_stats.strength) - defense;
		packet.hit = weapon_stats.hit_rate + this->m_stats.dexterity - other->m_stats.dexterity;
		packet.crit = weapon_stats.critical_rate + this->m_stats.resilience - other->m_stats.resilience;
		this->m_equipped_weapon->consume_durability();
		return packet;
	}
	void unit::receive_attack_packet(attack_packet packet, reference<unit> sender) {
		unsigned char offset = this->m_stats.luck - sender->m_stats.luck;
		unsigned char hit = static_cast<unsigned char>(random_number_generator::generate(0, 100)) + offset;
		unsigned char crit = static_cast<unsigned char>(random_number_generator::generate(0, 100)) + offset;
		if (hit <= packet.hit) {
			if (crit <= packet.crit) {
				packet.might *= 3;
			}
			this->m_hp -= packet.might;
		}
	}
}