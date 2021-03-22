#include "unit.h"
#include "_random.h"
#include "map.h"
#include "logger.h"
#ifdef max
#undef max
#endif
#include <limits>
#include <sstream>
#include <cassert>
#include "object_register.h"
namespace fe_engine {
	unit::unit(const unit_stats& stats, s8vec2 pos, unit_affiliation affiliation, map* m, const std::string& name) {
		this->m_stats = stats;
		this->m_pos = pos;
		this->m_hp = this->m_stats.max_hp;
		this->m_affiliation = affiliation;
		this->set_name(name);
		this->m_map = m;
		this->refresh_movement();
		this->m_initialized = false;
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
	int32_t unit::get_current_hp() const {
		return this->m_hp;
	}
	void unit::set_current_hp(int32_t hp) {
		this->m_hp = hp;
		if (this->m_hp > this->m_stats.max_hp) {
			this->m_hp = this->m_stats.max_hp;
		}
	}
	unit_affiliation unit::get_affiliation() const {
		return this->m_affiliation;
	}
	void unit::update() {
		auto item_register = object_registry::get_register<item>();
		auto unit_register = object_registry::get_register<unit>();
		this->m_inventory.remove_if([&](size_t index) {
			reference<item> i = item_register->get(index);
			if (i->get_item_flags() & item::weapon) {
				return reference<weapon>(i)->get_current_durability() <= 0;
			}
			return false;
		});
		for (size_t i = 0; i < this->m_inventory.size(); i++) {
			std::list<size_t>::iterator it = this->m_inventory.begin();
			std::advance(it, i);
			size_t index = *it;
			reference<item> _i = item_register->get(index);
			if (!_i->initialized()) {
				size_t unit_index = std::numeric_limits<size_t>::max();
				for (size_t j = 0; j < unit_register->size(); j++) {
					if (unit_register->get(j).get() == this) {
						unit_index = j;
						break;
					}
				}
				assert(unit_index != std::numeric_limits<size_t>::max());
				_i->init(index, unit_index);
			}
		}
		if (this->m_equipped_weapon != (size_t)-1) {
			reference<weapon> equipped_weapon = item_register->get(this->m_equipped_weapon);
			if (equipped_weapon->get_current_durability() <= 0) {
				this->m_equipped_weapon = (size_t)-1;
			}
		}
	}
	void unit::unit_update(reference<input_mapper> im) {
		if (this->m_affiliation != unit_affiliation::player) {
			if (!this->m_behavior) {
				this->m_can_move = false;
			}
		}
		if (this->m_behavior) {
			this->m_behavior->on_unit_update(im);
		}
	}
	void unit::move(s8vec2 offset, int8_t consumption_multiplier) {
		this->m_pos += offset;
		this->m_movement -= (abs(offset.x) + abs(offset.y)) * consumption_multiplier;
		this->m_can_move = (consumption_multiplier <= 0);
	}
	size_t unit::get_equipped_weapon() const {
		return this->m_equipped_weapon;
	}
	void unit::set_equipped_weapon(size_t register_index) {
		this->m_equipped_weapon = register_index;
	}
	const std::list<size_t>& unit::get_inventory() const {
		return this->m_inventory;
	}
	std::list<size_t>& unit::get_inventory() {
		return this->m_inventory;
	}
	void unit::attack(size_t to_attack) {
		auto unit_register = object_registry::get_register<unit>();
		auto item_register = object_registry::get_register<item>();
		reference<unit> _to_attack = unit_register->get(to_attack);
		reference<weapon> m_equipped_weapon = item_register->get(this->m_equipped_weapon);
		reference<weapon> other_equipped_weapon = item_register->get(_to_attack->get_equipped_weapon());
		_to_attack->receive_attack_packet(this->generate_attack_packet(_to_attack), reference<unit>(this));
		if (_to_attack->m_hp <= 0) return;
		if (other_equipped_weapon->get_current_durability() > 0) {
			s32vec2 range = other_equipped_weapon->get_stats().range;
			int32_t length = (this->m_pos - _to_attack->m_pos).taxicab();
			if (length >= range.x && length <= range.y) {
				this->receive_attack_packet(_to_attack->generate_attack_packet(reference<unit>(this)), _to_attack);
			}
		}
		if (this->m_hp <= 0) {
			return;
		}
		if (m_equipped_weapon->get_current_durability() > 0 && this->m_stats.speed > _to_attack->m_stats.speed) {
			_to_attack->receive_attack_packet(this->generate_attack_packet(_to_attack), reference<unit>(this));
		}
		if (_to_attack->m_hp <= 0) {
			return;
		}
		if (other_equipped_weapon->get_current_durability() > 0 && _to_attack->m_stats.speed > this->m_stats.speed) {
			this->receive_attack_packet(_to_attack->generate_attack_packet(reference<unit>(this)), _to_attack);
		}
	}
	unit::attack_packet unit::generate_attack_packet(reference<unit> other) {
		reference<weapon> equipped_weapon = object_registry::get_register<item>()->get(this->m_equipped_weapon);
		weapon::weapon_stats weapon_stats = equipped_weapon->get_stats();
		weapon::type weapon_type = equipped_weapon->get_type();
		attack_packet packet;
		bool magic = false;
		bool white_magic = false;
		if (weapon_type == weapon::type::darkmagic) magic = true;
		if (weapon_type == weapon::type::blackmagic) magic = true;
		if (weapon_type == weapon::type::whitemagic) {
			magic = true;
			white_magic = true;
		}
		int32_t defense = other->m_stats.defense;
		if (magic && !white_magic) defense = other->m_stats.resistance;
		packet.might = (int32_t)weapon_stats.attack + (magic ? this->m_stats.magic : this->m_stats.strength) - defense;
		packet.hit = (int32_t)weapon_stats.hit_rate + this->m_stats.dexterity - other->m_stats.dexterity;
		packet.crit = (int32_t)weapon_stats.critical_rate;
		equipped_weapon->consume_durability();
		return packet;
	}
	void unit::receive_attack_packet(attack_packet packet, reference<unit> sender) {
		int32_t offset = this->m_stats.luck - sender->m_stats.luck;
		int32_t hit = static_cast<int32_t>(random_number_generator::generate(0, 100)) + offset;
		if (hit < 0)
			hit = 0;
		if (hit > 100)
			hit = 100;
		int32_t crit = static_cast<int32_t>(random_number_generator::generate(0, 100)) + offset;
		if (crit < 0)
			crit = 0;
		if (crit > 100)
			crit = 100;
		if (hit <= packet.hit) {
			bool critical = crit <= packet.crit;
			if (critical) {
				logger::print("Critical Hit!", renderer::color::red);
				packet.might *= 3;
			}
			if (packet.might > this->m_hp) {
				packet.might = this->m_hp;
			}
			std::stringstream message;
			message << sender->get_name() << " hit " << this->m_name << " for ";
			message << packet.might;
			message << " damage!";
			if (critical) {
				message << " (" << (packet.might / 3) << " * 3)";
			}
			logger::print(message.str());
			this->m_hp -= packet.might;
		}
		else {
			logger::print(sender->get_name() + " missed " + this->m_name + "!");
		}
	}
	int32_t unit::get_available_movement() const {
		return this->m_movement;
	}
	void unit::set_available_movement(int32_t mv) {
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
		this->m_map_index = map_index;
		this->m_initialized = !this->m_behavior;
	}
	reference<behavior> unit::get_behavior() {
		return this->m_behavior;
	}
	void unit::set_name(const std::string& name) {
		if (name.empty()) {
			std::string affiliation;
			switch (this->m_affiliation) {
			case unit_affiliation::player:
				affiliation = "Player";
				break;
			case unit_affiliation::enemy:
				affiliation = "Enemy";
				break;
			case unit_affiliation::separate_enemy:
				affiliation = "Third Army";
				break;
			case unit_affiliation::ally:
				affiliation = "Ally";
				break;
			default:
				affiliation = "Unknown";
				break;
			}
			this->m_name = affiliation + " unit";
		}
		else {
			this->m_name = name;
		}
	}
	std::string unit::get_name() {
		return this->m_name;
	}
	bool unit::initialized() {
		return this->m_initialized;
	}
	void unit::init() {
		if (this->m_behavior) this->m_behavior->on_attach(this->m_map_index);
		this->m_initialized = true;
	}
	void unit::equip(size_t to_equip) {
		auto item_register = object_registry::get_register<item>();
		assert(item_register->get(to_equip)->get_item_flags() & item::weapon);
		this->get_inventory().remove_if([&](size_t index) { return index == to_equip; });
		reference<weapon> equipped = item_register->get(this->m_equipped_weapon);
		this->set_equipped_weapon(to_equip);
		if (equipped) this->get_inventory().push_back(equipped);
	}
	void unit::update_index() {
		reference<assembly> core;
		reference<cs_class> item_class, item_behavior_class;
		reference<cs_field> item_behavior_parent, item_parent_index;
		if (this->m_behavior)  {
			core = this->m_behavior->get_core();
			assert(core);
			reference<cs_class> unit_class = core->get_class("FEEngine", "Unit");
			reference<cs_class> behavior_class = core->get_class("FEEngine", "Behavior");
			reference<cs_field> behavior_parent = behavior_class->get_field("parent");
			reference<cs_property> unit_index = unit_class->get_property("Index");
			reference<cs_object> instance = this->m_behavior->get_object();
			reference<cs_object> parent = instance->get_field(behavior_parent);
			uint64_t index = *(uint64_t*)parent->get_property(unit_index)->unbox();
			index--;
			parent->set_property(unit_index, &index);
		}
		auto item_register = object_registry::get_register<item>();
		for (size_t index : this->m_inventory) {
			reference<item> i = item_register->get(index);
			reference<item_behavior> behavior = i->get_behavior();
			if (behavior) {
				if (!core) {
					core = behavior->get_core();
					assert(core);
				}
				if (!item_behavior_class) {
					item_behavior_class = core->get_class("FEEngine", "ItemBehavior");
				}
				if (!item_class) {
					item_class = core->get_class("FEEngine", "Item");
				}
				if (!item_behavior_parent) {
					item_behavior_parent = item_behavior_class->get_field("parent");
				}
				if (!item_parent_index) {
					item_parent_index = item_class->get_field("parentIndex");
				}
				reference<cs_object> instance = behavior->get_object();
				reference<cs_object> parent = instance->get_field(item_behavior_parent);
				uint64_t index = *(uint64_t*)parent->get_field(item_parent_index)->unbox();
				index--;
				parent->set_property(item_parent_index, &index);
			}
		}
	}
	void unit::wait() {
		this->m_can_move = false;
	}
}