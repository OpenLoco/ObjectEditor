using Dat.FileParsing;
using Gui.Models;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class G1ViewModel : BaseLocoFileViewModel
{
	public G1ViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		: base(currentFile, model) => Load();

	[Reactive]
	public ImageTableViewModel ImageTableViewModel { get; set; }

	public override void Load()
	{
		logger.Info($"Loading G1 from {CurrentFile.FileName}");
		Model.G1 = SawyerStreamReader.LoadG1(CurrentFile.FileName, Model.Logger);

		if (Model.G1 == null)
		{
			logger.Error($"G1 was unable to be loaded from {CurrentFile.FileName}");
			return;
		}

		var images = new List<Image<Rgba32>>();

		var i = 0;
		foreach (var e in Model.G1.G1Elements)
		{
			try
			{
				if (Model.PaletteMap.TryConvertG1ToRgba32Bitmap(e, Dat.ColourRemapSwatch.PrimaryRemap, Dat.ColourRemapSwatch.SecondaryRemap, out var image))
				{
					images.Add(image!);
				}
			}
			catch (Exception ex)
			{
				Model.Logger.Error($"[{i}] - [e]", ex);
			}

			i++;
		}

		ImageTableViewModel = new ImageTableViewModel(new ImageTableModel(images, Model.G1, Model.G1, Model.PaletteMap, logger));
	}

	public override void Save()
	{
		if (Model.G1 == null)
		{
			logger?.Error("G1 was null and was unable to saved");
			return;
		}

		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? Path.Combine(Model.Settings.ObjDataDirectory, CurrentFile.FileName)
			: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

		logger?.Info($"Saving G1.dat to {savePath}");
		SawyerStreamWriter.SaveG1(savePath, Model.G1);
	}

	public override void SaveAs()
	{
		if (Model.G1 == null)
		{
			logger?.Error("G1 was null and was unable to saved");
			return;
		}

		var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
		if (saveFile == null)
		{
			return;
		}

		var savePath = saveFile.Path.LocalPath;
		logger?.Info($"Saving G1.dat to {savePath}");
		SawyerStreamWriter.SaveG1(savePath, Model.G1);
	}
}
