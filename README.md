# OpenLoco Object Editor
A modern implementation of 'LocoTool' for Locomotion `dat` file parsing and editing

# How to use

## 1. Load an object folder
1. Click `File` -> `ObjData Directories` -> `Add New`, which will open a folder browser window
2. Navigate to a folder that contains Locomotion object files
3. Click `Select Folder` to close the folder browser window
4. The tool will load all objects in that folder and display them in the tree view on the left of the tool

## 3. Select an object to view/edit
1. Expand the tree-view of objects to one you wish to edit
2. Click on the object
3. It will appear in the editor view on the right side of the tool

# Features

- Locomotion `ObjData` folder:
  - Can open and edit all object types
  - Can save object types back to `dat` file format (albeit with no encoding)
  - Can display image table and string table of all objects (and sound data for SoundObject), and allow editing of them
- Locomotion `Data` folder:
  - Supported
    - `G1.dat` - the graphics file
      - display its graphics
      - export all images to folder
      - import all images from folder
    - `css1.dat` - the sound effects file
      - play each sound effect individually
      - export a sound effect to wav
      - import a sound effect from wav
      - write a new `css1.dat` file with your new sound effects
    - `css{2-5}.dat` - the 'system' music tracks
      - play each tracks
      - export a track to wav
      - import a track from wav
      - write a new `css{2-5}.dat` file with your new track
  - Unsupported
    - Tutorial files, eg `tut1024_1.dat`
    - Extra language files, eg `kanji.dat`
    - Map loading, eg `title.dat`, or any other save game

# Misc

## Settings
- The program settings file, `settings.json` will be created on first startup in `%APPDATA%\\Roaming\\OpenLoco Object Editor\\`
- This is where the users' object folder paths are saved, and other program data

## Indexing
- When the tool first loads an objdata directory it will scan every file to make an index and save that into `objectIndex.json` in that folder
- This indexing is relatively slow, but only needs to run once/when the folder contents change
- On subsequent uses of the tool, the index file will be loaded instead, and this is fast
- The tool will print a log message if it detects changes in the folder and thinks you need to reindex it

## Unit Testing
- All object types have a unit test which
  - Loads a pre-determined object file of that type
  - Asserts all (or most) of its values are correct
  - Saves the file to memory
  - Reloads the just-saved version of the file
  - Checks that the reloaded file is the same as the originally-loaded file, byte for byte. This is a byte comparison of the *decoded and decompressed* bytes, not the *on-disk* bytes

# Future Plans/Features
- Better flag editing support
- Validation of object limits/sane values
- Detection of bugged objects
- Support/edit tutorials
- Support/edit maps/savegames/scenarios
- Support language files
- Implement vehicle previewer
- Better G1 support including palette file editing
- Export/convert object to a future modern OpenLoco file format
- Dark modern
- Cross-platform support
- Full unit-testing suite
- Blank template objects for object creation from scratch
- Use a proper C# image library for image creation instead of WinForms
- ...many more things
