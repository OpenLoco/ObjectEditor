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



build_windows() {
    echo "Building the ${FG_BLUE}Editor${RESET} (win-x64)"
    dotnet publish Gui/Gui.csproj -c Release -p:WarningLevel=0 -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime win-x64

    echo "Building the ${FG_BLUE}Updater${RESET} (win-x64)"
    dotnet publish GuiUpdater/GuiUpdater.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime win-x64

    echo "Copying ${FG_BLUE}Updater${RESET} files into ${FG_BLUE}Gui${RESET} folders (win-x64)"
    cp GuiUpdater/bin/Release/$framework/win-x64/publish/* Gui/bin/Release/$framework/win-x64/publish

    echo "Zipping ${FG_BLUE}win-x64${RESET}"
    pushd "Gui/bin/Release/$framework/win-x64/publish"
    zip -r "object-editor-$version-win-x64.zip" .
    mv "object-editor-$version-win-x64.zip" ../../..
    popd
}

build_linux() {
    echo "Building the ${FG_BLUE}Editor${RESET} (linux-x64)"
    dotnet publish Gui/Gui.csproj -c Release -p:WarningLevel=0 -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime linux-x64

    echo "Building the ${FG_BLUE}Updater${RESET} (linux-x64)"
    dotnet publish GuiUpdater/GuiUpdater.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime linux-x64

    echo "Copying ${FG_BLUE}Updater${RESET} files into ${FG_BLUE}Gui${RESET} folders (linux-x64)"
    cp GuiUpdater/bin/Release/$framework/linux-x64/publish/* Gui/bin/Release/$framework/linux-x64/publish

    echo "Zipping ${FG_BLUE}linux-x64${RESET}"
    pushd "Gui/bin/Release/$framework/linux-x64/publish"
    chmod +x "./ObjectEditor"
    chmod +x "./ObjectEditorUpdater"
    touch "object-editor-$version-linux-x64.tar"
    tar --exclude="object-editor-$version-linux-x64.tar" -jcf "object-editor-$version-linux-x64.tar" .
    mv "object-editor-$version-linux-x64.tar" ../../..
    popd
}

build_macos() {
    # macOS bundle settings
    app_name="ObjectEditor"
    mac_bundle_id="com.openloco.objecteditor"
    macos_publish_dir="Gui/bin/Release/$framework/osx-x64/publish"
    macos_bundle_dir="$macos_publish_dir/$app_name.app"
    macos_plist_template="Gui/Packaging/macOS/Info.plist"

    echo "Building the ${FG_BLUE}Editor${RESET} (osx-x64)"
    dotnet publish Gui/Gui.csproj -c Release -p:WarningLevel=0 -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime osx-x64

    echo "Building the ${FG_BLUE}Updater${RESET} (osx-x64)"
    dotnet publish GuiUpdater/GuiUpdater.csproj -c Release -p:PublishSingleFile=true -p:Version=$version --self-contained --runtime osx-x64

    echo "Copying ${FG_BLUE}Updater${RESET} files into ${FG_BLUE}Gui${RESET} folders (osx-x64)"
    cp GuiUpdater/bin/Release/$framework/osx-x64/publish/* Gui/bin/Release/$framework/osx-x64/publish

    echo "Creating ${FG_BLUE}macOS .app bundle${RESET}"
    rm -rf "$macos_bundle_dir"
    mkdir -p "$macos_bundle_dir/Contents/MacOS" "$macos_bundle_dir/Contents/Resources"
    find "$macos_bundle_dir/Contents/MacOS" -mindepth 1 -exec rm -rf {} +
    find "$macos_publish_dir" -mindepth 1 -maxdepth 1 \( -name "$app_name.app" -o -name "." \) -prune -o -exec cp -R {} "$macos_bundle_dir/Contents/MacOS/" \;
    chmod +x "$macos_bundle_dir/Contents/MacOS/$app_name"
    chmod +x "$macos_bundle_dir/Contents/MacOS/ObjectEditorUpdater"

    cat > "$macos_bundle_dir/Contents/Info.plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
  <dict>
    <key>CFBundleName</key>
    <string>$app_name</string>
    <key>CFBundleDisplayName</key>
    <string>$app_name</string>
    <key>CFBundleExecutable</key>
    <string>$app_name</string>
    <key>CFBundleIdentifier</key>
    <string>$mac_bundle_id</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>$version</string>
    <key>CFBundleVersion</key>
    <string>$version</string>
    <key>LSMinimumSystemVersion</key>
    <string>11.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>CFBundleIconFile</key>
    <string>loco_icon.icns</string>
  </dict>
</plist>
EOF

    if [ -f "Gui/Assets/loco_icon.icns" ]; then
        cp "Gui/Assets/loco_icon.icns" "$macos_bundle_dir/Contents/Resources/"
    fi

    echo "Zipping ${FG_BLUE}osx-x64${RESET}"
    pushd "Gui/bin/Release/$framework/osx-x64/publish"
    zip -r "object-editor-$version-osx-x64.zip" "$app_name.app"
    mv "object-editor-$version-osx-x64.zip" ../../..
    popd
}

# 2. Write the version to version.txt. This is read by the UI to know the current version.
echo "$version" > Gui/version.txt

# Restore dependencies first to avoid race conditions during parallel builds
dotnet restore Gui/Gui.csproj
dotnet restore GuiUpdater/GuiUpdater.csproj

# 3. Build the editor for different platforms in parallel
build_windows
build_linux
build_macos


echo "=== Build and packaging ${FG_GREEN}complete${RESET}! ==="
