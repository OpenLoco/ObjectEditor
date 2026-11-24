#!/bin/bash

# example usage:
# ./build.sh 1.2.3

# immediately abort script on error
set -euo pipefail

# Define color variables for readability. tput fails when there is no terminal (eg CI), so fall back to no color.
if command -v tput > /dev/null 2>&1 && [ -n "${TERM:-}" ]; then
    FG_BLUE=$(tput setaf 39 2> /dev/null || true)
    FG_GREEN=$(tput setaf 46 2> /dev/null || true)
    RESET=$(tput sgr0 2> /dev/null || true)
else
    FG_BLUE=""
    FG_GREEN=""
    RESET=""
fi

# Always operate from the repository root, regardless of where the script is invoked from
cd "$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# 1. Get the version from the first command-line argument
if [ -z "${1:-}" ]; then
    echo "Error: Please provide the version as the first argument."
    exit 1
fi

version="$1"

if ! [[ "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z.-]+)?(\+[0-9A-Za-z.-]+)?$ ]]; then
    echo "Error: \"$version\" is not a valid semantic version (expected eg 1.2.3 or 1.2.3-beta.1)."
    exit 1
fi

# Apple's CFBundleShortVersionString/CFBundleVersion only accept digits and dots
base_version="${version%%-*}"
base_version="${base_version%%+*}"

# read .net version from csproj file
framework=$(grep -m1 '<TargetFramework>' Gui/Gui.csproj | sed 's/.*<TargetFramework>\(.*\)<\/TargetFramework>.*/\1/')

if [ -z "$framework" ]; then
    echo "Error: Unable to determine the target framework from Gui/Gui.csproj."
    exit 1
fi
# 2. Write the version to version.txt. This is read by the UI to know the current version.
echo "$version" > Definitions/version.txt

# The version is also baked into the assemblies via -p:Version below, which is what the editor reads at
# runtime (with this file as a fallback).

# Publish both the editor and the updater into a single staging folder for the given runtime identifier.
# The restore is intentionally left to `dotnet publish` (ie no --no-restore): a restore performed without a
# runtime identifier does not produce the rid-specific assets that publish needs (NETSDK1047).
publish_rid() {
    local rid="$1"
    local out="$publish_root/$rid"

    rm -rf "$out"
    mkdir -p "$out"

    echo "Publishing ${FG_BLUE}Editor${RESET} ($rid)"
    dotnet publish Gui/Gui.csproj -c Release -p:WarningLevel=0 -p:PublishSingleFile=true -p:Version="$version" --self-contained -r "$rid" -o "$out"

    echo "Publishing ${FG_BLUE}Updater${RESET} ($rid)"
    dotnet publish GuiUpdater/GuiUpdater.csproj -c Release -p:PublishSingleFile=true -p:Version="$version" --self-contained -r "$rid" -o "$out"

    # Debug symbols are not needed by end users but inflate the archives by ~100MB
    find "$out" -name '*.pdb' -delete
}

zip_dir() {
    local dir="$1"
    local archive="$2"
    rm -f "$archive"
    (cd "$dir" && zip -r $ZIP_SYMLINK_OPTS "$archive" .)
}

# -y stores symlinks as symlinks instead of following them, which macOS app bundles rely on.
# Some Info-ZIP builds (eg the one bundled with git on Windows) do not support it, so probe for real.
detect_zip_symlink_support() {
    local probe_dir
    probe_dir=$(mktemp -d)
    if (cd "$probe_dir" && echo probe > file.txt && zip -q -y probe.zip file.txt > /dev/null 2>&1); then
        ZIP_SYMLINK_OPTS="-y"
    else
        ZIP_SYMLINK_OPTS=""
    fi
    rm -rf "$probe_dir"
}
detect_zip_symlink_support

# GNU tar can force file modes from the command line, which is needed when building on Windows where
# chmod records nothing. macOS ships bsdtar, which has no --mode, so probe for it.
detect_tar_mode_support() {
    local probe_dir
    probe_dir=$(mktemp -d)
    if (cd "$probe_dir" && echo probe > file.txt && tar -cf probe.tar --mode='u+rwx,go+rx' file.txt > /dev/null 2>&1); then
        TAR_MODE_OPTS="--mode=u+rwx,go+rx"
    else
        TAR_MODE_OPTS=""
    fi
    rm -rf "$probe_dir"
}
detect_tar_mode_support

build_windows() {
    local rid="win-x64"
    local out="$publish_root/$rid"
    local archive="$artifacts_root/object-editor-$version-$rid.zip"

    publish_rid "$rid"

    echo "Zipping ${FG_BLUE}$rid${RESET}"
    zip_dir "$out" "$archive"
    # free the staging copy immediately; a self-contained publish is several hundred MB per rid
    rm -rf "$out"
}

build_linux() {
    local rid="linux-x64"
    local out="$publish_root/$rid"
    local archive="$artifacts_root/object-editor-$version-$rid.tar.gz"
    local tar_file="$artifacts_root/object-editor-$version-$rid.tar"

    publish_rid "$rid"

    echo "Creating gzip tarball for ${FG_BLUE}$rid${RESET}"
    rm -f "$archive" "$tar_file"
    # The binaries get explicit exec bits rather than relying on chmod, which records nothing on Windows hosts.
    # The archive lives outside $out, so it can never include itself.
    tar -cf "$tar_file" -C "$out" --exclude="./$app_name" --exclude="./$updater_name" .
    if [ -n "$TAR_MODE_OPTS" ]; then
        tar -rf "$tar_file" -C "$out" $TAR_MODE_OPTS ./$app_name ./$updater_name
    else
        # bsdtar/macOS: commit the bit on disk first, then add the entry
        chmod +x "$out/$app_name" "$out/$updater_name"
        tar -rf "$tar_file" -C "$out" ./$app_name ./$updater_name
    fi
    gzip -n -f "$tar_file"
    rm -rf "$out"
}

write_info_plist() {
    local destination="$1"

    cat > "$destination" <<EOF
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
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>$base_version</string>
    <key>CFBundleVersion</key>
    <string>$base_version</string>
    <key>CFBundleIconFile</key>
    <string>loco_icon</string>
    <key>LSMinimumSystemVersion</key>
    <string>$mac_min_os</string>
    <key>NSHighResolutionCapable</key>
    <true/>
  </dict>
</plist>
EOF
}

build_macos() {
    local rid="$1"
    local out="$publish_root/$rid"
    local bundle_root="$publish_root/macos/$rid"
    local bundle_dir="$bundle_root/$app_name.app"
    local archive="$artifacts_root/object-editor-$version-$rid.zip"

    publish_rid "$rid"

    echo "Creating ${FG_BLUE}macOS .app bundle ($rid)${RESET}"
    # the bundle is assembled outside the publish folder so publishing output never gets copied into itself
    rm -rf "$bundle_root"
    mkdir -p "$bundle_dir/Contents/MacOS" "$bundle_dir/Contents/Resources"

    cp -R "$out"/. "$bundle_dir/Contents/MacOS/"
    chmod +x "$bundle_dir/Contents/MacOS/$app_name" "$bundle_dir/Contents/MacOS/$updater_name"

    printf 'APPL????' > "$bundle_dir/Contents/PkgInfo"

    if [ ! -f "$mac_icon" ]; then
        echo "Error: macOS icon \"$mac_icon\" is missing."
        exit 1
    fi
    cp "$mac_icon" "$bundle_dir/Contents/Resources/loco_icon.icns"

    write_info_plist "$bundle_dir/Contents/Info.plist"

    # ad-hoc sign so Gatekeeper lets a locally-built bundle launch. Developer ID signing/notarisation
    # requires certificates and is done outside this script.
    if command -v codesign > /dev/null 2>&1; then
        echo "Ad-hoc signing ${FG_BLUE}$app_name.app${RESET}"
        codesign --force --deep --sign - "$bundle_dir"
    fi

    echo "Zipping ${FG_BLUE}$rid${RESET}"
    rm -f "$archive"
    if command -v ditto > /dev/null 2>&1; then
        # ditto preserves permissions and extended attributes, which macOS app bundles rely on
        ditto -c -k --sequesterRsrc --keepParent "$bundle_dir" "$archive"
    else
        # fallback for building macOS artifacts on non-macOS hosts. this zip does not carry unix file modes,
        # so the extracted binaries are not executable until the in-app updater fixes them.
        echo "Warning: ditto is unavailable; the macOS zip will not record executable permissions."
        (cd "$bundle_root" && zip -r $ZIP_SYMLINK_OPTS "$archive" "$app_name.app")
    fi

    # free the staging copies immediately; a self-contained publish is several hundred MB per rid
    rm -rf "$bundle_root" "$out"
}

# 2. Build the editor for each platform. These run sequentially because the publishes share the projects'
# obj/ folders, so running them concurrently would race.
build_windows
build_linux
build_macos "osx-x64"
build_macos "osx-arm64"

echo "=== Build and packaging ${FG_GREEN}complete${RESET}! Artifacts are in ${FG_BLUE}$artifacts_dir${RESET} ==="
ls -1 "$artifacts_root"/*.zip "$artifacts_root"/*.tar.gz 2> /dev/null || true
