#!/bin/bash

# get a list of runtimes from the dotnet command
# runtimes will be in the format of "{NAME} {VERSION} [{PATH}]" separated by newlines
RUNTIMES=$(dotnet --list-runtimes)

# get the runtime version
VERSION_PARTIAL=${RUNTIMES% *}
VERSION=${VERSION_PARTIAL##* }

RUNTIME_PATH_PARTIAL=${RUNTIMES%]*}
RUNTIME_PATH=${RUNTIME_PATH_PARTIAL##*[}

echo "Runtime version: $VERSION"
echo "Runtime base path: $RUNTIME_PATH"

export DOTNET_RUNTIME_SDK_PATH="$RUNTIME_PATH/$VERSION"
echo "Runtime path: $DOTNET_RUNTIME_SDK_PATH"