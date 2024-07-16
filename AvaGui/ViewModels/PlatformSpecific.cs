using AvaGui.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AvaGui.ViewModels
{
	public static class PlatformSpecific
	{
		public static void FolderOpenInDesktop()
		{
			var folderPath = ObjectEditorModel.SettingsPath;
			if (!Directory.Exists(folderPath))
			{
				throw new ArgumentException("The specified folder does not exist.", nameof(folderPath));
			}

			// Platform-specific command construction
			string command;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				command = "explorer.exe"; // Windows File Explorer
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				command = "open"; // macOS Finder
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				// Note: This assumes the user's desktop environment has a standard file manager
				command = "xdg-open";
			}
			else
			{
				//throw new PlatformNotSupportedException($"This platform ({RuntimeInformation.OSDescription}) is not currently supported. Please file a Github issue here: {GithubIssuePage}");
				throw new PlatformNotSupportedException($"This platform ({RuntimeInformation.OSDescription}) is not currently supported.");
			}

			// Process.Start to execute the command and open the folder
			var processStartInfo = new ProcessStartInfo
			{
				FileName = command,
				Arguments = folderPath,
				UseShellExecute = true // Use the shell for proper handling on each OS
			};

			using (Process.Start(processStartInfo))
			{ } // Start and dispose of the process
		}

		public static async Task<IReadOnlyList<IStorageFolder>> OpenFolderPicker()
		{
			// See IoCFileOps project for an example of how to accomplish this.
			if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
				|| desktop.MainWindow?.StorageProvider is not { } provider)
			{
				throw new ArgumentNullException("ApplicationLifetime|StorageProvider", "Missing StorageProvider instance.");
			}

			var folders = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
			{
				Title = "Select a folder containing objects",
				AllowMultiple = false
			});
			return folders;
		}
	}
}
