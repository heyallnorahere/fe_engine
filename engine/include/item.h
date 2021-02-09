#pragma once
#include "reference.h"
#include <string>
namespace fe_engine {
	class unit;
	class item : public ref_counted {
	public:
		static constexpr unsigned int usable = 0b001;
		static constexpr unsigned int equipable = 0b010;
		static constexpr unsigned int weapon = 0b100;
		using on_use_proc = void(*)(unit* u);
		item(const std::string& name, unsigned int flags = 0, on_use_proc on_use = NULL);
		std::string get_name() const;
		unsigned int get_item_flags() const;
		on_use_proc get_on_use_proc() const;
	protected:
		std::string m_name;
		unsigned int m_flags;
		on_use_proc m_on_use;
		// todo: add global item behavior
	};
}