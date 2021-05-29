project "Newtonsoft.Json.Schema"
    location "../../rewrite/dependencies"
    kind "SharedLib"
    language "C#"
    csversion (cs_version)
    framework (dotnet_framework_version)
    targetdir ("../../bin/" .. outputdir .. "/rewrite/dependencies/%{prj.name}")
    objdir ("../../bin-int/" .. outputdir .. "/rewrite/dependencies/%{prj.name}")
    files {
        "../../vendor/submodules/%{prj.name}/Src/%{prj.name}/**.cs"
    }
    excludes {
        "../../vendor/submodules/%{prj.name}/Src/%{prj.name}/obj/**.cs"
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
