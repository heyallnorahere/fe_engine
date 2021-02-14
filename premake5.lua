workspace "fe_engine"
    architecture "x64"
    targetdir "build"
    configurations {
        "Debug",
        "Release"
    }
    flags {
        "MultiProcessorCompile"
    }
    startproject "entrypoint"
outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"
project "engine"
    location "engine"
    kind "StaticLib"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"
    targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "%{prj.name}/src/**.h",
        "%{prj.name}/src/**.cpp",
        "%{prj.name}/include/**.h"
    }
    includedirs {
        "%{prj.name}/include",
        "vendor/include"
    }
    links {
        "vendor/binaries/${cfg.buildcfg}/lib/*.lib"
    }
    filter "configurations:Debug"
        symbols "On"
    filter "configurations:Release"
    optimize "On"
project "entrypoint"
    location "entrypoint"
    kind "ConsoleApp"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"
    targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "%{prj.name}/**.h",
        "%{prj.name}/**.cpp"
    }
    includedirs {
        "engine/include"
    }
    links {
        "engine"
    }
    filter "configurations:Debug"
        symbols "On"
    filter "configurations:Release"
        optimize "On"
    