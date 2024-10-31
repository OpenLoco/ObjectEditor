using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using OpenLoco.Dat.Types.SCV5;
using OpenLoco.Gui.Models;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLoco.Gui.ViewModels
{
	public class MusicViewModel : BaseLocoFileViewModel
	{
		public MusicViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model)
			=> Load();

		public override void Load()
		{
			var (header, data) = SawyerStreamReader.LoadWavFile(CurrentFile.Filename);
			SoundViewModel = new SoundViewModel(CurrentFile.DisplayName, header, data);
		}

		[Reactive]
		public SoundViewModel SoundViewModel { get; set; }

		public override void Save()
		{
			var savePath = CurrentFile.FileLocation == FileLocation.Local
				? Path.Combine(Model.Settings.DataDirectory, CurrentFile.Filename)
				: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

			Logger?.Info($"Saving music to {savePath}");
			var bytes = SawyerStreamWriter.SaveMusicToDat(SoundViewModel.Sound.Header, SoundViewModel.Sound.Data);
			File.WriteAllBytes(savePath, bytes);
		}

		public override void SaveAs()
		{
			var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
			if (saveFile == null)
			{
				return;
			}

			var savePath = saveFile.Path.LocalPath;
			Logger?.Info($"Saving music to {savePath}");
			var bytes = SawyerStreamWriter.SaveMusicToDat(SoundViewModel.Sound.Header, SoundViewModel.Sound.Data);
			File.WriteAllBytes(savePath, bytes);
		}
	}

	public class SoundEffectsViewModel : BaseLocoFileViewModel
	{
		public SoundEffectsViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load()
		{
			var soundIdNames = Enum.GetValues<SoundId>();
			SoundViewModels = SawyerStreamReader.LoadSoundEffectsFromCSS(CurrentFile.Filename)
				.Select((x, i) => new SoundViewModel(soundIdNames[i].ToString(), x.header, x.data))
				.ToBindingList();
		}

		[Reactive]
		public BindingList<SoundViewModel> SoundViewModels { get; set; } = [];

		public override void Save()
		{
			var savePath = CurrentFile.FileLocation == FileLocation.Local
				? Path.Combine(Model.Settings.DataDirectory, CurrentFile.Filename)
				: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

			Logger?.Info($"Saving sound effects to {savePath}");
			var bytes = SawyerStreamWriter.SaveSoundEffectsToCSS(SoundViewModels.Select(x => (x.Sound.Header, x.Sound.Data)).ToList());
			File.WriteAllBytes(savePath, bytes);
		}

		public override void SaveAs()
		{
			var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
			if (saveFile == null)
			{
				return;
			}

			var savePath = saveFile.Path.LocalPath;
			Logger?.Info($"Saving sound effects to {savePath}");
			var bytes = SawyerStreamWriter.SaveSoundEffectsToCSS(SoundViewModels.Select(x => (x.Sound.Header, x.Sound.Data)).ToList());
			File.WriteAllBytes(savePath, bytes);
		}
	}

	public class SCV5ViewModel : BaseLocoFileViewModel
	{
		public SCV5ViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		[Reactive]
		public S5File? CurrentS5File { get; set; }

		[Reactive]
		public BindingList<S5Header>? RequiredObjects { get; set; }

		[Reactive]
		public BindingList<S5Header>? PackedObjects { get; set; }

		[Reactive]
		public BindingList<TileElement>? TileElements { get; set; }

		//[Reactive]
		//public List<TileElement>[,] TileElementMap { get; set; }

		//[Reactive]
		//public Image<Rgba32> Map { get; set; }

		public override void Load()
		{
			Logger?.Info($"Loading scenario from {CurrentFile.Filename}");
			CurrentS5File = SawyerStreamReader.LoadSave(CurrentFile.Filename, Model.Logger);
			RequiredObjects = new BindingList<S5Header>(CurrentS5File!.RequiredObjects);
			PackedObjects = new BindingList<S5Header>(CurrentS5File!.PackedObjects.ConvertAll(x => x.Item1)); // note: cannot bind to this, but it'll allow us to display at least
			TileElements = new BindingList<TileElement>(CurrentS5File!.TileElements);

			//Map = new Image<Rgba32>(384, 384);

			// todo: find a way to actually render this in the UI. code here works fine, just dunno the ui
			//TileElementMap = new List<TileElement>[Limits.kMapColumns, Limits.kMapRows];

			//var i = 0;
			//for (var y = 0; y < Limits.kMapRows; ++y)
			//{
			//	for (var x = 0; x < Limits.kMapColumns; ++x)
			//	{
			//		TileElementMap[x, y] = [];
			//		do
			//		{
			//			TileElementMap[x, y].Add(CurrentS5File!.TileElements[i]);
			//			i++;
			//		}
			//		while (!CurrentS5File!.TileElements[i - 1].IsLast());
			//	}
			//}

			//TileElements = new BindingList<TileElement>(CurrentS5File!.TileElements);
		}

		public override void Save() => Logger?.Error("Saving SC5/SV5 is not implemented yet");

		public override void SaveAs() => Logger?.Error("Saving SC5/SV5 is not implemented yet");
	}
}
