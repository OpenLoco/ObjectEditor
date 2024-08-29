#!/bin/bash

# example usage:
# ./build.sh 1.2.3

# 1. Get the version from the first command-line argument
if [ -z "$1" ]; then
    echo "Error: Please provide the version as the first argument."
    exit 1
fi
version="$1"

echo "Building version $version"

dotnet publish ObjectService/ObjectService.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime linux-x64

echo "Build and packaging complete!"