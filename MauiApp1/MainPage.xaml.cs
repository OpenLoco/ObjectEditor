using OpenLoco.ObjectEditor.Data;
using System.Collections.ObjectModel;

namespace MauiApp1
{
	public class FileListViewModel
	{
		public ObservableCollection<DatFile> Filenames { get; } =
		[
			new DatFile("bob.dat", ObjectType.Airport, "some details"),
			new DatFile("bill.dat", ObjectType.TownNames, "more details"),
		];
	}

	public record DatFile(string FileName, ObjectType ObjectType, string Details);

	public partial class MainPage : ContentPage
	{
		int count = 0;

		public MainPage()
		{
			InitializeComponent();
			BindingContext = new FileListViewModel();
		}

		private void OnCounterClicked(object sender, EventArgs e)
		{
			count++;

			if (count == 1)
				CounterBtn.Text = $"Clicked {count} time";
			else
				CounterBtn.Text = $"Clicked {count} times";

			SemanticScreenReader.Announce(CounterBtn.Text);
		}
	}

}
