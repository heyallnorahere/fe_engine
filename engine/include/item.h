#pragma once
#include "reference.h"
#include <string>
namespace fe_engine {
	class unit;
	class item : public ref_counted {
	public:
		static constexpr uint32_t usable = 0b001;
		static constexpr uint32_t equipable = 0b010;
		static constexpr uint32_t weapon = 0b100;
		using on_use_proc = void(*)(unit* u);
		item(const std::string& name, uint32_t flags = 0, on_use_proc on_use = NULL);
		std::string get_name() const;
		void set_name(const std::string& name);
		uint32_t get_item_flags() const;
		on_use_proc get_on_use_proc() const;
	protected:
		std::string m_name;
		uint32_t m_flags;
		on_use_proc m_on_use;
		// todo: add global item behavior
	};
}