using OpenLoco.ObjectEditor.Types;

namespace OpenLoco.ObjectEditor.Gui
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			lbLogs = new ListBox();
			objectDirBrowser = new FolderBrowserDialog();
			tbFileFilter = new TextBox();
			lblFilenameRegex = new Label();
			saveFileDialog1 = new SaveFileDialog();
			flpImageTable = new FlowLayoutPanel();
			scObjectAndLogs = new SplitContainer();
			tcObjectOverview = new TabControl();
			tpObjectEditor = new TabPage();
			tcSubObjectView = new TabControl();
			tpObjectObject = new TabPage();
			pgObject = new PropertyGrid();
			tpObjectStringTable = new TabPage();
			ucStringTable = new StringTableUserControl();
			tpObjectGraphicsTable = new TabPage();
			pnImagePageControls = new Panel();
			tbCurrentPage = new TextBox();
			btnPagePrevious = new Button();
			btnPageNext = new Button();
			tsImageTable = new ToolStrip();
			tsbImportFromDirectory = new ToolStripButton();
			toolStripSeparator4 = new ToolStripSeparator();
			tsbExportToDirectory = new ToolStripButton();
			toolStripSeparator3 = new ToolStripSeparator();
			tslImageScaling = new ToolStripLabel();
			tstbImageScaling = new ToolStripTextBox();
			tpObjectPreview = new TabPage();
			gbBogieSprites = new GroupBox();
			trbBogieSprites = new TrackBar();
			btnBogieSpritePrevious = new Button();
			btnBogieSpriteNext = new Button();
			tbCurrentBogieSprite = new TextBox();
			gbBodySprites = new GroupBox();
			trbBodySprites = new TrackBar();
			btnBodySpritePrevious = new Button();
			btnBodySpriteNext = new Button();
			tbCurrentBodySprite = new TextBox();
			gbTilt = new GroupBox();
			trbTilt = new TrackBar();
			btnTiltDown = new Button();
			btnTiltUp = new Button();
			tbTiltValue = new TextBox();
			gbRotation = new GroupBox();
			trbRotate = new TrackBar();
			btnRotateLeft = new Button();
			btnRotateRight = new Button();
			tbRotateValue = new TextBox();
			pbVehicleBogiePreview = new PictureBoxWithInterpolationMode();
			pbVehicleBodyPreview = new PictureBoxWithInterpolationMode();
			scHeaders = new SplitContainer();
			pgS5Header = new PropertyGrid();
			pgObjHeader = new PropertyGrid();
			tpObjectAnnotator = new TabPage();
			scAnnnotationDump = new SplitContainer();
			tvDATDumpAnnotations = new TreeView();
			rtbDATDumpView = new RichTextBox();
			tsObjectEditor = new ToolStrip();
			btnSave = new ToolStripButton();
			scTop = new SplitContainer();
			tcFileSelector = new TabControl();
			tpCategory = new TabPage();
			tvObjType = new TreeView();
			tpFile = new TabPage();
			tvFileTree = new TreeView();
			pnFileFilter = new Panel();
			cbVanillaObjects = new CheckBox();
			menuStrip = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			objectDirectoriesToolStripMenuItem = new ToolStripMenuItem();
			setObjectDirectoryToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			dataDirectoriesToolStripMenuItem = new ToolStripMenuItem();
			setDataDirectoryToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator2 = new ToolStripSeparator();
			recreateIndexToolStripMenuItem = new ToolStripMenuItem();
			setPaletteToolStripMenuItem = new ToolStripMenuItem();
			imgContextMenu = new ContextMenuStrip(components);
			imgContextMenuSave = new ToolStripMenuItem();
			goToHeaderInDumpToolStripMenuItem = new ToolStripMenuItem();
			headerToolStripMenuItem = new ToolStripMenuItem();
			pictureDataToolStripMenuItem = new ToolStripMenuItem();
			locoObjectBindingSource = new BindingSource(components);
			((System.ComponentModel.ISupportInitialize)scObjectAndLogs).BeginInit();
			scObjectAndLogs.Panel1.SuspendLayout();
			scObjectAndLogs.Panel2.SuspendLayout();
			scObjectAndLogs.SuspendLayout();
			tcObjectOverview.SuspendLayout();
			tpObjectEditor.SuspendLayout();
			tcSubObjectView.SuspendLayout();
			tpObjectObject.SuspendLayout();
			tpObjectStringTable.SuspendLayout();
			tpObjectGraphicsTable.SuspendLayout();
			pnImagePageControls.SuspendLayout();
			tsImageTable.SuspendLayout();
			tpObjectPreview.SuspendLayout();
			gbBogieSprites.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trbBogieSprites).BeginInit();
			gbBodySprites.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trbBodySprites).BeginInit();
			gbTilt.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trbTilt).BeginInit();
			gbRotation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trbRotate).BeginInit();
			((System.ComponentModel.ISupportInitialize)pbVehicleBogiePreview).BeginInit();
			((System.ComponentModel.ISupportInitialize)pbVehicleBodyPreview).BeginInit();
			((System.ComponentModel.ISupportInitialize)scHeaders).BeginInit();
			scHeaders.Panel1.SuspendLayout();
			scHeaders.Panel2.SuspendLayout();
			scHeaders.SuspendLayout();
			tpObjectAnnotator.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scAnnnotationDump).BeginInit();
			scAnnnotationDump.Panel1.SuspendLayout();
			scAnnnotationDump.Panel2.SuspendLayout();
			scAnnnotationDump.SuspendLayout();
			tsObjectEditor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scTop).BeginInit();
			scTop.Panel1.SuspendLayout();
			scTop.Panel2.SuspendLayout();
			scTop.SuspendLayout();
			tcFileSelector.SuspendLayout();
			tpCategory.SuspendLayout();
			tpFile.SuspendLayout();
			pnFileFilter.SuspendLayout();
			menuStrip.SuspendLayout();
			imgContextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)locoObjectBindingSource).BeginInit();
			SuspendLayout();
			// 
			// lbLogs
			// 
			lbLogs.BorderStyle = BorderStyle.FixedSingle;
			lbLogs.Dock = DockStyle.Fill;
			lbLogs.DrawMode = DrawMode.OwnerDrawFixed;
			lbLogs.FormattingEnabled = true;
			lbLogs.HorizontalScrollbar = true;
			lbLogs.ItemHeight = 18;
			lbLogs.Location = new Point(0, 0);
			lbLogs.Name = "lbLogs";
			lbLogs.SelectionMode = SelectionMode.None;
			lbLogs.Size = new Size(1415, 186);
			lbLogs.TabIndex = 17;
			lbLogs.DrawItem += lbLogs_DrawItem;
			// 
			// tbFileFilter
			// 
			tbFileFilter.BorderStyle = BorderStyle.FixedSingle;
			tbFileFilter.Dock = DockStyle.Top;
			tbFileFilter.Location = new Point(4, 27);
			tbFileFilter.Margin = new Padding(4);
			tbFileFilter.Name = "tbFileFilter";
			tbFileFilter.Size = new Size(469, 23);
			tbFileFilter.TabIndex = 24;
			tbFileFilter.TextChanged += tbFileFilter_TextChanged;
			// 
			// lblFilenameRegex
			// 
			lblFilenameRegex.Dock = DockStyle.Top;
			lblFilenameRegex.Location = new Point(4, 4);
			lblFilenameRegex.Margin = new Padding(4);
			lblFilenameRegex.Name = "lblFilenameRegex";
			lblFilenameRegex.Size = new Size(469, 23);
			lblFilenameRegex.TabIndex = 25;
			lblFilenameRegex.Text = "Filename Filter";
			lblFilenameRegex.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// flpImageTable
			// 
			flpImageTable.AutoScroll = true;
			flpImageTable.BorderStyle = BorderStyle.FixedSingle;
			flpImageTable.Dock = DockStyle.Fill;
			flpImageTable.FlowDirection = FlowDirection.TopDown;
			flpImageTable.Location = new Point(2, 26);
			flpImageTable.Name = "flpImageTable";
			flpImageTable.Size = new Size(1389, 585);
			flpImageTable.TabIndex = 30;
			// 
			// scObjectAndLogs
			// 
			scObjectAndLogs.Dock = DockStyle.Fill;
			scObjectAndLogs.Location = new Point(0, 0);
			scObjectAndLogs.Name = "scObjectAndLogs";
			scObjectAndLogs.Orientation = Orientation.Horizontal;
			// 
			// scObjectAndLogs.Panel1
			// 
			scObjectAndLogs.Panel1.Controls.Add(tcObjectOverview);
			scObjectAndLogs.Panel1.Controls.Add(tsObjectEditor);
			// 
			// scObjectAndLogs.Panel2
			// 
			scObjectAndLogs.Panel2.Controls.Add(lbLogs);
			scObjectAndLogs.Size = new Size(1415, 1009);
			scObjectAndLogs.SplitterDistance = 819;
			scObjectAndLogs.TabIndex = 31;
			// 
			// tcObjectOverview
			// 
			tcObjectOverview.Controls.Add(tpObjectEditor);
			tcObjectOverview.Controls.Add(tpObjectAnnotator);
			tcObjectOverview.Dock = DockStyle.Fill;
			tcObjectOverview.Location = new Point(0, 25);
			tcObjectOverview.Name = "tcObjectOverview";
			tcObjectOverview.SelectedIndex = 0;
			tcObjectOverview.Size = new Size(1415, 794);
			tcObjectOverview.TabIndex = 34;
			// 
			// tpObjectEditor
			// 
			tpObjectEditor.Controls.Add(tcSubObjectView);
			tpObjectEditor.Controls.Add(scHeaders);
			tpObjectEditor.Location = new Point(4, 24);
			tpObjectEditor.Name = "tpObjectEditor";
			tpObjectEditor.Padding = new Padding(3);
			tpObjectEditor.Size = new Size(1407, 766);
			tpObjectEditor.TabIndex = 0;
			tpObjectEditor.Text = "Editor";
			tpObjectEditor.UseVisualStyleBackColor = true;
			// 
			// tcSubObjectView
			// 
			tcSubObjectView.Controls.Add(tpObjectObject);
			tcSubObjectView.Controls.Add(tpObjectStringTable);
			tcSubObjectView.Controls.Add(tpObjectGraphicsTable);
			tcSubObjectView.Controls.Add(tpObjectPreview);
			tcSubObjectView.Dock = DockStyle.Fill;
			tcSubObjectView.Location = new Point(3, 123);
			tcSubObjectView.Margin = new Padding(2, 1, 2, 1);
			tcSubObjectView.Name = "tcSubObjectView";
			tcSubObjectView.SelectedIndex = 0;
			tcSubObjectView.Size = new Size(1401, 640);
			tcSubObjectView.TabIndex = 33;
			// 
			// tpObjectObject
			// 
			tpObjectObject.Controls.Add(pgObject);
			tpObjectObject.Location = new Point(4, 24);
			tpObjectObject.Margin = new Padding(2, 1, 2, 1);
			tpObjectObject.Name = "tpObjectObject";
			tpObjectObject.Padding = new Padding(2, 1, 2, 1);
			tpObjectObject.Size = new Size(1393, 612);
			tpObjectObject.TabIndex = 2;
			tpObjectObject.Text = "Object";
			tpObjectObject.UseVisualStyleBackColor = true;
			// 
			// pgObject
			// 
			pgObject.Dock = DockStyle.Fill;
			pgObject.HelpVisible = false;
			pgObject.Location = new Point(2, 1);
			pgObject.Name = "pgObject";
			pgObject.Size = new Size(1389, 610);
			pgObject.TabIndex = 24;
			pgObject.ToolbarVisible = false;
			// 
			// tpObjectStringTable
			// 
			tpObjectStringTable.Controls.Add(ucStringTable);
			tpObjectStringTable.Location = new Point(4, 24);
			tpObjectStringTable.Margin = new Padding(2, 1, 2, 1);
			tpObjectStringTable.Name = "tpObjectStringTable";
			tpObjectStringTable.Padding = new Padding(2, 1, 2, 1);
			tpObjectStringTable.Size = new Size(1393, 612);
			tpObjectStringTable.TabIndex = 3;
			tpObjectStringTable.Text = "String Table";
			tpObjectStringTable.UseVisualStyleBackColor = true;
			// 
			// ucStringTable
			// 
			ucStringTable.Dock = DockStyle.Fill;
			ucStringTable.Location = new Point(2, 1);
			ucStringTable.Margin = new Padding(1, 0, 1, 0);
			ucStringTable.Name = "ucStringTable";
			ucStringTable.Size = new Size(1389, 610);
			ucStringTable.TabIndex = 0;
			// 
			// tpObjectGraphicsTable
			// 
			tpObjectGraphicsTable.Controls.Add(pnImagePageControls);
			tpObjectGraphicsTable.Controls.Add(flpImageTable);
			tpObjectGraphicsTable.Controls.Add(tsImageTable);
			tpObjectGraphicsTable.Location = new Point(4, 24);
			tpObjectGraphicsTable.Margin = new Padding(2, 1, 2, 1);
			tpObjectGraphicsTable.Name = "tpObjectGraphicsTable";
			tpObjectGraphicsTable.Padding = new Padding(2, 1, 2, 1);
			tpObjectGraphicsTable.Size = new Size(1393, 612);
			tpObjectGraphicsTable.TabIndex = 4;
			tpObjectGraphicsTable.Text = "Graphics/Sounds";
			tpObjectGraphicsTable.UseVisualStyleBackColor = true;
			// 
			// pnImagePageControls
			// 
			pnImagePageControls.Controls.Add(tbCurrentPage);
			pnImagePageControls.Controls.Add(btnPagePrevious);
			pnImagePageControls.Controls.Add(btnPageNext);
			pnImagePageControls.Dock = DockStyle.Bottom;
			pnImagePageControls.Location = new Point(2, 588);
			pnImagePageControls.Name = "pnImagePageControls";
			pnImagePageControls.Size = new Size(1389, 23);
			pnImagePageControls.TabIndex = 0;
			// 
			// tbCurrentPage
			// 
			tbCurrentPage.BorderStyle = BorderStyle.FixedSingle;
			tbCurrentPage.Dock = DockStyle.Bottom;
			tbCurrentPage.Enabled = false;
			tbCurrentPage.Location = new Point(253, 0);
			tbCurrentPage.Name = "tbCurrentPage";
			tbCurrentPage.Size = new Size(908, 23);
			tbCurrentPage.TabIndex = 33;
			tbCurrentPage.TextAlign = HorizontalAlignment.Center;
			// 
			// btnPagePrevious
			// 
			btnPagePrevious.Dock = DockStyle.Left;
			btnPagePrevious.Location = new Point(0, 0);
			btnPagePrevious.Name = "btnPagePrevious";
			btnPagePrevious.Size = new Size(253, 23);
			btnPagePrevious.TabIndex = 0;
			btnPagePrevious.Text = "← Previous Page";
			btnPagePrevious.UseVisualStyleBackColor = true;
			btnPagePrevious.Click += btnPagePrevious_Click;
			// 
			// btnPageNext
			// 
			btnPageNext.Dock = DockStyle.Right;
			btnPageNext.Location = new Point(1161, 0);
			btnPageNext.Name = "btnPageNext";
			btnPageNext.Size = new Size(228, 23);
			btnPageNext.TabIndex = 31;
			btnPageNext.Text = "Next Page →";
			btnPageNext.UseVisualStyleBackColor = true;
			btnPageNext.Click += btnPageNext_Click;
			// 
			// tsImageTable
			// 
			tsImageTable.Items.AddRange(new ToolStripItem[] { tsbImportFromDirectory, toolStripSeparator4, tsbExportToDirectory, toolStripSeparator3, tslImageScaling, tstbImageScaling });
			tsImageTable.Location = new Point(2, 1);
			tsImageTable.Name = "tsImageTable";
			tsImageTable.Size = new Size(1389, 25);
			tsImageTable.TabIndex = 31;
			tsImageTable.Text = "toolStrip2";
			// 
			// tsbImportFromDirectory
			// 
			tsbImportFromDirectory.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsbImportFromDirectory.Image = (Image)resources.GetObject("tsbImportFromDirectory.Image");
			tsbImportFromDirectory.ImageTransparentColor = Color.Magenta;
			tsbImportFromDirectory.Name = "tsbImportFromDirectory";
			tsbImportFromDirectory.Size = new Size(142, 22);
			tsbImportFromDirectory.Text = "📥 Import from Directory";
			tsbImportFromDirectory.Click += tsbImportFromDirectory_Click;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new Size(6, 25);
			// 
			// tsbExportToDirectory
			// 
			tsbExportToDirectory.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsbExportToDirectory.Image = (Image)resources.GetObject("tsbExportToDirectory.Image");
			tsbExportToDirectory.ImageTransparentColor = Color.Magenta;
			tsbExportToDirectory.Name = "tsbExportToDirectory";
			tsbExportToDirectory.Size = new Size(126, 22);
			tsbExportToDirectory.Text = "📤 Export To Directory";
			tsbExportToDirectory.Click += tsbExportToDirectory_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new Size(6, 25);
			// 
			// tslImageScaling
			// 
			tslImageScaling.Name = "tslImageScaling";
			tslImageScaling.Size = new Size(130, 22);
			tslImageScaling.Text = "🔍 Image Scaling (1-10)";
			// 
			// tstbImageScaling
			// 
			tstbImageScaling.BorderStyle = BorderStyle.FixedSingle;
			tstbImageScaling.Name = "tstbImageScaling";
			tstbImageScaling.Size = new Size(32, 25);
			tstbImageScaling.Text = "1";
			tstbImageScaling.ToolTipText = "Image Scaling";
			tstbImageScaling.TextChanged += tstbImageScaling_TextChanged;
			// 
			// tpObjectPreview
			// 
			tpObjectPreview.Controls.Add(gbBogieSprites);
			tpObjectPreview.Controls.Add(gbBodySprites);
			tpObjectPreview.Controls.Add(gbTilt);
			tpObjectPreview.Controls.Add(gbRotation);
			tpObjectPreview.Controls.Add(pbVehicleBogiePreview);
			tpObjectPreview.Controls.Add(pbVehicleBodyPreview);
			tpObjectPreview.Location = new Point(4, 24);
			tpObjectPreview.Name = "tpObjectPreview";
			tpObjectPreview.Size = new Size(1393, 612);
			tpObjectPreview.TabIndex = 5;
			tpObjectPreview.Text = "Vehicle Rotator";
			tpObjectPreview.UseVisualStyleBackColor = true;
			// 
			// gbBogieSprites
			// 
			gbBogieSprites.Controls.Add(trbBogieSprites);
			gbBogieSprites.Controls.Add(btnBogieSpritePrevious);
			gbBogieSprites.Controls.Add(btnBogieSpriteNext);
			gbBogieSprites.Controls.Add(tbCurrentBogieSprite);
			gbBogieSprites.Location = new Point(266, 265);
			gbBogieSprites.Name = "gbBogieSprites";
			gbBogieSprites.Size = new Size(256, 104);
			gbBogieSprites.TabIndex = 41;
			gbBogieSprites.TabStop = false;
			gbBogieSprites.Text = "Bogie Sprites";
			// 
			// trbBogieSprites
			// 
			trbBogieSprites.Location = new Point(6, 22);
			trbBogieSprites.Maximum = 1;
			trbBogieSprites.Name = "trbBogieSprites";
			trbBogieSprites.Size = new Size(244, 45);
			trbBogieSprites.TabIndex = 29;
			trbBogieSprites.TickStyle = TickStyle.Both;
			trbBogieSprites.Scroll += trbBogieSprites_Scroll;
			// 
			// btnBogieSpritePrevious
			// 
			btnBogieSpritePrevious.Location = new Point(6, 72);
			btnBogieSpritePrevious.Name = "btnBogieSpritePrevious";
			btnBogieSpritePrevious.Size = new Size(23, 23);
			btnBogieSpritePrevious.TabIndex = 32;
			btnBogieSpritePrevious.Text = "<";
			btnBogieSpritePrevious.UseVisualStyleBackColor = true;
			btnBogieSpritePrevious.Click += btnBogieSpritePrevious_Click;
			// 
			// btnBogieSpriteNext
			// 
			btnBogieSpriteNext.Location = new Point(227, 73);
			btnBogieSpriteNext.Name = "btnBogieSpriteNext";
			btnBogieSpriteNext.Size = new Size(23, 23);
			btnBogieSpriteNext.TabIndex = 33;
			btnBogieSpriteNext.Text = ">";
			btnBogieSpriteNext.UseVisualStyleBackColor = true;
			btnBogieSpriteNext.Click += btnBogieSpriteNext_Click;
			// 
			// tbCurrentBogieSprite
			// 
			tbCurrentBogieSprite.BorderStyle = BorderStyle.FixedSingle;
			tbCurrentBogieSprite.Enabled = false;
			tbCurrentBogieSprite.Location = new Point(35, 73);
			tbCurrentBogieSprite.Name = "tbCurrentBogieSprite";
			tbCurrentBogieSprite.Size = new Size(186, 23);
			tbCurrentBogieSprite.TabIndex = 34;
			tbCurrentBogieSprite.Text = "0";
			tbCurrentBogieSprite.TextAlign = HorizontalAlignment.Center;
			// 
			// gbBodySprites
			// 
			gbBodySprites.Controls.Add(trbBodySprites);
			gbBodySprites.Controls.Add(btnBodySpritePrevious);
			gbBodySprites.Controls.Add(btnBodySpriteNext);
			gbBodySprites.Controls.Add(tbCurrentBodySprite);
			gbBodySprites.Location = new Point(4, 265);
			gbBodySprites.Name = "gbBodySprites";
			gbBodySprites.Size = new Size(256, 104);
			gbBodySprites.TabIndex = 40;
			gbBodySprites.TabStop = false;
			gbBodySprites.Text = "Body Sprites";
			// 
			// trbBodySprites
			// 
			trbBodySprites.Location = new Point(6, 22);
			trbBodySprites.Maximum = 3;
			trbBodySprites.Name = "trbBodySprites";
			trbBodySprites.Size = new Size(244, 45);
			trbBodySprites.TabIndex = 29;
			trbBodySprites.TickStyle = TickStyle.Both;
			trbBodySprites.Scroll += trbBodySprites_Scroll;
			// 
			// btnBodySpritePrevious
			// 
			btnBodySpritePrevious.Location = new Point(6, 73);
			btnBodySpritePrevious.Name = "btnBodySpritePrevious";
			btnBodySpritePrevious.Size = new Size(23, 23);
			btnBodySpritePrevious.TabIndex = 32;
			btnBodySpritePrevious.Text = "<";
			btnBodySpritePrevious.UseVisualStyleBackColor = true;
			btnBodySpritePrevious.Click += btnBodySpritePrevious_Click;
			// 
			// btnBodySpriteNext
			// 
			btnBodySpriteNext.Location = new Point(227, 73);
			btnBodySpriteNext.Name = "btnBodySpriteNext";
			btnBodySpriteNext.Size = new Size(23, 23);
			btnBodySpriteNext.TabIndex = 33;
			btnBodySpriteNext.Text = ">";
			btnBodySpriteNext.UseVisualStyleBackColor = true;
			btnBodySpriteNext.Click += btnBodySpriteNext_Click;
			// 
			// tbCurrentBodySprite
			// 
			tbCurrentBodySprite.BorderStyle = BorderStyle.FixedSingle;
			tbCurrentBodySprite.Enabled = false;
			tbCurrentBodySprite.Location = new Point(37, 74);
			tbCurrentBodySprite.Name = "tbCurrentBodySprite";
			tbCurrentBodySprite.Size = new Size(184, 23);
			tbCurrentBodySprite.TabIndex = 34;
			tbCurrentBodySprite.Text = "0";
			tbCurrentBodySprite.TextAlign = HorizontalAlignment.Center;
			// 
			// gbTilt
			// 
			gbTilt.Controls.Add(trbTilt);
			gbTilt.Controls.Add(btnTiltDown);
			gbTilt.Controls.Add(btnTiltUp);
			gbTilt.Controls.Add(tbTiltValue);
			gbTilt.Location = new Point(266, 454);
			gbTilt.Name = "gbTilt";
			gbTilt.Size = new Size(256, 104);
			gbTilt.TabIndex = 40;
			gbTilt.TabStop = false;
			gbTilt.Text = "Tilt";
			// 
			// trbTilt
			// 
			trbTilt.Location = new Point(6, 22);
			trbTilt.Maximum = 5;
			trbTilt.Name = "trbTilt";
			trbTilt.Size = new Size(244, 45);
			trbTilt.TabIndex = 30;
			trbTilt.TickStyle = TickStyle.Both;
			trbTilt.Scroll += trbTilt_Scroll;
			// 
			// btnTiltDown
			// 
			btnTiltDown.Location = new Point(6, 73);
			btnTiltDown.Name = "btnTiltDown";
			btnTiltDown.Size = new Size(23, 23);
			btnTiltDown.TabIndex = 35;
			btnTiltDown.Text = "<";
			btnTiltDown.UseVisualStyleBackColor = true;
			btnTiltDown.Click += btnTiltDown_Click;
			// 
			// btnTiltUp
			// 
			btnTiltUp.Location = new Point(227, 73);
			btnTiltUp.Name = "btnTiltUp";
			btnTiltUp.Size = new Size(23, 23);
			btnTiltUp.TabIndex = 36;
			btnTiltUp.Text = ">";
			btnTiltUp.UseVisualStyleBackColor = true;
			btnTiltUp.Click += btnTiltUp_Click;
			// 
			// tbTiltValue
			// 
			tbTiltValue.BorderStyle = BorderStyle.FixedSingle;
			tbTiltValue.Enabled = false;
			tbTiltValue.Location = new Point(35, 73);
			tbTiltValue.Name = "tbTiltValue";
			tbTiltValue.Size = new Size(186, 23);
			tbTiltValue.TabIndex = 37;
			tbTiltValue.Text = "0";
			tbTiltValue.TextAlign = HorizontalAlignment.Center;
			// 
			// gbRotation
			// 
			gbRotation.Controls.Add(trbRotate);
			gbRotation.Controls.Add(btnRotateLeft);
			gbRotation.Controls.Add(btnRotateRight);
			gbRotation.Controls.Add(tbRotateValue);
			gbRotation.Location = new Point(4, 454);
			gbRotation.Name = "gbRotation";
			gbRotation.Size = new Size(256, 104);
			gbRotation.TabIndex = 39;
			gbRotation.TabStop = false;
			gbRotation.Text = "Rotation";
			// 
			// trbRotate
			// 
			trbRotate.Location = new Point(6, 22);
			trbRotate.Name = "trbRotate";
			trbRotate.Size = new Size(244, 45);
			trbRotate.TabIndex = 29;
			trbRotate.TickStyle = TickStyle.Both;
			trbRotate.Scroll += trbRotate_Scroll;
			// 
			// btnRotateLeft
			// 
			btnRotateLeft.Location = new Point(6, 73);
			btnRotateLeft.Name = "btnRotateLeft";
			btnRotateLeft.Size = new Size(23, 23);
			btnRotateLeft.TabIndex = 32;
			btnRotateLeft.Text = "<";
			btnRotateLeft.UseVisualStyleBackColor = true;
			btnRotateLeft.Click += btnRotateLeft_Click;
			// 
			// btnRotateRight
			// 
			btnRotateRight.Location = new Point(227, 73);
			btnRotateRight.Name = "btnRotateRight";
			btnRotateRight.Size = new Size(23, 23);
			btnRotateRight.TabIndex = 33;
			btnRotateRight.Text = ">";
			btnRotateRight.UseVisualStyleBackColor = true;
			btnRotateRight.Click += btnRotateRight_Click;
			// 
			// tbRotateValue
			// 
			tbRotateValue.BorderStyle = BorderStyle.FixedSingle;
			tbRotateValue.Enabled = false;
			tbRotateValue.Location = new Point(35, 73);
			tbRotateValue.Name = "tbRotateValue";
			tbRotateValue.Size = new Size(186, 23);
			tbRotateValue.TabIndex = 34;
			tbRotateValue.Text = "0";
			tbRotateValue.TextAlign = HorizontalAlignment.Center;
			// 
			// pbVehicleBogiePreview
			// 
			pbVehicleBogiePreview.BorderStyle = BorderStyle.FixedSingle;
			pbVehicleBogiePreview.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
			pbVehicleBogiePreview.Location = new Point(266, 3);
			pbVehicleBogiePreview.Name = "pbVehicleBogiePreview";
			pbVehicleBogiePreview.Size = new Size(256, 256);
			pbVehicleBogiePreview.TabIndex = 38;
			pbVehicleBogiePreview.TabStop = false;
			// 
			// pbVehicleBodyPreview
			// 
			pbVehicleBodyPreview.BorderStyle = BorderStyle.FixedSingle;
			pbVehicleBodyPreview.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
			pbVehicleBodyPreview.Location = new Point(4, 3);
			pbVehicleBodyPreview.Name = "pbVehicleBodyPreview";
			pbVehicleBodyPreview.Size = new Size(256, 256);
			pbVehicleBodyPreview.TabIndex = 28;
			pbVehicleBodyPreview.TabStop = false;
			// 
			// scHeaders
			// 
			scHeaders.Dock = DockStyle.Top;
			scHeaders.Location = new Point(3, 3);
			scHeaders.Name = "scHeaders";
			// 
			// scHeaders.Panel1
			// 
			scHeaders.Panel1.Controls.Add(pgS5Header);
			// 
			// scHeaders.Panel2
			// 
			scHeaders.Panel2.Controls.Add(pgObjHeader);
			scHeaders.Size = new Size(1401, 120);
			scHeaders.SplitterDistance = 698;
			scHeaders.TabIndex = 34;
			// 
			// pgS5Header
			// 
			pgS5Header.Dock = DockStyle.Fill;
			pgS5Header.HelpVisible = false;
			pgS5Header.Location = new Point(0, 0);
			pgS5Header.Name = "pgS5Header";
			pgS5Header.Size = new Size(698, 120);
			pgS5Header.TabIndex = 22;
			pgS5Header.ToolbarVisible = false;
			// 
			// pgObjHeader
			// 
			pgObjHeader.Dock = DockStyle.Fill;
			pgObjHeader.HelpVisible = false;
			pgObjHeader.Location = new Point(0, 0);
			pgObjHeader.Name = "pgObjHeader";
			pgObjHeader.Size = new Size(699, 120);
			pgObjHeader.TabIndex = 23;
			pgObjHeader.ToolbarVisible = false;
			// 
			// tpObjectAnnotator
			// 
			tpObjectAnnotator.Controls.Add(scAnnnotationDump);
			tpObjectAnnotator.Location = new Point(4, 24);
			tpObjectAnnotator.Name = "tpObjectAnnotator";
			tpObjectAnnotator.Padding = new Padding(3);
			tpObjectAnnotator.Size = new Size(1407, 766);
			tpObjectAnnotator.TabIndex = 1;
			tpObjectAnnotator.Text = "Hex Dump Annotator";
			tpObjectAnnotator.UseVisualStyleBackColor = true;
			// 
			// scAnnnotationDump
			// 
			scAnnnotationDump.Dock = DockStyle.Fill;
			scAnnnotationDump.Location = new Point(3, 3);
			scAnnnotationDump.Name = "scAnnnotationDump";
			// 
			// scAnnnotationDump.Panel1
			// 
			scAnnnotationDump.Panel1.AutoScroll = true;
			scAnnnotationDump.Panel1.Controls.Add(tvDATDumpAnnotations);
			// 
			// scAnnnotationDump.Panel2
			// 
			scAnnnotationDump.Panel2.Controls.Add(rtbDATDumpView);
			scAnnnotationDump.Size = new Size(1401, 760);
			scAnnnotationDump.SplitterDistance = 481;
			scAnnnotationDump.TabIndex = 2;
			// 
			// tvDATDumpAnnotations
			// 
			tvDATDumpAnnotations.Dock = DockStyle.Fill;
			tvDATDumpAnnotations.Location = new Point(0, 0);
			tvDATDumpAnnotations.Name = "tvDATDumpAnnotations";
			tvDATDumpAnnotations.Size = new Size(481, 760);
			tvDATDumpAnnotations.TabIndex = 1;
			tvDATDumpAnnotations.AfterSelect += dataDumpAnnotations_AfterSelect;
			// 
			// rtbDATDumpView
			// 
			rtbDATDumpView.Dock = DockStyle.Fill;
			rtbDATDumpView.Font = new Font("Cascadia Mono", 9F);
			rtbDATDumpView.HideSelection = false;
			rtbDATDumpView.Location = new Point(0, 0);
			rtbDATDumpView.Name = "rtbDATDumpView";
			rtbDATDumpView.ReadOnly = true;
			rtbDATDumpView.ShowSelectionMargin = true;
			rtbDATDumpView.Size = new Size(916, 760);
			rtbDATDumpView.TabIndex = 0;
			rtbDATDumpView.Text = "";
			rtbDATDumpView.WordWrap = false;
			// 
			// tsObjectEditor
			// 
			tsObjectEditor.Items.AddRange(new ToolStripItem[] { btnSave });
			tsObjectEditor.Location = new Point(0, 0);
			tsObjectEditor.Name = "tsObjectEditor";
			tsObjectEditor.Size = new Size(1415, 25);
			tsObjectEditor.TabIndex = 35;
			tsObjectEditor.Text = "toolStrip1";
			// 
			// btnSave
			// 
			btnSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
			btnSave.Image = (Image)resources.GetObject("btnSave.Image");
			btnSave.ImageTransparentColor = Color.Magenta;
			btnSave.Name = "btnSave";
			btnSave.Size = new Size(84, 22);
			btnSave.Text = "Save Changes";
			btnSave.Click += btnSave_Click;
			// 
			// scTop
			// 
			scTop.Dock = DockStyle.Fill;
			scTop.Location = new Point(4, 28);
			scTop.Name = "scTop";
			// 
			// scTop.Panel1
			// 
			scTop.Panel1.Controls.Add(tcFileSelector);
			scTop.Panel1.Controls.Add(pnFileFilter);
			// 
			// scTop.Panel2
			// 
			scTop.Panel2.Controls.Add(scObjectAndLogs);
			scTop.Size = new Size(1896, 1009);
			scTop.SplitterDistance = 477;
			scTop.TabIndex = 33;
			// 
			// tcFileSelector
			// 
			tcFileSelector.Controls.Add(tpCategory);
			tcFileSelector.Controls.Add(tpFile);
			tcFileSelector.Dock = DockStyle.Fill;
			tcFileSelector.Location = new Point(0, 74);
			tcFileSelector.Name = "tcFileSelector";
			tcFileSelector.SelectedIndex = 0;
			tcFileSelector.Size = new Size(477, 935);
			tcFileSelector.TabIndex = 29;
			// 
			// tpCategory
			// 
			tpCategory.Controls.Add(tvObjType);
			tpCategory.Location = new Point(4, 24);
			tpCategory.Name = "tpCategory";
			tpCategory.Padding = new Padding(3);
			tpCategory.Size = new Size(469, 907);
			tpCategory.TabIndex = 1;
			tpCategory.Text = "Category";
			tpCategory.UseVisualStyleBackColor = true;
			// 
			// tvObjType
			// 
			tvObjType.Dock = DockStyle.Fill;
			tvObjType.Location = new Point(3, 3);
			tvObjType.Name = "tvObjType";
			tvObjType.Size = new Size(463, 901);
			tvObjType.TabIndex = 26;
			tvObjType.AfterSelect += tv_AfterSelect;
			// 
			// tpFile
			// 
			tpFile.Controls.Add(tvFileTree);
			tpFile.Location = new Point(4, 24);
			tpFile.Name = "tpFile";
			tpFile.Padding = new Padding(3);
			tpFile.Size = new Size(469, 907);
			tpFile.TabIndex = 0;
			tpFile.Text = "File";
			tpFile.UseVisualStyleBackColor = true;
			// 
			// tvFileTree
			// 
			tvFileTree.Dock = DockStyle.Fill;
			tvFileTree.Location = new Point(3, 3);
			tvFileTree.Name = "tvFileTree";
			tvFileTree.Size = new Size(463, 901);
			tvFileTree.TabIndex = 1;
			tvFileTree.AfterSelect += tv_AfterSelect;
			// 
			// pnFileFilter
			// 
			pnFileFilter.Controls.Add(cbVanillaObjects);
			pnFileFilter.Controls.Add(tbFileFilter);
			pnFileFilter.Controls.Add(lblFilenameRegex);
			pnFileFilter.Dock = DockStyle.Top;
			pnFileFilter.Location = new Point(0, 0);
			pnFileFilter.Margin = new Padding(4);
			pnFileFilter.Name = "pnFileFilter";
			pnFileFilter.Padding = new Padding(4);
			pnFileFilter.Size = new Size(477, 74);
			pnFileFilter.TabIndex = 30;
			// 
			// cbVanillaObjects
			// 
			cbVanillaObjects.AutoSize = true;
			cbVanillaObjects.Dock = DockStyle.Top;
			cbVanillaObjects.Location = new Point(4, 50);
			cbVanillaObjects.Name = "cbVanillaObjects";
			cbVanillaObjects.Size = new Size(469, 19);
			cbVanillaObjects.TabIndex = 26;
			cbVanillaObjects.Text = "Vanilla Objects Only";
			cbVanillaObjects.UseVisualStyleBackColor = true;
			cbVanillaObjects.CheckedChanged += cbVanillaObjects_CheckedChanged;
			// 
			// menuStrip
			// 
			menuStrip.ImageScalingSize = new Size(32, 32);
			menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
			menuStrip.Location = new Point(4, 4);
			menuStrip.Name = "menuStrip";
			menuStrip.Size = new Size(1896, 24);
			menuStrip.TabIndex = 23;
			menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { objectDirectoriesToolStripMenuItem, dataDirectoriesToolStripMenuItem, recreateIndexToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// objectDirectoriesToolStripMenuItem
			// 
			objectDirectoriesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setObjectDirectoryToolStripMenuItem, toolStripSeparator1 });
			objectDirectoriesToolStripMenuItem.Name = "objectDirectoriesToolStripMenuItem";
			objectDirectoriesToolStripMenuItem.Size = new Size(176, 22);
			objectDirectoriesToolStripMenuItem.Text = "ObjData Directories";
			// 
			// setObjectDirectoryToolStripMenuItem
			// 
			setObjectDirectoryToolStripMenuItem.Name = "setObjectDirectoryToolStripMenuItem";
			setObjectDirectoryToolStripMenuItem.Size = new Size(123, 22);
			setObjectDirectoryToolStripMenuItem.Text = "Add New";
			setObjectDirectoryToolStripMenuItem.Click += setObjectDirectoryToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(120, 6);
			// 
			// dataDirectoriesToolStripMenuItem
			// 
			dataDirectoriesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setDataDirectoryToolStripMenuItem, toolStripSeparator2 });
			dataDirectoriesToolStripMenuItem.Name = "dataDirectoriesToolStripMenuItem";
			dataDirectoriesToolStripMenuItem.Size = new Size(176, 22);
			dataDirectoriesToolStripMenuItem.Text = "Data Directories";
			// 
			// setDataDirectoryToolStripMenuItem
			// 
			setDataDirectoryToolStripMenuItem.Name = "setDataDirectoryToolStripMenuItem";
			setDataDirectoryToolStripMenuItem.Size = new Size(123, 22);
			setDataDirectoryToolStripMenuItem.Text = "Add New";
			setDataDirectoryToolStripMenuItem.Click += setDataDirectoryToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new Size(120, 6);
			// 
			// recreateIndexToolStripMenuItem
			// 
			recreateIndexToolStripMenuItem.Name = "recreateIndexToolStripMenuItem";
			recreateIndexToolStripMenuItem.Size = new Size(176, 22);
			recreateIndexToolStripMenuItem.Text = "Recreate Index";
			recreateIndexToolStripMenuItem.Click += recreateIndexToolStripMenuItem_Click;
			// 
			// setPaletteToolStripMenuItem
			// 
			setPaletteToolStripMenuItem.Name = "setPaletteToolStripMenuItem";
			setPaletteToolStripMenuItem.Size = new Size(180, 22);
			setPaletteToolStripMenuItem.Text = "Load Palette Bitmap";
			// 
			// imgContextMenu
			// 
			imgContextMenu.ImageScalingSize = new Size(32, 32);
			imgContextMenu.Items.AddRange(new ToolStripItem[] { imgContextMenuSave, goToHeaderInDumpToolStripMenuItem });
			imgContextMenu.Name = "imgContextMenu";
			imgContextMenu.Size = new Size(155, 48);
			// 
			// imgContextMenuSave
			// 
			imgContextMenuSave.Name = "imgContextMenuSave";
			imgContextMenuSave.Size = new Size(154, 22);
			imgContextMenuSave.Text = "Export Image";
			imgContextMenuSave.Click += imgContextMenuSave_Click;
			// 
			// goToHeaderInDumpToolStripMenuItem
			// 
			goToHeaderInDumpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { headerToolStripMenuItem, pictureDataToolStripMenuItem });
			goToHeaderInDumpToolStripMenuItem.Name = "goToHeaderInDumpToolStripMenuItem";
			goToHeaderInDumpToolStripMenuItem.Size = new Size(154, 22);
			goToHeaderInDumpToolStripMenuItem.Text = "Select In Dump";
			// 
			// headerToolStripMenuItem
			// 
			headerToolStripMenuItem.Name = "headerToolStripMenuItem";
			headerToolStripMenuItem.Size = new Size(138, 22);
			headerToolStripMenuItem.Text = "Header";
			headerToolStripMenuItem.Click += headerToolStripMenuItem_Click;
			// 
			// pictureDataToolStripMenuItem
			// 
			pictureDataToolStripMenuItem.Name = "pictureDataToolStripMenuItem";
			pictureDataToolStripMenuItem.Size = new Size(138, 22);
			pictureDataToolStripMenuItem.Text = "Picture Data";
			pictureDataToolStripMenuItem.Click += pictureDataToolStripMenuItem_Click;
			// 
			// locoObjectBindingSource
			// 
			locoObjectBindingSource.DataSource = typeof(LocoObject);
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1904, 1041);
			Controls.Add(scTop);
			Controls.Add(menuStrip);
			Icon = (Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menuStrip;
			Name = "MainForm";
			Padding = new Padding(4);
			Text = "OpenLoco Object Editor";
			Load += MainForm_Load;
			scObjectAndLogs.Panel1.ResumeLayout(false);
			scObjectAndLogs.Panel1.PerformLayout();
			scObjectAndLogs.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scObjectAndLogs).EndInit();
			scObjectAndLogs.ResumeLayout(false);
			tcObjectOverview.ResumeLayout(false);
			tpObjectEditor.ResumeLayout(false);
			tcSubObjectView.ResumeLayout(false);
			tpObjectObject.ResumeLayout(false);
			tpObjectStringTable.ResumeLayout(false);
			tpObjectGraphicsTable.ResumeLayout(false);
			tpObjectGraphicsTable.PerformLayout();
			pnImagePageControls.ResumeLayout(false);
			pnImagePageControls.PerformLayout();
			tsImageTable.ResumeLayout(false);
			tsImageTable.PerformLayout();
			tpObjectPreview.ResumeLayout(false);
			gbBogieSprites.ResumeLayout(false);
			gbBogieSprites.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trbBogieSprites).EndInit();
			gbBodySprites.ResumeLayout(false);
			gbBodySprites.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trbBodySprites).EndInit();
			gbTilt.ResumeLayout(false);
			gbTilt.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trbTilt).EndInit();
			gbRotation.ResumeLayout(false);
			gbRotation.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trbRotate).EndInit();
			((System.ComponentModel.ISupportInitialize)pbVehicleBogiePreview).EndInit();
			((System.ComponentModel.ISupportInitialize)pbVehicleBodyPreview).EndInit();
			scHeaders.Panel1.ResumeLayout(false);
			scHeaders.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scHeaders).EndInit();
			scHeaders.ResumeLayout(false);
			tpObjectAnnotator.ResumeLayout(false);
			scAnnnotationDump.Panel1.ResumeLayout(false);
			scAnnnotationDump.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scAnnnotationDump).EndInit();
			scAnnnotationDump.ResumeLayout(false);
			tsObjectEditor.ResumeLayout(false);
			tsObjectEditor.PerformLayout();
			scTop.Panel1.ResumeLayout(false);
			scTop.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scTop).EndInit();
			scTop.ResumeLayout(false);
			tcFileSelector.ResumeLayout(false);
			tpCategory.ResumeLayout(false);
			tpFile.ResumeLayout(false);
			pnFileFilter.ResumeLayout(false);
			pnFileFilter.PerformLayout();
			menuStrip.ResumeLayout(false);
			menuStrip.PerformLayout();
			imgContextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)locoObjectBindingSource).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private ListBox lbLogs;
		private FolderBrowserDialog objectDirBrowser;
		private TextBox tbFileFilter;
		private Label lblFilenameRegex;
		private SaveFileDialog saveFileDialog1;
		private FlowLayoutPanel flpImageTable;
		private SplitContainer scTop;
		private SplitContainer scObjectAndLogs;
		private Panel pnFileFilter;
		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem objectDirectoriesToolStripMenuItem;
		private ToolStripMenuItem recreateIndexToolStripMenuItem;
		private ToolStripMenuItem setPaletteToolStripMenuItem;
		private ToolStripMenuItem dataDirectoriesToolStripMenuItem;
		private ContextMenuStrip imgContextMenu;
		private ToolStripMenuItem imgContextMenuSave;
		private Button btnPageNext;
		private Button btnPagePrevious;
		private CheckBox cbVanillaObjects;
		private TextBox tbCurrentPage;
		private TabControl tcObjectOverview;
		private TabPage tpObjectEditor;
		private TabPage tpObjectAnnotator;
		private TabControl tcFileSelector;
		private TabPage tpCategory;
		private TreeView tvObjType;
		private TabPage tpFile;
		private TreeView tvFileTree;
		private RichTextBox rtbDATDumpView;
		private TreeView tvDATDumpAnnotations;
		private ToolStripMenuItem goToHeaderInDumpToolStripMenuItem;
		private ToolStripMenuItem headerToolStripMenuItem;
		private ToolStripMenuItem pictureDataToolStripMenuItem;
		private SplitContainer scAnnnotationDump;
		private ToolStripMenuItem setObjectDirectoryToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem setDataDirectoryToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private TabControl tcSubObjectView;
		private TabPage tpObjectObject;
		private TabPage tpObjectStringTable;
		private TabPage tpObjectGraphicsTable;
		private TabPage tpObjectPreview;
		private BindingSource locoObjectBindingSource;
		private PropertyGrid pgS5Header;
		private PropertyGrid pgObjHeader;
		private PropertyGrid pgObject;
		private StringTableUserControl ucStringTable;
		private SplitContainer scHeaders;
		private ToolStrip tsObjectEditor;
		private ToolStripButton btnSave;
		private ToolStrip tsImageTable;
		private ToolStripButton tsbImportFromDirectory;
		private ToolStripButton tsbExportToDirectory;
		private ToolStripTextBox tstbImageScaling;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripLabel tslImageScaling;
		private Panel pnImagePageControls;
		private PictureBoxWithInterpolationMode pbVehicleBodyPreview;
		private TrackBar trbTilt;
		private TrackBar trbRotate;
		private Button btnRotateRight;
		private Button btnRotateLeft;
		private TextBox tbRotateValue;
		private TextBox tbTiltValue;
		private Button btnTiltUp;
		private Button btnTiltDown;
		private PictureBoxWithInterpolationMode pbVehicleBogiePreview;
		private GroupBox gbTilt;
		private GroupBox gbRotation;
		private GroupBox gbBogieSprites;
		private Button btnBogieSpritePrevious;
		private Button btnBogieSpriteNext;
		private TextBox tbCurrentBogieSprite;
		private GroupBox gbBodySprites;
		private Button btnBodySpritePrevious;
		private Button btnBodySpriteNext;
		private TextBox tbCurrentBodySprite;
		private TrackBar trbBogieSprites;
		private TrackBar trbBodySprites;
	}
}
