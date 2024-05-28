using OpenLoco.ObjectEditor.Data;
using System.Collections.ObjectModel;
using MauiApp1.ViewModels;
using AvaGui.ViewModels;
using AvaGui.Models;
using ReactiveUI;

namespace MauiApp1
{
	public partial class MainPage : ContentPage
	{
		int count = 0;
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		//public ObjectEditorViewModel ObjectEditorViewModel { get; }

		public MainPage()
		{
			InitializeComponent();

			Model = new();
			FolderTreeViewModel = new FolderTreeViewModel(Model);
			//ObjectEditorViewModel = new ObjectEditorViewModel(Model);

			//_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
			//	.Subscribe(o => ObjectEditorViewModel.CurrentlySelectedObject = o);

			BindingContext = FolderTreeViewModel; //ObjectEditorViewModel; //(@"Q:\Games\Locomotion\OriginalObjects");
		}

		private void OnCounterClicked(object sender, EventArgs e)
		{
			count++;

			//if (count == 1)
			//	CounterBtn.Text = $"Clicked {count} time";
			//else
			//	CounterBtn.Text = $"Clicked {count} times";

			//SemanticScreenReader.Announce(CounterBtn.Text);
		}
	}

}
