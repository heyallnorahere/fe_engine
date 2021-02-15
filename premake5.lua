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
        "vendor/binaries/%{cfg.buildcfg}/lib/*.lib"
    }
    filter "configurations:Debug"
        symbols "On"
    filter "configurations:Release"
        optimize "On"
project "scriptcore"
    location "scriptcore"
    kind "SharedLib"
    language "C#"
    targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "%{prj.name}/src/**.cs"
    }
    postbuildcommands {
        '{COPY} "%{cfg.targetdir}/%{prj.name}.dll" "../entrypoint/script-assemblies"'
    }
project "sample-scripts"
    location "sample-scripts"
    kind "SharedLib"
    language "C#"
        targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "%{prj.name}/src/**.cs"
    }
    links {
        "scriptcore"
    }
    postbuildcommands {
    }
project "entrypoint"
    location "entrypoint"
    kind "ConsoleApp"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"
    targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "%{prj.name}/src/**.h",
        "%{prj.name}/src/**.cpp"
    }
    includedirs {
        "engine/include"
    }
    links {
        "engine",
        "scriptcore",
        "sample-scripts"
    }
    postbuildcommands {
        '{COPY} "../vendor/binaries/%{cfg.buildcfg}/bin/*.dll" "%{cfg.targetdir}"',
        '{COPY} "%{cfg.targetdir}/../scriptcore/scriptcore.dll" "script-assemblies/"',
        '{COPY} "%{cfg.targetdir}/../sample-scripts/sample-scripts.dll" "script-assemblies/"'
    }
    filter "configurations:Debug"
        symbols "On"
    filter "configurations:Release"
        optimize "On"
    