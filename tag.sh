#!/bin/bash

# example usage:
# ./tag.sh 1.2.3

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

echo "Tagging complete!"