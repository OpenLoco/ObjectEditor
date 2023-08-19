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
			tvFileTree = new TreeView();
			lbLogs = new ListBox();
			btnSaveChanges = new Button();
			pgObject = new PropertyGrid();
			btnSetDirectory = new Button();
			objectDirBrowser = new FolderBrowserDialog();
			tbFileFilter = new TextBox();
			lblFilenameRegex = new Label();
			saveFileDialog1 = new SaveFileDialog();
			tvObjType = new TreeView();
			objectSelector = new TabControl();
			tabPage2 = new TabPage();
			tabPage1 = new TabPage();
			flpImageTable = new FlowLayoutPanel();
			splitContainer1 = new SplitContainer();
			splitContainer2 = new SplitContainer();
			splitContainer3 = new SplitContainer();
			objectSelector.SuspendLayout();
			tabPage2.SuspendLayout();
			tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
			splitContainer2.Panel1.SuspendLayout();
			splitContainer2.Panel2.SuspendLayout();
			splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
			splitContainer3.Panel1.SuspendLayout();
			splitContainer3.Panel2.SuspendLayout();
			splitContainer3.SuspendLayout();
			SuspendLayout();
			// 
			// tvFileTree
			// 
			tvFileTree.Dock = DockStyle.Fill;
			tvFileTree.Location = new Point(3, 3);
			tvFileTree.Name = "tvFileTree";
			tvFileTree.Size = new Size(286, 597);
			tvFileTree.TabIndex = 1;
			tvFileTree.AfterSelect += tv_AfterSelect;
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
			lbLogs.Size = new Size(897, 134);
			lbLogs.TabIndex = 17;
			// 
			// btnSaveChanges
			// 
			btnSaveChanges.Dock = DockStyle.Top;
			btnSaveChanges.Enabled = false;
			btnSaveChanges.Location = new Point(0, 26);
			btnSaveChanges.Name = "btnSaveChanges";
			btnSaveChanges.Size = new Size(306, 26);
			btnSaveChanges.TabIndex = 18;
			btnSaveChanges.Text = "Save Changes";
			btnSaveChanges.UseVisualStyleBackColor = true;
			btnSaveChanges.Visible = false;
			btnSaveChanges.Click += btnSaveChanges_Click;
			// 
			// pgObject
			// 
			pgObject.Dock = DockStyle.Fill;
			pgObject.HelpVisible = false;
			pgObject.Location = new Point(0, 0);
			pgObject.Name = "pgObject";
			pgObject.Size = new Size(297, 607);
			pgObject.TabIndex = 22;
			pgObject.ToolbarVisible = false;
			// 
			// btnSetDirectory
			// 
			btnSetDirectory.Dock = DockStyle.Top;
			btnSetDirectory.Location = new Point(0, 0);
			btnSetDirectory.Name = "btnSetDirectory";
			btnSetDirectory.Size = new Size(306, 26);
			btnSetDirectory.TabIndex = 23;
			btnSetDirectory.Text = "Set ObjData Directory";
			btnSetDirectory.UseVisualStyleBackColor = true;
			btnSetDirectory.Click += btnSetDirectory_Click;
			// 
			// tbFileFilter
			// 
			tbFileFilter.BorderStyle = BorderStyle.FixedSingle;
			tbFileFilter.Location = new Point(109, 61);
			tbFileFilter.Name = "tbFileFilter";
			tbFileFilter.Size = new Size(278, 23);
			tbFileFilter.TabIndex = 24;
			tbFileFilter.TextChanged += tbFileFilter_TextChanged;
			// 
			// lblFilenameRegex
			// 
			lblFilenameRegex.BorderStyle = BorderStyle.FixedSingle;
			lblFilenameRegex.Location = new Point(3, 61);
			lblFilenameRegex.Name = "lblFilenameRegex";
			lblFilenameRegex.Size = new Size(100, 23);
			lblFilenameRegex.TabIndex = 25;
			lblFilenameRegex.Text = "Filename Filter";
			lblFilenameRegex.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// tvObjType
			// 
			tvObjType.Dock = DockStyle.Fill;
			tvObjType.Location = new Point(3, 3);
			tvObjType.Name = "tvObjType";
			tvObjType.Size = new Size(292, 597);
			tvObjType.TabIndex = 26;
			tvObjType.AfterSelect += tv_AfterSelect;
			// 
			// objectSelector
			// 
			objectSelector.Controls.Add(tabPage2);
			objectSelector.Controls.Add(tabPage1);
			objectSelector.Dock = DockStyle.Bottom;
			objectSelector.Location = new Point(0, 114);
			objectSelector.Name = "objectSelector";
			objectSelector.SelectedIndex = 0;
			objectSelector.Size = new Size(306, 631);
			objectSelector.TabIndex = 29;
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add(tvObjType);
			tabPage2.Location = new Point(4, 24);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(3);
			tabPage2.Size = new Size(298, 603);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Category";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add(tvFileTree);
			tabPage1.Location = new Point(4, 24);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(3);
			tabPage1.Size = new Size(292, 603);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "File";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// flpImageTable
			// 
			flpImageTable.BorderStyle = BorderStyle.FixedSingle;
			flpImageTable.Dock = DockStyle.Fill;
			flpImageTable.FlowDirection = FlowDirection.TopDown;
			flpImageTable.Location = new Point(0, 0);
			flpImageTable.Name = "flpImageTable";
			flpImageTable.Size = new Size(596, 607);
			flpImageTable.TabIndex = 30;
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = DockStyle.Fill;
			splitContainer1.Location = new Point(0, 0);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Orientation = Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(lbLogs);
			splitContainer1.Size = new Size(897, 745);
			splitContainer1.SplitterDistance = 607;
			splitContainer1.TabIndex = 31;
			// 
			// splitContainer2
			// 
			splitContainer2.Dock = DockStyle.Fill;
			splitContainer2.Location = new Point(0, 0);
			splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			splitContainer2.Panel1.Controls.Add(pgObject);
			// 
			// splitContainer2.Panel2
			// 
			splitContainer2.Panel2.Controls.Add(flpImageTable);
			splitContainer2.Size = new Size(897, 607);
			splitContainer2.SplitterDistance = 297;
			splitContainer2.TabIndex = 32;
			// 
			// splitContainer3
			// 
			splitContainer3.Dock = DockStyle.Fill;
			splitContainer3.Location = new Point(0, 0);
			splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			splitContainer3.Panel1.Controls.Add(btnSaveChanges);
			splitContainer3.Panel1.Controls.Add(btnSetDirectory);
			splitContainer3.Panel1.Controls.Add(objectSelector);
			splitContainer3.Panel1.Controls.Add(tbFileFilter);
			splitContainer3.Panel1.Controls.Add(lblFilenameRegex);
			// 
			// splitContainer3.Panel2
			// 
			splitContainer3.Panel2.Controls.Add(splitContainer1);
			splitContainer3.Size = new Size(1207, 745);
			splitContainer3.SplitterDistance = 306;
			splitContainer3.TabIndex = 33;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1207, 745);
			Controls.Add(splitContainer3);
			Name = "MainForm";
			Text = "OpenLocoTool";
			Load += MainForm_Load;
			objectSelector.ResumeLayout(false);
			tabPage2.ResumeLayout(false);
			tabPage1.ResumeLayout(false);
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			splitContainer2.Panel1.ResumeLayout(false);
			splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
			splitContainer2.ResumeLayout(false);
			splitContainer3.Panel1.ResumeLayout(false);
			splitContainer3.Panel1.PerformLayout();
			splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
			splitContainer3.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private TreeView tvFileTree;
		private ListBox lbLogs;
		private Button btnSaveChanges;
		private PropertyGrid pgObject;
		private Button btnSetDirectory;
		private FolderBrowserDialog objectDirBrowser;
		private TextBox tbFileFilter;
		private Label lblFilenameRegex;
		private SaveFileDialog saveFileDialog1;
		private TreeView tvObjType;
		private TabControl objectSelector;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private FlowLayoutPanel flpImageTable;
		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;
		private SplitContainer splitContainer3;
	}
}