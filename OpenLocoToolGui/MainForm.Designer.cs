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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			lbLogs = new ListBox();
			pgObject = new PropertyGrid();
			objectDirBrowser = new FolderBrowserDialog();
			tbFileFilter = new TextBox();
			lblFilenameRegex = new Label();
			saveFileDialog1 = new SaveFileDialog();
			flpImageTable = new FlowLayoutPanel();
			scObjectAndLogs = new SplitContainer();
			ObjectTabViewControl = new TabControl();
			scObjectViewerTabPage = new TabPage();
			scObjectViewer = new SplitContainer();
			scImageViewer = new SplitContainer();
			btnPagePrevious = new Button();
			btnPageNext = new Button();
			ObjectDumpTab = new TabPage();
			tvDATDumpAnnotations = new TreeView();
			rtbDATDumpView = new RichTextBox();
			scTop = new SplitContainer();
			tcFileSelector = new TabControl();
			tabPage2 = new TabPage();
			tvObjType = new TreeView();
			tabPage1 = new TabPage();
			tvFileTree = new TreeView();
			pnFileFilter = new Panel();
			menuStrip = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			setObjectDirectoryToolStripMenuItem = new ToolStripMenuItem();
			setDataDirectoryToolStripMenuItem = new ToolStripMenuItem();
			setPaletteToolStripMenuItem = new ToolStripMenuItem();
			recreateIndexToolStripMenuItem = new ToolStripMenuItem();
			saveChangesToolStripMenuItem = new ToolStripMenuItem();
			fileToolStripMenuItem1 = new ToolStripMenuItem();
			imgContextMenu = new ContextMenuStrip(components);
			imgContextMenuSave = new ToolStripMenuItem();
			goToHeaderInDumpToolStripMenuItem = new ToolStripMenuItem();
			headerToolStripMenuItem = new ToolStripMenuItem();
			pictureDataToolStripMenuItem = new ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)scObjectAndLogs).BeginInit();
			scObjectAndLogs.Panel1.SuspendLayout();
			scObjectAndLogs.Panel2.SuspendLayout();
			scObjectAndLogs.SuspendLayout();
			ObjectTabViewControl.SuspendLayout();
			scObjectViewerTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scObjectViewer).BeginInit();
			scObjectViewer.Panel1.SuspendLayout();
			scObjectViewer.Panel2.SuspendLayout();
			scObjectViewer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scImageViewer).BeginInit();
			scImageViewer.Panel1.SuspendLayout();
			scImageViewer.Panel2.SuspendLayout();
			scImageViewer.SuspendLayout();
			ObjectDumpTab.SuspendLayout();
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
			SuspendLayout();
			// 
			// lbLogs
			// 
			lbLogs.Dock = DockStyle.Fill;
			lbLogs.FormattingEnabled = true;
			lbLogs.HorizontalScrollbar = true;
			lbLogs.ItemHeight = 15;
			lbLogs.Location = new Point(0, 0);
			lbLogs.Name = "lbLogs";
			lbLogs.SelectionMode = SelectionMode.None;
			lbLogs.Size = new Size(892, 129);
			lbLogs.TabIndex = 17;
			// 
			// pgObject
			// 
			pgObject.Dock = DockStyle.Fill;
			pgObject.HelpVisible = false;
			pgObject.Location = new Point(0, 0);
			pgObject.Name = "pgObject";
			pgObject.Size = new Size(290, 546);
			pgObject.TabIndex = 22;
			pgObject.ToolbarVisible = false;
			// 
			// tbFileFilter
			// 
			tbFileFilter.BorderStyle = BorderStyle.FixedSingle;
			tbFileFilter.Dock = DockStyle.Top;
			tbFileFilter.Location = new Point(4, 27);
			tbFileFilter.Margin = new Padding(4);
			tbFileFilter.Name = "tbFileFilter";
			tbFileFilter.Size = new Size(295, 23);
			tbFileFilter.TabIndex = 24;
			tbFileFilter.TextChanged += tbFileFilter_TextChanged;
			// 
			// lblFilenameRegex
			// 
			lblFilenameRegex.Dock = DockStyle.Top;
			lblFilenameRegex.Location = new Point(4, 4);
			lblFilenameRegex.Margin = new Padding(4);
			lblFilenameRegex.Name = "lblFilenameRegex";
			lblFilenameRegex.Size = new Size(295, 23);
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
			flpImageTable.Size = new Size(584, 517);
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
			scObjectAndLogs.Panel1.Controls.Add(ObjectTabViewControl);
			// 
			// scObjectAndLogs.Panel2
			// 
			scObjectAndLogs.Panel2.Controls.Add(lbLogs);
			scObjectAndLogs.Size = new Size(892, 713);
			scObjectAndLogs.SplitterDistance = 580;
			scObjectAndLogs.TabIndex = 31;
			// 
			// ObjectTabViewControl
			// 
			ObjectTabViewControl.Controls.Add(scObjectViewerTabPage);
			ObjectTabViewControl.Controls.Add(ObjectDumpTab);
			ObjectTabViewControl.Dock = DockStyle.Fill;
			ObjectTabViewControl.Location = new Point(0, 0);
			ObjectTabViewControl.Name = "ObjectTabViewControl";
			ObjectTabViewControl.SelectedIndex = 0;
			ObjectTabViewControl.Size = new Size(892, 580);
			ObjectTabViewControl.TabIndex = 34;
			// 
			// scObjectViewerTabPage
			// 
			scObjectViewerTabPage.Controls.Add(scObjectViewer);
			scObjectViewerTabPage.Location = new Point(4, 24);
			scObjectViewerTabPage.Name = "scObjectViewerTabPage";
			scObjectViewerTabPage.Padding = new Padding(3);
			scObjectViewerTabPage.Size = new Size(884, 552);
			scObjectViewerTabPage.TabIndex = 0;
			scObjectViewerTabPage.Text = "Object View";
			scObjectViewerTabPage.UseVisualStyleBackColor = true;
			// 
			// scObjectViewer
			// 
			scObjectViewer.Dock = DockStyle.Fill;
			scObjectViewer.Location = new Point(3, 3);
			scObjectViewer.Name = "scObjectViewer";
			// 
			// scObjectViewer.Panel1
			// 
			scObjectViewer.Panel1.Controls.Add(pgObject);
			// 
			// scObjectViewer.Panel2
			// 
			scObjectViewer.Panel2.Controls.Add(scImageViewer);
			scObjectViewer.Size = new Size(878, 546);
			scObjectViewer.SplitterDistance = 290;
			scObjectViewer.TabIndex = 32;
			// 
			// scImageViewer
			// 
			scImageViewer.Dock = DockStyle.Fill;
			scImageViewer.IsSplitterFixed = true;
			scImageViewer.Location = new Point(0, 0);
			scImageViewer.Name = "scImageViewer";
			scImageViewer.Orientation = Orientation.Horizontal;
			// 
			// scImageViewer.Panel1
			// 
			scImageViewer.Panel1.Controls.Add(flpImageTable);
			// 
			// scImageViewer.Panel2
			// 
			scImageViewer.Panel2.Controls.Add(btnPagePrevious);
			scImageViewer.Panel2.Controls.Add(btnPageNext);
			scImageViewer.Size = new Size(584, 546);
			scImageViewer.SplitterDistance = 517;
			scImageViewer.TabIndex = 32;
			// 
			// btnPagePrevious
			// 
			btnPagePrevious.Dock = DockStyle.Left;
			btnPagePrevious.Location = new Point(0, 0);
			btnPagePrevious.Name = "btnPagePrevious";
			btnPagePrevious.Size = new Size(294, 25);
			btnPagePrevious.TabIndex = 0;
			btnPagePrevious.Text = "Previous Page";
			btnPagePrevious.UseVisualStyleBackColor = true;
			btnPagePrevious.Click += btnPagePrevious_Click;
			// 
			// btnPageNext
			// 
			btnPageNext.Dock = DockStyle.Right;
			btnPageNext.Location = new Point(290, 0);
			btnPageNext.Name = "btnPageNext";
			btnPageNext.Size = new Size(294, 25);
			btnPageNext.TabIndex = 31;
			btnPageNext.Text = "Next Page";
			btnPageNext.UseVisualStyleBackColor = true;
			btnPageNext.Click += btnPageNext_Click;
			// 
			// ObjectDumpTab
			// 
			ObjectDumpTab.Controls.Add(tvDATDumpAnnotations);
			ObjectDumpTab.Controls.Add(rtbDATDumpView);
			ObjectDumpTab.Location = new Point(4, 24);
			ObjectDumpTab.Name = "ObjectDumpTab";
			ObjectDumpTab.Padding = new Padding(3);
			ObjectDumpTab.Size = new Size(884, 552);
			ObjectDumpTab.TabIndex = 1;
			ObjectDumpTab.Text = "Object Data";
			ObjectDumpTab.UseVisualStyleBackColor = true;
			// 
			// tvDATDumpAnnotations
			// 
			tvDATDumpAnnotations.Dock = DockStyle.Left;
			tvDATDumpAnnotations.Location = new Point(3, 3);
			tvDATDumpAnnotations.Name = "tvDATDumpAnnotations";
			tvDATDumpAnnotations.Size = new Size(200, 546);
			tvDATDumpAnnotations.TabIndex = 1;
			tvDATDumpAnnotations.AfterSelect += dataDumpAnnotations_AfterSelect;
			// 
			// rtbDATDumpView
			// 
			rtbDATDumpView.Dock = DockStyle.Right;
			rtbDATDumpView.Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point);
			rtbDATDumpView.HideSelection = false;
			rtbDATDumpView.Location = new Point(209, 3);
			rtbDATDumpView.Name = "rtbDATDumpView";
			rtbDATDumpView.ReadOnly = true;
			rtbDATDumpView.ShowSelectionMargin = true;
			rtbDATDumpView.Size = new Size(672, 546);
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
			scTop.Size = new Size(1199, 713);
			scTop.SplitterDistance = 303;
			scTop.TabIndex = 33;
			// 
			// tcFileSelector
			// 
			tcFileSelector.Controls.Add(tabPage2);
			tcFileSelector.Controls.Add(tabPage1);
			tcFileSelector.Dock = DockStyle.Fill;
			tcFileSelector.Location = new Point(0, 57);
			tcFileSelector.Name = "tcFileSelector";
			tcFileSelector.SelectedIndex = 0;
			tcFileSelector.Size = new Size(303, 656);
			tcFileSelector.TabIndex = 29;
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add(tvObjType);
			tabPage2.Location = new Point(4, 24);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(3);
			tabPage2.Size = new Size(295, 628);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Category";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// tvObjType
			// 
			tvObjType.Dock = DockStyle.Fill;
			tvObjType.Location = new Point(3, 3);
			tvObjType.Name = "tvObjType";
			tvObjType.Size = new Size(289, 622);
			tvObjType.TabIndex = 26;
			tvObjType.AfterSelect += tv_AfterSelect;
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add(tvFileTree);
			tabPage1.Location = new Point(4, 24);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(3);
			tabPage1.Size = new Size(295, 628);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "File";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// tvFileTree
			// 
			tvFileTree.Dock = DockStyle.Fill;
			tvFileTree.Location = new Point(3, 3);
			tvFileTree.Name = "tvFileTree";
			tvFileTree.Size = new Size(289, 622);
			tvFileTree.TabIndex = 1;
			tvFileTree.AfterSelect += tv_AfterSelect;
			// 
			// pnFileFilter
			// 
			pnFileFilter.Controls.Add(tbFileFilter);
			pnFileFilter.Controls.Add(lblFilenameRegex);
			pnFileFilter.Dock = DockStyle.Top;
			pnFileFilter.Location = new Point(0, 0);
			pnFileFilter.Margin = new Padding(4);
			pnFileFilter.Name = "pnFileFilter";
			pnFileFilter.Padding = new Padding(4);
			pnFileFilter.Size = new Size(303, 57);
			pnFileFilter.TabIndex = 30;
			// 
			// menuStrip
			// 
			menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
			menuStrip.Location = new Point(4, 4);
			menuStrip.Name = "menuStrip";
			menuStrip.Size = new Size(1199, 24);
			menuStrip.TabIndex = 23;
			menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setObjectDirectoryToolStripMenuItem, setDataDirectoryToolStripMenuItem, setPaletteToolStripMenuItem, recreateIndexToolStripMenuItem, saveChangesToolStripMenuItem, fileToolStripMenuItem1 });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// setObjectDirectoryToolStripMenuItem
			// 
			setObjectDirectoryToolStripMenuItem.Name = "setObjectDirectoryToolStripMenuItem";
			setObjectDirectoryToolStripMenuItem.Size = new Size(217, 22);
			setObjectDirectoryToolStripMenuItem.Text = "Set ObjData Directory";
			setObjectDirectoryToolStripMenuItem.Click += setObjectDirectoryToolStripMenuItem_Click;
			// 
			// setDataDirectoryToolStripMenuItem
			// 
			setDataDirectoryToolStripMenuItem.Name = "setDataDirectoryToolStripMenuItem";
			setDataDirectoryToolStripMenuItem.Size = new Size(217, 22);
			setDataDirectoryToolStripMenuItem.Text = "Set Data Directory";
			setDataDirectoryToolStripMenuItem.Click += setDataDirectoryToolStripMenuItem_Click;
			// 
			// setPaletteToolStripMenuItem
			// 
			setPaletteToolStripMenuItem.Name = "setPaletteToolStripMenuItem";
			setPaletteToolStripMenuItem.Size = new Size(217, 22);
			setPaletteToolStripMenuItem.Text = "Load Palette Bitmap";
			setPaletteToolStripMenuItem.Click += setPaletteToolStripMenuItem_Click;
			// 
			// recreateIndexToolStripMenuItem
			// 
			recreateIndexToolStripMenuItem.Name = "recreateIndexToolStripMenuItem";
			recreateIndexToolStripMenuItem.Size = new Size(217, 22);
			recreateIndexToolStripMenuItem.Text = "Recreate Index";
			recreateIndexToolStripMenuItem.Click += recreateIndexToolStripMenuItem_Click;
			// 
			// saveChangesToolStripMenuItem
			// 
			saveChangesToolStripMenuItem.Name = "saveChangesToolStripMenuItem";
			saveChangesToolStripMenuItem.Size = new Size(217, 22);
			saveChangesToolStripMenuItem.Text = "Save Object Changes (WIP)";
			saveChangesToolStripMenuItem.Click += saveChangesToolStripMenuItem_Click;
			// 
			// fileToolStripMenuItem1
			// 
			fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
			fileToolStripMenuItem1.Size = new Size(217, 22);
			fileToolStripMenuItem1.Text = "File";
			// 
			// imgContextMenu
			// 
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
			headerToolStripMenuItem.Size = new Size(180, 22);
			headerToolStripMenuItem.Text = "Header";
			headerToolStripMenuItem.Click += headerToolStripMenuItem_Click;
			// 
			// pictureDataToolStripMenuItem
			// 
			pictureDataToolStripMenuItem.Name = "pictureDataToolStripMenuItem";
			pictureDataToolStripMenuItem.Size = new Size(180, 22);
			pictureDataToolStripMenuItem.Text = "Picture Data";
			pictureDataToolStripMenuItem.Click += pictureDataToolStripMenuItem_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1207, 745);
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
			ObjectTabViewControl.ResumeLayout(false);
			scObjectViewerTabPage.ResumeLayout(false);
			scObjectViewer.Panel1.ResumeLayout(false);
			scObjectViewer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scObjectViewer).EndInit();
			scObjectViewer.ResumeLayout(false);
			scImageViewer.Panel1.ResumeLayout(false);
			scImageViewer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scImageViewer).EndInit();
			scImageViewer.ResumeLayout(false);
			ObjectDumpTab.ResumeLayout(false);
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
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private ListBox lbLogs;
		private PropertyGrid pgObject;
		private FolderBrowserDialog objectDirBrowser;
		private TextBox tbFileFilter;
		private Label lblFilenameRegex;
		private SaveFileDialog saveFileDialog1;
		private FlowLayoutPanel flpImageTable;
		private SplitContainer scTop;
		private SplitContainer scObjectAndLogs;
		private SplitContainer scObjectViewer;
		private Panel pnFileFilter;
		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem setObjectDirectoryToolStripMenuItem;
		private ToolStripMenuItem recreateIndexToolStripMenuItem;
		private ToolStripMenuItem saveChangesToolStripMenuItem;
		private ToolStripMenuItem setPaletteToolStripMenuItem;
		private ToolStripMenuItem setDataDirectoryToolStripMenuItem;
		private ToolStripMenuItem fileToolStripMenuItem1;
		private ContextMenuStrip imgContextMenu;
		private ToolStripMenuItem imgContextMenuSave;
		private SplitContainer scImageViewer;
		private Button btnPageNext;
		private Button btnPagePrevious;
		private TabControl ObjectTabViewControl;
		private TabPage scObjectViewerTabPage;
		private TabPage ObjectDumpTab;
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
	}
}