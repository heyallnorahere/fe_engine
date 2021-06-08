#ifndef WINDOW_H
#define WINDOW_H
typedef struct window_ {
    window_t window;
    msg_t message;
    int main_window;
} window;
#endif