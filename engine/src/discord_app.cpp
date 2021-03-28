#include "discord_app.h"
#include <discord.h>
#include "discord_app_data.h"
static void discord_result_callback(discord::Result result) { }
namespace fe_engine {
	discord_app::discord_app(int64_t app_id) {
#ifdef FEENGINE_WINDOWS
		this->m_data = new internal::discord_app_data;
		discord::Core::Create(app_id, DiscordCreateFlags_Default, &this->m_data->core);
#endif
	}
	discord_app::~discord_app() {
#ifdef FEENGINE_WINDOWS
		delete this->m_data->core;
		delete this->m_data;
#endif
	}
	void discord_app::update() {
#ifdef FEENGINE_WINDOWS
		this->m_data->core->RunCallbacks();
#endif
	}
	void discord_app::update_activity(const discord_activity& activity) {
#ifdef FEENGINE_WINDOWS
		discord::Activity _activity{};
		_activity.SetType((discord::ActivityType)activity.type);
		_activity.SetState(activity.state.c_str());
		_activity.SetDetails(activity.details.c_str());
		this->m_data->core->ActivityManager().UpdateActivity(_activity, discord_result_callback);
#endif
	}
	void discord_app::clear_activity() {
#ifdef FEENGINE_WINDOWS
		this->m_data->core->ActivityManager().ClearActivity(discord_result_callback);
#endif
	}
}