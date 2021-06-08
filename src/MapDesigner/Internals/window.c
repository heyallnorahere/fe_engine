#include "pch.h"
#ifdef FEENGINE_WINDOWS
EXPORT LRESULT __stdcall wndproc(HWND window, uint32_t message, WPARAM wparam, LPARAM lparam) {
    switch (message) {
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    }
    return DefWindowProcA(window, message, wparam, lparam);
}
#endif