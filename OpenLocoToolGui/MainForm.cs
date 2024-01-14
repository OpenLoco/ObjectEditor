using NAudio.Gui;
using NAudio.Wave;
using OpenLocoTool;
using OpenLocoTool.Data;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;
using System.Drawing.Imaging;
using System.Xml.Linq;

namespace OpenLocoToolGui
{
	public partial class MainForm : Form
	{
		readonly MainFormModel model;
		readonly ILogger logger;

		// could use pgObject.SelectedObjectsChanged event, but we'll just do this for now
		public UiLocoObject? CurrentUIObject
		{
			get => currentUIObject;
			set
			{
				currentUIObject = value;
				RefreshObjectUI();
			}
		}
		UiLocoObject? currentUIObject;

		IList<Control> CurrentUIImages
		{
			get => currentUIImages;
			set
			{
				currentUIImages = value;
				CurrentUIImagePageNumber = 0;
			}
		}
		IList<Control> currentUIImages = new List<Control>();

		int CurrentUIImagePageNumber
		{
			get => currentUIImagePageNumber;
			set
			{
				currentUIImagePageNumber = value;
				var controls = GetPictureBoxesForPage(currentUIImagePageNumber);
				flpImageTable.SuspendLayout();
				flpImageTable.Controls.Clear();
				flpImageTable.Controls.AddRange(controls.ToArray());
				var pages = (CurrentUIImages.Count / imagesPerPage) + 1;
				tbCurrentPage.Text = $"Page ({currentUIImagePageNumber + 1} / {pages}) ";
				flpImageTable.ResumeLayout(true);
			}
		}
		int currentUIImagePageNumber;

		// DAT Dump viewer fields
		IList<Annotation> DATDumpAnnotations;
		readonly Dictionary<string, (int, int)> DATDumpAnnotationIdentifiers = [];
		readonly Dictionary<string, TreeNode> imageHeaderIndexToNode = [];
		readonly Dictionary<string, TreeNode> imageDataIndexToNode = [];
		const int bytesPerDumpLine = 32;
		const int addressStringSizeBytes = 8;
		const int addressStringSizePrependBytes = addressStringSizeBytes + 2;
		const int dumpWordSize = 4;
		readonly Dictionary<string, Action<string>> tvUniqueLoadValues = [];
		// End DAT Dump viewer fields

		const int imagesPerPage = 50;

		const string SettingsFile = "./settings.json";

		public MainForm()
		{
			InitializeComponent();

			logger = new Logger
			{
				Level = LogLevel.Debug2
			};

			model = new MainFormModel(logger, SettingsFile);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			// pre-add any existing log lines
			lbLogs.Items.AddRange(((Logger)logger).Logs.ToArray());
			// can only do this after window handle has been created (so can't do in constructor)
			((Logger)logger).LogAdded += (s, e) => lbLogs.Invoke(() => lbLogs.Items.Insert(0, e.Log));

			// setup dark mode???
			//DarkModify(this);

			InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);
		}

		readonly Color DarkModeBackColor = Color.FromArgb(31, 31, 31);
		readonly Color DarkModeForeColor = Color.White;

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

		void InitUI(bool vanillaOnly, string filter)
		{
			// required to load the object type images from g1.dat
			if (Directory.Exists(model.Settings.DataDirectory))
			{
				model.LoadDataDirectory(model.Settings.DataDirectory);
			}

			InitFileTreeView(vanillaOnly, filter);
			InitCategoryTreeView(vanillaOnly, filter);
			InitToolStripMenuItems();
		}

		bool LoadObjDataDirectory(string directory, bool useExistingIndex)
		{
			if (string.IsNullOrEmpty(directory))
			{
				logger.Warning("Invalid directory");
				return false;
			}

			var allFiles = Directory.GetFiles(directory, "*.dat", SearchOption.AllDirectories);
			using (var progressForm = new ProgressBarForm())
			{
				progressForm.Text = $"Indexing {allFiles.Length} files";
				var progress = new Progress<float>(f => progressForm.SetProgress((int)(f * 100)));
				// can probably use a task instead of a thread, but its good enough
				var thread = new Thread(() =>
				{
					model.LoadObjDirectory(directory, progress, useExistingIndex);
					progressForm.CloseForm();
				});
				thread.Start();
				progressForm.ShowDialog();
			}

			return true;
		}

		static Bitmap MakeOriginalLocoIcon(bool isOriginal)
		{
			var bitmap = new Bitmap(16, 16);
			if (isOriginal)
			{
				var g = Graphics.FromImage(bitmap);
				g.FillEllipse(Brushes.MediumSpringGreen, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
				g.Dispose();
			}

			return bitmap;
		}

		ImageList MakeImageList(MainFormModel model)
		{
			var imageList = new ImageList();
			var blankImage = MakeOriginalLocoIcon(false);
			var originalImage = MakeOriginalLocoIcon(true);
			imageList.Images.Add(blankImage);
			imageList.Images.Add(originalImage);

			if (model.G1 != null)
			{
				var objectTypes = Enum.GetValues<ObjectType>().Length;
				var g1TabElements = model.G1.G1Elements.Skip(Constants.G1ObjectTabsOffset).Take(objectTypes).ToList();

				var images = CreateImages(g1TabElements, model.Palette, true).ToArray();
				imageList.Images.AddRange(images);
			}

			return imageList;
		}

		static void AddObjectNode(string key, string text, string objName, TreeView tv)
		{
			var imageIndex = OriginalObjectFiles.Names.Contains(objName.Trim()) ? 1 : 0;
			_ = tv.Nodes.Add(key, text, imageIndex, imageIndex);
		}
		static void AddObjectNode(string key, string text, string objName, TreeNode tn)
		{
			var imageIndex = OriginalObjectFiles.Names.Contains(objName.Trim()) ? 1 : 0;
			_ = tn.Nodes.Add(key, text, imageIndex, imageIndex);
		}

		void InitFileTreeView(bool vanillaOnly, string fileFilter)
		{
			tvFileTree.SuspendLayout();
			tvFileTree.Nodes.Clear();

			var filteredFiles = string.IsNullOrEmpty(fileFilter)
				? model.HeaderIndex
				: model.HeaderIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			filteredFiles = filteredFiles.Where(f => !vanillaOnly || OriginalObjectFiles.Names.Contains(f.Value.Name.Trim()));

			tvFileTree.ImageList = MakeImageList(model);

			foreach (var obj in filteredFiles)
			{
				var relative = Path.GetRelativePath(model.Settings.ObjDataDirectory, obj.Key);
				AddObjectNode(obj.Key, relative, obj.Value.Name, tvFileTree);
			}

			tvFileTree.Sort();
			tvFileTree.ResumeLayout(true);
		}

		void InitCategoryTreeView(bool vanillaOnly, string fileFilter)
		{
			tvObjType.SuspendLayout();
			tvObjType.Nodes.Clear();

			if (Directory.Exists(model.Settings.DataDirectory))
			{
				InitDataCategoryTree();
			}

			if (Directory.Exists(model.Settings.ObjDataDirectory))
			{
				InitObjectCategoryTree(vanillaOnly, fileFilter);
			}

			tvObjType.ResumeLayout(true);
		}

		private void InitDataCategoryTree()
		{
			var dataNode = new TreeNode("Data");

			var musicNode = new TreeNode("Music");
			var sfxNode = new TreeNode("Sound Effects");
			var tutorialsNode = new TreeNode("Tutorials");
			var miscNode = new TreeNode("Uncategorised");

			//var unknownNode = new TreeNode("Unknown");

			var allDataFiles = Directory.GetFiles(model.Settings.DataDirectory).Select(f => Path.GetFileName(f).ToLower());

			// music
			foreach (var music in model.Music)
			{
				musicNode.Nodes.Add(music.Key, music.Key, 1, 1);
				tvUniqueLoadValues[music.Key] = LoadMusic;
			}

			// sound effects
			foreach (var sfx in model.SoundEffects)
			{
				sfxNode.Nodes.Add(sfx.Key, sfx.Key, 1, 1);
				tvUniqueLoadValues[sfx.Key] = LoadNull;
			}

			// tutorials
			foreach (var tut in model.Tutorials)
			{
				tutorialsNode.Nodes.Add(tut.Key, tut.Key, 1, 1);
				tvUniqueLoadValues[tut.Key] = LoadNull;
			}

			// uncategorised
			foreach (var misc in model.MiscFiles)
			{
				miscNode.Nodes.Add(misc, misc, 1, 1);

				if (misc == "g1.dat")
				{
					tvUniqueLoadValues["g1.dat"] = LoadG1;
				}
				else
				{
					tvUniqueLoadValues[misc] = LoadNull;
				}
			}

			dataNode.Nodes.Add(musicNode);
			dataNode.Nodes.Add(sfxNode);
			dataNode.Nodes.Add(tutorialsNode);
			dataNode.Nodes.Add(miscNode);

			tvObjType.Nodes.Add(dataNode);
		}

		private void InitObjectCategoryTree(bool vanillaOnly, string fileFilter)
		{
			var filteredFiles = string.IsNullOrEmpty(fileFilter)
				? model.HeaderIndex
				: model.HeaderIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			filteredFiles = filteredFiles.Where(f => !vanillaOnly || OriginalObjectFiles.Names.Contains(f.Value.Name.Trim()));

			tvObjType.ImageList = MakeImageList(model);

			var nodesToAdd = new List<TreeNode>();
			foreach (var group in filteredFiles.GroupBy(kvp => kvp.Value.ObjectType))
			{
				var imageListOffset = model.G1 == null ? 0 : ((int)group.Key) + 2; // + 2 because we have a vanilla+custom image first
				var objTypeNode = new TreeNode(group.Key.ToString(), imageListOffset, imageListOffset);
				if (group.Key != ObjectType.Vehicle)
				{
					foreach (var obj in group)
					{
						AddObjectNode(obj.Key, obj.Value.Name, obj.Value.Name, objTypeNode);
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
							AddObjectNode(veh.Key, veh.Value.Name, veh.Value.Name, vehicleTypeNode);
						}

						objTypeNode.Nodes.Add(vehicleTypeNode);
					}
				}

				nodesToAdd.Add(objTypeNode);
			}

			var objDataNode = new TreeNode("ObjData");
			objDataNode.Nodes.AddRange([.. nodesToAdd]);
			tvObjType.Nodes.Add(objDataNode);
			tvObjType.Sort();
		}

		void InitToolStripMenuItems()
		{
			// clear dynamic items
			while (objectDirectoriesToolStripMenuItem.DropDownItems.Count > 2)
			{
				objectDirectoriesToolStripMenuItem.DropDownItems.RemoveAt(2);
			}

			if (model.Settings.ObjDataDirectories != null)
			{
				// regenerate them
				List<ToolStripMenuItem> newObjDirs = [];
				foreach (var objDir in model.Settings.ObjDataDirectories)
				{
					var tsmi = new ToolStripMenuItem(objDir + (model.Settings.ObjDataDirectory == objDir ? " (Current)" : string.Empty));
					tsmi.Click += (sender, e) => setObjectDirectoryToolStripMenuItem_ClickCore(objDir);
					newObjDirs.Add(tsmi);
				}

				objectDirectoriesToolStripMenuItem.DropDownItems.AddRange(newObjDirs.ToArray());
			}

			// clear dynamic items
			while (dataDirectoriesToolStripMenuItem.DropDownItems.Count > 2)
			{
				dataDirectoriesToolStripMenuItem.DropDownItems.RemoveAt(2);
			}

			if (model.Settings.DataDirectories != null)
			{
				// regenerate them
				List<ToolStripMenuItem> newDataDirs = [];
				foreach (var dataDir in model.Settings.DataDirectories)
				{
					var tsmi = new ToolStripMenuItem(dataDir + (model.Settings.DataDirectory == dataDir ? " (Current)" : string.Empty));
					tsmi.Click += (sender, e) => setDataDirectoryToolStripMenuItem_ClickCore(dataDir);
					newDataDirs.Add(tsmi);
				}

				dataDirectoriesToolStripMenuItem.DropDownItems.AddRange(newDataDirs.ToArray());
			}
		}

		private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void setObjectDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (objectDirBrowser.ShowDialog(this) == DialogResult.OK)
			{
				setObjectDirectoryToolStripMenuItem_ClickCore(objectDirBrowser.SelectedPath);
			}
		}

		private void setObjectDirectoryToolStripMenuItem_ClickCore(string path)
		{
			if (LoadObjDataDirectory(path, true))
			{
				InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);
			}
		}

		private void setDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (objectDirBrowser.ShowDialog(this) == DialogResult.OK)
			{
				setDataDirectoryToolStripMenuItem_ClickCore(objectDirBrowser.SelectedPath);
			}
		}

		private void setDataDirectoryToolStripMenuItem_ClickCore(string path)
		{
			if (model.LoadDataDirectory(path))
			{
				pgS5Header.SelectedObject = model.G1;
				var images = CreateImages(model.G1.G1Elements, model.Palette);
				CurrentUIImages = CreateImageControls(images, model.G1.G1Elements).ToList();
			}
		}

		IEnumerable<Control> GetPictureBoxesForPage(int page) => CurrentUIImages.Skip(page * imagesPerPage).Take(imagesPerPage);

		private void recreateIndexToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LoadObjDataDirectory(model.Settings.ObjDataDirectory, false))
			{
				InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);
			}
		}

		void tbFileFilter_TextChanged(object sender, EventArgs e)
			=> InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);

		void LoadDataDump(string path, bool isG1 = false)
		{
			if (File.Exists(path))
			{
				var byteList = File.ReadAllBytes(path);
				var resultingByteList = byteList;
				DATDumpAnnotations = isG1
					? ObjectAnnotator.AnnotateG1Data(byteList)
					: ObjectAnnotator.Annotate(byteList, out resultingByteList);

				var extraLine = resultingByteList.Length % bytesPerDumpLine;
				if (extraLine > 0)
				{
					extraLine = 1;
				}

				var dumpLines = resultingByteList
						.Select(b => string.Format("{0,2:X2}", b))
						.Chunk(dumpWordSize)
						.Select(c => string.Format("{0} ", string.Concat(c)))
						.Chunk(bytesPerDumpLine / dumpWordSize)
						.Zip(Enumerable.Range(0, (resultingByteList.Length / bytesPerDumpLine) + extraLine))
						.Select(l => string.Format("{0:X" + addressStringSizeBytes + "}: {1}", l.Second * bytesPerDumpLine, string.Concat(l.First)))
						.ToArray();

				tvDATDumpAnnotations.SuspendLayout();
				tvDATDumpAnnotations.Nodes.Clear();
				var currentParent = new TreeNode();

				static string constructAnnotationText(Annotation annotation)
					=> string.Format("{0} (0x{1:X}-0x{2:X})", annotation.Name, annotation.Start, annotation.End);

				var parents = new Dictionary<string, TreeNode>();

				foreach (var annotation in DATDumpAnnotations)
				{
					var annotationText = constructAnnotationText(annotation);
					parents[annotationText] = new TreeNode(annotationText);
					DATDumpAnnotationIdentifiers[annotationText] = (annotation.Start, annotation.End);
					if (annotation.Parent == null)
					{
						tvDATDumpAnnotations.Nodes.Add(parents[constructAnnotationText(annotation)]);
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

				tvDATDumpAnnotations.ResumeLayout();
				rtbDATDumpView.Text = string.Join("\n", dumpLines);
			}
		}

		static bool MusicIsPlaying { get; set; } = false;

		void LoadMusic(string dataKey)
		{
			var music = model.Music[dataKey];
			var (header, pcmData) = SawyerStreamReader.LoadMusicTrack(music);

			pgS5Header.SelectedObject = header;

			if (!header.Validate())
			{
				// invalid music file
				return;
			}

			PlayMusic(header, pcmData);
		}

		void PlayMusic(MusicHeader hdr, byte[] pcmData)
		{
			CreateSoundUI(pcmData, (int)hdr.SampleRate, hdr.BitsPerSample, hdr.NumberOfChannels);
		}

		void CreateSounds(SoundObject soundObject)
		{
			var hdr = soundObject.SoundObjectData.PcmHeader;
			CreateSoundUI(soundObject.RawPcmData, hdr.SampleRate, hdr.BitsPerSample, hdr.NumberOfChannels);
		}

		void CreateSoundUI(byte[] pcmData, int samplesPerSecond, int bits, int numberOfChannels)
		{
			flpImageTable.SuspendLayout();
			flpImageTable.Controls.Clear();

			// for some reason the SoundObject files have the wrong bitspersample set
			if (bits != 16)
			{
				bits = 16;
			}

			if (MusicIsPlaying)
			{
				MusicIsPlaying = false;
				Thread.Sleep(100);
			}

			var playButton = new Button
			{
				Size = new Size(64, 64),
				Text = "Play",
			};

			var stopButton = new Button
			{
				Size = new Size(64, 64),
				Text = "Stop",
			};
			stopButton.Click += (args, sender) => CurrentWOEvent?.Stop();

			var pauseButton = new Button
			{
				Size = new Size(64, 64),
				Text = "Pause",
			};
			pauseButton.Click += (args, sender) => CurrentWOEvent?.Pause();

			var waveViewer = new WaveViewer
			{
				BorderStyle = BorderStyle.FixedSingle,
				WaveStream = new RawSourceWaveStream(new MemoryStream(pcmData), new WaveFormat(samplesPerSecond, bits, numberOfChannels)),
				Size = new Size(1024, 128),
			};
			waveViewer.SamplesPerPixel = pcmData.Length / waveViewer.Width / numberOfChannels / 2, // dunno why i need /2

			playButton.Click += (args, sender) =>
			{
				if (CurrentWOEvent != null)
				{
					if (CurrentWOEvent.PlaybackState == PlaybackState.Playing)
					{
						return;
					}

					if (CurrentWOEvent.PlaybackState == PlaybackState.Paused)
					{
						CurrentWOEvent.Play();
						return;
					}
				}

				// do it asyncly to a) give user ui control and b) allow multiple sounds to play at once
				_ = Task.Run(() =>
				{
					if (CurrentWOEvent?.PlaybackState == PlaybackState.Stopped)
					{
						Thread.Sleep(100); // give time to wait until previous sound is disposed
					}
					CurrentWOEvent?.Dispose();

					using (var ms = new MemoryStream(pcmData))
					using (var rs = new RawSourceWaveStream(ms, new WaveFormat(samplesPerSecond, bits, numberOfChannels)))
					using (CurrentWOEvent = new WaveOutEvent())
					using (var transparentBrush = new SolidBrush(Color.FromArgb(27, 0, 0, 0)))
					{
						var g = waveViewer.CreateGraphics();

						// clear the previously-drawn progress overlay
						g.Clear(Color.White);
						waveViewer.Invoke(() => waveViewer.Refresh());

						MusicIsPlaying = true;
						CurrentWOEvent.Init(rs);
						CurrentWOEvent.Play();

						var prevX = 0;

						while (CurrentWOEvent?.PlaybackState != PlaybackState.Stopped && MusicIsPlaying)
						{
							if (CurrentWOEvent == null)
							{
								break;
							}

							var progressInBytes = CurrentWOEvent.GetPosition();
							var percentPlayed = progressInBytes / (double)pcmData.Length;
							var newX = (int)(percentPlayed * waveViewer.Width);
							var diff = newX - prevX;
							g.FillRectangle(transparentBrush, new Rectangle(prevX, 0, (int)diff, waveViewer.Height));
							prevX = newX;

							Thread.Sleep(50);
						}
					}
					CurrentWOEvent = null;
					MusicIsPlaying = false;
				});
			};

			flpImageTable.Controls.Add(waveViewer);
			flpImageTable.Controls.Add(playButton);
			flpImageTable.Controls.Add(pauseButton);
			flpImageTable.Controls.Add(stopButton);
			flpImageTable.ResumeLayout(true);
		}

		WaveOutEvent? CurrentWOEvent { get; set; }

		void LoadNull(string dataKey)
		{
			CurrentUIObject = null;
			CurrentUIImages = new List<Control>();
		}

		void LoadG1(string filename)
		{
			pgS5Header.SelectedObject = model.G1;
			var images = CreateImages(model.G1.G1Elements, model.Palette);
			CurrentUIImages = CreateImageControls(images, model.G1.G1Elements).ToList();
			LoadDataDump(filename, true);
		}

		void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e?.Node == null)
			{
				return;
			}

			var nodeText = e.Node.Text.ToLower();
			if (tvUniqueLoadValues.TryGetValue(nodeText, out var value)) // for custom functions for the individual data files
			{
				value.Invoke(e.Node.Name);
			}
			else if (Path.GetExtension(e.Node.Name).Equals(".dat", StringComparison.CurrentCultureIgnoreCase))
			{
				var filename = e.Node.Name;
				CurrentUIObject = model.LoadAndCacheObject(filename);

				//try
				{
					LoadDataDump(filename);
				}
				//catch (Exception ex)
				{
					//	logger?.Error(ex, $"Unable to annotate file \"{filename}\"");
				}
			}
		}

		IEnumerable<Control> CreateImageControls(IEnumerable<Bitmap> images, List<G1Element32> g1Elements) // g1Elements is simply used for metadata at this stage
		{
			// on these controls we could add a right_click handler to replace image with user-created one
			var count = 0;
			const int scale = 4;
			foreach (var img in images)
			{
				var panel = new FlowLayoutPanel
				{
					AutoSize = true,
					BackColor = Color.LightGray,
					BorderStyle = BorderStyle.FixedSingle,
					FlowDirection = FlowDirection.TopDown,
				};

				var pb = new PictureBoxWithInterpolationMode
				{
					BorderStyle = BorderStyle.FixedSingle,
					Image = img,
					InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor,
					SizeMode = PictureBoxSizeMode.StretchImage,
					Size = new Size(img.Width * scale, img.Height * scale),
					ContextMenuStrip = imgContextMenu,
					//Dock = DockStyle.Bottom,
				};

				var tb = new TextBox
				{
					MinimumSize = new Size(96, 16),
					Text = $"i={count} w={g1Elements[count].Width} h={g1Elements[count].Height}",
					Dock = DockStyle.Top
				};
				count++;

				panel.Controls.Add(tb);
				panel.Controls.Add(pb);

				yield return panel;
			}
		}

		IEnumerable<Bitmap> CreateImages(List<G1Element32> G1Elements, Color[] palette, bool useTransparency = false)
		{
			if (palette is null)
			{
				logger.Error("Palette was empty; please load a valid palette file");
				yield break;
			}

			for (var i = 0; i < G1Elements.Count; ++i)
			{
				var currElement = G1Elements[i];
				if (currElement.ImageData.Length == 0)
				{
					logger.Info($"skipped loading g1 element {i} with 0 length");
					continue;
				}

				if (currElement.Flags.HasFlag(G1ElementFlags.IsR8G8B8Palette))
				{
					var imageData = currElement.ImageData;
					var dstImg = new Bitmap(currElement.Width, currElement.Height);
					var rect = new Rectangle(0, 0, currElement.Width, currElement.Height);
					var dstImgData = dstImg.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

					var k = 0;
					for (var j = 0; j < currElement.Width; ++j) // += 4 for a 32-bit ptr++
					{
						var b = imageData[k++];
						var g = imageData[k++];
						var r = imageData[k++];
						ImageHelpers.SetPixel(dstImgData, j, 1, Color.FromArgb(r, g, b));
					}

					dstImg.UnlockBits(dstImgData);
					yield return dstImg;
				}
				else
				{
					var bmp = G1ElementToBitmap(currElement, palette, useTransparency);
					if (bmp != null)
					{
						yield return bmp;
					}
				}
			}
		}

		static Bitmap? G1ElementToBitmap(G1Element32 currElement, Color[] palette, bool useTransparency = false)
		{
			var imageData = currElement.ImageData;
			var dstImg = new Bitmap(currElement.Width, currElement.Height);
			var rect = new Rectangle(0, 0, currElement.Width, currElement.Height);
			var dstImgData = dstImg.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			for (var y = 0; y < currElement.Height; ++y)
			{
				for (var x = 0; x < currElement.Width; ++x)
				{
					var paletteIndex = imageData[(y * currElement.Width) + x];

					// the issue with greyscale here is it isn't normalised so all heightmaps are really dark and hard to see
					//var colour = obj.Object is HillShapesObject
					//	? Color.FromArgb(paletteIndex, paletteIndex, paletteIndex) // for hillshapes, its just a heightmap so lets put it in greyscale
					//	: palette[paletteIndex];

					if (paletteIndex == 0 && useTransparency)
					{
						//ImageHelpers.SetPixel(dstImgData, x, y, colour);
					}
					else
					{
						var colour = palette[paletteIndex];
						ImageHelpers.SetPixel(dstImgData, x, y, colour);
					}
				}
			}

			dstImg.UnlockBits(dstImgData);
			return dstImg;
		}

		void SelectNewPalette()
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
				openFileDialog.Filter = "Palette Image Files(*.png)|*.png|All files (*.*)|*.*";
				openFileDialog.FilterIndex = 1;
				openFileDialog.RestoreDirectory = true;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					model.PaletteFile = openFileDialog.FileName;
					RefreshObjectUI();
				}
			}
		}

		private void setPaletteToolStripMenuItem_Click(object sender, EventArgs e)
			=> SelectNewPalette();

		private void RefreshObjectUI()
		{
			MusicIsPlaying = false;

			if (CurrentUIObject == null)
			{
				pgS5Header.SelectedObject = null;
				pgObjHeader.SelectedObject = null;
				pgObject.SelectedObject = null;
				ucStringTable.SetDataBinding(null);

				if (tvFileTree.SelectedNode == null)
				{
					return;
				}

				var selectedFile = tvFileTree.SelectedNode.Text;
				MessageBox.Show($"File \"{selectedFile}\" couldn't be loaded. Does it exist? Suggest recreating object index.");
				logger.Error($"File \"{selectedFile}\" couldn't be loaded. Does it exist? Suggest recreating object index.");
				return;
			}

			flpImageTable.SuspendLayout();
			flpImageTable.Controls.Clear();

			if (CurrentUIObject.LocoObject.G1Elements != null && CurrentUIObject.LocoObject?.G1Elements.Count != 0)
			{
				if (model.Palette is null)
				{
					MessageBox.Show("No palette file loaded - please load one from File -> Load Palette. You can use palette.png in the top level folder of this repo.");
					logger.Error("No palette file loaded - please load one from File -> Load Palette. You can use palette.png in the top level folder of this repo.");
					return;
					//SelectNewPalette();
				}

				var images = CreateImages(CurrentUIObject.LocoObject.G1Elements, model.Palette);
				CurrentUIImages = CreateImageControls(images, CurrentUIObject.LocoObject.G1Elements).ToArray();
			}
			else
			{
				CurrentUIImages = new List<Control>();
			}

			if (CurrentUIObject?.LocoObject.Object is SoundObject soundObject)
			{
				CreateSounds(soundObject);
			}

			flpImageTable.ResumeLayout(true);

			pgS5Header.SelectedObject = CurrentUIObject?.DatFileInfo.S5Header;
			pgObjHeader.SelectedObject = CurrentUIObject?.DatFileInfo.ObjectHeader;
			pgObject.SelectedObject = CurrentUIObject?.LocoObject.Object;
			ucStringTable.SetDataBinding(CurrentUIObject?.LocoObject.StringTable);
			//pgS5Header.SelectedObject = CurrentUIObject; // done above with flpImageTable
		}

		private void imgContextMenuSave_Click(object sender, EventArgs e)
		{
			if (imgContextMenu.SourceControl is PictureBox pb)
			{
				using (var saveFileDialog = new SaveFileDialog())
				{
					saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
					saveFileDialog.Filter = "PNG Files(*.png)|*.png|All files (*.*)|*.*";
					saveFileDialog.FilterIndex = 1;
					saveFileDialog.RestoreDirectory = true;

					if (saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						pb.Image.Save(saveFileDialog.FileName);
						logger.Info($"Saved image to {saveFileDialog.FileName}");
					}
				}
			}
		}

		private void btnPagePrevious_Click(object sender, EventArgs e)
			=> CurrentUIImagePageNumber = Math.Max(CurrentUIImagePageNumber - 1, 0);

		private void btnPageNext_Click(object sender, EventArgs e)
		{
			if (currentUIImages?.Count > 0)
			{
				CurrentUIImagePageNumber = Math.Min(CurrentUIImagePageNumber + 1, CurrentUIImages.Count / imagesPerPage);
			}
		}

		private void dataDumpAnnotations_AfterSelect(object sender, TreeViewEventArgs e)
		{
			int dumpPositionToRTBPosition(int position) => rtbDATDumpView.GetFirstCharIndexFromLine(
				position / bytesPerDumpLine)
				+ (position % bytesPerDumpLine * 2)            // Bytes are displayed 2 characters wide
				+ (position % bytesPerDumpLine / dumpWordSize) // Every word is separated by an extra space
				+ addressStringSizePrependBytes;               // Each line starts with 10 characters indicating address

			if (DATDumpAnnotationIdentifiers.TryGetValue(e.Node!.Text, out var positionValues))
			{
				//var linePosStart = rtbDATDumpView.GetFirstCharIndexFromLine(positionValues.Item1 / bytesPerDumpLine);
				//var linePosEnd = rtbDATDumpView.GetFirstCharIndexFromLine(positionValues.Item2 / bytesPerDumpLine);

				var selectPositionStart = dumpPositionToRTBPosition(positionValues.Item1);
				var selectPositionEnd = Math.Min(dumpPositionToRTBPosition(positionValues.Item2), rtbDATDumpView.TextLength - 1);
				rtbDATDumpView.Select(selectPositionStart, selectPositionEnd - selectPositionStart);
			}
		}

		private void headerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (imgContextMenu.SourceControl is PictureBox pb)
			{
				var index = currentUIImages.IndexOf(pb);
				var keys = "Header " + (index + 1);
				if (index >= 0 && imageHeaderIndexToNode.TryGetValue(keys, out var value))
				{
					tcObjectOverview.SelectedIndex = 1;
					tvDATDumpAnnotations.SelectedNode = value;
					dataDumpAnnotations_AfterSelect(sender, new TreeViewEventArgs(value));
					tvDATDumpAnnotations.Focus();
				}
			}
		}

		private void pictureDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (imgContextMenu.SourceControl is PictureBox pb)
			{
				var index = currentUIImages.IndexOf(pb);
				var keys = "Image " + (index + 1);
				if (index >= 0 && imageDataIndexToNode.TryGetValue(keys, out var value))
				{
					tcObjectOverview.SelectedIndex = 1;
					tvDATDumpAnnotations.SelectedNode = value;
					dataDumpAnnotations_AfterSelect(sender, new TreeViewEventArgs(value));
					tvDATDumpAnnotations.Focus();
				}
			}
		}

		private void cbVanillaObjects_CheckedChanged(object sender, EventArgs e) => InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);

		private void lbLogs_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();

			if (e.Index < 0)
				return;

			var item = (LogLine)lbLogs.Items[e.Index];
			var backgroundColour = item.Level switch
			{
				LogLevel.Debug2 => lbLogs.BackColor,
				LogLevel.Debug => lbLogs.BackColor,
				LogLevel.Info => lbLogs.BackColor,
				LogLevel.Warning => Color.LightGray,
				LogLevel.Error => Color.LightGray,
				_ => throw new NotImplementedException(),
			};

			using (var brush = new SolidBrush(backgroundColour))
			{
				e.Graphics.FillRectangle(brush, e.Bounds);
			}

			var foregroundBrush = item.Level switch
			{
				LogLevel.Debug2 => Brushes.LightGray,
				LogLevel.Debug => Brushes.Gray,
				LogLevel.Info => Brushes.Green,
				LogLevel.Warning => Brushes.Yellow,
				LogLevel.Error => Brushes.Red,
				_ => throw new NotImplementedException(),
			};
			e.Graphics.DrawString(item.ToString(), e.Font, foregroundBrush, e.Bounds.Left, e.Bounds.Y);

			//e.DrawFocusRectangle();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (CurrentUIObject is not UiLocoObject obj)
			{
				return;
			}

			if (!SaveEnabledObjects.Types.Contains(obj.DatFileInfo.S5Header.ObjectType))
			{
				var msg = $"Saving is not currently implemented for {obj.DatFileInfo.S5Header.ObjectType} objects";
				logger.Error(msg);
				logger.Info($"Saving is currently supported for the following objects: [{SaveEnabledObjects.Types.Aggregate("", (a, b) => a + ", " + b)}]");
				MessageBox.Show(msg);
				return;
			}

			saveFileDialog1.InitialDirectory = model.Settings.ObjDataDirectory;
			saveFileDialog1.DefaultExt = "dat";
			saveFileDialog1.Filter = "Locomotion DAT files (.dat)|*.dat";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				var filename = saveFileDialog1.FileName;

				try
				{
					var exists = File.Exists(filename);
					MainFormModel.SaveFile(filename, obj);

					if (!exists)
					{
						// we made a new file (as opposed to overwriting an existing one) so lets update the UI to show it
						InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);
					}

					logger.Info($"File \"{filename}\" saved successfully");
				}
				catch (Exception ex)
				{
					logger.Error($"Error saving \"{filename}\": {ex.Message}");
					MessageBox.Show($"Error saving \"{filename}\": {ex.Message}");
				}
			}
		}
	}
}
