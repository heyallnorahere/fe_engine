newoption {
    trigger = "architecture",
    value = "ARCH",
    description = "The architecture to target",
    default = "x64"
}
architecture_ = _OPTIONS["architecture"]
include "premake/mono.lua"
default_mono_includedir = determine_mono_include(architecture_)
default_mono_libdir = determine_mono_libdir(architecture_)
newoption {
    trigger = "mono-include",
    value = "PATH",
    description = "The path to the Mono include directory",
    default = (default_mono_includedir)
}
newoption {
    trigger = "mono-libdir",
    value = "PATH",
    description = "The path to the Mono library directory",
    default = (default_mono_libdir)
}
includedirs_table = {}
libdirs_table = {}
includedirs_table["mono"] = _OPTIONS["mono-include"]
includedirs_table["cxxopts"] = "vendor/submodules/cxxopts/include"
libdirs_table["mono"] = _OPTIONS["mono-libdir"]
workspace "fe_engine-rewrite"
    architecture (architecture_)
    targetdir "bin"
    configurations {
        "Debug",
        "Release"
    }
    flags {
        "MultiProcessorCompile"
    }
    startproject "host"
    filter "system:windows"
        defines {
            "FEENGINE_WINDOWS"
        }
    filter "system:macosx"
        defines {
            "FEENGINE_MACOSX"
        }
    filter "system:linux"
        defines {
            "FEENGINE_LINUX"
        }
    filter "configurations:Debug"
        defines {
            "FEENGINE_DEBUG"
        }
    filter "configurations:Release"
        defines {
            "FEENGINE_RELEASE"
        }
outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"
group "engine"
project "FEEngine"
    location "rewrite/src/FEEngine"
    kind "SharedLib"
    language "C#"
    targetdir ("bin/" .. outputdir .. "/rewrite/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/%{prj.name}")
    files {
        "rewrite/src/%{prj.name}/**.cs"
    }
project "host"
    location "rewrite/src/host"
    kind "ConsoleApp"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"
    targetname "FEEngine"
    targetdir ("bin/" .. outputdir .. "/rewrite/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/%{prj.name}")
    files {
        "rewrite/src/%{prj.name}/**.cpp",
        "rewrite/src/%{prj.name}/**.h",
        _SCRIPT
    }
    pchheader "pch.h"
    pchsource "rewrite/src/%{prj.name}/pch.cpp"
    sysincludedirs {
        "%{includedirs_table.mono}",
        "%{includedirs_table.cxxopts}",
    }
    libdirs {
        "%{libdirs_table.mono}",
    }
    defines {
        'MONO_CS_LIBDIR="%{libdirs_table.mono}"'
    }
    postbuildcommands {
        '{COPY} "%{cfg.targetdir}/../FEEngine/FEEngine.dll" "."',
        '{COPY} "%{cfg.targetdir}/../examples/ExampleGame/ExampleGame.dll" "."'
    }
    dependson {
        "FEEngine",
        "ExampleGame"
    }
    filter "system:windows"
        links {
            "mono-2.0-sgen.lib"
        }
        defines {
            "_CRT_SECURE_NO_WARNINGS"
        }
        postbuildcommands {
            '{COPY} "%{libdirs_table.mono}/../bin/mono-2.0-sgen.dll" "%{cfg.targetdir}"',
        }
    filter "system:not windows"
        links {
            "monosgen-2.0"
        }
    filter "system:macosx"
        links {
            "z"
        }
        libdirs {
            "/usr/local/opt/zlib/lib"
        }
    filter "system:linux"
        links {
            "pthread",
            "stdc++fs"
        }
group ""
group "examples"
project "ExampleGame"
    location "rewrite/examples/ExampleGame"
    kind "SharedLib"
    language "C#"
    targetdir ("bin/" .. outputdir .. "/rewrite/examples/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/examples/%{prj.name}")
    files {
        "rewrite/examples/%{prj.name}/**.cs"
    }
    links {
        "FEEngine"
    }
group ""