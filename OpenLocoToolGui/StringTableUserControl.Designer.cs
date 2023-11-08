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
			dgvLanguageSelector.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			dgvLanguageSelector.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgvLanguageSelector.Location = new Point(397, 3);
			dgvLanguageSelector.Name = "dgvLanguageSelector";
			dgvLanguageSelector.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
			dgvLanguageSelector.RowTemplate.Height = 41;
			dgvLanguageSelector.Size = new Size(1028, 622);
			dgvLanguageSelector.TabIndex = 1;
			// 
			// lbStringSelector
			// 
			lbStringSelector.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			lbStringSelector.FormattingEnabled = true;
			lbStringSelector.ItemHeight = 32;
			lbStringSelector.Location = new Point(3, 3);
			lbStringSelector.Name = "lbStringSelector";
			lbStringSelector.Size = new Size(388, 612);
			lbStringSelector.TabIndex = 2;
			// 
			// StringTableUserControl
			// 
			AutoScaleDimensions = new SizeF(13F, 32F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(lbStringSelector);
			Controls.Add(dgvLanguageSelector);
			Name = "StringTableUserControl";
			Size = new Size(1428, 628);
			((System.ComponentModel.ISupportInitialize)dgvLanguageSelector).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private DataGridView dgvLanguageSelector;
		private ListBox lbStringSelector;
	}
}
