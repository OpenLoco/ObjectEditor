using System.Drawing.Drawing2D;

namespace OpenLocoToolGui
{
	// https://stackoverflow.com/questions/29157/how-do-i-make-a-picturebox-use-nearest-neighbor-resampling
	public class PictureBoxWithInterpolationMode : PictureBox
	{
		public InterpolationMode InterpolationMode { get; set; } = InterpolationMode.Default;

		protected override void OnPaint(PaintEventArgs paintEventArgs)
		{
			if (!DesignMode)
			{
				paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
			}

			base.OnPaint(paintEventArgs);
		}
	}
}
