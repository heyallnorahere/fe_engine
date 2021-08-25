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
print("Configuring...")
if subprocess.call([parsed_args.cmake_command, path.join(parsed_args.root_dir, "vendor", "fnalibs"), "-B", FNALIBS_BUILD_DIRECTORY, "-DCMAKE_BUILD_TYPE=Release", "-DBUILD_SHARED_LIBS=ON"]) != 0:
    exit(1)
print("Building...")
if subprocess.call([parsed_args.cmake_command, "--build", FNALIBS_BUILD_DIRECTORY, "--config", "Release"]) != 0:
    exit(1)
system = platform.system()
print("Installing...")
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
print("Finished installing FNA native libraries")