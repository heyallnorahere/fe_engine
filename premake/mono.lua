local function get_windows_path_stub(architecture)
    local path = "C:/Program Files"
    if architecture == "x86" or architecture == "x32" then
        path = path .. " (x86)"
    end
    return path
end
local mono_framework_path = "/Library/Frameworks/Mono.Framework"
function determine_mono_include(architecture)
    if os.istarget("windows") then
        return get_windows_path_stub(architecture) .. "/Mono/include/mono-2.0"
    elseif os.istarget("macosx") then
        return mono_framework_path .. "/Headers/mono-2.0"
    else
        return "/usr/include/mono-2.0"
    end
end
function determine_mono_libdir(architecture)
    if os.istarget("windows") then
        return get_windows_path_stub(architecture) .. "/Mono/lib"
    elseif os.istarget("macosx") then
        return mono_framework_path .. "/Libraries"
    else
        return "/usr/lib"
    end
end