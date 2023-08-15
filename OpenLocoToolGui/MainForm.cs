using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoToolCommon;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLocoToolGui
{
	public partial class MainForm : Form
	{
		private ILogger logger;
		private SawyerStreamReader reader;
		private SawyerStreamWriter writer;
		private const string BaseDirectory = @"Q:\Steam\steamapps\common\Locomotion\ObjData";
		private const string IndexFilename = "ObjectIndex.json";

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

		Dictionary<string, ObjectHeader> headerIndex = new(); // key is full path/filename
		Dictionary<string, ILocoObject> objectCache = new(); // key is full path/filename
		string currentDir;

		private void MainForm_Load(object sender, EventArgs e)
		{
			Init(BaseDirectory);
		}

		void Init(string directory)
		{
			currentDir = directory;
			InitialiseIndex();
			// SerialiseHeaderIndexToFile(); // optional - index creation is so fast it's not really necessary to cache this
			InitFileTreeView();
			InitCategoryTreeView();
		}

		void InitialiseIndex()
		{
			var allFiles = Directory.GetFiles(currentDir, "*.dat", SearchOption.AllDirectories);
			headerIndex.Clear();
			foreach (var file in allFiles)
			{
				var objectHeader = reader.LoadHeader(file);
				if (!headerIndex.TryAdd(file, objectHeader))
				{
					logger.Warning($"Didn't add file {file} - already exists (how???)");
				}
			}
		}

		void SerialiseHeaderIndexToFile()
		{
			var json = JsonSerializer.Serialize(headerIndex, new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, });
			File.WriteAllText(Path.Combine(currentDir, IndexFilename), json);
		}

		void InitFileTreeView(string fileFilter = "")
		{
			tvFileTree.SuspendLayout();
			tvFileTree.Nodes.Clear();
			var filteredFiles = headerIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			foreach (var obj in filteredFiles)
			{
				var relative = Path.GetRelativePath(currentDir, obj.Key);
				tvFileTree.Nodes.Add(obj.Key, relative);
			}
			tvFileTree.ResumeLayout(true);
		}

		void InitCategoryTreeView(string fileFilter = "")
		{
			tvObjType.SuspendLayout();
			tvObjType.Nodes.Clear();
			var filteredFiles = headerIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			foreach (var group in filteredFiles.GroupBy(kvp => kvp.Value.ObjectType))
			{
				var typeNode = new TreeNode(group.Key.ToString());
				foreach (var obj in group)
				{
					typeNode.Nodes.Add(obj.Key, obj.Value.Name);
				}
				tvObjType.Nodes.Add(typeNode);
			}

			tvObjType.ResumeLayout(true);
		}

		// note: doesn't work atm
		private void btnSaveChanges_Click(object sender, EventArgs e)
		{
			var obj = (ILocoObject)pgObject.SelectedObject;
			saveFileDialog1.InitialDirectory = currentDir;
			saveFileDialog1.DefaultExt = "dat";
			saveFileDialog1.Filter = "Locomotion DAT files (.dat)|*.dat";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				var filename = saveFileDialog1.FileName;

				try
				{
					//writer.Save(path, obj);
					MessageBox.Show($"File \"{filename}\" saved successfully");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error saving \"{filename}\": " + ex.Message);
				}
			}
		}

		private void btnSetDirectory_Click(object sender, EventArgs e)
		{
			if (objectDirBrowser.ShowDialog(this) == DialogResult.OK)
			{
				Init(objectDirBrowser.SelectedPath);
			}
		}

		private void tbFileFilter_TextChanged(object sender, EventArgs e)
		{
			InitFileTreeView();
			InitCategoryTreeView();
		}

		private void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}

			pgObject.SelectedObject = LoadAndCacheObject(e.Node.Name);
		}

		ILocoObject LoadAndCacheObject(string filename)
		{
			if (!objectCache.ContainsKey(filename) && !string.IsNullOrEmpty(filename) && filename.EndsWith(".dat", StringComparison.InvariantCultureIgnoreCase))
			{
				objectCache.Add(filename, reader.LoadFull(filename));
			}

			return objectCache[filename];
		}
	}
}