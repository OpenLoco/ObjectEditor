namespace OpenLoco.ObjectEditor.Gui
{
	partial class ProgressBarForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			pbProgress = new ProgressBar();
			SuspendLayout();
			// 
			// pbProgress
			// 
			pbProgress.Dock = DockStyle.Fill;
			pbProgress.Location = new Point(4, 4);
			pbProgress.Name = "pbProgress";
			pbProgress.Size = new Size(488, 17);
			pbProgress.TabIndex = 0;
			// 
			// ProgressBarForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(496, 25);
			Controls.Add(pbProgress);
			Name = "ProgressBarForm";
			Padding = new Padding(4);
			Text = "ProgressBarForm";
			ResumeLayout(false);
		}

		#endregion

		private ProgressBar pbProgress;
	}
}