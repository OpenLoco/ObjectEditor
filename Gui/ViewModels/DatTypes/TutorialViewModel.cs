using OpenLoco.Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.IO;

namespace OpenLoco.Gui.ViewModels
{
	public interface ITutorialAction;
	public record TutorialActionA(uint16_t Unk, uint16_t MouseX, uint16_t MouseY) : ITutorialAction;
	public record TutorialActionB(uint16_t KeyModifier, uint16_t MouseX, uint16_t MouseY, uint16_t MouseButton) : ITutorialAction;

	public class TutorialViewModel : BaseLocoFileViewModel
	{
		public TutorialViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		[Reactive]
		public BindingList<ITutorialAction> TutorialInputs { get; set; } = [];

		public override void Load()
		{
			Logger?.Info($"Loading tutorial from {CurrentFile.Filename}");
			var bytes = File.ReadAllBytes(CurrentFile.Filename).AsSpan();

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

					var tAction = new TutorialActionA(
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

					var tAction = new TutorialActionB(
						BitConverter.ToUInt16(bytes[(i + 0)..(i + 2)]),
						BitConverter.ToUInt16(bytes[(i + 2)..(i + 4)]),
						BitConverter.ToUInt16(bytes[(i + 4)..(i + 6)]),
						BitConverter.ToUInt16(bytes[(i + 6)..(i + 8)]));
					TutorialInputs.Add(tAction);
					i += 8;
				}
			}

			this.RaisePropertyChanged(nameof(TutorialInputs));
		}

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}

	public class ScoresViewModel : BaseLocoFileViewModel
	{
		public ScoresViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load() => Logger?.Info($"Loading scores from {CurrentFile.Filename}");

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}

	public class LanguageViewModel : BaseLocoFileViewModel
	{
		public LanguageViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load() => Logger?.Info($"Loading languages from {CurrentFile.Filename}");

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}
}
