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
#include <assert.h>
#include <stdint.h>
#ifndef __cplusplus
#define bool int
#endif
typedef HWND window_t;
typedef MSG msg_t;
#endif