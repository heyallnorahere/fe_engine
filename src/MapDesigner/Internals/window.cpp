#include "pch.h"
#include "window.h"
#ifdef FEENGINE_WINDOWS
#ifdef CreateWindow
#undef CreateWindow
#endif
#ifdef PeekMessage
#undef PeekMessage
#endif
constexpr auto classname = "MapDesigner";
std::map<HWND, mapdesigner::window*> windowmap;
static LRESULT __stdcall wndproc(HWND window, uint32_t message, WPARAM wparam, LPARAM lparam) {
    switch (message) {
    case WM_CREATE:
    {
        CREATESTRUCT* cs = (CREATESTRUCT*)lparam;
        windowmap[window] = (mapdesigner::window*)cs->lpCreateParams;
    }
        break;
    case WM_DESTROY:
        if (windowmap[window]->main_window) {
            PostQuitMessage(0);
        }
        break;
    }
    return DefWindowProcA(window, message, wparam, lparam);
}
EXPORT void InitWindow() {
    WNDCLASSA wc;
    memset(&wc, 0, sizeof(WNDCLASSA));
    wc.lpszClassName = classname;
    wc.hbrBackground = (HBRUSH)COLOR_WINDOW;
    wc.hCursor = LoadCursor(NULL, IDC_ARROW);
    wc.lpfnWndProc = wndproc;
    if (!RegisterClass(&wc)) {
        exit(-1);
    }
}
EXPORT mapdesigner::window* CreateWindow(const char* title, int32_t width, int32_t height, bool main_window) {
    mapdesigner::window* window = new mapdesigner::window;
    memset(&window->message, 0, sizeof(MSG));
    window->window = CreateWindowA(classname, title, WS_VISIBLE | WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT, width, height, NULL, NULL, NULL, window);
    window->main_window = main_window;
    return window;
}
EXPORT bool PeekMessage(mapdesigner::window* window) {
    return PeekMessageA(&window->message, NULL, NULL, NULL, PM_REMOVE);
}
EXPORT void RelayMessage(mapdesigner::window* window) {
    TranslateMessage(&window->message);
    DispatchMessageA(&window->message);
}
EXPORT bool ShouldClose(mapdesigner::window* window) {
    return window->message.message == WM_QUIT;
}
EXPORT void DestroyWindow_(mapdesigner::window* window) {
    DestroyWindow(window->window);
    delete window;
}
#endif