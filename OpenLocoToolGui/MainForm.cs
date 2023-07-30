using System.Runtime.InteropServices;
using OpenLocoTool;
using OpenLocoToolCommon;
using OpenLocoTool.Objects;

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
				LocoObject lo;
				//if (cache.ContainsKey(filename))
				//{
				//	lo = cache[filename];
				//}
				//else
				{
					lo = reader.Load(filename);
					lo.DatFileHeader.Checksum = 123;
					var foo = lo.DatFileHeader.Checksum;

					//cache.Add(filename, lo);
				}

				pgDatHeader.SelectedObject = lo.DatFileHeader;
				pgObjHeader.SelectedObject = lo.ObjHeader;

				//var genericMethod = typeof(LocoObject).GetMethod("DataAs");
				//var meth = genericMethod.MakeGenericMethod(loaded.UnderlyingObjectType());
				//meth.Invoke(loaded);

				SetGridObject(pgObject, lo);
				//pgObject.SelectedObject = lo.ObjectData;
			}
		}

		private void SetGridObject(PropertyGrid grid, LocoObject data)
		{
			if (data.DatFileHeader.ObjectType == ObjectType.vehicle)
			{
				grid.SelectedObject = data.DataAs<VehicleObject>();
			}
			//return data.DatFileHeader.ObjectType switch
			{
				//ObjectType.bridge => data.DataAs<BridgeObject>(),
				//ObjectType.building => MemoryMarshal.Read<BuildingObject>(data),
				//ObjectType.cargo => MemoryMarshal.Read<CargoObject>(data),
				//ObjectType.cliffEdge => MemoryMarshal.Read<CliffEdgeObject>(data),
				//ObjectType.climate => MemoryMarshal.Read<ClimateObject>(data),
				//ObjectType.competitor => MemoryMarshal.Read<CompetitorObject>(data),
				//ObjectType.currency => MemoryMarshal.Read<CurrencyObject>(data),
				//ObjectType.dock => MemoryMarshal.Read<DockObject>(data),
				//ObjectType.hillShapes => MemoryMarshal.Read<HillShapesObject>(data),
				//ObjectType.industry => MemoryMarshal.Read<IndustryObject>(data),
				//ObjectType.track => MemoryMarshal.Read<TrackObject>(data),
				//ObjectType.trackSignal => MemoryMarshal.Read<TrainSignalObject>(data),
				//ObjectType.tree => MemoryMarshal.Read<TreeObject>(data),
				//ObjectType.vehicle => MemoryMarshal.Read<VehicleObject>(data),
				//_ => null,
			};

			//Logger.Info(ReflectionLogger.ToString(obj));
			//return obj;
		}

		//Dictionary<string, LocoObject> cache = new();

		private void btnSaveChanges_Click(object sender, EventArgs e)
		{
			// todo: implement sawyerstreamwriter
		}

		private void btnSetDirectory_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.ShowDialog(this);
		}
	}
}