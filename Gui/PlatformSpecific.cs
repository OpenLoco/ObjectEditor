using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Gui;

public static class PlatformSpecific
{
	public static void FolderOpenInDesktop(string directory, ILogger logger, string? filename = null)
	{
		try
		{
			FolderOpenInDesktopCore(directory, filename);
		}
		catch (Exception ex)
		{
			logger.Error(ex);
		}
	}

	static void FolderOpenInDesktopCore(string directory, string? filename = null)
	{
		if (!Directory.Exists(directory))
		{
			throw new ArgumentException("The specified folder does not exist. Folder=\"{directory}\"", nameof(directory));
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
			Arguments = filename == null ? directory : $"/select, \"{Path.Combine(directory, filename)}\"",
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

		return await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
		{
			Title = "Select a folder containing objects",
			AllowMultiple = false
		});
	}

	public static readonly IReadOnlyList<FilePickerFileType> JsonFileTypes = [new("JSON Files") { Patterns = ["*.json", "*.JSON"] }];
	public static readonly IReadOnlyList<FilePickerFileType> DatFileTypes = [new("Locomotion DAT Files") { Patterns = ["*.dat", "*.DAT"] }];
	public static readonly IReadOnlyList<FilePickerFileType> PngFileTypes = [new("PNG Files") { Patterns = ["*.png", "*.PNG"] }];
	public static readonly IReadOnlyList<FilePickerFileType> SCV5FileTypes = [new("SC5/SV5 Files") { Patterns = ["*.sc5", "*.SC5", "*.sv5", "*.SV5"] }];

	public static readonly IReadOnlyList<FilePickerFileType> AudioFileImportTypes = [new("WAV Files") { Patterns = ["*.wav", "*.WAV"] }, new("MP3 Files") { Patterns = ["*.mp3", "*.MP3"] }];
	public static readonly IReadOnlyList<FilePickerFileType> AudioFileExportTypes = [new("WAV Files") { Patterns = ["*.wav", "*.WAV"] }];

	public static async Task<IReadOnlyList<IStorageFile>> OpenFilePicker(IReadOnlyList<FilePickerFileType> filetypes)
	{
		// See IoCFileOps project for an example of how to accomplish this.
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
			|| desktop.MainWindow?.StorageProvider is not { } provider)
		{
			throw new ArgumentNullException("ApplicationLifetime|StorageProvider", "Missing StorageProvider instance.");
		}

		return await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
		{
			Title = "Select a file",
			AllowMultiple = false,
			FileTypeFilter = filetypes,
		});
	}

	public static async Task<IStorageFile?> SaveFilePicker(IReadOnlyList<FilePickerFileType> filetypes)
	{
		// See IoCFileOps project for an example of how to accomplish this.
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
			|| desktop.MainWindow?.StorageProvider is not { } provider)
		{
			throw new ArgumentNullException("ApplicationLifetime|StorageProvider", "Missing StorageProvider instance.");
		}

		return await provider.SaveFilePickerAsync(new FilePickerSaveOptions()
		{
			ShowOverwritePrompt = true,
			Title = "Select a new file name",
			FileTypeChoices = filetypes,
		});
	}
}
