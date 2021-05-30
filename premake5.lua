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
newoption {
    trigger = "cs-version",
    value = "VER",
    description = "C# version to compile code as",
    default = "9.0"
}
newoption {
    trigger = "clear-mode",
    value = "MODE",
    description = "How to clear the screen each frame",
    default = "cursor-position",
    allowed = {
        { "cursor-position", "Reset the cursor position" },
        { "full-clear", "Clear every character on the screen (worse display)" }
    }
}
includedirs_table = {}
libdirs_table = {}
includedirs_table["mono"] = _OPTIONS["mono-include"]
includedirs_table["cxxopts"] = "vendor/submodules/cxxopts/include"
libdirs_table["mono"] = _OPTIONS["mono-libdir"]
cs_version = _OPTIONS["cs-version"]
dotnet_framework_version = "4.5" -- sorry, option removed
dotnet_assembly_path = "%{libdirs_table.mono}/mono"
version_table = {}
version_table["System"] = "2.0.0.0"
version_table["SystemCore"] = "3.5.0.0"
workspace "fe_engine"
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
    filter "options:clear-mode=full-clear"
        defines {
            "CLEAR_MODE_FULL_CLEAR"
        }
    filter "configurations:Debug"
        defines {
            "FEENGINE_DEBUG"
        }
        symbols "on"
    filter "configurations:Release"
        defines {
            "FEENGINE_RELEASE"
        }
        optimize "on"
outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"
include "premake/dependencies"
group "engine"
project "FEEngine"
    location "src/FEEngine"
    kind "SharedLib"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    clr "unsafe"
    targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "src/%{prj.name}/**.cs",
        _SCRIPT
    }
    links {
        "Newtonsoft.Json",
        "System"
    }
project "host"
    location "src/host"
    kind "ConsoleApp"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"
    targetname "FEEngine"
    targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "src/%{prj.name}/**.cpp",
        "src/%{prj.name}/**.h"
    }
    includedirs {
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
        '{MOVE} "%{cfg.targetdir}/FEEngine.dll" "."',
        '{MOVE} "%{cfg.targetdir}/ExampleGame.exe" "."',
        '{MOVE} "%{cfg.targetdir}/Newtonsoft.Json.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/%{dotnet_framework_version}/System.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/%{dotnet_framework_version}/System.Core.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/%{dotnet_framework_version}/System.Data.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/%{dotnet_framework_version}/System.Numerics.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/%{dotnet_framework_version}/System.Runtime.Serialization.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/%{dotnet_framework_version}/System.Xml.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/%{dotnet_framework_version}/System.Xml.Linq.dll" "."',
    }
    links {
        "FEEngine",
        "ExampleGame",
        "Newtonsoft.Json"
    }
    filter "configurations:Debug"
        targetsuffix "-d"
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
    filter "action:not gmake*"
        pchheader "pch.h"
        pchsource "src/%{prj.name}/pch.cpp"
group ""
group "tools"
project "SchemaGenerator"
    location "src/SchemaGenerator"
    kind "ConsoleApp"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("bin/" .. outputdir .. "/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
    files {
        "src/%{prj.name}/**.cs"
    }
    links {
        "FEEngine",
        "Newtonsoft.Json",
        "Newtonsoft.Json.Schema",
        "System"
    }
group ""
group "examples"
project "ExampleGame"
    location "examples/ExampleGame"
    kind "ConsoleApp"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("bin/" .. outputdir .. "/examples/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/examples/%{prj.name}")
    files {
        "examples/%{prj.name}/**.cs"
    }
    links {
        "FEEngine",
        "System"
    }
group ""