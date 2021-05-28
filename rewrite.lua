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
    trigger = "framework-version",
    value = "VER",
    description = ".NET Framework version to use",
    default = "3.5"
}
includedirs_table = {}
libdirs_table = {}
includedirs_table["mono"] = _OPTIONS["mono-include"]
includedirs_table["cxxopts"] = "vendor/submodules/cxxopts/include"
libdirs_table["mono"] = _OPTIONS["mono-libdir"]
cs_version = _OPTIONS["cs-version"]
dotnet_framework_version = _OPTIONS["framework-version"]
dotnet_assembly_path = "%{libdirs_table.mono}/mono"
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
        symbols "on"
    filter "configurations:Release"
        defines {
            "FEENGINE_RELEASE"
        }
        optimize "on"
outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"
group "dependencies"
project "Newtonsoft.Json"
    location "rewrite/dependencies"
    kind "SharedLib"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("bin/" .. outputdir .. "/rewrite/dependencies/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/dependencies/%{prj.name}")
    files {
        "vendor/submodules/%{prj.name}/Src/%{prj.name}/**.cs"
    }
    excludes {
        "vendor/submodules/%{prj.name}/Src/%{prj.name}/obj/**.cs"
    }
    links {
        "System",
        "System.Data",
        "System.Runtime.Serialization",
        "System.Xml",
        "System.Xml.Linq"
    }
    filter "options:framework-version=3.5"
        defines {
            "NET35",
            "HAVE_ADO_NET",
            "HAVE_APP_DOMAIN",
            "HAVE_BINARY_FORMATTER",
            "HAVE_BINARY_SERIALIZATION",
            "HAVE_BINARY_EXCEPTION_SERIALIZATION",
            "HAVE_CAS",
            "HAVE_CHAR_TO_STRING_WITH_CULTURE",
            "HAVE_CHAR_TO_LOWER_WITH_CULTURE",
            "HAVE_COM_ATTRIBUTES",
            "HAVE_COMPONENT_MODEL",
            "HAVE_DATA_CONTRACTS",
            "HAVE_DATE_TIME_OFFSET",
            "HAVE_DB_NULL_TYPE_CODE",
            "HAVE_EMPTY_TYPES",
            "HAVE_ENTITY_FRAMEWORK",
            "HAVE_FAST_REVERSE",
            "HAVE_FULL_REFLECTION",
            "HAVE_HASH_SET",
            "HAVE_ICLONEABLE",
            "HAVE_ICONVERTIBLE",
            "HAVE_INOTIFY_PROPERTY_CHANGING",
            "HAVE_LINQ",
            "HAVE_MEMORY_BARRIER",
            "HAVE_NON_SERIALIZED_ATTRIBUTE",
            "HAVE_REFLECTION_EMIT",
            "HAVE_STREAM_READER_WRITER_CLOSE",
            "HAVE_TIME_ZONE_INFO",
            "HAVE_TRACE_WRITER",
            "HAVE_TYPE_DESCRIPTOR",
            "HAVE_UNICODE_SURROGATE_DETECTION",
            "HAVE_XLINQ",
            "HAVE_XML_DOCUMENT",
            "HAVE_XML_DOCUMENT_TYPE"
        }
    filter "options:framework-version=2.0"
        defines {
            "NET20",
            "HAVE_ADO_NET",
            "HAVE_APP_DOMAIN",
            "HAVE_BINARY_FORMATTER",
            "HAVE_BINARY_SERIALIZATION",
            "HAVE_BINARY_EXCEPTION_SERIALIZATION",
            "HAVE_CAS",
            "HAVE_CHAR_TO_LOWER_WITH_CULTURE",
            "HAVE_CHAR_TO_STRING_WITH_CULTURE",
            "HAVE_COM_ATTRIBUTES",
            "HAVE_COMPONENT_MODEL",
            "HAVE_DB_NULL_TYPE_CODE",
            "HAVE_EMPTY_TYPES",
            "HAVE_FAST_REVERSE",
            "HAVE_FULL_REFLECTION",
            "HAVE_ICLONEABLE",
            "HAVE_ICONVERTIBLE",
            "HAVE_MEMORY_BARRIER",
            "HAVE_NON_SERIALIZED_ATTRIBUTE",
            "HAVE_REFLECTION_EMIT",
            "HAVE_STREAM_READER_WRITER_CLOSE",
            "HAVE_TRACE_WRITER",
            "HAVE_TYPE_DESCRIPTOR",
            "HAVE_UNICODE_SURROGATE_DETECTION",
            "HAVE_XML_DOCUMENT",
            "HAVE_XML_DOCUMENT_TYPE"
        }
project "Newtonsoft.Json.Schema"
    location "rewrite/dependencies"
    kind "SharedLib"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("bin/" .. outputdir .. "/rewrite/dependencies/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/dependencies/%{prj.name}")
    files {
        "vendor/submodules/%{prj.name}/Src/%{prj.name}/**.cs"
    }
    excludes {
        "vendor/submodules/%{prj.name}/Src/%{prj.name}/obj/**.cs"
    }
    links {
        "Newtonsoft.Json",
        "System",
        "System.Data",
        "System.Runtime.Serialization"
    }
    filter "options:framework-version=3.5"
        defines {
            "HAVE_ADO_NET",
            "HAVE_DATE_TIME_OFFSET",
            "HAVE_FULL_REFLECTION",
            "NET35"
        }
group ""
group "engine"
project "FEEngine"
    location "rewrite/src/FEEngine"
    kind "SharedLib"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    clr "unsafe"
    targetdir ("bin/" .. outputdir .. "/rewrite/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/%{prj.name}")
    files {
        "rewrite/src/%{prj.name}/**.cs",
        _SCRIPT
    }
    links {
        "Newtonsoft.Json",
        "Newtonsoft.Json.Schema"
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
        "rewrite/src/%{prj.name}/**.h"
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
        '{COPY} "%{dotnet_assembly_path}/2.0-api/System.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/2.0-api/System.Data.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/2.0-api/System.Runtime.Serialization.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/2.0-api/System.Xml.dll" "."',
        '{COPY} "%{dotnet_assembly_path}/2.0-api/System.Xml.Linq.dll" "."'
    }
    links {
        "FEEngine",
        "ExampleGame",
        "Newtonsoft.Json",
        "Newtonsoft.Json.Schema"
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
        pchsource "rewrite/src/%{prj.name}/pch.cpp"
group ""
group "examples"
project "ExampleGame"
    location "rewrite/examples/ExampleGame"
    kind "ConsoleApp"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("bin/" .. outputdir .. "/rewrite/examples/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/examples/%{prj.name}")
    files {
        "rewrite/examples/%{prj.name}/**.cs"
    }
    links {
        "FEEngine",
        "System"
    }
group ""