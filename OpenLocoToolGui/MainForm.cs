using OpenLocoTool;
using OpenLocoToolCommon;

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

		private void MainForm_Load(object sender, EventArgs e) => ListDirectory(treeView1, BasePath);

		private void ListDirectory(TreeView treeView, string path)
		{
			treeView.Nodes.Clear();
			var rootDirectoryInfo = new DirectoryInfo(path);
			treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
		}

		private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			var directoryNode = new TreeNode(directoryInfo.Name);
			foreach (var directory in directoryInfo.GetDirectories())
			{
				directoryNode.Nodes.Add(CreateDirectoryNode(directory));
			}

			foreach (var file in directoryInfo.GetFiles())
			{
				directoryNode.Nodes.Add(new TreeNode(file.Name));
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

			var name = PathInTreeView(e.Node);
			if (name.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
			{
				// selected node changed - updated view
				//MessageBox.Show(e.Node.Text);
				var filename = Path.Combine(BasePath, name);
				var loaded = reader.Load(filename);

				pgDatHeader.SelectedObject = loaded.datHdr;
				pgObjHeader.SelectedObject = loaded.objHdr;
				pgObject.SelectedObject = loaded.obj;
			}
		}

		private void btnSaveChanges_Click(object sender, EventArgs e)
		{
			// todo: implement sawyerstreamwriter
		}
	}
}