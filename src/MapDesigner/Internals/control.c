#include "pch.h"
#include "control.h"
#ifdef FEENGINE_WINDOWS
EXPORT control* CreateControl_(HWND window, const char* class_name, const char* text, int32_t width, int32_t height, int32_t x, int32_t y) {
    control* c = (control*)malloc(sizeof(control));
    assert(c);
    memset(c, 0, sizeof(control));
    c->window = CreateWindow(class_name, text, WS_VISIBLE | WS_CHILDWINDOW, x, y, width, height, window, NULL, NULL, NULL);
    if (!c->window) {
        fprintf(stderr, "Error: %u\n", GetLastError());
    }
    return c;
}
EXPORT HWND GetControlHandle_(control* c) {
    return c->window;
}
EXPORT void DestroyControl_(control* c) {
    DestroyWindow(c->window);
    free(c);
}
#endif