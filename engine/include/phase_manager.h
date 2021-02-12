#pragma once
#include "unit.h"
#include "map.h"
namespace fe_engine {
	class phase_manager : public ref_counted {
	public:
		phase_manager();
		unit_affiliation get_current_phase() const;
		void cycle_phase(reference<map> m);
	private:
		unit_affiliation m_current_phase; // why NOT use the affiliation enum
		void cycle_enum();
	};
}