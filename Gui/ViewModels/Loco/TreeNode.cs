using System.Collections.ObjectModel;

namespace Gui.ViewModels.Loco;

public class TreeNode(string title, string offsetText, ObservableCollection<TreeNode> nodes)
{
	public ObservableCollection<TreeNode> Nodes { get; } = nodes;
	public string Title { get; } = title;
	public string OffsetText { get; } = offsetText;
	public TreeNode() : this("<empty>", "<empty>") { }

	public TreeNode(string title, string offsetText) : this(title, offsetText, []) { }
}
