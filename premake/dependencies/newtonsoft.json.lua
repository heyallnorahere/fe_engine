project "Newtonsoft.Json"
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
