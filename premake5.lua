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
        "%{prj.name}/include"
    }
    sysincludedirs {
        "vendor/include/%{cfg.system}"
    }
    filter "system:windows"
        links {
            "vendor/binaries/windows/%{cfg.buildcfg}/lib/*.lib"
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
    sysincludedirs {
        "engine/include"
    }
    links {
        "engine",
        "scriptcore",
        "sample-scripts"
    }
    postbuildcommands {
        '{COPY} "%{cfg.targetdir}/../scriptcore/scriptcore.dll" "script-assemblies/"',
        '{COPY} "%{cfg.targetdir}/../sample-scripts/sample-scripts.dll" "script-assemblies/"'
    }
    filter "configurations:Debug"
        symbols "On"
        defines {
            "DEBUG_ENABLED"
        }
    filter "configurations:Release"
        optimize "On"
    filter "system:windows"
        prelinkcommands { 'del /q "script-assemblies\\*.dll"' }
        postbuildcommands {
            '{COPY} "../vendor/binaries/%{cfg.buildcfg}/bin/*.dll" "%{cfg.targetdir}"'
        }
    filter "system:macosx"
        links {
            "monosgen-2.0",
            "z"
        }
        syslibdirs {
            "/usr/local/lib",
            "/usr/local/opt/zlib/lib"
        }

    