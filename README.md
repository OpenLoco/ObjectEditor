# OpenLoco Object Editor
A modern implementation of 'LocoTool' for Locomotion `dat` file parsing and editing

# Screenshots

| **Object property editing** | **Graphics viewing and editing** |
|:-----:|:-----:|
| ![image](https://github.com/user-attachments/assets/1adb4be5-cc8e-46a0-9174-83e0634c2ad2) | ![image](https://github.com/user-attachments/assets/17bf5419-6e58-4903-9998-cda708be359a) |
| **String table editing** | **Light/dark mode** |
| ![image](https://github.com/user-attachments/assets/a60f6f62-084c-4bbd-ba66-0175177becef) | ![image](https://github.com/user-attachments/assets/3d0d36d2-17b9-4449-8940-f9820dcfdc5b) |
| **Hex dump viewer** | **Object repository browsing and downloading** |
| ![image](https://github.com/user-attachments/assets/4c20b844-fdbc-4c35-b9da-5b7b3bbf879b) | ![image](https://github.com/user-attachments/assets/98c37f5f-1325-4795-9729-2f1c8e1d4ce7) |
| **Scenario/Landscape/SaveGame data viewer** | **Palette swapping** |
| ![image](https://github.com/user-attachments/assets/6bffd1e1-fc74-4979-9b0f-ce3c23c74e0e) | ![image](https://github.com/user-attachments/assets/baef3750-91d8-46ef-bb64-e8d8cd8004b2) |


# How to use

## 1. Load an object folder
1. Click `Objects` -> `Add new folder`, which will open a folder browser window
2. Navigate to a folder that contains Locomotion object files
3. Click `Select Folder` to close the folder browser window
4. The editor will load all objects in that folder (recursively) and display them in the tree view on the left of the editor

## 2. Select an object to view/edit
1. Expand the tree-view of objects to one you wish to view or edit
2. Click on the object
3. It will appear in the editor view on the right side of the editor

## 3. Play
* Edit any of the properties and save the object to use in-game
* Export images from objects with graphics

# Features

## 3.0.0+
- Online mode
  - Can connect to the object repository to view and download any object stored in it
  - Automatic upload of 'undiscovered' dat files (ie dat files that don't exist in the object repository)
- Property/hex viewer has been ported over from 1.0.5/the WinForms editor (wasn't present in the 2.x releases)
- Vehicle animator
- G1.dat viewing/exporting/saving
- Editing of the inner list properties of vehicle objects (more to come soon)
- Indexed PNG image support
- Sprite animation
- Steam/GoG object variation support
- Savegame/Scenario/Landscape data viewer
- Palette hotswapping
- Music and sound effect file loading/saving as well as audio import and export

## 2.0.0+

- Cross-platform
- Dark mode
- Flag editing support
- Locomotion `ObjData` folder:
  - Can open and edit all object types
  - Can save object types back to `dat` file format (albeit with no encoding)
  - Can display image table and string table of all objects (and sound data for SoundObject), and allow editing of them
  - Can display image properties of images in the image table
  - Can handle many more objects than the 1.x.x version
  - Can change background colour of image viewer
  - Can play sound objects

## 1.0.5+

- Windows-only
- Property/hex viewer
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
- The program settings file, `settings.json` will be created on first startup. It is located at:
  - Windows: `%APPDATA%\\Roaming\\OpenLoco Object Editor\\`
  - Linux/macOSX: `/~/<user>/.config/OpenLoco Object Editor`
- This file is used to store where the users' object folder paths are, and other program data
- Feel free to inspect it and change it if necessary, or even delete it if you mess things up too much - the editor will recreate a fresh one!

## Indexing
- When the editor first loads a directory containing objects it will scan every file to make an index and save that into `objectIndex.json` in that folder
- This indexing is relatively slow, but only needs to run once/when the folder contents change
- On subsequent uses of the editor, the index file will be loaded instead, and this is fast
- The editor will print a log message if it detects changes in the folder and thinks you need to reindex it

## Unit Testing
- All object types have a unit test which
  - Loads a pre-determined object file of that type
  - Asserts all (or most) of its values are correct
  - Saves the file to memory
  - Reloads the just-saved version of the file
  - Checks that the reloaded file is the same as the originally-loaded file, byte for byte. This is a byte comparison of the *decoded and decompressed* bytes, not the *on-disk* bytes

# Future Plans/Features
- [x] Better flag editing support
- [ ] Validation of object limits/sane values
- [x] Detection of bugged objects
- [ ] Support/edit tutorials
- [x] Support/edit maps/savegames/scenarios
- [ ] Support language files
- [x] Vehicle previewer
- [ ] Better G1 support including palette file editing
- [ ] Export/convert object to a future modern OpenLoco file format
- [x] Dark mode
- [x] Cross-platform support
- [x] Full unit-testing suite
- [ ] Blank template objects for object creation from scratch
- [x] Use a proper C# image library for image creation instead of WinForms
- [x] Export/import sounds

...many more things

# Building
- Open `ObjectEditor.sln` in Visual Studio
  - You'll need the `Avalonia for Visual Studio` plugin to use the visual XAML previewer, but it isn't required to actually build or run the editor

# Deploying
See `build.sh` and `tag.sh`

# Disclaimer and Terms of Service
When you run the editor and select a folder to browse, you give permission for the editor to read all DAT objects in that folder (and no other type of object or file). By doing this, you hereby give permission for the editor to check the DAT files in that directory against the master list in the object service. If a file is discovered that is not listed in the service, the editor will automatically upload the file to the service for archiving. These are the terms for using the object editor and object service. If you disagree with these terms, do not use the editor or service.
