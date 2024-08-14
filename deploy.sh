#!/bin/bash

# example usage:
# ./deploy.sh 1.2.3

# 1. Get the version from the first command-line argument
if [ -z "$1" ]; then
    echo "Error: Please provide the version as the first argument."
    exit 1
fi
version="$1"

echo "Building version $version"

# 2. Write the version to version.txt. This is purely to generate a commit on master for new version
echo "$version" > AvaGui/version.txt

# 3. Make a release commit
git add AvaGui/version.txt
git commit -m "prepare $version"
git push

## 4. Make a tag
git tag -a "$version" -m "tag $version"
git push --tags

## 5. Build the project for different platforms
echo "Building"
dotnet publish AvaGui/AvaGui.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime win-x64
dotnet publish AvaGui/AvaGui.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime linux-x64
dotnet publish AvaGui/AvaGui.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime osx-x64

# 6. Create the ZIP and tar archives
echo "Zipping"

pushd "AvaGui/bin/Release/net8.0/"

pushd "win-x64/publish"
zip -r "object-editor-$version-win-x64.zip" .
mv "object-editor-$version-win-x64.zip" ../..
popd

pushd "linux-x64/publish"
chmod +x "./ObjectEditor"
tar -jcvf "object-editor-$version-linux-x64.tar" .
mv "object-editor-$version-linux-x64.tar" ../..
popd

pushd "osx-x64/publish"
chmod +x "./ObjectEditor"
tar -jcvf "object-editor-$version-osx-x64.tar" .
mv "object-editor-$version-osx-x64.tar" ../..
popd

popd
echo "Build and packaging complete!"