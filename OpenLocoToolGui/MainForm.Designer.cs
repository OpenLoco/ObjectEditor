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
			tabPage1 = new TabPage();
			tabPage2 = new TabPage();
			objectSelector.SuspendLayout();
			tabPage1.SuspendLayout();
			tabPage2.SuspendLayout();
			SuspendLayout();
			// 
			// tvFileTree
			// 
			tvFileTree.Dock = DockStyle.Fill;
			tvFileTree.Location = new Point(3, 3);
			tvFileTree.Name = "tvFileTree";
			tvFileTree.Size = new Size(244, 571);
			tvFileTree.TabIndex = 1;
			tvFileTree.AfterSelect += tv_AfterSelect;
			// 
			// lbLogs
			// 
			lbLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			lbLogs.FormattingEnabled = true;
			lbLogs.HorizontalScrollbar = true;
			lbLogs.ItemHeight = 15;
			lbLogs.Location = new Point(802, 71);
			lbLogs.Name = "lbLogs";
			lbLogs.SelectionMode = SelectionMode.None;
			lbLogs.Size = new Size(399, 604);
			lbLogs.TabIndex = 17;
			// 
			// btnSaveChanges
			// 
			btnSaveChanges.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnSaveChanges.Location = new Point(276, 12);
			btnSaveChanges.Name = "btnSaveChanges";
			btnSaveChanges.Size = new Size(520, 52);
			btnSaveChanges.TabIndex = 18;
			btnSaveChanges.Text = "Save Changes";
			btnSaveChanges.UseVisualStyleBackColor = true;
			btnSaveChanges.Click += btnSaveChanges_Click;
			// 
			// pgObject
			// 
			pgObject.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
			pgObject.HelpVisible = false;
			pgObject.Location = new Point(276, 70);
			pgObject.Name = "pgObject";
			pgObject.Size = new Size(520, 605);
			pgObject.TabIndex = 22;
			pgObject.ToolbarVisible = false;
			// 
			// btnSetDirectory
			// 
			btnSetDirectory.Location = new Point(12, 12);
			btnSetDirectory.Name = "btnSetDirectory";
			btnSetDirectory.Size = new Size(231, 23);
			btnSetDirectory.TabIndex = 23;
			btnSetDirectory.Text = "Set ObjData Directory";
			btnSetDirectory.UseVisualStyleBackColor = true;
			btnSetDirectory.Click += btnSetDirectory_Click;
			// 
			// tbFileFilter
			// 
			tbFileFilter.BorderStyle = BorderStyle.FixedSingle;
			tbFileFilter.Location = new Point(112, 41);
			tbFileFilter.Name = "tbFileFilter";
			tbFileFilter.Size = new Size(154, 23);
			tbFileFilter.TabIndex = 24;
			tbFileFilter.TextChanged += tbFileFilter_TextChanged;
			// 
			// lblFilenameRegex
			// 
			lblFilenameRegex.BorderStyle = BorderStyle.FixedSingle;
			lblFilenameRegex.Location = new Point(12, 41);
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
			tvObjType.Size = new Size(244, 571);
			tvObjType.TabIndex = 26;
			tvObjType.AfterSelect += tv_AfterSelect;
			// 
			// objectSelector
			// 
			objectSelector.Controls.Add(tabPage1);
			objectSelector.Controls.Add(tabPage2);
			objectSelector.Location = new Point(12, 70);
			objectSelector.Name = "objectSelector";
			objectSelector.SelectedIndex = 0;
			objectSelector.Size = new Size(258, 605);
			objectSelector.TabIndex = 29;
			// 
			// tabPage1
			// 
			tabPage1.Controls.Add(tvFileTree);
			tabPage1.Location = new Point(4, 24);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(3);
			tabPage1.Size = new Size(250, 577);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "File";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			tabPage2.Controls.Add(tvObjType);
			tabPage2.Location = new Point(4, 24);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(3);
			tabPage2.Size = new Size(250, 577);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Category";
			tabPage2.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1213, 689);
			Controls.Add(objectSelector);
			Controls.Add(lblFilenameRegex);
			Controls.Add(tbFileFilter);
			Controls.Add(btnSetDirectory);
			Controls.Add(pgObject);
			Controls.Add(btnSaveChanges);
			Controls.Add(lbLogs);
			Name = "MainForm";
			Text = "OpenLocoTool";
			Load += MainForm_Load;
			objectSelector.ResumeLayout(false);
			tabPage1.ResumeLayout(false);
			tabPage2.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
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
	}
}