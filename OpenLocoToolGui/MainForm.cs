using NAudio.Wave;
using OpenLocoTool;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;
using System.Drawing.Imaging;

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
		MainFormModel model;
		ILogger logger;

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
			// can only do this after window handle has been created (so can't do in cstr)
			((Logger)logger).LogAdded += (s, e) => lbLogs.Invoke(() => lbLogs.Items.Insert(0, e.Log.ToString()));

			// setup dark mode???
			//DarkModify(this);

			InitUI();
		}

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

		void InitUI(string filter = "")
		{
			InitFileTreeView(filter);
			InitCategoryTreeView(filter);
		}

		bool LoadDirectory(string directory, bool useExistingIndex)
		{
			if (string.IsNullOrEmpty(directory))
			{
				logger.Warning("Settings.ObjectDirectory not set");
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
					model.LoadDirectory(directory, progress, useExistingIndex);
					progressForm.CloseForm();
				});
				thread.Start();
				progressForm.ShowDialog();
			}

			return true;
		}

		Bitmap MakeOriginalLocoIcon(bool isOriginal)
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

		ImageList MakeImageList()
		{
			var imageList = new ImageList();
			var blankImage = MakeOriginalLocoIcon(false);
			var originalImage = MakeOriginalLocoIcon(true);
			imageList.Images.Add(blankImage);
			imageList.Images.Add(originalImage);
			return imageList;
		}

		void InitFileTreeView(string fileFilter = "")
		{
			tvFileTree.SuspendLayout();
			tvFileTree.Nodes.Clear();
			var filteredFiles = model.HeaderIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			tvFileTree.ImageList = MakeImageList();

			foreach (var obj in filteredFiles)
			{
				var relative = Path.GetRelativePath(model.Settings.ObjectDirectory, obj.Key);
				var imageIndex = OriginalObjects.Names.Contains(obj.Value.Name.Trim()) ? 1 : 0;
				var node = tvFileTree.Nodes.Add(obj.Key, relative, imageIndex, imageIndex);
			}

			tvFileTree.Sort();
			tvFileTree.ResumeLayout(true);
		}

		void InitCategoryTreeView(string fileFilter = "")
		{
			tvObjType.SuspendLayout();
			tvObjType.Nodes.Clear();

			var filteredFiles = model.HeaderIndex.Where(hdr => hdr.Key.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase));

			tvObjType.ImageList = MakeImageList();

			var nodesToAdd = new List<TreeNode>();
			foreach (var group in filteredFiles.GroupBy(kvp => kvp.Value.ObjectType))
			{
				var typeNode = new TreeNode(group.Key.ToString());
				if (group.Key != ObjectType.Vehicle)
				{
					foreach (var obj in group)
					{
						var imageIndex = OriginalObjects.Names.Contains(obj.Value.Name.Trim()) ? 1 : 0;
						typeNode.Nodes.Add(obj.Key, obj.Value.Name, imageIndex, imageIndex);
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
							var imageIndex = OriginalObjects.Names.Contains(veh.Value.Name.Trim()) ? 1 : 0;
							vehicleTypeNode.Nodes.Add(veh.Key, veh.Value.Name, imageIndex, imageIndex);
						}
						typeNode.Nodes.Add(vehicleTypeNode);
					}
				}

				nodesToAdd.Add(typeNode);
			}

			tvObjType.Sort();
			tvObjType.Nodes.AddRange(nodesToAdd.ToArray());

			tvObjType.ResumeLayout(true);
		}

		// note: doesn't work atm
		private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pgObject.SelectedObject is not ILocoObject obj)
			{
				return;
			}

			saveFileDialog1.InitialDirectory = model.Settings.ObjectDirectory;
			saveFileDialog1.DefaultExt = "dat";
			saveFileDialog1.Filter = "Locomotion DAT files (.dat)|*.dat";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				var filename = saveFileDialog1.FileName;

				try
				{
					var exists = File.Exists(filename);
					model.SaveFile(filename, obj);

					if (!exists)
					{
						// we made a new file (as opposed to overwriting an existing one) so lets update the UI to show it
						InitUI();
					}
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
				if (LoadDirectory(objectDirBrowser.SelectedPath, true))
				{
					InitUI();
				}
			}
		}

		private void recreateIndexToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LoadDirectory(model.Settings.ObjectDirectory, false))
			{
				InitUI();
			}
		}

		void tbFileFilter_TextChanged(object sender, EventArgs e)
		{
			InitUI(tbFileFilter.Text);
		}

		void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}

			flpImageTable.Controls.Clear();

			var obj = model.LoadAndCacheObject(e.Node.Name);

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

			var paletteBitmap = new Bitmap(model.Settings.PaletteFile);
			var palette = PaletteHelpers.PaletteFromBitmap(paletteBitmap);

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
						ImageHelpers.SetPixel(dstImgData, x, y, colour);
					}
				}

				dstImg.UnlockBits(dstImgData);

				// on these controls we could add a right_click handler to replace image with user-created one
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
	}
}