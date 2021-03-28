#pragma once
#include "reference.h"
#include <cstdint>
#include <string>
namespace fe_engine {
	namespace internal {
		struct discord_app_data;
	}
	struct discord_activity {
		enum class activity_type {
			playing,
			streaming,
			listening,
			watching,
		};
		activity_type type = activity_type::playing;
		std::string state, details;
	};
	class discord_app : public ref_counted {
	public:
		discord_app(int64_t app_id);
		~discord_app();
		void update();
		void update_activity(const discord_activity& activity);
		void clear_activity();
	private:
		internal::discord_app_data* m_data;
	};
}