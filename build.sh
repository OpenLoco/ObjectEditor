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

# 2. Write the version to version.txt. This is read by the UI to know the current version.
echo "$version" > Gui/version.txt

# 3. Build the project for different platforms
echo "Building"
dotnet publish Gui/Gui.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime win-x64
dotnet publish Gui/Gui.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime linux-x64
dotnet publish Gui/Gui.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime osx-x64

# 4. Create the ZIP and tar archives
echo "Zipping"

pushd "Gui/bin/Release/net10.0/"

pushd "win-x64/publish"
zip -r "object-editor-$version-win-x64.zip" .
mv "object-editor-$version-win-x64.zip" ../..
popd

pushd "linux-x64/publish"
chmod +x "./ObjectEditor"
tar -jcf "object-editor-$version-linux-x64.tar" .
mv "object-editor-$version-linux-x64.tar" ../..
popd

pushd "osx-x64/publish"
chmod +x "./ObjectEditor"
tar -jcf "object-editor-$version-osx-x64.tar" .
mv "object-editor-$version-osx-x64.tar" ../..
popd

popd
echo "Build and packaging complete!"
