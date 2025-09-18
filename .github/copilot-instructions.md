# Project Overview

This project consists of two main applications:
- A GUI application called "ObjectEditor", located in `/Gui` folder
- A web server called "ObjectService", located in `/ObjectService` folder

The ObjectEditor is used to edit `.dat` objects from the game "Chris Sawyer's Locomotion". It is built using C# and the latest .NET framework (currently version 10 preview).

The ObjectService is used to host a SQLite database and a folder of all known custom objects. The ObjectService can be interacted with by clients using a simple REST api, and the primary client is the ObjectEditor.

## Folder Structure

- `/Gui`: Contains the source code for the AvaloniaUI-based application called "ObjectEditor".
- `/Definitions`: Countains all data types shared throughout the projects in this solution.
- `/Dat`: Contains the source code for reading, saving, decoding and encoding dat-format objects.
- `/Common`: Contains source code for any generic and shared code.
- `/Index`: Contains source code for creating an 'Object Index File' which is a json file storing a list of all parsed .dat objects for a specific user-selected folder. It is used by the ObjectEditor and ObjectService to manage/reference the on-disk .dat objects.
- `/Tests`: Contains the source code for all unit tests for all projects.
- The other folders contain small sub-projects that have specific purposes, but are not necessary or used in the build of the main ObjectEditor and ObjectService applications.

## Libraries and Frameworks

- C#
- .NET 10
- ASP.NET Core
- SQLite
- System.Text.Json
- Avalonia UI and ReactiveUI

## Coding Standards

- Use the coding rules defined in `.editorconfig` - that's what that file is for.
- If in doubt, use the existing coding conventions in the code - do not use arbitrary conventions from other projects.
- Do not rewrite code to older styles, eg do not convert the collection initializer `[]` into `new()`. Use the modern, existing syntax.

## ObjectEditor UI guidelines

- Use AXAML/XAML for all UI layouers, and always use XAML-first (not code-behind) solutions, unless strictly not possible, in which case warn the user before making any code change.
- Separate views and viewmodels as necessary, and as evidenced by the existing `/Gui/Views`, `/Gui/ViewModels` and `/Gui/Models` folders. Note that many 'models' are defined in the `/Definitions` project as well.
- The application should have a modern and clean design utilising FluentUI principles and library.
- It should try to use MVVM style UI design, and where this design doesn't exist in the code, inform the user how to rewrite the code to better adhere to the standard MVVM/Avalonia UI style.
- The UI code uses ReactiveUI, so any UI suggestions must use that as well. We do not use INotifyPropertyChanged, for example, since ReactiveUI handles that automatically, so do not provide any suggestions related to INotifyPropertyChanged.

## AI Agent Behavior

Generate Code, Not Boilerplate: You don't need to generate entire classes from scratch unless explicitly asked. The goal is to provide helpful code snippets, method implementations, or test cases. You don't need to excessively add comments to code explaining every line; only add comments when you need to explain "why" the code does something, not "what" it does.
