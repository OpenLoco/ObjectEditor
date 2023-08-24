using NAudio.Wave;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLocoToolGui
{
	// how this program works
	//
	// 1. open UI, no loading
	// 2. user selects a directory
	// 3. if no open-loco-tool index file exists, open-loco-tool fully loads all dat files in directory, creates an index and writes it to `objectIndex.json` in that folder. this is SLOW (currently)
	// 4. next time that directory is opened, the index is read instead of loading all files. this is FAST

	public partial class MainForm : Form
	{
		private ILogger logger;
		private SawyerStreamReader reader;
		private SawyerStreamWriter writer;

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

		Dictionary<string, IndexObjectHeader> headerIndex = new(); // key is full path/filename
		Dictionary<string, ILocoObject> objectCache = new(); // key is full path/filename

		GuiSettings Settings { get; set; }

		private void MainForm_Load(object sender, EventArgs e)
		{
			// setup dark mode???
			//DarkModify(this);

			// restore previous session, if any
			LoadSettings();
			if (File.Exists(Settings.IndexFilePath))
			{
				logger.Info($"Loading header index from \"{Settings.IndexFileName}\"");
				DeserialiseHeaderIndexFromFile();
				InitFileTreeView();
				InitCategoryTreeView();
			}
		}

		const string SettingsFile = "./settings.json";

		Color DarkModeBackColor = Color.FromArgb(31, 31, 31);
		Color DarkModeForeColor = Color.White;

		// poor-mans dark mode
		void DarkModify(Control control)
		{
			foreach (Control c in control.Controls)
			{
				c.BackColor = DarkModeBackColor;
				c.ForeColor = DarkModeForeColor;

				DarkModify(c);
			}
		}

		void LoadSettings()
		{
			if (!File.Exists(SettingsFile))
			{
				Settings = new();
				SaveSettings();
				return;
			}

			var text = File.ReadAllText(SettingsFile);
			Settings = JsonSerializer.Deserialize<GuiSettings>(text);

			ValidateSettings(Settings, logger);
		}

		static void ValidateSettings(GuiSettings settings, ILogger logger)
		{
			if (settings == null)
			{
				logger.Error($"Unable to load settings");
				return;
			}

			if (string.IsNullOrEmpty(settings.ObjectDirectory))
			{
				logger.Warning("Object directory was null or empty");
				return;
			}

			if (!Directory.Exists(settings.ObjectDirectory))
			{
				logger.Warning($"Directory \"{settings.ObjectDirectory}\" does not exist");
				return;
			}
		}

		void SaveSettings()
		{
			var text = JsonSerializer.Serialize(Settings, new JsonSerializerOptions() { WriteIndented = true });
			File.WriteAllText(SettingsFile, text);
		}

		void InitUI()
		{
			if (File.Exists(Settings.IndexFilePath))
			{
				logger.Info($"Loading header index from \"{Settings.IndexFileName}\"");
				DeserialiseHeaderIndexFromFile();
			}
			else
			{
				logger.Info($"Index file doesn't exist; creating file \"{Settings.IndexFileName}\"");
				if (CreateIndex())
				{
					SerialiseHeaderIndexToFile();
				}
			}

			InitFileTreeView();
			InitCategoryTreeView();
		}

		bool CreateIndex()
		{
			if (string.IsNullOrEmpty(Settings.ObjectDirectory))
			{
				logger.Warning("Settings.ObjectDirectory not set");
				return false;
			}

			var allFiles = Directory.GetFiles(Settings.ObjectDirectory, "*.dat", SearchOption.AllDirectories);
			using (var progressForm = new ProgressBarForm())
			{
				progressForm.Text = $"Indexing {allFiles.Length} files";
				var progress = new Progress<float>(f => progressForm.SetProgress((int)(f * 100)));
				var thread = new Thread(() =>
				{
					CreateIndexCore(allFiles, progress);
					progressForm.CloseForm();
				});
				thread.Start();
				progressForm.ShowDialog();
			}

			return true;
		}

		// this method loads every single object entirely. it takes a long time to run
		void CreateIndexCore(string[] allFiles, IProgress<float> progress)
		{
			ConcurrentDictionary<string, IndexObjectHeader> ccHeaderIndex = new(); // key is full path/filename
			ConcurrentDictionary<string, ILocoObject> ccObjectCache = new(); // key is full path/filename

			var total = (float)allFiles.Length;
			int count = 0;
			Parallel.ForEach(allFiles, (file) =>
			{
				var locoObject = reader.LoadFull(file);
				if (!ccObjectCache.TryAdd(file, locoObject))
				{
					//logger.Warning($"Didn't add file {file} to cache - already exists (how???)");
				}

				VehicleType? veh = null;
				if (locoObject.Object is VehicleObject vo)
					veh = vo.Type;

				var indexObjectHeader = new IndexObjectHeader(locoObject.ObjectHeader.Name, locoObject.ObjectHeader.ObjectType, veh);
				if (!ccHeaderIndex.TryAdd(file, indexObjectHeader))
				{
					//logger.Warning($"Didn't add file {file} to index - already exists (how???)");
				}

				Interlocked.Increment(ref count);
				progress.Report(count / total);
			});

			headerIndex = ccHeaderIndex.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			objectCache = ccObjectCache.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		void SerialiseHeaderIndexToFile()
		{
			var json = JsonSerializer.Serialize(headerIndex, new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, });
			File.WriteAllText(Path.Combine(Settings.ObjectDirectory, Settings.IndexFilePath), json);
		}

		void DeserialiseHeaderIndexFromFile()
		{
			var json = File.ReadAllText(Settings.IndexFilePath);
			headerIndex = JsonSerializer.Deserialize<Dictionary<string, IndexObjectHeader>>(json, new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, });
		}

		void InitFileTreeView(string fileFilter = "")
		{
			tvFileTree.SuspendLayout();
			tvFileTree.Nodes.Clear();
			var filteredFiles = headerIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			foreach (var obj in filteredFiles)
			{
				var relative = Path.GetRelativePath(Settings.ObjectDirectory, obj.Key);
				tvFileTree.Nodes.Add(obj.Key, relative);
			}

			tvFileTree.Sort();
			tvFileTree.ResumeLayout(true);
		}

		void InitCategoryTreeView(string fileFilter = "")
		{
			tvObjType.SuspendLayout();
			tvObjType.Nodes.Clear();

			var filteredFiles = headerIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			var nodesToAdd = new List<TreeNode>();
			foreach (var group in filteredFiles.GroupBy(kvp => kvp.Value.ObjectType))
			{
				var typeNode = new TreeNode(group.Key.ToString());
				if (group.Key != ObjectType.vehicle)
				{
					foreach (var obj in group)
					{
						typeNode.Nodes.Add(obj.Key, obj.Value.Name);
					}
				}
				else
				{
					var vehicleGroup = group.GroupBy(o => o.Value.VehicleType);
					foreach (var vehicleType in vehicleGroup)
					{
						var vehicleTypeNode = new TreeNode(vehicleType.Key.ToString());
						foreach (var veh in vehicleType)
						{
							vehicleTypeNode.Nodes.Add(veh.Key, veh.Value.Name);
						}
						typeNode.Nodes.Add(vehicleTypeNode);
					}
				}

				nodesToAdd.Add(typeNode);
			}

			//nodesToAdd.Sort((a, b) => a.Text.CompareTo(b.Text));

			tvObjType.Sort();
			tvObjType.Nodes.AddRange(nodesToAdd.ToArray());

			tvObjType.ResumeLayout(true);
		}

		// note: doesn't work atm
		private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var obj = (ILocoObject)pgObject.SelectedObject;
			saveFileDialog1.InitialDirectory = Settings.ObjectDirectory;
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

		private void setObjectDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (objectDirBrowser.ShowDialog(this) == DialogResult.OK)
			{
				Settings.ObjectDirectory = objectDirBrowser.SelectedPath;
				SaveSettings();
				logger.Info($"Settings.ObjectDirectory set to \"{Settings.ObjectDirectory}\"");
				InitUI();
			}
		}

		private void recreateIndexToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (CreateIndex())
			{
				SerialiseHeaderIndexToFile();
			}
		}

		void tbFileFilter_TextChanged(object sender, EventArgs e)
		{
			InitFileTreeView(tbFileFilter.Text);
			InitCategoryTreeView(tbFileFilter.Text);
		}

		void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}

			var obj = LoadAndCacheObject(e.Node.Name);

			if (obj != null && obj.G1Elements != null && obj.G1Header != null && obj.G1Header.TotalSize != 0 && obj.G1Elements.Count != 0)
			{
				CreateImages(obj);
			}

			if (obj != null && obj.Object is SoundObject soundObject)
			{
				CreateSounds(soundObject);
			}

			pgObject.SelectedObject = obj;
		}

		void CreateSounds(SoundObject soundObject)
		{
			flpImageTable.SuspendLayout();
			flpImageTable.Controls.Clear();

			var pcmHeader = soundObject.SoundObjectData.PcmHeader;

			var soundButton = new Button
			{
				Size = new Size(100, 100),
				Text = "Play sound",
			};

			soundButton.Click += (args, sender) =>
			{
				// do it asyncly to a) give user ui control and b) allow multiple sounds to play at once
				Task.Run(() =>
				{
					using (var ms = new MemoryStream(soundObject.RawPcmData))
					using (var rs = new RawSourceWaveStream(ms, new WaveFormat(pcmHeader.SamplesPerSecond, 16, pcmHeader.NumberChannels)))
					using (var wo = new WaveOutEvent())
					{
						wo.Init(rs);
						wo.Play();
						while (wo.PlaybackState == PlaybackState.Playing)
						{
							Thread.Sleep(50);
						}
					}
				});
			};

			flpImageTable.Controls.Add(soundButton);

			flpImageTable.ResumeLayout(true);
		}

		void CreateImages(ILocoObject obj)
		{
			flpImageTable.SuspendLayout();
			flpImageTable.Controls.Clear();

			// todo: add user to supply this file
			var paletteBitmap = new Bitmap(Settings.PaletteFile);
			var palette = PaletteFromBitmap(paletteBitmap);

			for (var i = 0; i < obj.G1Elements.Count; ++i)
			{
				var currElement = obj.G1Elements[i];
				var imageData = currElement.ImageData;

				if (currElement.ImageData.Length == 0 || currElement.flags.HasFlag(G1ElementFlags.IsR8G8B8Palette))
				{
					logger.Info($"skipped loading g1 element {i} with flags {currElement.flags}");
					continue;
				}

				var dstImg = new Bitmap(currElement.width, currElement.height);
				var rect = new Rectangle(0, 0, currElement.width, currElement.height);
				var dstImgData = dstImg.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				for (var y = 0; y < currElement.height; ++y)
				{
					for (var x = 0; x < currElement.width; ++x)
					{
						var paletteIndex = imageData[(y * currElement.width) + x];

						// the issue with greyscale here is it isn't normalised so all heightmaps are really dark and hard to see
						//var colour = obj.Object is HillShapesObject
						//	? Color.FromArgb(paletteIndex, paletteIndex, paletteIndex) // for hillshapes, its just a heightmap so lets put it in greyscale
						//	: palette[paletteIndex];

						var colour = palette[paletteIndex];
						SetPixel(dstImgData, x, y, colour);
					}
				}

				dstImg.UnlockBits(dstImgData);

				var pb = new PictureBox
				{
					Image = dstImg,
					BorderStyle = BorderStyle.FixedSingle,
					SizeMode = PictureBoxSizeMode.AutoSize,
				};
				flpImageTable.Controls.Add(pb);
			}

			flpImageTable.ResumeLayout(true);
		}

		ILocoObject? LoadAndCacheObject(string filename)
		{
			if (!objectCache.ContainsKey(filename) && !string.IsNullOrEmpty(filename) && filename.EndsWith(".dat", StringComparison.InvariantCultureIgnoreCase))
			{
				objectCache.TryAdd(filename, reader.LoadFull(filename));
			}

			if (objectCache.ContainsKey(filename))
			{
				return objectCache[filename];
			}

			return null;
		}

		Color[] PaletteFromBitmap(Bitmap img)
		{
			var palette = new Color[256];
			var rect = new Rectangle(0, 0, img.Width, img.Height);
			var imgData = img.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			for (var y = 0; y < 16; ++y)
			{
				for (var x = 0; x < 16; ++x)
				{
					var pixel = GetPixel(imgData, x, y);
					palette[(y * 16) + x] = pixel;
				}
			}

			img.UnlockBits(imgData);
			return palette;
		}

		unsafe Color GetPixel(BitmapData d, int X, int Y)
		{
			var ptr = GetPtrToFirstPixel(d, X, Y);
			return Color.FromArgb(ptr[2], ptr[1], ptr[0]); // alpha is ptr[3]);
		}

		unsafe void SetPixel(BitmapData d, Point p, Color c)
			=> SetPixel(d, p.X, p.Y, c);

		unsafe void SetPixel(BitmapData d, int X, int Y, Color c)
			=> SetPixel(GetPtrToFirstPixel(d, X, Y), c);

		static unsafe byte* GetPtrToFirstPixel(BitmapData d, int X, int Y)
			=> (byte*)d.Scan0.ToPointer() + (Y * d.Stride) + (X * (Image.GetPixelFormatSize(d.PixelFormat) / 8));

		//private static unsafe void SetPixel(byte* ptr, Color c)
		//{
		//	ptr[0] = c.B; // Blue
		//	ptr[1] = c.G; // Green
		//	ptr[2] = c.R; // Red
		//	ptr[3] = c.A; // Alpha
		//}

		static unsafe void SetPixel(byte* ptr, Color c)
		{
			ptr[0] = (byte)(c.B); // Blue
			ptr[1] = (byte)(c.G); // Green
			ptr[2] = (byte)(c.R); // Red
			ptr[3] = 255; // (byte)(c.A * 255); // Alpha
		}
	}
}