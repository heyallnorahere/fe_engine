@echo off
git submodule update --init --recursive
call "vendor/binaries/windows/premake5.exe" vs2019 --fatal %*
pause