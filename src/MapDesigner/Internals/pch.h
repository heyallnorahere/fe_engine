#pragma once
#ifdef FEENGINE_WINDOWS
#ifdef UNICODE
#undef UNICODE
#endif
#include <Windows.h>
#include <CommCtrl.h>
#define EXPORT __declspec(dllexport)
#include <mono/jit/jit.h>
#include <malloc.h>
typedef HWND window_t;
typedef MSG msg_t;
#endif