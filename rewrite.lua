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
        "System.Core",
        "System.Data",
        "System.Numerics",
        "System.Runtime.Serialization",
        "System.Xml",
        "System.Xml.Linq"
    }
    defines {
        "HAVE_ADO_NET",
        "HAVE_APP_DOMAIN",
        "HAVE_ASYNC",
        "HAVE_BIG_INTEGER",
        "HAVE_BINARY_FORMATTER",
        "HAVE_BINARY_SERIALIZATION",
        "HAVE_BINARY_EXCEPTION_SERIALIZATION",
        "HAVE_CAS",
        "HAVE_CHAR_TO_LOWER_WITH_CULTURE",
        "HAVE_CHAR_TO_STRING_WITH_CULTURE",
        "HAVE_COM_ATTRIBUTES",
        "HAVE_COMPONENT_MODEL",
        "HAVE_CONCURRENT_COLLECTIONS",
        "HAVE_COVARIANT_GENERICS",
        "HAVE_DATA_CONTRACTS",
        "HAVE_DATE_TIME_OFFSET",
        "HAVE_DB_NULL_TYPE_CODE",
        "HAVE_DYNAMIC",
        "HAVE_EMPTY_TYPES",
        "HAVE_ENTITY_FRAMEWORK",
        "HAVE_EXPRESSIONS",
        "HAVE_FAST_REVERSE",
        "HAVE_FSHARP_TYPES",
        "HAVE_FULL_REFLECTION",
        "HAVE_GUID_TRY_PARSE",
        "HAVE_HASH_SET",
        "HAVE_ICLONEABLE",
        "HAVE_ICONVERTIBLE",
        "HAVE_IGNORE_DATA_MEMBER_ATTRIBUTE",
        "HAVE_INOTIFY_COLLECTION_CHANGED",
        "HAVE_INOTIFY_PROPERTY_CHANGING",
        "HAVE_ISET",
        "HAVE_LINQ",
        "HAVE_MEMORY_BARRIER",
        "HAVE_METHOD_IMPL_ATTRIBUTE",
        "HAVE_NON_SERIALIZED_ATTRIBUTE",
        "HAVE_READ_ONLY_COLLECTIONS",
        "HAVE_REFLECTION_EMIT",
        "HAVE_REGEX_TIMEOUTS",
        "HAVE_SECURITY_SAFE_CRITICAL_ATTRIBUTE",
        "HAVE_SERIALIZATION_BINDER_BIND_TO_NAME",
        "HAVE_STREAM_READER_WRITER_CLOSE",
        "HAVE_STRING_JOIN_WITH_ENUMERABLE",
        "HAVE_TIME_SPAN_PARSE_WITH_CULTURE",
        "HAVE_TIME_SPAN_TO_STRING_WITH_CULTURE",
        "HAVE_TIME_ZONE_INFO",
        "HAVE_TRACE_WRITER",
        "HAVE_TYPE_DESCRIPTOR",
        "HAVE_UNICODE_SURROGATE_DETECTION",
        "HAVE_VARIANT_TYPE_PARAMETERS",
        "HAVE_VERSION_TRY_PARSE",
        "HAVE_XLINQ",
        "HAVE_XML_DOCUMENT",
        "HAVE_XML_DOCUMENT_TYPE",
        "HAVE_CONCURRENT_DICTIONARY"
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
        "System.Numerics",
        "System.Runtime.Serialization"
    }
    defines {
        "HAVE_ADO_NET",
        "HAVE_BIG_INTEGER",
        "HAVE_DATE_TIME_OFFSET",
        "HAVE_FULL_REFLECTION"
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
        "Newtonsoft.Json"
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
        pchsource "rewrite/src/%{prj.name}/pch.cpp"
group ""
group "tools"
project "SchemaGenerator"
    location "rewrite/src/SchemaGenerator"
    kind "ConsoleApp"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("bin/" .. outputdir .. "/rewrite/%{prj.name}")
    objdir ("bin-int/" .. outputdir .. "/rewrite/%{prj.name}")
    files {
        "rewrite/src/%{prj.name}/**.cs"
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