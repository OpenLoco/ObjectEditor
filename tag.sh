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

# 2. Work out which remote/branch this commit should be published to
branch=$(git branch --show-current)
if [ -z "$branch" ]; then
    echo "Error: HEAD is detached. Check out the branch you want to release before tagging."
    exit 1
fi

if upstream=$(git rev-parse --abbrev-ref --symbolic-full-name '@{u}' 2> /dev/null); then
    remote="${upstream%%/*}"
    remote_branch="${upstream#*/}"
else
    remote="origin"
    remote_branch="$branch"
    echo "No upstream is configured for \"$branch\"; assuming \"$remote/$remote_branch\"."
fi

remote_ref="$remote/$remote_branch"

if git ls-remote --exit-code --tags "$remote" "refs/tags/$version" > /dev/null 2>&1; then
    echo "Error: Tag \"$version\" already exists on \"$remote\"."
    exit 1
fi

# 3. Make sure the commit being tagged is on the remote. CI builds the tag from a fresh checkout, so if the
#    commit has not been pushed the tag would point at something the remote does not have.
if ! git ls-remote --exit-code --heads "$remote" "$remote_branch" > /dev/null 2>&1; then
    echo "\"$remote/$remote_branch\" does not exist yet; pushing \"$branch\" and setting it as upstream"
    git push --set-upstream "$remote" "$branch:$remote_branch"
else
    # refresh the remote-tracking ref so the comparison below reflects the actual remote state
    git fetch --quiet "$remote" "+refs/heads/$remote_branch:refs/remotes/$remote_ref"

    if git merge-base --is-ancestor HEAD "$remote_ref"; then
        echo "Commit $(git rev-parse --short HEAD) is already pushed to \"$remote_ref\"."
    elif git merge-base --is-ancestor "$remote_ref" HEAD; then
        echo "Pushing \"$branch\" to \"$remote_ref\""
        git push "$remote" "$branch:$remote_branch"
    else
        echo "Error: \"$branch\" has diverged from \"$remote_ref\" (both have commits the other does not)."
        echo "       Rebase or merge before tagging a release so the tagged commit is a normal fast-forward."
        exit 1
    fi
fi

# 4. Tag the current commit. The version comes from this tag via MinVer (see Directory.Build.props), so there
#    is no version file to commit first.
echo "Tagging $version"
git tag -a "$version" -m "tag $version"

# 5. Push the tag. This triggers the release workflow, which builds and attaches the artifacts.
echo "Pushing tag $version"
git push "$remote" "$version"

echo "Tagging complete!"