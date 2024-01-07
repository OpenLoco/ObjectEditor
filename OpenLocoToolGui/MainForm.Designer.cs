namespace OpenLocoToolGui
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
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			lbLogs = new ListBox();
			objectDirBrowser = new FolderBrowserDialog();
			tbFileFilter = new TextBox();
			lblFilenameRegex = new Label();
			saveFileDialog1 = new SaveFileDialog();
			flpImageTable = new FlowLayoutPanel();
			scObjectAndLogs = new SplitContainer();
			pnObjectEditor = new Panel();
			toolStrip1 = new ToolStrip();
			toolStripButton1 = new ToolStripButton();
			tcObjectOverview = new TabControl();
			tpObjectView = new TabPage();
			tcSubObjectView = new TabControl();
			tpObjectObject = new TabPage();
			pgObject = new PropertyGrid();
			tpObjectStringTable = new TabPage();
			ucStringTable = new StringTableUserControl();
			tpObjectGraphicsTable = new TabPage();
			scImageViewer = new SplitContainer();
			tbCurrentPage = new TextBox();
			btnPagePrevious = new Button();
			btnPageNext = new Button();
			scHeaders = new SplitContainer();
			pgS5Header = new PropertyGrid();
			pgObjHeader = new PropertyGrid();
			tpObjectDump = new TabPage();
			scAnnnotationDump = new SplitContainer();
			tvDATDumpAnnotations = new TreeView();
			rtbDATDumpView = new RichTextBox();
			scTop = new SplitContainer();
			tcFileSelector = new TabControl();
			tabPage2 = new TabPage();
			tvObjType = new TreeView();
			tabPage1 = new TabPage();
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
			setPaletteToolStripMenuItem = new ToolStripMenuItem();
			recreateIndexToolStripMenuItem = new ToolStripMenuItem();
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
			pnObjectEditor.SuspendLayout();
			toolStrip1.SuspendLayout();
			tcObjectOverview.SuspendLayout();
			tpObjectView.SuspendLayout();
			tcSubObjectView.SuspendLayout();
			tpObjectObject.SuspendLayout();
			tpObjectStringTable.SuspendLayout();
			tpObjectGraphicsTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scImageViewer).BeginInit();
			scImageViewer.Panel1.SuspendLayout();
			scImageViewer.Panel2.SuspendLayout();
			scImageViewer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scHeaders).BeginInit();
			scHeaders.Panel1.SuspendLayout();
			scHeaders.Panel2.SuspendLayout();
			scHeaders.SuspendLayout();
			tpObjectDump.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scAnnnotationDump).BeginInit();
			scAnnnotationDump.Panel1.SuspendLayout();
			scAnnnotationDump.Panel2.SuspendLayout();
			scAnnnotationDump.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scTop).BeginInit();
			scTop.Panel1.SuspendLayout();
			scTop.Panel2.SuspendLayout();
			scTop.SuspendLayout();
			tcFileSelector.SuspendLayout();
			tabPage2.SuspendLayout();
			tabPage1.SuspendLayout();
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
			flpImageTable.Location = new Point(0, 0);
			flpImageTable.Name = "flpImageTable";
			flpImageTable.Size = new Size(1381, 584);
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
			scObjectAndLogs.Panel1.Controls.Add(pnObjectEditor);
			// 
			// scObjectAndLogs.Panel2
			// 
			scObjectAndLogs.Panel2.Controls.Add(lbLogs);
			scObjectAndLogs.Size = new Size(1415, 1009);
			scObjectAndLogs.SplitterDistance = 819;
			scObjectAndLogs.TabIndex = 31;
			// 
			// pnObjectEditor
			// 
			pnObjectEditor.BackColor = SystemColors.ControlDark;
			pnObjectEditor.Controls.Add(toolStrip1);
			pnObjectEditor.Controls.Add(tcObjectOverview);
			pnObjectEditor.Dock = DockStyle.Fill;
			pnObjectEditor.Location = new Point(0, 0);
			pnObjectEditor.Name = "pnObjectEditor";
			pnObjectEditor.Padding = new Padding(4);
			pnObjectEditor.Size = new Size(1415, 819);
			pnObjectEditor.TabIndex = 35;
			// 
			// toolStrip1
			// 
			toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });
			toolStrip1.Location = new Point(4, 4);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new Size(1407, 25);
			toolStrip1.TabIndex = 35;
			toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton1
			// 
			toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
			toolStripButton1.ImageTransparentColor = Color.Magenta;
			toolStripButton1.Name = "toolStripButton1";
			toolStripButton1.Size = new Size(84, 22);
			toolStripButton1.Text = "Save Changes";
			toolStripButton1.Click += toolStripButton1_Click;
			// 
			// tcObjectOverview
			// 
			tcObjectOverview.Controls.Add(tpObjectView);
			tcObjectOverview.Controls.Add(tpObjectDump);
			tcObjectOverview.Dock = DockStyle.Fill;
			tcObjectOverview.Location = new Point(4, 4);
			tcObjectOverview.Name = "tcObjectOverview";
			tcObjectOverview.SelectedIndex = 0;
			tcObjectOverview.Size = new Size(1407, 811);
			tcObjectOverview.TabIndex = 34;
			// 
			// tpObjectView
			// 
			tpObjectView.Controls.Add(tcSubObjectView);
			tpObjectView.Controls.Add(scHeaders);
			tpObjectView.Location = new Point(4, 24);
			tpObjectView.Name = "tpObjectView";
			tpObjectView.Padding = new Padding(3);
			tpObjectView.Size = new Size(1399, 783);
			tpObjectView.TabIndex = 0;
			tpObjectView.Text = "Object View";
			tpObjectView.UseVisualStyleBackColor = true;
			// 
			// tcSubObjectView
			// 
			tcSubObjectView.Controls.Add(tpObjectObject);
			tcSubObjectView.Controls.Add(tpObjectStringTable);
			tcSubObjectView.Controls.Add(tpObjectGraphicsTable);
			tcSubObjectView.Dock = DockStyle.Fill;
			tcSubObjectView.Location = new Point(3, 123);
			tcSubObjectView.Margin = new Padding(2, 1, 2, 1);
			tcSubObjectView.Name = "tcSubObjectView";
			tcSubObjectView.SelectedIndex = 0;
			tcSubObjectView.Size = new Size(1393, 657);
			tcSubObjectView.TabIndex = 33;
			// 
			// tpObjectObject
			// 
			tpObjectObject.Controls.Add(pgObject);
			tpObjectObject.Location = new Point(4, 24);
			tpObjectObject.Margin = new Padding(2, 1, 2, 1);
			tpObjectObject.Name = "tpObjectObject";
			tpObjectObject.Padding = new Padding(2, 1, 2, 1);
			tpObjectObject.Size = new Size(1385, 629);
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
			pgObject.Size = new Size(1381, 627);
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
			tpObjectStringTable.Size = new Size(1385, 629);
			tpObjectStringTable.TabIndex = 3;
			tpObjectStringTable.Text = "Strings";
			tpObjectStringTable.UseVisualStyleBackColor = true;
			// 
			// ucStringTable
			// 
			ucStringTable.Dock = DockStyle.Fill;
			ucStringTable.Location = new Point(2, 1);
			ucStringTable.Margin = new Padding(1, 0, 1, 0);
			ucStringTable.Name = "ucStringTable";
			ucStringTable.Size = new Size(1381, 627);
			ucStringTable.TabIndex = 0;
			// 
			// tpObjectGraphicsTable
			// 
			tpObjectGraphicsTable.Controls.Add(scImageViewer);
			tpObjectGraphicsTable.Location = new Point(4, 24);
			tpObjectGraphicsTable.Margin = new Padding(2, 1, 2, 1);
			tpObjectGraphicsTable.Name = "tpObjectGraphicsTable";
			tpObjectGraphicsTable.Padding = new Padding(2, 1, 2, 1);
			tpObjectGraphicsTable.Size = new Size(1385, 629);
			tpObjectGraphicsTable.TabIndex = 4;
			tpObjectGraphicsTable.Text = "Graphics";
			tpObjectGraphicsTable.UseVisualStyleBackColor = true;
			// 
			// scImageViewer
			// 
			scImageViewer.Dock = DockStyle.Fill;
			scImageViewer.IsSplitterFixed = true;
			scImageViewer.Location = new Point(2, 1);
			scImageViewer.Name = "scImageViewer";
			scImageViewer.Orientation = Orientation.Horizontal;
			// 
			// scImageViewer.Panel1
			// 
			scImageViewer.Panel1.Controls.Add(flpImageTable);
			// 
			// scImageViewer.Panel2
			// 
			scImageViewer.Panel2.Controls.Add(tbCurrentPage);
			scImageViewer.Panel2.Controls.Add(btnPagePrevious);
			scImageViewer.Panel2.Controls.Add(btnPageNext);
			scImageViewer.Size = new Size(1381, 627);
			scImageViewer.SplitterDistance = 584;
			scImageViewer.TabIndex = 32;
			// 
			// tbCurrentPage
			// 
			tbCurrentPage.BorderStyle = BorderStyle.FixedSingle;
			tbCurrentPage.Dock = DockStyle.Bottom;
			tbCurrentPage.Enabled = false;
			tbCurrentPage.Location = new Point(253, 16);
			tbCurrentPage.Name = "tbCurrentPage";
			tbCurrentPage.Size = new Size(900, 23);
			tbCurrentPage.TabIndex = 33;
			tbCurrentPage.TextAlign = HorizontalAlignment.Center;
			// 
			// btnPagePrevious
			// 
			btnPagePrevious.Dock = DockStyle.Left;
			btnPagePrevious.Location = new Point(0, 0);
			btnPagePrevious.Name = "btnPagePrevious";
			btnPagePrevious.Size = new Size(253, 39);
			btnPagePrevious.TabIndex = 0;
			btnPagePrevious.Text = "Previous Page";
			btnPagePrevious.UseVisualStyleBackColor = true;
			btnPagePrevious.Click += btnPagePrevious_Click;
			// 
			// btnPageNext
			// 
			btnPageNext.Dock = DockStyle.Right;
			btnPageNext.Location = new Point(1153, 0);
			btnPageNext.Name = "btnPageNext";
			btnPageNext.Size = new Size(228, 39);
			btnPageNext.TabIndex = 31;
			btnPageNext.Text = "Next Page";
			btnPageNext.UseVisualStyleBackColor = true;
			btnPageNext.Click += btnPageNext_Click;
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
			scHeaders.Size = new Size(1393, 120);
			scHeaders.SplitterDistance = 695;
			scHeaders.TabIndex = 34;
			// 
			// pgS5Header
			// 
			pgS5Header.Dock = DockStyle.Fill;
			pgS5Header.HelpVisible = false;
			pgS5Header.Location = new Point(0, 0);
			pgS5Header.Name = "pgS5Header";
			pgS5Header.Size = new Size(695, 120);
			pgS5Header.TabIndex = 22;
			pgS5Header.ToolbarVisible = false;
			// 
			// pgObjHeader
			// 
			pgObjHeader.Dock = DockStyle.Fill;
			pgObjHeader.HelpVisible = false;
			pgObjHeader.Location = new Point(0, 0);
			pgObjHeader.Name = "pgObjHeader";
			pgObjHeader.Size = new Size(694, 120);
			pgObjHeader.TabIndex = 23;
			pgObjHeader.ToolbarVisible = false;
			// 
			// tpObjectDump
			// 
			tpObjectDump.Controls.Add(scAnnnotationDump);
			tpObjectDump.Location = new Point(4, 24);
			tpObjectDump.Name = "tpObjectDump";
			tpObjectDump.Padding = new Padding(3);
			tpObjectDump.Size = new Size(1399, 783);
			tpObjectDump.TabIndex = 1;
			tpObjectDump.Text = "Object Dump";
			tpObjectDump.UseVisualStyleBackColor = true;
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
			scAnnnotationDump.Size = new Size(1393, 777);
			scAnnnotationDump.SplitterDistance = 479;
			scAnnnotationDump.TabIndex = 2;
			// 
			// tvDATDumpAnnotations
			// 
			tvDATDumpAnnotations.Dock = DockStyle.Fill;
			tvDATDumpAnnotations.Location = new Point(0, 0);
			tvDATDumpAnnotations.Name = "tvDATDumpAnnotations";
			tvDATDumpAnnotations.Size = new Size(479, 777);
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
			rtbDATDumpView.Size = new Size(910, 777);
			rtbDATDumpView.TabIndex = 0;
			rtbDATDumpView.Text = "";
			rtbDATDumpView.WordWrap = false;
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
			tcFileSelector.Controls.Add(tabPage2);
			tcFileSelector.Controls.Add(tabPage1);
			tcFileSelector.Dock = DockStyle.Fill;
			tcFileSelector.Location = new Point(0, 74);
			tcFileSelector.Name = "tcFileSelector";
			tcFileSelector.SelectedIndex = 0;
			tcFileSelector.Size = new Size(477, 935);
			tcFileSelector.TabIndex = 29;
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add(tvObjType);
			tabPage2.Location = new Point(4, 24);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(3);
			tabPage2.Size = new Size(469, 907);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Category";
			tabPage2.UseVisualStyleBackColor = true;
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
			// tabPage1
			// 
			tabPage1.Controls.Add(tvFileTree);
			tabPage1.Location = new Point(4, 24);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(3);
			tabPage1.Size = new Size(469, 907);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "File";
			tabPage1.UseVisualStyleBackColor = true;
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
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { objectDirectoriesToolStripMenuItem, dataDirectoriesToolStripMenuItem, setPaletteToolStripMenuItem, recreateIndexToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// objectDirectoriesToolStripMenuItem
			// 
			objectDirectoriesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setObjectDirectoryToolStripMenuItem, toolStripSeparator1 });
			objectDirectoriesToolStripMenuItem.Name = "objectDirectoriesToolStripMenuItem";
			objectDirectoriesToolStripMenuItem.Size = new Size(180, 22);
			objectDirectoriesToolStripMenuItem.Text = "ObjData Directories";
			// 
			// setObjectDirectoryToolStripMenuItem
			// 
			setObjectDirectoryToolStripMenuItem.Name = "setObjectDirectoryToolStripMenuItem";
			setObjectDirectoryToolStripMenuItem.Size = new Size(180, 22);
			setObjectDirectoryToolStripMenuItem.Text = "Add New";
			setObjectDirectoryToolStripMenuItem.Click += setObjectDirectoryToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(177, 6);
			// 
			// dataDirectoriesToolStripMenuItem
			// 
			dataDirectoriesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setDataDirectoryToolStripMenuItem, toolStripSeparator2 });
			dataDirectoriesToolStripMenuItem.Name = "dataDirectoriesToolStripMenuItem";
			dataDirectoriesToolStripMenuItem.Size = new Size(180, 22);
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
			// setPaletteToolStripMenuItem
			// 
			setPaletteToolStripMenuItem.Name = "setPaletteToolStripMenuItem";
			setPaletteToolStripMenuItem.Size = new Size(180, 22);
			setPaletteToolStripMenuItem.Text = "Load Palette Bitmap";
			setPaletteToolStripMenuItem.Click += setPaletteToolStripMenuItem_Click;
			// 
			// recreateIndexToolStripMenuItem
			// 
			recreateIndexToolStripMenuItem.Name = "recreateIndexToolStripMenuItem";
			recreateIndexToolStripMenuItem.Size = new Size(180, 22);
			recreateIndexToolStripMenuItem.Text = "Recreate Index";
			recreateIndexToolStripMenuItem.Click += recreateIndexToolStripMenuItem_Click;
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
			locoObjectBindingSource.DataSource = typeof(OpenLocoTool.DatFileParsing.LocoObject);
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
			Text = "OpenLocoTool";
			Load += MainForm_Load;
			scObjectAndLogs.Panel1.ResumeLayout(false);
			scObjectAndLogs.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scObjectAndLogs).EndInit();
			scObjectAndLogs.ResumeLayout(false);
			pnObjectEditor.ResumeLayout(false);
			pnObjectEditor.PerformLayout();
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			tcObjectOverview.ResumeLayout(false);
			tpObjectView.ResumeLayout(false);
			tcSubObjectView.ResumeLayout(false);
			tpObjectObject.ResumeLayout(false);
			tpObjectStringTable.ResumeLayout(false);
			tpObjectGraphicsTable.ResumeLayout(false);
			scImageViewer.Panel1.ResumeLayout(false);
			scImageViewer.Panel2.ResumeLayout(false);
			scImageViewer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)scImageViewer).EndInit();
			scImageViewer.ResumeLayout(false);
			scHeaders.Panel1.ResumeLayout(false);
			scHeaders.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scHeaders).EndInit();
			scHeaders.ResumeLayout(false);
			tpObjectDump.ResumeLayout(false);
			scAnnnotationDump.Panel1.ResumeLayout(false);
			scAnnnotationDump.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scAnnnotationDump).EndInit();
			scAnnnotationDump.ResumeLayout(false);
			scTop.Panel1.ResumeLayout(false);
			scTop.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scTop).EndInit();
			scTop.ResumeLayout(false);
			tcFileSelector.ResumeLayout(false);
			tabPage2.ResumeLayout(false);
			tabPage1.ResumeLayout(false);
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
		private SplitContainer scImageViewer;
		private Button btnPageNext;
		private Button btnPagePrevious;
		private CheckBox cbVanillaObjects;
		private TextBox tbCurrentPage;
		private TabControl tcObjectOverview;
		private TabPage tpObjectView;
		private TabPage tpObjectDump;
		private TabControl tcFileSelector;
		private TabPage tabPage2;
		private TreeView tvObjType;
		private TabPage tabPage1;
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
		private BindingSource locoObjectBindingSource;
		private PropertyGrid pgS5Header;
		private PropertyGrid pgObjHeader;
		private PropertyGrid pgObject;
		private StringTableUserControl ucStringTable;
		private SplitContainer scHeaders;
		private Panel pnObjectEditor;
		private ToolStrip toolStrip1;
		private ToolStripButton toolStripButton1;
	}
}