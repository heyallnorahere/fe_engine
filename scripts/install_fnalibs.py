import subprocess
import os.path as path
import argparse
import platform
import glob
import shutil
argparser = argparse.ArgumentParser()
argparser.add_argument("-r", "--root-dir", default=".")
argparser.add_argument("-c", "--cmake-command", default="cmake")
parsed_args = argparser.parse_args()
FNALIBS_BUILD_DIRECTORY="fnalibs-build"
def build_binary(path_: str):
    print(f"Configuring {path_}...")
    if subprocess.call([parsed_args.cmake_command, path_, "-B", FNALIBS_BUILD_DIRECTORY, "-DCMAKE_BUILD_TYPE=Release", "-DBUILD_SHARED_LIBS=ON"]) != 0:
        exit(1)
    print(f"Building {path_}...")
    if subprocess.call([parsed_args.cmake_command, "--build", FNALIBS_BUILD_DIRECTORY, "--config", "Release"]) != 0:
        exit(1)
    system = platform.system()
    print(f"Installing {path_}...")
    if system == "Linux":
        if subprocess.call(["sudo", parsed_args.cmake_command, "--install", FNALIBS_BUILD_DIRECTORY]) != 0:
            exit(1)
    else:
        extensions = {
            "Windows": "dll",
            "Darwin": "dylib"
        }
        glob_path = path.join(FNALIBS_BUILD_DIRECTORY, "**", f"*.{extensions[system]}")
        for file in glob.glob(glob_path, recursive=True):
            shutil.copy(file, parsed_args.root_dir)
    print(f"Finished installing {path_}")
    shutil.rmtree(FNALIBS_BUILD_DIRECTORY)
def get_path(library_name: str):
    return path.join(parsed_args.root_dir, "vendor", "fnalibs", library_name)
if platform.system() == "Linux":
    build_binary(get_path("SDL"))
    build_binary(get_path("FAudio"))
    build_binary(get_path("FNA3D"))
else:
    build_binary(get_path("."))