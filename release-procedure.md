1. Switch to `master`
1. Update `version.txt` and the `AssemblyVersion` tag in `Gui.csproj` and/or `AvaGui.csproj`
1. Add, commit, push these changes with the commit message "prepare <x.x.x>"
1. `git tag -a 'x.x.x'`, where the x's are the new version number
1. `git push --tags` with the tag message "tag <x.x.x>"
1. Locally in Visual Studio, create a release build for each platform
  1. `dotnet publish AvaGui/AvaGui.csproj -c Release -p:PublishSingleFile=true --runtime win-x64 --self-contained`
  1. `dotnet publish AvaGui/AvaGui.csproj -c Release -p:PublishSingleFile=true --runtime linux-x64 --self-contained`
  1. `dotnet publish AvaGui/AvaGui.csproj -c Release -p:PublishSingleFile=true --runtime osx-x64 --self-contained`
1. For the OSX and Linux builds, `chmod +x` the binary to make it executable
1. Go into the `publish` directory for each build, and zip each appropriately
  1. Windows - make a ZIP file, named `object-editor-x.x.x-win-x64`
  1. Linux - `tar -jcvf object-editor-x.x.x-linux-x64 .`
  1. OSX (Mac) - `tar -jcvf object-editor-x.x.x-osx-x64 osx-x64`
1. In GitHub, create a new release from that tag
1. On that release page, add the zipped files as build artefacts
