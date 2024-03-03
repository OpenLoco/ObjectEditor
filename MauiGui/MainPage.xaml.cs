namespace MauiGui
{
	public partial class MainPage : ContentPage
	{
		int count;

		public MainPage()
		{
			InitializeComponent();
		}

		private void OnCounterClicked(object sender, EventArgs e)
		{
			count++;

			CounterBtn.Text = count == 1
				? $"Clicked {count} time"
				: $"Clicked {count} times";

			//SemanticScreenReader.Announce(CounterBtn.Text);
		}
	}

}
