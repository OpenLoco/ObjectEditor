using AvaGui.Models;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.FileParsing;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AvaGui.ViewModels
{
	public class G1ViewModel : BaseLocoFileViewModel
	{
		public G1ViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		{
			CurrentFile = currentFile;
			Model = model;

			LoadG1();

			ReloadCommand = ReactiveCommand.Create(LoadG1);
			SaveCommand = ReactiveCommand.Create(SaveG1);
			SaveAsCommand = ReactiveCommand.Create(SaveAsG1);
		}

		ObjectEditorModel Model { get; init; }
		ILogger? Logger => Model.Logger;

		[Reactive] public ImageTableViewModel ImageTableViewModel { get; set; }

		public void LoadG1()
		{
			Logger?.Info($"Loading G1 from {CurrentFile.Filename}");
			Model.G1 = SawyerStreamReader.LoadG1(CurrentFile.Filename, Model.Logger);

			if (Model.G1 == null)
			{
				Logger?.Error($"G1 was unable to be loaded from {CurrentFile.Filename}");
				return;
			}

			var images = new List<Image<Rgba32>>();

			var i = 0;
			foreach (var e in Model.G1.G1Elements)
			{
				try
				{
					images.Add(Model.PaletteMap.ConvertG1ToRgba32Bitmap(e)!);
				}
				catch (Exception ex)
				{
					Model.Logger.Error($"[{i}] - [e]", ex);
				}
				i++;
			}

			ImageTableViewModel = new ImageTableViewModel(Model.G1, Model.G1, Model.PaletteMap, images, Logger);
		}

		public void SaveG1()
		{
			if (Model.G1 == null)
			{
				Logger?.Error("G1 was null and was unable to saved");
				return;
			}

			var savePath = CurrentFile.FileLocation == FileLocation.Local
				? Path.Combine(Model.Settings.ObjDataDirectory, CurrentFile.Filename)
				: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

			Logger?.Info($"Saving G1.dat to {savePath}");
			SawyerStreamWriter.SaveG1(savePath, Model.G1);
		}

		public void SaveAsG1()
		{
			if (Model.G1 == null)
			{
				Logger?.Error("G1 was null and was unable to saved");
				return;
			}

			var saveFile = Task.Run(PlatformSpecific.SaveFilePicker).Result;
			if (saveFile == null)
			{
				return;
			}

			var savePath = saveFile.Path.LocalPath;
			Logger?.Info($"Saving G1.dat to {savePath}");
			SawyerStreamWriter.SaveG1(savePath, Model.G1);
		}
	}
}
