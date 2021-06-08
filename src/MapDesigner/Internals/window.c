#include "pch.h"
#include "window.h"
#ifdef FEENGINE_WINDOWS
#define CLASSNAME TEXT("MapDesigner")
static LRESULT __stdcall wndproc(HWND window, uint32_t message, WPARAM wparam, LPARAM lparam) {
    switch (message) {
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    }
    return DefWindowProcA(window, message, wparam, lparam);
}
EXPORT void InitWindow_() {
    WNDCLASSA wc;
    memset(&wc, 0, sizeof(WNDCLASSA));
    wc.lpszClassName = CLASSNAME;
    wc.hbrBackground = (HBRUSH)COLOR_WINDOW;
    wc.hCursor = LoadCursor(NULL, IDC_ARROW);
    wc.lpfnWndProc = wndproc;
    if (!RegisterClass(&wc)) {
        exit(-1);
    }
}
EXPORT window* CreateWindow_(const char* title, int32_t width, int32_t height, int main_window) {
    window* w = (window*)malloc(sizeof(window));
    assert(w);
    memset(w, 0, sizeof(window));
    w->window = CreateWindow(CLASSNAME, title, WS_VISIBLE | WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT, width, height, NULL, NULL, NULL, NULL);
    w->main_window = main_window;
    return w;
}
EXPORT bool PeekMessage_(window* window) {
    return PeekMessage(&window->message, NULL, 0, 0, PM_REMOVE);
}
EXPORT void RelayMessage_(window* window) {
    TranslateMessage(&window->message);
    DispatchMessage(&window->message);
}
EXPORT bool ShouldClose_(window* window) {
    return (window->message.message == WM_QUIT);
}
EXPORT void DestroyWindow_(window* window) {
    DestroyWindow(window->window);
    free(window);
}
#endif