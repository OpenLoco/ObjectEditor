#!/bin/bash

# example usage:
# ./tag.sh 1.2.3

# immediately abort script on error
set -euo pipefail

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

if [ -n "$(git status --porcelain)" ]; then
    echo "Error: The working tree has uncommitted changes. Commit or stash them before tagging a release."
    exit 1
fi

if git rev-parse -q --verify "refs/tags/$version" > /dev/null; then
    echo "Error: Tag \"$version\" already exists."
    exit 1
fi

# 2. Tag the current commit. The version comes from this tag via MinVer (see Directory.Build.props), so there
#    is no version file to commit first.
echo "Tagging $version"
git tag -a "$version" -m "tag $version"

# 3. Push the tag. This triggers the release workflow, which builds and attaches the artifacts.
echo "Pushing tag $version"
git push origin "$version"

echo "Tagging complete!"