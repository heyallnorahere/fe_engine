#pragma once
#ifdef FEENGINE_WINDOWS
#ifdef UNICODE
#undef UNICODE
#endif
#include <Windows.h>
#include <CommCtrl.h>
#define EXPORT extern "C" __declspec(dllexport)
#include <mono/jit/jit.h>
#include <map>
using window_t = HWND;
using msg_t = MSG;
#endif