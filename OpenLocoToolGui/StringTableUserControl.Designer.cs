namespace OpenLocoToolGui
{
	partial class StringTableUserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			dgvLanguageSelector = new DataGridView();
			lbStringSelector = new ListBox();
			((System.ComponentModel.ISupportInitialize)dgvLanguageSelector).BeginInit();
			SuspendLayout();
			// 
			// dgvLanguageSelector
			// 
			dgvLanguageSelector.AllowUserToResizeRows = false;
			dgvLanguageSelector.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			dgvLanguageSelector.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgvLanguageSelector.Location = new Point(214, 1);
			dgvLanguageSelector.Margin = new Padding(2, 1, 2, 1);
			dgvLanguageSelector.MultiSelect = false;
			dgvLanguageSelector.Name = "dgvLanguageSelector";
			dgvLanguageSelector.RowHeadersWidth = 82;
			dgvLanguageSelector.RowTemplate.Height = 41;
			dgvLanguageSelector.Size = new Size(554, 292);
			dgvLanguageSelector.TabIndex = 1;
			// 
			// lbStringSelector
			// 
			lbStringSelector.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			lbStringSelector.FormattingEnabled = true;
			lbStringSelector.ItemHeight = 15;
			lbStringSelector.Location = new Point(2, 1);
			lbStringSelector.Margin = new Padding(2, 1, 2, 1);
			lbStringSelector.Name = "lbStringSelector";
			lbStringSelector.Size = new Size(211, 289);
			lbStringSelector.TabIndex = 2;
			// 
			// StringTableUserControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(lbStringSelector);
			Controls.Add(dgvLanguageSelector);
			Margin = new Padding(2, 1, 2, 1);
			Name = "StringTableUserControl";
			Size = new Size(769, 294);
			((System.ComponentModel.ISupportInitialize)dgvLanguageSelector).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private DataGridView dgvLanguageSelector;
		private ListBox lbStringSelector;
	}
}
