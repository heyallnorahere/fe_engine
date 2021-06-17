#!/bin/bash

# get a list of sdks from the dotnet command
# sdks will be in the format of "{VERSION} [{PATH}]" separated by newlines
SDKS=$(dotnet --list-sdks)

# get the sdk base path
SDK_PATH_PARTIAL=${SDKS##*[}
SDK_PATH=${SDK_PATH_PARTIAL%]*}

# get the sdk version
VERSION_PARTIAL=${SDKS##*"\n"}
VERSION=${VERSION_PARTIAL% *}

export DOTNET_SDK_PATH="$SDK_PATH/$VERSION"

# export environment variable for assembly locations
ASSEMBLY_PATHS=$(find /usr/share/dotnet/packs/Microsoft.NETCore.App.Ref -maxdepth 3 -name net*)
readarray -t strarr <<< "$ASSEMBLY_PATHS"
PATH_COUNT=${#strarr[*]}
INDEX=$(($PATH_COUNT-1))
export DOTNET_ASSEMBLY_PATH="${strarr[$INDEX]}"

echo ".NET Core assembly path: $DOTNET_ASSEMBLY_PATH"
echo "SDK version: $VERSION"
echo "SDK base path: $SDK_PATH"

echo "SDK path: $DOTNET_SDK_PATH"

# set up proxy shell script
echo "dotnet \"$DOTNET_SDK_PATH/Roslyn/bincore/csc.dll\" \"\$@\"" > "bin/csc"
chmod +x bin/csc
