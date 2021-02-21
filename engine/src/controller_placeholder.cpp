#if !defined(FEENGINE_WINDOWS)
#include "controller.h"
bool is_controller_connected(size_t index) {
    return true;
}
fe_engine::controller::buttons get_controller_state(size_t index) {
    fe_engine::controller::buttons b;
    memset(&b, 0, sizeof(fe_engine::controller::buttons));
    return b;
}
#endif
