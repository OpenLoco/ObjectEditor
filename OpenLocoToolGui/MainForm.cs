using OpenLocoTool.DatFileParsing;
using OpenLocoToolCommon;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace OpenLocoToolGui
{
	public partial class MainForm : Form
	{
		private ILogger logger;
		private SawyerStreamReader reader;
		private SawyerStreamWriter writer;
		private const string BasePath = @"Q:\Steam\steamapps\common\Locomotion\ObjData";

		public MainForm()
		{
			InitializeComponent();

			logger = new Logger
			{
				Level = LogLevel.Debug2
			};
			((Logger)logger).LogAdded += (s, e) => lbLogs.Items.Insert(0, e.Log.ToString());

			reader = new SawyerStreamReader(logger);
			writer = new SawyerStreamWriter(logger);
		}

		Dictionary<string, ILocoObject> cache = new();
		private BackgroundWorker worker;

		private void MainForm_Load(object sender, EventArgs e)
		{
			ListDirectory();
		}

		private void CreateIndex_DoWork()
		{
			// load every object
			var rootDirectoryInfo = new DirectoryInfo(BasePath);
			var files = rootDirectoryInfo.GetFiles().Where(f => f.Extension.Equals(".dat", StringComparison.OrdinalIgnoreCase)).ToList();
			var count = (float)files.Count;
			var counter = 0;
			foreach (var fileInfo in files.Where(fi => !cache.ContainsKey(fi.Name)))
			{
				cache.Add(fileInfo.Name, reader.Load(fileInfo));
				var percentCompletion = (int)(++counter / count * 100);
				progressBar1.Value = percentCompletion;
				//worker.ReportProgress(percentCompletion, $"Processed {fileInfo.Name}");
			}

			// generate treeview
			var grouping = cache.Values.GroupBy(kvp => kvp.ObjectHeader.ObjectType);
			tvObjType.SuspendLayout();
			var categoryNodes = new List<TreeNode>();
			foreach (var group in grouping)
			{
				var categoryNode = new TreeNode(group.Key.ToString());
				foreach (var obj in group)
				{
					categoryNode.Nodes.Add(new TreeNode(obj.Filename));
				}
				categoryNode.Collapse();
				categoryNodes.Add(categoryNode);
			}
			//categoryNodes.Sort();
			//tvObjType.Nodes.AddRange(categoryNodes);
			var sorted = categoryNodes.OrderBy(tn => tn.Text).ToArray();
			tvObjType.Nodes.AddRange(sorted);
			tvObjType.ResumeLayout(true);
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//progressDialog.Close();
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar1.Value = e.ProgressPercentage;
			logger.Info(e.UserState.ToString());
			//progressDialog.UpdateProgress(e.ProgressPercentage, e.UserState.ToString());
		}

		void ListDirectory()
			=> ListDirectory(tvFileTree, BasePath, tbFileFilter.Text);

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

			var fileInfo = new FileInfo(Path.Combine(BasePath, name));
			ILocoObject locoObject;
			if (!cache.ContainsKey(fileInfo.Name))
			{
				locoObject = reader.Load(fileInfo);
				cache.Add(fileInfo.Name, locoObject);
			}

			locoObject = cache[fileInfo.Name];
			pgObject.SelectedObject = locoObject;
		}

		private void btnSaveChanges_Click(object sender, EventArgs e)
		{
			var obj = (ILocoObject)pgObject.SelectedObject;
			saveFileDialog1.InitialDirectory = BasePath;
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				var path = saveFileDialog1.FileName;
				saveFileDialog1.DefaultExt = "dat";
				saveFileDialog1.Filter = "Locomotion DAT files (.dat)|*.dat";

				try
				{
					writer.Save(path, obj);
					MessageBox.Show("File saved successfully");
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error: " + ex.Message);
				}
			}
		}

		private void btnSetDirectory_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.ShowDialog(this);
		}

		private void tbFileFilter_TextChanged(object sender, EventArgs e)
		{
			ListDirectory();
		}

		private void btnParseAll_Click(object sender, EventArgs e)
		{
			CreateIndex_DoWork();
			//worker = new BackgroundWorker();
			//worker.DoWork += CreateIndex_DoWork;
			//worker.ProgressChanged += worker_ProgressChanged;
			//worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			//worker.RunWorkerAsync();
		}

		private void tvObjType_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (cache.ContainsKey(e.Node.Text))
			{
				pgObject.SelectedObject = cache[e.Node.Text];
			}
		}
	}
}