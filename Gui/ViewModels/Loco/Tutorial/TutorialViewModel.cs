using Common.Logging;
using Gui.Models;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Gui.ViewModels.Loco.Tutorial;

public class TutorialViewModel : BaseFileViewModel<DummyModel>
{
	public TutorialViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
	{
		SaveIsVisible = false;
		SaveAsIsVisible = false;
		_ = LoadAsync();
	}

	[Reactive]
	public ObservableCollection<ITutorialAction> TutorialInputs { get; set; } = [];

	public override void Load()
	{
		Logger.LogInformation("Loading tutorial from {FileName}", CurrentFile.FileName);
		if (CurrentFile.FileName == null)
		{
			Logger.LogError("Tutorial file name was null");
			return;
		}

		var bytes = File.ReadAllBytes(CurrentFile.FileName).AsSpan();

		// each tutorial action is 4 parts of 2 bytes each, so 8 bytes per action
		for (var i = 0; i < bytes.Length;)
		{
			var x = BitConverter.ToUInt16(bytes[(i + 0)..(i + 2)]);
			if ((x & 0x80) != 0)
			{
				if (i > bytes.Length - 6)
				{
					break;
				}

				var tAction = new TutorialActionB(
					BitConverter.ToUInt16(bytes[(i + 0)..(i + 2)]),
					BitConverter.ToUInt16(bytes[(i + 2)..(i + 4)]),
					BitConverter.ToUInt16(bytes[(i + 4)..(i + 6)]));
				TutorialInputs.Add(tAction);
				i += 6;
			}
			else
			{
				if (i > bytes.Length - 8)
				{
					break;
				}

				var tAction = new TutorialActionA(
					(KeyModifier)BitConverter.ToUInt16(bytes[(i + 0)..(i + 2)]),
					BitConverter.ToUInt16(bytes[(i + 2)..(i + 4)]),
					BitConverter.ToUInt16(bytes[(i + 4)..(i + 6)]),
					(MouseButton)BitConverter.ToUInt16(bytes[(i + 6)..(i + 8)]));
				TutorialInputs.Add(tAction);
				i += 8;
			}
		}

		// todo: may be leftover bytes still

		this.RaisePropertyChanged(nameof(TutorialInputs));
	}

	public override void Save()
		=> Logger.LogWarning("Save is not currently implemented");

	public override Task<string?> SaveAsAsync(SaveParameters saveParameters)
	{
		Logger.LogWarning("SaveAs is not currently implemented");
		return Task.FromResult<string?>(null);
	}
}
