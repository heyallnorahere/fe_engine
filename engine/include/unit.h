#pragma once
#include "reference.h"
#include "_math.h"
#include "controller.h"
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
		};
		unit(const unit_stats& stats, u8vec2 pos, unit_affiliation affiliation);
		~unit();
		const unit_stats& get_stats() const;
		u8vec2 get_pos() const;
		unit_stats::stat_type get_current_hp() const;
		void update();
	private:
		unit_stats m_stats;
		u8vec2 m_pos;
		unit_stats::stat_type m_hp;
		unit_affiliation m_affiliation;
	};
}