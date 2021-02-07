cmake_minimum_required (VERSION 3.8)
include_directories ("include")
file (GLOB sourcefiles "src/*.cpp" "src/*.h")
add_library (engine STATIC ${sourcefiles})