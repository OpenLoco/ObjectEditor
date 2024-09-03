using AvaGui.Models;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects.Sound;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaGui.ViewModels
{
	public record HexAnnotationLine(string Address, string Data, int? SelectionStart, int? SelectionEnd);

	public record AnimationSequence(string Name, int StartIndex, int EndIndex, int CurrentFrame = 0);

	public class DatObjectEditorViewModel : ReactiveObject, ILocoFileViewModel
	{
		public ReactiveCommand<Unit, Unit> ReloadObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveAsObjectCommand { get; init; }

		[Reactive]
		public StringTableViewModel? StringTableViewModel { get; set; }

		[Reactive]
		public IExtraContentViewModel? ExtraContentViewModel { get; set; }

		ObjectEditorModel Model { get; init; }

		[Reactive]
		public UiLocoFile? CurrentObject { get; private set; }

		[Reactive]
		public ObservableCollection<TreeNode> CurrentHexAnnotations { get; private set; }

		[Reactive]
		public TreeNode? CurrentlySelectedHexAnnotation { get; set; }

		[Reactive]
		public HexAnnotationLine[]? CurrentHexDumpLines { get; set; }

		[Reactive]
		public FileSystemItem CurrentFile { get; init; }

		public string ReloadText => CurrentFile.FileLocation == FileLocation.Local ? "Reload" : "Redownload";
		public string SaveText => CurrentFile.FileLocation == FileLocation.Local ? "Save" : "Download";
		public string SaveAsText => $"{SaveText} As";

		public string ReloadIcon => CurrentFile.FileLocation == FileLocation.Local ? "DatabaseRefresh" : "FileSync";
		public string SaveIcon => CurrentFile.FileLocation == FileLocation.Local ? "ContentSave" : "FileDownload";
		public string SaveAsIcon => CurrentFile.FileLocation == FileLocation.Local ? "ContentSavePlus" : "FileDownloadOutline";

		byte[] currentByteList;

		Dictionary<string, (int Start, int End)> DATDumpAnnotationIdentifiers = [];
		const int bytesPerDumpLine = 32;
		const int addressStringSizeBytes = 8;
		const int addressStringSizePrependBytes = addressStringSizeBytes + 2;
		const int dumpWordSize = 4;

		ILogger? Logger => Model.Logger;

		public DatObjectEditorViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		{
			CurrentFile = currentFile;
			Model = model;

			LoadObject();

			ReloadObjectCommand = ReactiveCommand.Create(LoadObject);
			SaveObjectCommand = ReactiveCommand.Create(SaveCurrentObject);
			SaveAsObjectCommand = ReactiveCommand.Create(SaveAsCurrentObject);

			_ = this.WhenAnyValue(o => o.CurrentlySelectedHexAnnotation)
				.Subscribe(_ => UpdateHexDumpView());
		}

		public void UpdateHexDumpView()
		{
			if (CurrentlySelectedHexAnnotation != null && DATDumpAnnotationIdentifiers.TryGetValue(CurrentlySelectedHexAnnotation.Title, out var positionValues))
			{
				CurrentHexDumpLines = GetDumpLines(currentByteList, positionValues.Start, positionValues.End).ToArray();
			}
			else
			{
				CurrentHexDumpLines = GetDumpLines(currentByteList, null, null).ToArray();
			}
		}

		public void LoadObject()
		{
			// this stops any currently-playing sounds
			if (ExtraContentViewModel is SoundViewModel svm)
			{
				svm.Dispose();
			}

			if (CurrentFile is not FileSystemItem cf)
			{
				return;
			}

			Logger?.Info($"Loading {cf.Name} from {cf.Filename}");

			if (Model.TryLoadObject(cf, out var newObj))
			{
				CurrentObject = newObj;

				if (CurrentObject?.LocoObject != null)
				{
					StringTableViewModel = new(CurrentObject.LocoObject.StringTable);
					ExtraContentViewModel = CurrentObject.LocoObject.Object is SoundObject
						? new SoundViewModel(CurrentObject.LocoObject)
						: new ImageTableViewModel(CurrentObject.LocoObject, Model.PaletteMap, CurrentObject.Images);

					var (treeView, annotationIdentifiers) = AnnotateFile(Path.Combine(Model.Settings.ObjDataDirectory, cf.Filename), false, null);
					CurrentHexAnnotations = new(treeView);
					DATDumpAnnotationIdentifiers = annotationIdentifiers;
				}
				else
				{
					StringTableViewModel = null;
					ExtraContentViewModel = null;
				}
			}
			else
			{
				// todo: show warnings here
				CurrentObject = null;
			}
		}

		public void SaveCurrentObject()
		{
			if (CurrentObject?.LocoObject == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			var savePath = CurrentFile.FileLocation == FileLocation.Local ? CurrentFile.Filename : Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.Name, ".dat"));
			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {savePath}");
			SawyerStreamWriter.Save(savePath, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject);
		}

		public void SaveAsCurrentObject()
		{

			var saveFile = Task.Run(PlatformSpecific.SaveFilePicker).Result;
			if (saveFile == null)
			{
				return;
			}

			if (CurrentObject?.LocoObject == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			var savePath = saveFile.Path.LocalPath;
			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {savePath}");
			SawyerStreamWriter.Save(savePath, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject);
		}

		(IList<TreeNode> treeView, Dictionary<string, (int, int)> annotationIdentifiers) AnnotateFile(string path, bool isG1 = false, ILogger? logger = null)
		{
			try
			{
				if (!File.Exists(path))
				{
					return ([], []);
				}

				IList<HexAnnotation> annotations = [];

				currentByteList = File.ReadAllBytes(path);
				var resultingByteList = currentByteList;
				annotations = isG1
					? ObjectAnnotator.AnnotateG1Data(currentByteList)
					: ObjectAnnotator.Annotate(currentByteList, out resultingByteList);

				return AnnotateFileCore(annotations);
			}
			catch (Exception ex)
			{
				logger?.Error(ex);
				return ([], []);
			}
		}

		static (IList<TreeNode> treeView, Dictionary<string, (int, int)> annotationIdentifiers) AnnotateFileCore(IList<HexAnnotation> annotations)
		{
			var treeView = new List<TreeNode>();
			var parents = new Dictionary<string, TreeNode>();
			Dictionary<string, TreeNode> imageHeaderIndexToNode = [];
			Dictionary<string, TreeNode> imageDataIndexToNode = [];
			Dictionary<string, (int, int)> datDumpAnnotationIdentifiers = [];

			foreach (var annotation in annotations)
			{
				var annotationText = annotation.Name;
				parents[annotationText] = new TreeNode(annotation.Name, annotation.OffsetText);
				datDumpAnnotationIdentifiers[annotationText] = (annotation.Start, annotation.End);
				if (annotation.Parent == null)
				{
					treeView.Add(parents[annotation.Name]);
				}
				else if (parents.TryGetValue(annotation.Parent.Name, out var parentNode))
				{
					parentNode.Nodes.Add(parents[annotationText]);

					if (annotation.Parent.Name == "Headers")
					{
						imageHeaderIndexToNode[annotation.Name] = parents[annotationText];
					}

					if (annotation.Parent.Name == "Images")
					{
						imageDataIndexToNode[annotation.Name] = parents[annotationText];
					}
				}
			}

			return (treeView, datDumpAnnotationIdentifiers);
		}

		private static IEnumerable<HexAnnotationLine> GetDumpLines(byte[] byteList, int? selectionStart, int? selectionEnd)
		{
			if (byteList == null)
			{
				yield break;
			}

			var count = 0;

			foreach (var b in byteList.Chunk(bytesPerDumpLine))
			{
				int? sStart = selectionStart != null && (selectionStart >= count || selectionEnd > count)
					? (Math.Max(selectionStart.Value, count) * 2) - (count * 2) : null;

				int? sEnd = selectionEnd != null && sStart != null && selectionEnd >= count
					? (Math.Min(selectionEnd.Value, count + bytesPerDumpLine) * 2) - (count * 2) : null;

				yield return new HexAnnotationLine(
					string.Format("{0:X" + addressStringSizeBytes + "}", count),
					string.Join(" ", b.Chunk(4).Select(x => string.Concat(x.Select(y => y.ToString("X2"))))),
					sStart + (sStart / 8),
					sEnd + (sEnd / 8));

				count += bytesPerDumpLine;
			}
		}
	}

	public class TreeNode(string title, string offsetText, ObservableCollection<TreeNode> nodes)
	{
		public ObservableCollection<TreeNode>? Nodes { get; } = nodes;
		public string Title { get; } = title;
		public string OffsetText { get; } = offsetText;
		public TreeNode() : this("<empty>", "<empty>") { }

		public TreeNode(string title, string offsetText) : this(title, offsetText, []) { }
	}
}
