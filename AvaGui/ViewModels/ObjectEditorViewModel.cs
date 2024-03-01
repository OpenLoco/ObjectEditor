using Avalonia.Reactive;
using ReactiveUI;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using Avalonia;
using System.Collections.ObjectModel;
using System.Linq;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject
	{
		public ObjectEditorViewModel()
		{
			// We can listen to any property changes with "WhenAnyValue" and do whatever we want in "Subscribe".
			//this.WhenAnyValue(o => o.Name).Subscribe(o => this.RaisePropertyChanged(nameof(Greeting)));

			// Use "Subscribe" with an anonymous class implementing IObserver<string>
			_ = this.WhenAnyValue(o => o.Name)
				.Subscribe(new AnonymousObserver<string>(
					onNext: value => this.RaisePropertyChanged(nameof(Greeting)),
					onError: error => { /* Handle error */ },
					onCompleted: () => { /* Perform actions on completion (optional) */ }));

			_ = this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(new AnonymousObserver<string>(
					onNext: value => this.RaisePropertyChanged(nameof(Files)),
					onError: error => { /* Handle error */ },
					onCompleted: () => { /* Perform actions on completion (optional) */ }));
		}

		string _Name = string.Empty;
		public string Name
		{
			get => _Name;
			set => this.RaiseAndSetIfChanged(ref _Name, value);
		}

		// Greeting will change based on a Name.
		public string Greeting
		{
			get
			{
				if (string.IsNullOrEmpty(Name))
				{
					// If no Name is provided, use a default Greeting
					return "Hello World from Avalonia.Samples";
				}
				else
				{
					// else greet the User.
					return $"Hello {Name}";
				}
			}
		}

		//IStorageProvider? GetProvider()
		//{
		//	if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || desktop.MainWindow?.StorageProvider is not { } provider)
		//		throw new NullReferenceException("Missing StorageProvider instance.");
		//	return provider;
		//}

		public ObservableCollection<string> Files
		{
			get => new ObservableCollection<string>(new string[] { "A", "B" }); //get => new ObservableCollection<string>(Directory.GetFiles(CurrentDirectory));
		}

		public string CurrentDirectory { get; set; }
			= "C:\\";

		//public async Task<string> GetSelectedFolderPath()
		//{
		//	// Get the top-level window (adjust based on your window access)
		//	var window = this.GetTopLevel();

		//	// Create a new OpenFolderDialog instance
		//	var dialog = new OpenFolderDialog();

		//	// Optionally set properties like initial directory or title
		//	dialog.Title = "Select Folder";
		//	dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		//	// Show the dialog asynchronously
		//	var result = await dialog.ShowAsync(window);

		//	// Return the selected folder path if available, otherwise null
		//	return result;
		//}

		private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
		{
			// Get top level from the current control. Alternatively, you can use Window reference instead.
			//var topLevel = Window.GetTopLevel();

			// Start async operation to open the dialog.
			//var files = await GetProvider().OpenFilePickerAsync(new FilePickerOpenOptions
			//{
			//	Title = "Open Text File",
			//	AllowMultiple = false
			//});

			//if (files.Count >= 1)
			//{
			//	// Open reading stream from the first file.
			//	await using var stream = await files[0].OpenReadAsync();
			//	using var streamReader = new StreamReader(stream);
			//	// Reads all the content of file as a text.
			//	var fileContent = await streamReader.ReadToEndAsync();
			//}
		}
	}
}
