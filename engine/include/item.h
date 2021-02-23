#pragma once
#include "reference.h"
#include <string>
#include "item_behavior.h"
namespace fe_engine {
	class unit;
	class item : public ref_counted {
	public:
		static constexpr uint32_t usable = 0b001;
		static constexpr uint32_t equipable = 0b010;
		static constexpr uint32_t weapon = 0b100;
		item(const std::string& name, uint32_t flags = 0, reference<item_behavior> itembehavior = reference<item_behavior>());
		std::string get_name() const;
		void set_name(const std::string& name);
		uint32_t get_item_flags() const;
		bool used();
		void set_used(bool used);
		bool initialized();
		void init(uint64_t index, uint64_t parent_index);
		reference<item_behavior> get_behavior();
	protected:
		std::string m_name;
		uint32_t m_flags;
		reference<item_behavior> m_behavior;
		bool m_initialized, m_used;
	};
}