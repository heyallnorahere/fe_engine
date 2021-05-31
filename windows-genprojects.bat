@echo off
call "vendor/binaries/windows/premake5.exe" vs2019 --fatal %*
pause