#include "pch.h"
#include "renderer.h"
#ifdef FEENGINE_WINDOWS
#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <Windows.h>
#define BITFLAG(bits) (1 << bits)
static void write_char_windows(char character, int32_t color) {
    HANDLE console = GetStdHandle(STD_OUTPUT_HANDLE);
    DWORD attrib = 0;
    if (color & BITFLAG(0)) {
        attrib |= FOREGROUND_RED;
    }
    if (color & BITFLAG(1)) {
        attrib |= FOREGROUND_GREEN;
    }
    if (color & BITFLAG(2)) {
        attrib |= FOREGROUND_BLUE;
    }
    SetConsoleTextAttribute(console, attrib);
    WriteConsoleA(console, &character, 1, NULL, NULL);
}
static void clear_screen_windows() {
    HANDLE console = GetStdHandle(STD_OUTPUT_HANDLE);
#ifdef CLEAR_MODE_FULL_CLEAR
    CONSOLE_SCREEN_BUFFER_INFO screen;
    DWORD written;
    GetConsoleScreenBufferInfo(console, &screen);
    FillConsoleOutputCharacterA(console, ' ', screen.dwSize.X * screen.dwSize.Y, { 0, 0 }, &written);
    FillConsoleOutputAttribute(console, FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE, screen.dwSize.X * screen.dwSize.Y, { 0, 0 }, &written);
#endif
    SetConsoleCursorPosition(console, { 0, 0 });
}
static void disable_cursor_windows() {
    CONSOLE_CURSOR_INFO info;
    info.dwSize = 20;
    info.bVisible = false;
    SetConsoleCursorInfo(GetStdHandle(STD_OUTPUT_HANDLE), &info);
}
#define write_char write_char_windows
#define clear_screen clear_screen_windows
#define disable_cursor disable_cursor_windows
#else
static void write_char_unix(char character, int32_t color) {
    std::cout << "\033[" << color << "m" << character << std::flush;
}
static void clear_screen_unix() {
    std::cout << "\033[2J\033[H" << std::flush;
}
static void disable_cursor_unix() {
    std::cout << "\033[?1c" << std::flush;
}
#define write_char write_char_unix
#define clear_screen clear_screen_unix
#define disable_cursor disable_cursor_unix
#endif
struct pair {
    char character;
    int32_t color;
};
std::vector<pair> text;
void Renderer_WriteColoredChar(char character, int32_t color) {
    text.push_back({ character, color });
}
void Renderer_ClearNativeBuffer() {
    text.clear();
}
void Renderer_Present() {
    clear_screen();
    for (const auto& p : text) {
        write_char(p.character, p.color);
    }
}
void Renderer_DisableCursor() {
    disable_cursor();
}