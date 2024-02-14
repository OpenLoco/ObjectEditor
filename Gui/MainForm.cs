using NAudio.Gui;
using NAudio.Wave;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Types;
using OpenLoco.ObjectEditor.Logging;
using System.Data;
using System.Drawing.Imaging;
using System.Reflection;
using OpenLoco.ObjectEditor.Data;
using Core.Objects.Sound;
using Zenith.Core;
using System.Text;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using System.IO;

namespace OpenLoco.ObjectEditor.Gui
{
	public partial class MainForm : Form
	{
		readonly MainFormModel model;
		readonly ILogger logger;

		public IUiObject? CurrentUIObject
		{
			get => currentUIObject;
			set
			{
				currentUIObject = value;
				RefreshObjectUI();
			}
		}
		IUiObject? currentUIObject;

		List<Control> CurrentUIImages
		{
			get => currentUIImages;
			set
			{
				currentUIImages = value;
				CurrentUIImagePageNumber = 0;
			}
		}
		List<Control> currentUIImages = [];

		List<Bitmap> currentUIObjectImages = [];

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
				var pages = (CurrentUIImages.Count / ImagesPerPage) + 1;
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

		const int ImagesPerPage = 50;

		const string SettingsFile = "./settings.json";

		public MainForm()
		{
			InitializeComponent();

			logger = new Logger
			{
				Level = LogLevel.Debug2
			};

			var assembly = Assembly.GetExecutingAssembly();
			var paletteFilename = "Gui.palette.png";
			using (var stream = assembly.GetManifestResourceStream(paletteFilename))
			{
				//var paletteBitmap = (Bitmap)Image.FromStream(stream!);
				//var palette = PaletteHelpers.PaletteFromBitmap(paletteBitmap);
				//model = new MainFormModel(logger, SettingsFile, palette);

				var paletteBitmap = SixLabors.ImageSharp.Image.Load<Rgb24>(stream!);
				var palette = PaletteHelpers.PaletteFromBitmapIS(paletteBitmap);
				model = new MainFormModel(logger, SettingsFile, palette);
			}

			var versionFilename = "Gui.version.txt";
			using (var stream = assembly.GetManifestResourceStream(versionFilename))
			{
				var buf = new byte[5];
				var arr = stream!.Read(buf);
				Text = $"OpenLoco Object Editor - {Encoding.ASCII.GetString(buf)}";
			}
		}

		void MainForm_Load(object sender, EventArgs e)
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
				_ = model.LoadDataDirectory(model.Settings.DataDirectory);
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
				_ = progressForm.ShowDialog();
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

		static ImageList MakeImageList(MainFormModel model, ILogger? logger = null)
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

				var images = CreateImages(g1TabElements, model.Palette, true, logger).ToArray();
				imageList.Images.AddRange(images);
			}

			return imageList;
		}

		static void AddObjectNode(string key, string text, string objName, uint objChecksum, TreeView tv)
		{
			var imageIndex = IsOriginalFile(objName, objChecksum) ? 1 : 0;
			_ = tv.Nodes.Add(key, text, imageIndex, imageIndex);
		}

		static void AddObjectNode(string key, string text, string objName, uint objChecksum, TreeNode tn)
		{
			var imageIndex = IsOriginalFile(objName, objChecksum) ? 1 : 0;
			_ = tn.Nodes.Add(key, text, imageIndex, imageIndex);
		}

		static bool IsOriginalFile(string name, uint checksum)
			=> OriginalObjectFiles.Names.TryGetValue(name.Trim(), out var expectedChecksum) && expectedChecksum == checksum;

		void InitFileTreeView(bool vanillaOnly, string fileFilter)
		{
			tvFileTree.SuspendLayout();
			tvFileTree.Nodes.Clear();

			var filteredFiles = string.IsNullOrEmpty(fileFilter)
				? model.HeaderIndex
				: model.HeaderIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			filteredFiles = filteredFiles.Where(f => !vanillaOnly || IsOriginalFile(f.Value.Name, f.Value.Checksum));

			tvFileTree.ImageList = MakeImageList(model);

			foreach (var obj in filteredFiles)
			{
				var relative = Path.GetRelativePath(model.Settings.ObjDataDirectory, obj.Key);
				AddObjectNode(obj.Key, relative, obj.Value.Name, obj.Value.Checksum, tvFileTree);
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

		void InitDataCategoryTree()
		{
			var dataNode = new TreeNode("Data");

			var musicNode = new TreeNode("Music");
			var miscTrackNode = new TreeNode("Misc Tracks");
			var sfxNode = new TreeNode("Sound Effects");
			var tutorialsNode = new TreeNode("Tutorials");
			var miscNode = new TreeNode("Uncategorised");

			var allDataFiles = Directory.GetFiles(model.Settings.DataDirectory).Select(f => Path.GetFileName(f).ToLower());

			// music
			foreach (var music in model.Music)
			{
				var displayName = $"{OriginalDataFiles.Music[music.Key]} ({music.Key})";
				_ = musicNode.Nodes.Add(music.Key, displayName, 1, 1);
			}

			//misc tracks
			foreach (var miscTrack in model.MiscellaneousTracks)
			{
				var displayName = $"{OriginalDataFiles.MiscellaneousTracks[miscTrack.Key]} ({miscTrack.Key})";
				_ = miscTrackNode.Nodes.Add(miscTrack.Key, displayName, 1, 1);
			}

			// sound effects
			//foreach (var sfx in model.SoundEffects)
			{
				var displayName = OriginalDataFiles.SoundEffect;
				_ = sfxNode.Nodes.Add(displayName, displayName, 1, 1);
			}

			// tutorials
			foreach (var tut in model.Tutorials)
			{
				_ = tutorialsNode.Nodes.Add(tut.Key, tut.Key, 1, 1);
				tvUniqueLoadValues[tut.Key] = LoadNull;
			}

			// uncategorised
			foreach (var misc in model.MiscFiles)
			{
				_ = miscNode.Nodes.Add(misc, misc, 1, 1);

				if (misc == "g1.dat")
				{
					tvUniqueLoadValues["g1.dat"] = LoadG1;
				}
				else
				{
					tvUniqueLoadValues[misc] = LoadNull;
				}
			}

			_ = dataNode.Nodes.Add(musicNode);
			_ = dataNode.Nodes.Add(miscTrackNode);
			_ = dataNode.Nodes.Add(sfxNode);
			_ = dataNode.Nodes.Add(tutorialsNode);
			_ = dataNode.Nodes.Add(miscNode);

			_ = tvObjType.Nodes.Add(dataNode);
		}

		void InitObjectCategoryTree(bool vanillaOnly, string fileFilter)
		{
			var filteredFiles = string.IsNullOrEmpty(fileFilter)
				? model.HeaderIndex
				: model.HeaderIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			filteredFiles = filteredFiles.Where(f => !vanillaOnly || IsOriginalFile(f.Value.Name, f.Value.Checksum));

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
						AddObjectNode(obj.Key, obj.Value.Name, obj.Value.Name, obj.Value.Checksum, objTypeNode);
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
							AddObjectNode(veh.Key, veh.Value.Name, veh.Value.Name, veh.Value.Checksum, vehicleTypeNode);
						}

						_ = objTypeNode.Nodes.Add(vehicleTypeNode);
					}
				}

				nodesToAdd.Add(objTypeNode);
			}

			var objDataNode = new TreeNode("ObjData");
			objDataNode.Nodes.AddRange([.. nodesToAdd]);
			_ = tvObjType.Nodes.Add(objDataNode);
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

		void setObjectDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (objectDirBrowser.ShowDialog(this) == DialogResult.OK)
			{
				setObjectDirectoryToolStripMenuItem_ClickCore(objectDirBrowser.SelectedPath);
			}
		}

		void setObjectDirectoryToolStripMenuItem_ClickCore(string path)
		{
			if (LoadObjDataDirectory(path, true))
			{
				InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);
			}
		}

		void setDataDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (objectDirBrowser.ShowDialog(this) == DialogResult.OK)
			{
				setDataDirectoryToolStripMenuItem_ClickCore(objectDirBrowser.SelectedPath);
			}
		}

		void setDataDirectoryToolStripMenuItem_ClickCore(string path)
		{
			if (model.LoadDataDirectory(path))
			{
				InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);
			}
		}

		IEnumerable<Control> GetPictureBoxesForPage(int page) => CurrentUIImages.Skip(page * ImagesPerPage).Take(ImagesPerPage);

		void recreateIndexToolStripMenuItem_Click(object sender, EventArgs e)
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
			try
			{
				LoadDataDumpCore(path, isG1);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
		}

		void LoadDataDumpCore(string path, bool isG1 = false)
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
						_ = tvDATDumpAnnotations.Nodes.Add(parents[constructAnnotationText(annotation)]);
					}
					else if (parents.ContainsKey(constructAnnotationText(annotation.Parent)))
					{
						var parentText = constructAnnotationText(annotation.Parent);
						_ = parents[parentText].Nodes.Add(parents[annotationText]);

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

		static bool MusicIsPlaying { get; set; }

		void LoadAndPlaySound(byte[] data, string soundName)
		{
			var (header, pcmData) = SawyerStreamReader.LoadWavFile(data);
			var uiSoundObj = new UiSoundObject { Data = pcmData, Header = header, SoundName = soundName };
			var uiSoundObjectList = new UiSoundObjectList();
			uiSoundObjectList.Audio.Add(uiSoundObj);
			CurrentUIObject = uiSoundObjectList;

			if (!header.Validate())
			{
				// invalid file
				logger?.Warning($"Invalid music track");
				return;
			}
		}

		void LoadSoundEffectFile(byte[] data)
		{
			var sfxs = SawyerStreamReader.LoadSoundEffectsFromCSS(data);

			var i = 0;
			var uiSoundObjectList = new UiSoundObjectList();

			foreach (var (header, pcmData) in sfxs)
			{
				var uiSoundObj = new UiSoundObject { Data = pcmData, Header = SawyerStreamWriter.WaveFormatExToRiff(header, pcmData.Length), SoundName = Enum.GetValues<SoundId>().ToList()[i++].ToString() };
				uiSoundObjectList.Audio.Add(uiSoundObj);
			}

			CurrentUIObject = uiSoundObjectList;
		}

		static FlowLayoutPanel GetNewSoundUIFLP()
		{
			var flp = new FlowLayoutPanel
			{
				FlowDirection = FlowDirection.TopDown,
				Dock = DockStyle.Fill,
				AutoSize = true,
				WrapContents = false
			};
			return flp;
		}

		public void ImportWave(string soundNameToUpdate)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
				openFileDialog.Filter = "WAV Files(*.wav)|*.wav|All files (*.*)|*.*";
				openFileDialog.FilterIndex = 1;
				openFileDialog.RestoreDirectory = true;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					// open dialog
					var bytes = File.ReadAllBytes(openFileDialog.FileName);
					var (header, pcmData) = SawyerStreamReader.LoadWavFile(bytes);

					if (currentUIObject is UiSoundObjectList uiSoundObjList)
					{
						var soundObj = uiSoundObjList.Audio.Single(s => s.SoundName == soundNameToUpdate);
						soundObj.Header = header;
						soundObj.Data = pcmData;
						RefreshObjectUI();
					}

					// if for music - just replace current data
					if (OriginalDataFiles.Music.ContainsKey(soundNameToUpdate))
					{
						logger.Info($"Replacing music track {soundNameToUpdate} with {openFileDialog.FileName}");
					}
					else
					{
						logger.Warning($"Sound name {soundNameToUpdate} was not recognised - no action will be taken.");
					}
				}
			}
		}

		public void ImportImages()
		{
			using (var fbDialog = new FolderBrowserDialog())
			{
				fbDialog.InitialDirectory = Directory.GetCurrentDirectory();

				if (fbDialog.ShowDialog() == DialogResult.OK)
				{
					currentUIObjectImages.Clear();
					var files = Directory.GetFiles(fbDialog.SelectedPath);
					var sorted = files.OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)[5..]));
					foreach (var file in sorted)
					{
						var img = (Bitmap)Image.FromFile(file);
						currentUIObjectImages.Add(img);
					}

					RefreshImageControls();
				}
			}
		}

		public void RefreshImageControls()
			=> CurrentUIImages = CreateImageControls(currentUIObjectImages).ToList();

		public void ExportImages()
		{
			using (var fbDialog = new FolderBrowserDialog())
			{
				fbDialog.InitialDirectory = Directory.GetCurrentDirectory();

				if (fbDialog.ShowDialog() == DialogResult.OK)
				{
					var counter = 0;
					foreach (var image in currentUIObjectImages)
					{
						var imageName = GetImageName(CurrentUIObject, counter++);
						var path = Path.Combine(fbDialog.SelectedPath, $"{imageName}.png");
						logger.Debug($"Saving image to {path}");
						image.Save(path);
					}

					logger.Info($"Saved {counter} images to {fbDialog.SelectedPath}");
				}
			}
		}

		public string GetImageName(IUiObject? uiObj, int counter)
		{
			IImageTableStrings? its = null;
			var objectName = string.Empty;

			if (uiObj is UiLocoObject uiLocoObj && uiLocoObj.LocoObject != null && uiLocoObj.LocoObject.Object is IImageTableStrings itss)
			{
				its = itss;
				objectName = uiLocoObj.DatFileInfo.S5Header.Name;
			}
			else if (uiObj is UiG1 uiG1 && uiG1.G1 is IImageTableStrings itsg)
			{
				its = itsg;
				objectName = "g1.dat";
			}

			if (its != null)
			{
				if (!its.TryGetImageName(counter, out var value) || value == null)
				{
					logger.Warning($"Object {objectName} does not have an image for id {counter}");
					return $"{counter}-{objectName}";
				}

				return $"{counter}-{value}";

			}

			return $"{counter}-image";
		}

		public void ExportMusic(UiSoundObject uiSoundObj)
		{
			using (var sfDialog = new SaveFileDialog())
			{
				sfDialog.InitialDirectory = Directory.GetCurrentDirectory();
				sfDialog.Filter = "WAV Files(*.wav)|*.wav|All files (*.*)|*.*";
				sfDialog.FilterIndex = 1;
				sfDialog.RestoreDirectory = true;

				// suggested filename for the save dialog
				sfDialog.FileName = OriginalDataFiles.Music.TryGetValue(tvObjType.SelectedNode.Name, out var value)
					? value
					: "export.wav";

				if (sfDialog.ShowDialog() == DialogResult.OK)
				{
					SawyerStreamWriter.ExportMusicAsWave(sfDialog.FileName, uiSoundObj.Header, uiSoundObj.Data);
					logger.Info($"Saved music to {sfDialog.FileName}");

				}
			}
		}

		Panel CreateSoundUI(UiSoundObject uiSoundObj)
		{
			if (MusicIsPlaying)
			{
				MusicIsPlaying = false;
				Thread.Sleep(100);
			}

			var playButton = new Button
			{
				Size = new Size(64, 64),
				Text = "\u23f5 Play",
			};

			var stopButton = new Button
			{
				Size = new Size(64, 64),
				Text = "\u23f9 Stop",
			};
			stopButton.Click += (args, sender) => CurrentWOEvent?.Stop();

			var pauseButton = new Button
			{
				Size = new Size(64, 64),
				Text = "\u23f8 Pause",
			};
			pauseButton.Click += (args, sender) => CurrentWOEvent?.Pause();

			var exportButton = new Button
			{
				Size = new Size(64, 64),
				Text = "Export",
			};
			exportButton.Click += (args, sender) => ExportMusic(uiSoundObj);

			var importButton = new Button
			{
				Size = new Size(64, 64),
				Text = "Import",
			};
			importButton.Click += (args, sender) => ImportWave(uiSoundObj.SoundName);

			var waveViewer = new WaveViewer
			{
				BorderStyle = BorderStyle.FixedSingle,
				WaveStream = new RawSourceWaveStream(new MemoryStream(uiSoundObj.Data), new WaveFormat((int)uiSoundObj.Header.SampleRate, uiSoundObj.Header.BitsPerSample, uiSoundObj.Header.NumberOfChannels)),
				Size = new Size(512, 64),
			};
			waveViewer.SamplesPerPixel = uiSoundObj.Data.Length / waveViewer.Width / uiSoundObj.Header.NumberOfChannels / (uiSoundObj.Header.BitsPerSample / 8);

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

					using (var ms = new MemoryStream(uiSoundObj.Data))
					using (var rs = new RawSourceWaveStream(ms, new WaveFormat((int)uiSoundObj.Header.SampleRate, uiSoundObj.Header.BitsPerSample, uiSoundObj.Header.NumberOfChannels)))
					using (CurrentWOEvent = new WaveOutEvent())
					using (var transparentBrush = new SolidBrush(Color.FromArgb(63, 0, 0, 0)))
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
							var percentPlayed = progressInBytes / (double)uiSoundObj.Data.Length;
							var newX = (int)(percentPlayed * waveViewer.Width);
							var diff = newX - prevX;
							g.FillRectangle(transparentBrush, new Rectangle(prevX, 0, diff, waveViewer.Height));
							prevX = newX;

							Thread.Sleep(50);
						}

						// complete overlay to the very end
						g.FillRectangle(transparentBrush, new Rectangle(prevX, 0, waveViewer.Width - prevX, waveViewer.Height));
					}

					CurrentWOEvent = null;
					MusicIsPlaying = false;
				});
			};

			// text
			var tb = new TextBox
			{
				Text = uiSoundObj.SoundName,
				Enabled = false,
				Width = 128,
				Height = 32,
				Dock = DockStyle.Top
			};

			// object controls
			var flp = new FlowLayoutPanel
			{
				FlowDirection = FlowDirection.LeftToRight,
				Dock = DockStyle.Fill,
				AutoSize = true
			};

			flp.Controls.Add(tb);
			flp.Controls.Add(waveViewer);
			flp.Controls.Add(playButton);
			flp.Controls.Add(pauseButton);
			flp.Controls.Add(stopButton);
			flp.Controls.Add(exportButton);
			flp.Controls.Add(importButton);

			var pn = new Panel
			{
				Dock = DockStyle.Fill,
				BorderStyle = BorderStyle.Fixed3D,
				AutoSize = true,
				Tag = uiSoundObj.SoundName
			};
			pn.Controls.Add(flp);
			pn.Controls.Add(tb);

			return pn;
		}

		WaveOutEvent? CurrentWOEvent { get; set; }

		void LoadNull(string dataKey)
		{
			CurrentUIObject = null;
			CurrentUIImages = [];
		}

		void LoadG1(string filename)
		{
			CurrentUIObject = new UiG1 { G1 = model.G1 };
			LoadDataDump(filename, true);
		}

		void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e?.Node == null)
			{
				return;
			}

			flpImageTable.SuspendLayout();
			flpImageTable.Controls.Clear();

			var nodeText = e.Node.Text.ToLower();
			if (tvUniqueLoadValues.TryGetValue(nodeText, out var value)) // for custom functions for the individual data files
			{
				logger.Debug($"Loading special object {e.Node.Name}");
				value.Invoke(e.Node.Name);
			}
			else if (OriginalDataFiles.Music.ContainsKey(e.Node.Name.ToLower()))
			{
				logger.Debug($"Loading music for {e.Node.Name} ({e.Node.Text})");
				var music = model.Music[e.Node.Name];
				LoadAndPlaySound(music, e.Node.Text);
			}
			else if (OriginalDataFiles.MiscellaneousTracks.ContainsKey(e.Node.Name.ToLower()))
			{
				logger.Debug($"Loading miscellaneous track {e.Node.Name} ({e.Node.Text})");
				var misc = model.MiscellaneousTracks[e.Node.Name];
				LoadAndPlaySound(misc, e.Node.Text);
			}
			else if (OriginalDataFiles.SoundEffect.Equals(e.Node.Name, StringComparison.OrdinalIgnoreCase))
			{
				logger.Debug($"Loading sound effects for {e.Node.Name}");
				var sfx = model.SoundEffects[e.Node.Name];
				LoadSoundEffectFile(sfx);
			}
			else if (Path.GetExtension(e.Node.Name).Equals(".dat", StringComparison.OrdinalIgnoreCase))
			{
				logger.Debug($"Loading object {e.Node.Name}");
				var filename = e.Node.Name;
				CurrentUIObject = model.LoadAndCacheObject(filename);

#if DEBUG
				LoadDataDump(filename);
#else
				try
				{
					LoadDataDump(filename);
				}
				catch (Exception ex)
				{
					logger?.Error(ex, $"Unable to annotate file \"{filename}\"");
				}
#endif
			}

			flpImageTable.ResumeLayout(true);
		}

		public int ImageScale
		{
			get => imageScale;
			set
			{
				if (value is < 1 or > 10)
				{
					value = 1;
				}

				if (value != imageScale)
				{
					imageScale = value;
					RefreshImageControls();
				}
			}
		}
		int imageScale = 1;

		IEnumerable<Control> CreateImageControls(IEnumerable<Bitmap> images)
		{
			// todo: on these controls we could add a right_click handler to replace image with user-created one
			var count = 0;

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
					Size = new Size(img.Width * ImageScale, img.Height * ImageScale),
					ContextMenuStrip = imgContextMenu,
				};

				var text = GetImageName(CurrentUIObject, count);
				var tb = new TextBox
				{
					Text = GetImageName(CurrentUIObject, count),
					Dock = DockStyle.Top
				};
				var size = TextRenderer.MeasureText(text, tb.Font);
				tb.MinimumSize = new Size(size.Width, 16);

				count++;

				panel.Controls.Add(tb);
				panel.Controls.Add(pb);

				yield return panel;
			}
		}

		static IEnumerable<Bitmap> CreateImages(List<G1Element32> G1Elements, SixLabors.ImageSharp.Color[] palette, bool useTransparency = false, ILogger? logger = null)
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
					logger?.Info($"skipped loading g1 element {i} with 0 length");
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

		static Bitmap? G1ElementToBitmap(G1Element32 currElement, SixLabors.ImageSharp.Color[] palette, bool useTransparency = false)
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

					if (paletteIndex == 0 && useTransparency)
					{
						// transparent pixel - no need to set anything as that's the default pixel value
						//ImageHelpers.SetPixel(dstImgData, x, y, colour);
						//Debugger.Break();
					}
					else
					{
						//if (paletteIndex >= 154 && paletteIndex <= 165)
						//{
						//	//Debugger.Break();
						//}
						var colour = palette[paletteIndex];
						var pixel = colour.ToPixel<Rgb24>();
						ImageHelpers.SetPixel(dstImgData, x, y, Color.FromArgb(pixel.R, pixel.G, pixel.B));
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

				if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
				{
					//model.PaletteFile = openFileDialog.FileName;
					var paletteBitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);
					model.Palette = PaletteHelpers.PaletteFromBitmap(paletteBitmap);

					RefreshObjectUI();
				}
			}
		}

		void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
			=> SelectNewPalette();

		void RefreshObjectUI()
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
				logger.Error($"File \"{selectedFile}\" couldn't be loaded. Does it exist? Suggest recreating object index.");
				return;
			}

			flpImageTable.SuspendLayout();
			flpImageTable.Controls.Clear();

			CurrentUIImages = [];

			if (CurrentUIObject is UiLocoObject uiLocoObj)
			{
				tsImageTable.Enabled = true;
				tsImageTable.Visible = true;

				pgS5Header.SelectedObject = uiLocoObj.DatFileInfo.S5Header;
				pgObjHeader.SelectedObject = uiLocoObj.DatFileInfo.ObjectHeader;

				if (uiLocoObj.LocoObject != null)
				{
					if (uiLocoObj.LocoObject.G1Elements != null && uiLocoObj.LocoObject.G1Elements.Count != 0)
					{
						//if (model.Palette is null)
						//{
						//	logger.Error("No palette file loaded - please load one from File -> Load Palette. You can use palette.png in the top level folder of this repo.");
						//	return;
						//}

						currentUIObjectImages = CreateImages(uiLocoObj.LocoObject.G1Elements, model.Palette, logger: logger).ToList();
						RefreshImageControls();
					}

					if (uiLocoObj.LocoObject.Object is SoundObject soundObject)
					{
						tsImageTable.Enabled = false;
						tsImageTable.Visible = false;

						var hdr = soundObject.SoundObjectData.PcmHeader;
						var text = uiLocoObj.LocoObject.StringTable.Table["Name"][LanguageId.english_uk] ?? "<null>";
						var pn = CreateSoundUI(new UiSoundObject { Data = soundObject.PcmData, Header = SawyerStreamWriter.WaveFormatExToRiff(hdr, soundObject.PcmData.Length), SoundName = text });
						flpImageTable.Controls.Add(pn);
					}

					pgObject.SelectedObject = uiLocoObj.LocoObject.Object;
					ucStringTable.SetDataBinding(uiLocoObj.LocoObject.StringTable);
				}
			}

			if (CurrentUIObject is UiSoundObjectList uiSoundObjList)
			{
				tsImageTable.Enabled = false;
				tsImageTable.Visible = false;

				pgS5Header.SelectedObject = uiSoundObjList.Audio[0].Header;

				var flp = GetNewSoundUIFLP();
				foreach (var x in uiSoundObjList.Audio)
				{
					var pn = CreateSoundUI(x);
					flp.Controls.Add(pn);
				}

				flpImageTable.Controls.Add(flp);
			}

			if (CurrentUIObject is UiG1 uiG1)
			{
				pgS5Header.SelectedObject = uiG1.G1.G1Header;
				currentUIObjectImages = CreateImages(uiG1.G1.G1Elements, model.Palette).ToList();
				RefreshImageControls();
			}

			flpImageTable.ResumeLayout(true);
		}

		void imgContextMenuSave_Click(object sender, EventArgs e)
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

		void btnPageFirst_Click(object sender, EventArgs e)
			=> CurrentUIImagePageNumber = 0;

		void btnPagePrevious_Click(object sender, EventArgs e)
			=> CurrentUIImagePageNumber = Math.Max(CurrentUIImagePageNumber - 1, 0);

		void btnPageNext_Click(object sender, EventArgs e)
		{
			if (currentUIImages?.Count > 0)
			{
				CurrentUIImagePageNumber = Math.Min(CurrentUIImagePageNumber + 1, CurrentUIImages.Count / ImagesPerPage);
			}
		}

		void btnPageLast_Click(object sender, EventArgs e)
			=> CurrentUIImagePageNumber = (CurrentUIImages.Count / ImagesPerPage);

		void dataDumpAnnotations_AfterSelect(object sender, TreeViewEventArgs e)
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

		void headerToolStripMenuItem_Click(object sender, EventArgs e)
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
					_ = tvDATDumpAnnotations.Focus();
				}
			}
		}

		void pictureDataToolStripMenuItem_Click(object sender, EventArgs e)
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
					_ = tvDATDumpAnnotations.Focus();
				}
			}
		}

		void cbVanillaObjects_CheckedChanged(object sender, EventArgs e)
			=> InitUI(cbVanillaObjects.Checked, tbFileFilter.Text);

		void lbLogs_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();

			if (e.Index < 0)
			{
				return;
			}

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

			Verify.NotNull(e.Font);

			e.Graphics.DrawString(item.ToString(), e.Font!, foregroundBrush, e.Bounds.Left, e.Bounds.Y);

			//e.DrawFocusRectangle();
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			if (CurrentUIObject is null)
			{
				logger.Warning("Current UI object is null");
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

					if (currentUIObject is UiLocoObject obj)
					{
						model.SaveFile(filename, obj);
					}
					else if (currentUIObject is UiSoundObjectList uiSoundObjList)
					{
						if (tvObjType.SelectedNode.Name == "css1.dat")
						{
							var rawBytes = SawyerStreamWriter.SaveSoundEffectsToCSS(uiSoundObjList.Audio.Select(uis => (uis.Header, uis.Data)).ToList());
							File.WriteAllBytes(filename, rawBytes);
						}
						else
						{
							// save regular sound
						}
					}

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
				}
			}
		}

		void tsbImportFromDirectory_Click(object sender, EventArgs e) => ImportImages();

		void tsbExportToDirectory_Click(object sender, EventArgs e) => ExportImages();

		void tstbImageScaling_TextChanged(object sender, EventArgs e)
		{
			var parsed = int.TryParse(tstbImageScaling.Text, out var scale);
			if (parsed)
			{
				ImageScale = scale;
			}
		}
	}
}
