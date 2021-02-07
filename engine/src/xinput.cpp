#ifdef _WIN32
#include "controller.h"
#define _AMD64_
#include <Xinput.h>
#include <Windows.h>
bool is_controller_connected(size_t index) {
	XINPUT_STATE state;
	ZeroMemory(&state, sizeof(XINPUT_STATE));
	DWORD r = XInputGetState(index, &state);
	return (r == ERROR_SUCCESS);
}
fe_engine::controller::buttons get_controller_state(size_t index) {
	fe_engine::controller::buttons b;
	XINPUT_STATE state;
	ZeroMemory(&state, sizeof(XINPUT_STATE));
	XInputGetState(index, &state);
	b.a.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_A;
	b.b.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_B;
	b.x.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_X;
	b.y.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_Y;
	b.lb.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_LEFT_SHOULDER;
	b.rb.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_RIGHT_SHOULDER;
	b.ls.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_LEFT_THUMB;
	b.rs.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_RIGHT_THUMB;
	b.up.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_UP;
	b.down.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_DOWN;
	b.left.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_LEFT;
	b.right.held = state.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_RIGHT;
	return b;
}
#endif