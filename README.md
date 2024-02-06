# OpenLoco Object Editor
A modern implementation of 'LocoTool' for dat file parsing and editing

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

# Misc

## Settings
- In the same folder as the tool executable, a `settings.json` file will be created on first startup
- This is where the users' object folder paths are saved, and other program data

## Indexing
- When the tool first loads an objdata directory it will scan every file to make an index and save that into `objectIndex.json` in that folder
- This indexing is relatively slow, but only needs to run once/when the folder contents change
- On subsequent uses of the tool, the index file will be loaded instead, and this is fast
- The tool will print a log message if it detects changes in the folder and thinks you need to reindex it