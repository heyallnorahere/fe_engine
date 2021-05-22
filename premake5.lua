newoption {
    trigger = "pathfinder",
    value = "algorithm",
    description = "Choose an algorithm for pathfinding",
    default = "astar",
    allowed = {
        { "astar", "AStar" }
    }
}
newoption {
    trigger = "architecture",
    value = "ARCH",
    description = "Architecture to target",
    default = "x64"
}
workspace "fe_engine"
    architecture (_OPTIONS["architecture"])
    targetdir "build"
    configurations {
        "Debug",
        "Release"
    }
    flags {
        "MultiProcessorCompile"
    }
    startproject "entrypoint"
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
        "%{prj.name}/include/**.h",
    }
    includedirs {
        "%{prj.name}/include"
    }
    sysincludedirs {
        "vendor/include",
        "vendor/submodules/json/include",
        "vendor/other/discord-game-sdk/include"
    }
    filter "system:windows"
        files {
            "vendor/other/discord-game-sdk/src/**.cpp",
            "vendor/other/discord-game-sdk/src/**.h",
            "vendor/other/discord-game-sdk/include/**.h",    
        }
        links {
            "vendor/binaries/windows/%{cfg.buildcfg}/lib/*.lib",
            "vendor/binaries/%{cfg.system}/other/lib/*.lib"
        }
    filter "options:pathfinder=astar"
        defines {
            "FEENGINE_PATHFINDING_ALGORITHM_ASTAR"
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
group ""
group "examples"
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
        "%{prj.name}/src/**.cpp",
        "%{prj.name}/data/**.json"
    }
    sysincludedirs {
        "engine/include",
        "vendor/submodules/json/include"
    }
    links {
        "engine"
    }
    dependson {
        "scriptcore",
        "sample-scripts"
    }
    filter "configurations:Debug"
        symbols "On"
    filter "configurations:Release"
        optimize "On"
    filter "system:windows"
        prelinkcommands {
            'del /q "script-assemblies\\*.dll"',
        }
        postbuildcommands {
            '{COPY} "../vendor/binaries/windows/%{cfg.buildcfg}/bin/*.dll" "%{cfg.targetdir}\\"',
            '{COPY} "../vendor/binaries/windows/other/bin/*.dll" "%{cfg.targetdir}\\"',
            '{COPY} "%{cfg.targetdir}/../scriptcore/scriptcore.dll" "script-assemblies\\"',
            '{COPY} "%{cfg.targetdir}/../sample-scripts/sample-scripts.dll" "script-assemblies\\"'
        }
    filter "system:not windows"
        postbuildcommands {
            '{COPY} "%{cfg.targetdir}/../scriptcore/scriptcore.dll" "script-assemblies/"',
            '{COPY} "%{cfg.targetdir}/../sample-scripts/sample-scripts.dll" "script-assemblies/"'
        }
    filter "system:not windows"
        links {
            "monosgen-2.0"
        }
        libdirs {
            "/usr/local/lib",
        }
    filter "system:macosx"
        links {
            "z"
        }
        libdirs {
            "/usr/local/opt/zlib/lib"
        }
    filter "system:linux"
        linkoptions {
            "-pthread"
        }
        links {
            "stdc++fs"
        }
group ""