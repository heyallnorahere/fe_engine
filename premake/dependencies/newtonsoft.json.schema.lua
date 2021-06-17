project "Newtonsoft.Json.Schema"
    location "../../dependencies"
    kind "SharedLib"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("../../bin/" .. outputdir .. "/dependencies/%{prj.name}")
    objdir ("../../bin-int/" .. outputdir .. "/dependencies/%{prj.name}")
    files {
        "../../vendor/submodules/%{prj.name}/Src/%{prj.name}/**.cs"
    }
    excludes {
        "../../vendor/submodules/%{prj.name}/Src/%{prj.name}/obj/**.cs"
    }
    links {
        "Newtonsoft.Json"
    }
    defines {
        "HAVE_ADO_NET",
        "HAVE_BIG_INTEGER",
        "HAVE_DATE_TIME_OFFSET",
        "HAVE_FULL_REFLECTION"
    }
    filter "system:windows"
        links {
            "System",
            "System.Data",
            "System.Numerics",
            "System.Runtime.Serialization"
        }