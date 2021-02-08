#pragma once
#include "reference.h"
#include <string>
namespace fe_engine {
	class item : public ref_counted {
	public:
		item(const std::string& name);
		std::string get_name() const;
	private:
		std::string m_name;
		// todo: add global item behavior
	};
}