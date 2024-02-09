namespace OpenLoco.ObjectEditor.Gui
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
			lbStringSelector = new ListBox();
			flpLanguageStrings = new FlowLayoutPanel();
			SuspendLayout();
			// 
			// lbStringSelector
			// 
			lbStringSelector.Dock = DockStyle.Left;
			lbStringSelector.FormattingEnabled = true;
			lbStringSelector.ItemHeight = 15;
			lbStringSelector.Location = new Point(0, 0);
			lbStringSelector.Margin = new Padding(2, 1, 2, 1);
			lbStringSelector.Name = "lbStringSelector";
			lbStringSelector.Size = new Size(211, 294);
			lbStringSelector.TabIndex = 2;
			// 
			// flpLanguageStrings
			// 
			flpLanguageStrings.BorderStyle = BorderStyle.Fixed3D;
			flpLanguageStrings.Dock = DockStyle.Fill;
			flpLanguageStrings.FlowDirection = FlowDirection.TopDown;
			flpLanguageStrings.Location = new Point(211, 0);
			flpLanguageStrings.Name = "flpLanguageStrings";
			flpLanguageStrings.Size = new Size(558, 294);
			flpLanguageStrings.TabIndex = 3;
			// 
			// StringTableUserControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(flpLanguageStrings);
			Controls.Add(lbStringSelector);
			Margin = new Padding(2, 1, 2, 1);
			Name = "StringTableUserControl";
			Size = new Size(769, 294);
			ResumeLayout(false);
		}

		#endregion
		private ListBox lbStringSelector;
		private FlowLayoutPanel flpLanguageStrings;
	}
}
