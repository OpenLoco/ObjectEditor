#!/bin/bash

# example usage:
# ./build.sh 1.2.3

# immediately abort script on error
set -e

# Define color variables for readability
FG_BLUE=$(tput setaf 39)
BG_BLUE=$(tput setab 39)
FG_GREEN=$(tput setaf 46)
BG_GREEN=$(tput setab 46)
RESET=$(tput sgr0)


# 1. Get the version from the first command-line argument
if [ -z "$1" ]; then
    echo "Error: Please provide the version as the first argument."
    exit 1
fi

version="$1"

echo "=== Building Object Editor v${FG_BLUE}$version${RESET} for ${FG_BLUE}$framework${RESET} ==="

# read .net version from csproj file
framework=$(grep '<TargetFramework>' Gui/Gui.csproj | sed 's/.*<TargetFramework>\(.*\)<\/TargetFramework>.*/\1/')

# 2. Write the version to version.txt. This is read by the UI to know the current version.
echo "$version" > Gui/version.txt

# 3. Build the editor for different platforms
echo "Building the ${FG_BLUE}Editor${RESET}"
dotnet publish Gui/Gui.csproj -c Release -p:WarningLevel=0 -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime win-x64
dotnet publish Gui/Gui.csproj -c Release -p:WarningLevel=0 -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime linux-x64
dotnet publish Gui/Gui.csproj -c Release -p:WarningLevel=0 -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime osx-x64

echo "Building the ${FG_BLUE}Updater${RESET}"
dotnet publish GuiUpdater/GuiUpdater.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime win-x64
dotnet publish GuiUpdater/GuiUpdater.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime linux-x64
dotnet publish GuiUpdater/GuiUpdater.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime osx-x64

# 4. Copy updater into release folders
echo "Copying ${FG_BLUE}Updater${RESET} files into ${FG_BLUE}Gui${RESET} folders"
cp GuiUpdater/bin/Release/$framework/win-x64/publish/* Gui/bin/Release/$framework/win-x64/publish
cp GuiUpdater/bin/Release/$framework/linux-x64/publish/* Gui/bin/Release/$framework/linux-x64/publish
cp GuiUpdater/bin/Release/$framework/osx-x64/publish/* Gui/bin/Release/$framework/osx-x64/publish

# 5. Create the zip and tar archives
pushd "Gui/bin/Release/$framework/"

echo "Zipping ${FG_BLUE}win-x64${RESET}"
pushd "win-x64/publish"
zip -r "object-editor-$version-win-x64.zip" .
mv "object-editor-$version-win-x64.zip" ../../..
popd

echo "Zipping ${FG_BLUE}linux-x64${RESET}"
pushd "linux-x64/publish"
chmod +x "./ObjectEditor"
chmod +x "./ObjectEditorUpdater"
# error happens if you make tar file inside the dir its zipping. hack is to touch it first then exclude it
touch "object-editor-$version-linux-x64.tar"
tar --exclude="object-editor-$version-linux-x64.tar" -jcf "object-editor-$version-linux-x64.tar" .
mv "object-editor-$version-linux-x64.tar" ../../..
popd

echo "Zipping ${FG_BLUE}osx-x64${RESET}"
pushd "osx-x64/publish"
chmod +x "./ObjectEditor"
chmod +x "./ObjectEditorUpdater"
# error happens if you make tar file inside the dir its zipping. hack is to touch it first then exclude it
touch "object-editor-$version-osx-x64.tar"
tar --exclude="object-editor-$version-osx-x64.tar" -jcf "object-editor-$version-osx-x64.tar" .
mv "object-editor-$version-osx-x64.tar" ../../..
popd

popd
echo "=== Build and packaging ${FG_GREEN}complete${RESET}! ==="
