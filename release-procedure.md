1. Switch to `master`
1. Update `version.txt` and the `AssemblyVersion` tag in `Gui.csproj` and/or `AvaGui.csproj`
1. Add, commit, push these changes
1. `git tag -a 'x.x.x'`, where the x's are the new version number
1. `git push --tags`
1. Locally in Visual Studio, create a release build, and zip up the folder created
1. In GitHub, create a new release from that tag
1. On that release page, add the zip folder as the build artefact
