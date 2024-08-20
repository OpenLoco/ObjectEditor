using ReactiveUI;
using AvaGui.Models;
using OpenLoco.Dat.FileParsing;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Threading.Tasks;
using OpenLoco.Common;
using OpenLoco.Dat.Objects.Sound;
using OpenLoco.Common.Logging;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;
using OpenLoco.Dat;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject, ILocoFileViewModel
	{
		public ReactiveCommand<Unit, Unit> ReloadObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveAsObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveMetadataCommand { get; init; }

		[Reactive]
		public StringTableViewModel? StringTableViewModel { get; set; }

		[Reactive]
		public IExtraContentViewModel? ExtraContentViewModel { get; set; }

		ObjectEditorModel Model { get; init; }

		[Reactive]
		public UiLocoFile? CurrentObject { get; private set; }

		[Reactive]
		public ObjectMetadata CurrentMetadata { get; private set; }

		[Reactive]
		public ObservableCollection<TreeNode> CurrentHexAnnotations { get; private set; }

		[Reactive]
		public TreeNode? CurrentlySelectedHexAnnotation { get; set; }

		[Reactive]
		public string[]? CurrentHexDumpLines { get; set; }

		[Reactive]
		public FileSystemItemBase CurrentFile { get; init; }


		Dictionary<string, (int, int)> DATDumpAnnotationIdentifiers = [];
		const int bytesPerDumpLine = 32;
		const int addressStringSizeBytes = 8;
		const int addressStringSizePrependBytes = addressStringSizeBytes + 2;
		const int dumpWordSize = 4;

		ILogger? Logger => Model.Logger;

		public ObjectEditorViewModel(FileSystemItemBase currentFile, ObjectEditorModel model)
		{
			CurrentFile = currentFile;
			Model = model;

			LoadObject();

			ReloadObjectCommand = ReactiveCommand.Create(LoadObject);
			SaveObjectCommand = ReactiveCommand.Create(SaveCurrentObject);
			SaveAsObjectCommand = ReactiveCommand.Create(SaveAsCurrentObject);
			SaveMetadataCommand = ReactiveCommand.Create(SaveCurrentMetadata);

			_ = this.WhenAnyValue(o => o.CurrentlySelectedHexAnnotation)
				.Subscribe(_ => UpdateHexDumpView());
		}

		public void UpdateHexDumpView()
		{
			//int dumpPositionToRTBPosition(int position) => rtbDATDumpView.GetFirstCharIndexFromLine(
			//	position / bytesPerDumpLine)
			//	+ (position % bytesPerDumpLine * 2)            // Bytes are displayed 2 characters wide
			//	+ (position % bytesPerDumpLine / dumpWordSize) // Every word is separated by an extra space
			//	+ addressStringSizePrependBytes;               // Each line starts with 10 characters indicating address

			//if (DATDumpAnnotationIdentifiers.TryGetValue(CurrentlySelectedHexAnnotation!.Title, out var positionValues))
			//{
			//	//var linePosStart = rtbDATDumpView.GetFirstCharIndexFromLine(positionValues.Item1 / bytesPerDumpLine);
			//	//var linePosEnd = rtbDATDumpView.GetFirstCharIndexFromLine(positionValues.Item2 / bytesPerDumpLine);

			//	var selectPositionStart = dumpPositionToRTBPosition(positionValues.Item1);
			//	var selectPositionEnd = Math.Min(dumpPositionToRTBPosition(positionValues.Item2), rtbDATDumpView.TextLength - 1);
			//	rtbDATDumpView.Select(selectPositionStart, selectPositionEnd - selectPositionStart);
			//}
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

			Logger?.Info($"Loading {cf.Name} from {cf.Path}");

			if (Model.TryLoadObject(cf, out var newObj))
			{
				CurrentObject = newObj;

				if (CurrentObject?.LocoObject != null)
				{
					StringTableViewModel = new(CurrentObject.LocoObject.StringTable);
					ExtraContentViewModel = CurrentObject.LocoObject.Object is SoundObject
						? new SoundViewModel(CurrentObject.LocoObject)
						: new ImageTableViewModel(CurrentObject.LocoObject, Model.PaletteMap);

					var name = CurrentObject.DatFileInfo.S5Header.Name;
					CurrentMetadata = Utils.LoadObjectMetadata(name, CurrentObject.DatFileInfo.S5Header.Checksum, Model.Metadata); // in future this will be an online-only service

					var (treeView, dumpLines, annotationIdentifiers) = AnnotateFile(cf.Path, false);
					CurrentHexAnnotations = new(treeView);
					CurrentHexDumpLines = dumpLines;
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

			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {CurrentFile.Path}");
			SawyerStreamWriter.Save(CurrentFile.Path, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject);
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

			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {saveFile.Path.AbsolutePath}");
			SawyerStreamWriter.Save(saveFile.Path.AbsolutePath, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject);
		}

		public void SaveCurrentMetadata()
		{
			Utils.SaveMetadata(Model.MetadataFilename, Model.Metadata);
		}

		static (IList<TreeNode> treeView, string[] dumpLines, Dictionary<string, (int, int)> annotationIdentifiers) AnnotateFile(string path, bool isG1 = false, ILogger? logger = null)
		{
			try
			{
				if (!File.Exists(path))
				{
					return ([], [], []);
				}

				IList<HexAnnotation> annotations = [];

				var byteList = File.ReadAllBytes(path);
				var resultingByteList = byteList;
				annotations = isG1
					? ObjectAnnotator.AnnotateG1Data(byteList)
					: ObjectAnnotator.Annotate(byteList, out resultingByteList);

				var (treeView, annotationIdentifiers) = AnnotateFileCore(annotations);
				var dumpLines = GetDumpLines(byteList);
				return (treeView, dumpLines, annotationIdentifiers);
			}
			catch (Exception ex)
			{
				logger?.Error(ex);
				return ([], [], []);
			}
		}

		static (IList<TreeNode> treeView, Dictionary<string, (int, int)> annotationIdentifiers) AnnotateFileCore(IList<HexAnnotation> annotations)
		{
			//tvDATDumpAnnotations.SuspendLayout();
			//tvDATDumpAnnotations.Nodes.Clear();
			var currentParent = new TreeNode();
			var tvDATDumpAnnotations = new List<TreeNode>();

			static string constructAnnotationText(HexAnnotation annotation)
				=> string.Format("{0} (0x{1:X}-0x{2:X})", annotation.Name, annotation.Start, annotation.End);

			var parents = new Dictionary<string, TreeNode>();
			Dictionary<string, TreeNode> imageHeaderIndexToNode = [];
			Dictionary<string, TreeNode> imageDataIndexToNode = [];
			Dictionary<string, (int, int)> datDumpAnnotationIdentifiers = [];

			foreach (var annotation in annotations)
			{
				var annotationText = constructAnnotationText(annotation);
				parents[annotationText] = new TreeNode(annotationText);
				datDumpAnnotationIdentifiers[annotationText] = (annotation.Start, annotation.End);
				if (annotation.Parent == null)
				{
					tvDATDumpAnnotations.Add(parents[constructAnnotationText(annotation)]);
				}
				else if (parents.ContainsKey(constructAnnotationText(annotation.Parent)))
				{
					var parentText = constructAnnotationText(annotation.Parent);
					parents[parentText].Nodes.Add(parents[annotationText]);

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

			//tvDATDumpAnnotations.ResumeLayout();
			//rtbDATDumpView.Text = string.Join("\n", dumpLines);

			return (tvDATDumpAnnotations, datDumpAnnotationIdentifiers);
		}

		private static string[] GetDumpLines(byte[] byteList)
		{
			var extraLine = byteList.Length % bytesPerDumpLine;
			if (extraLine > 0)
			{
				extraLine = 1;
			}


			var dumpLines = byteList
					.Select(b => string.Format("{0,2:X2}", b))
					.Chunk(dumpWordSize)
					.Select(c => string.Format("{0} ", string.Concat(c)))
					.Chunk(bytesPerDumpLine / dumpWordSize)
					.Zip(Enumerable.Range(0, (byteList.Length / bytesPerDumpLine) + extraLine))
					.Select(l => string.Format("{0:X" + addressStringSizeBytes + "}: {1}", l.Second * bytesPerDumpLine, string.Concat(l.First)))
					.ToArray();
			return dumpLines;
		}
	}

	public class TreeNode(string title, ObservableCollection<TreeNode> nodes)
	{
		public ObservableCollection<TreeNode>? Nodes { get; } = nodes;
		public string Title { get; } = title;

		public TreeNode() : this("<empty>") { }

		public TreeNode(string title) : this(title, []) { }
	}
}
