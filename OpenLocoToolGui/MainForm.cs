using OpenLocoTool;
using OpenLocoToolCommon;
using System.Text.RegularExpressions;

namespace OpenLocoToolGui
{
	public partial class MainForm : Form
	{
		private Logger logger;
		private SawyerStreamReader reader;
		private const string BasePath = @"Q:\Steam\steamapps\common\Locomotion\ObjData";

		public MainForm()
		{
			InitializeComponent();

			logger = new Logger
			{
				Level = LogLevel.Debug2
			};
			logger.LogAdded += (s, e) => lbLogs.Items.Insert(0, e.Log.ToString());

			reader = new SawyerStreamReader(logger);
		}

		private void MainForm_Load(object sender, EventArgs e) => ListDirectory();

		void ListDirectory()
			=> ListDirectory(treeView1, BasePath, tbFileFilter.Text);

		private static void ListDirectory(TreeView treeView, string path, string regexFilter)
		{
			treeView.Nodes.Clear();
			var rootDirectoryInfo = new DirectoryInfo(path);
			treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo, regexFilter));
			treeView.TopNode.Expand();
		}

		private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo, string regexFilter)
		{
			var directoryNode = new TreeNode(directoryInfo.Name);
			foreach (var directory in directoryInfo.GetDirectories())
			{
				var newDir = CreateDirectoryNode(directory, regexFilter);
				if (newDir.Nodes.Count > 0)
				{
					directoryNode.Nodes.Add(newDir);
				}
			}

			foreach (var file in directoryInfo.GetFiles())
			{
				if (Regex.Matches(file.Name, regexFilter, RegexOptions.IgnoreCase).Count > 0)
				{
					directoryNode.Nodes.Add(new TreeNode(file.Name));
				}
			}

			return directoryNode;
		}

		string PathInTreeView(TreeNode node)
		{
			if (node == null)
			{
				return string.Empty;
			}

			var str = string.Empty;
			if (node.Parent != null)
			{
				str = Path.Combine(PathInTreeView(node.Parent), node.Text);
			}
			return str;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}

			var name = PathInTreeView(e.Node); // use e.Node.FullPath instead...
			if (!name.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			var filename = Path.Combine(BasePath, name);
			ILocoObject locoObject;
			if (!cache.ContainsKey(filename))
			{
				locoObject = reader.Load(filename);
				cache.Add(filename, locoObject);
			}

			locoObject = cache[filename];
			pgObject.SelectedObject = locoObject;
		}

		Dictionary<string, ILocoObject> cache = new();

		private void btnSaveChanges_Click(object sender, EventArgs e)
		{
			// todo: implement sawyerstreamwriter
		}

		private void btnSetDirectory_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.ShowDialog(this);
		}

		private void tbFileFilter_TextChanged(object sender, EventArgs e)
		{
			ListDirectory();
		}
	}
}