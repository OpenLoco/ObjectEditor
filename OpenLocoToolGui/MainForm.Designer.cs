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
			treeView1 = new TreeView();
			cbObjFilter = new ComboBox();
			lbLogs = new ListBox();
			btnSaveChanges = new Button();
			pgDatHeader = new PropertyGrid();
			pgObjHeader = new PropertyGrid();
			pgObject = new PropertyGrid();
			SuspendLayout();
			// 
			// treeView1
			// 
			treeView1.Location = new Point(12, 41);
			treeView1.Name = "treeView1";
			treeView1.Size = new Size(190, 678);
			treeView1.TabIndex = 1;
			treeView1.AfterSelect += treeView1_AfterSelect;
			// 
			// cbObjFilter
			// 
			cbObjFilter.FormattingEnabled = true;
			cbObjFilter.Location = new Point(12, 12);
			cbObjFilter.Name = "cbObjFilter";
			cbObjFilter.Size = new Size(190, 23);
			cbObjFilter.TabIndex = 15;
			// 
			// lbLogs
			// 
			lbLogs.FormattingEnabled = true;
			lbLogs.HorizontalScrollbar = true;
			lbLogs.ItemHeight = 15;
			lbLogs.Location = new Point(12, 725);
			lbLogs.Name = "lbLogs";
			lbLogs.SelectionMode = SelectionMode.None;
			lbLogs.Size = new Size(395, 124);
			lbLogs.TabIndex = 17;
			// 
			// btnSaveChanges
			// 
			btnSaveChanges.Location = new Point(413, 818);
			btnSaveChanges.Name = "btnSaveChanges";
			btnSaveChanges.Size = new Size(199, 31);
			btnSaveChanges.TabIndex = 18;
			btnSaveChanges.Text = "Save";
			btnSaveChanges.UseVisualStyleBackColor = true;
			btnSaveChanges.Click += btnSaveChanges_Click;
			btnSaveChanges.Enabled = false;
			// 
			// pgDatHeader
			// 
			pgDatHeader.HelpVisible = false;
			pgDatHeader.Location = new Point(208, 12);
			pgDatHeader.Name = "pgDatHeader";
			pgDatHeader.PropertySort = PropertySort.Alphabetical;
			pgDatHeader.Size = new Size(199, 200);
			pgDatHeader.TabIndex = 20;
			pgDatHeader.ToolbarVisible = false;
			// 
			// pgObjHeader
			// 
			pgObjHeader.HelpVisible = false;
			pgObjHeader.Location = new Point(413, 12);
			pgObjHeader.Name = "pgObjHeader";
			pgObjHeader.PropertySort = PropertySort.Alphabetical;
			pgObjHeader.Size = new Size(199, 200);
			pgObjHeader.TabIndex = 21;
			pgObjHeader.ToolbarVisible = false;
			// 
			// pgObject
			// 
			pgObject.HelpVisible = false;
			pgObject.Location = new Point(208, 218);
			pgObject.Name = "pgObject";
			pgObject.PropertySort = PropertySort.Alphabetical;
			pgObject.Size = new Size(404, 501);
			pgObject.TabIndex = 22;
			pgObject.ToolbarVisible = false;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(621, 861);
			Controls.Add(pgObject);
			Controls.Add(pgObjHeader);
			Controls.Add(pgDatHeader);
			Controls.Add(btnSaveChanges);
			Controls.Add(lbLogs);
			Controls.Add(cbObjFilter);
			Controls.Add(treeView1);
			Name = "MainForm";
			Text = "OpenLocoTool";
			Load += MainForm_Load;
			ResumeLayout(false);
		}

		#endregion
		private TreeView treeView1;
		private ComboBox cbObjFilter;
		private ListBox lbLogs;
		private Button btnSaveChanges;
		private PropertyGrid pgDatHeader;
		private PropertyGrid pgObjHeader;
		private PropertyGrid pgObject;
	}
}