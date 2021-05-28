local function get_windows_path_stub(architecture)
    local path = "C:/Program Files"
    if architecture == "x86" or architecture == "x32" then
        path = path .. " (x86)"
    end
    return path
end
local function get_unix_path_stub()
    local path = "/usr"
    if os.istarget("macosx") then
        path = path .. "/local"
    end
    return path;
end
function determine_mono_include(architecture)
    if os.istarget("windows") then
        return get_windows_path_stub(architecture) .. "/Mono/include/mono-2.0"
    else
        return get_unix_path_stub() .. "/include/mono-2.0"
    end
end
function determine_mono_libdir(architecture)
    if os.istarget("windows") then
        return get_windows_path_stub(architecture) .. "/Mono/lib"
    else
        return get_unix_path_stub() .. "/lib"
    end
end