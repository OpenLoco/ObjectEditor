﻿namespace OpenLoco.ObjectEditor.Gui
{
	public partial class ProgressBarForm : Form
	{
		public ProgressBarForm() => InitializeComponent();

		public void SetProgress(int value)
		{
			if (InvokeRequired)
			{
				// Update progress bar using Invoke if not on the UI thread
				_ = Invoke(new Action<int>(SetProgress), value);
			}
			else
			{
				pbProgress.Value = value;
			}
		}

		public void CloseForm()
		{
			if (InvokeRequired)
			{
				// Close the form using Invoke if not on the UI thread
				Invoke(new Action(CloseForm));
			}
			else
			{
				Close();
			}
		}
	}
}
