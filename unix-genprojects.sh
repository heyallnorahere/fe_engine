#!/bin/bash

if [[ "$(which csc)" != "$PWD/bin/csc" ]]; then
    echo ".NET Core has not been set up correctly for this workspace. Run \"source setup-dotnet && export PATH=\$PWD/bin:\$PATH\" to set up the environment."
    exit 1
fi

if [[ "$OSTYPE" == "linux-gnu" ]]; then
    OS_ID="linux"
elif [[ "$OSTYPE" == "darwin" ]]; then
    OS_ID="macosx"
else
    OS_ID="windows" # we assume this script is being run from a windows machine
fi

vendor/binaries/$OS_ID/premake5 gmake