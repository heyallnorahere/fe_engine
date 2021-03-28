project "discord-game-sdk"
    location "other/discord-game-sdk"
    kind "StaticLib"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"
    targetdir ("bin/" .. outputdir .. "/%{prj.name")
    objdir ("bin-int/" .. outputdir .. "/%{prj.name")
    files {
        "other/%{prj.name}/src/**.cpp",
        "other/%{prj.name}/src/**.h",
        "other/%{prj.name}/include/**.h",
    }
    includedirs {
        "other/%{prj.name}/include"
    }
    filter "system:windows"
        libdirs {
            "binaries/%{cfg.system}/other/lib"
        }
        links {
            "discord_game_sdk.dll.lib"
        }
    filter "configurations:Debug"
        symbols "On"
    filter "configurations:Release"
        optimize "On"