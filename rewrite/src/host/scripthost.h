#pragma once
#include "managed_objects.h"
class scripthost : public std::enable_shared_from_this<scripthost> {
public:
    scripthost();
    ~scripthost();
    std::shared_ptr<managed_assembly> get_core();
    std::shared_ptr<managed_assembly> load_assembly(const std::string& path);
    template<typename R, typename... A> void register_function(const std::string& name, R(*function)(A...));
private:
    void init();
    void shutdown();
    MonoDomain* m_domain;
    std::shared_ptr<managed_assembly> m_core;
};
template<typename R, typename ...A> inline void scripthost::register_function(const std::string& name, R(*function)(A...)) {
    mono_add_internal_call(name.c_str(), (void*)function);
}
