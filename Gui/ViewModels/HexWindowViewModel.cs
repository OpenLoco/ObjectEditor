using Common.Logging;
using Dat;
using Dat.FileParsing;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Gui.ViewModels;

public class HexWindowViewModel : ViewModelBase
{
	public ObservableCollection<TreeNode> CurrentHexAnnotations { get; } = [];

	[Reactive]
	public TreeNode? CurrentlySelectedHexAnnotation { get; set; }

	[Reactive]
	public HexAnnotationLine[] CurrentHexDumpLines { get; set; } = [];

	byte[] currentByteList = [];

	readonly Dictionary<string, (int Start, int End)> DATDumpAnnotationIdentifiers = [];
	const int bytesPerDumpLine = 32;
	const int addressStringSizeBytes = 8;

	public HexWindowViewModel()
	{ }

	public HexWindowViewModel(string filename, ILogger logger)
	{
		_ = this.WhenAnyValue(o => o.CurrentlySelectedHexAnnotation)
			.Subscribe(_ => UpdateHexDumpView());

		var (treeView, annotationIdentifiers) = AnnotateFile(filename, logger, false);
		CurrentHexAnnotations = new(treeView);
		DATDumpAnnotationIdentifiers = annotationIdentifiers;
	}

	public void UpdateHexDumpView()
		=> CurrentHexDumpLines = CurrentlySelectedHexAnnotation != null && DATDumpAnnotationIdentifiers.TryGetValue(CurrentlySelectedHexAnnotation.Title, out var positionValues)
			? [.. GetDumpLines(currentByteList, positionValues.Start, positionValues.End)]
			: [.. GetDumpLines(currentByteList, null, null)];

	static IEnumerable<HexAnnotationLine> GetDumpLines(byte[] byteList, int? selectionStart, int? selectionEnd)
	{
		if (byteList == null)
		{
			yield break;
		}

		var count = 0;

		foreach (var b in byteList.Chunk(bytesPerDumpLine))
		{
			int? sStart = selectionStart != null && (selectionStart >= count || selectionEnd > count)
				? (Math.Max(selectionStart.Value, count) * 2) - (count * 2)
				: null;

			int? sEnd = selectionEnd != null && sStart != null && selectionEnd >= count
				? (Math.Min(selectionEnd.Value, count + bytesPerDumpLine) * 2) - (count * 2)
				: null;

			yield return new HexAnnotationLine(
				string.Format("{0:X" + addressStringSizeBytes + "}", count),
				string.Join(" ", b.Chunk(4).Select(x => string.Concat(x.Select(y => y.ToString("X2"))))),
				sStart + (sStart / 8),
				sEnd + (sEnd / 8));

			count += bytesPerDumpLine;
		}
	}

	(IList<TreeNode> treeView, Dictionary<string, (int, int)> annotationIdentifiers) AnnotateFile(string path, ILogger logger, bool isG1 = false)
	{
		if (!File.Exists(path))
		{
			return ([], []);
		}

		IList<HexAnnotation> annotations;
		currentByteList = File.ReadAllBytes(path);

		try
		{
			annotations = isG1
				? ObjectAnnotator.AnnotateG1Data(currentByteList)
				: ObjectAnnotator.Annotate(currentByteList);

			return AnnotateFileCore(annotations);
		}
		catch (Exception ex)
		{
			logger.Error(ex);
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

}
