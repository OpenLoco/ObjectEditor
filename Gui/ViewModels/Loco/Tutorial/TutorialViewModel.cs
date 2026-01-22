using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Gui.ViewModels.Loco.Tutorial;

public class TutorialViewModel : BaseFileViewModel
{
	public TutorialViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
	{
		SaveIsVisible = false;
		SaveAsIsVisible = false;
		Load();
	}

	[Reactive]
	public ObservableCollection<ITutorialAction> TutorialInputs { get; set; } = [];

	public override void Load()
	{
		logger?.Info($"Loading tutorial from {CurrentFile.FileName}");
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
		=> logger?.Warning("Save is not currently implemented");

	public override string? SaveAs(SaveParameters saveParameters)
	{
		logger?.Warning("SaveAs is not currently implemented");
		return null;
	}
}
