using Common.Logging;
using Definitions;
using Definitions.DTO;
using Definitions.ObjectModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class ObjectMetadataViewModel : BaseViewModelWithEditorContext<ObjectMetadata>, IViewModel
{
	public string DisplayName
		=> "Metadata";

	readonly ObjectServiceClient? objectServiceClient;
	readonly ILogger? logger;

	public ObjectMetadataViewModel() : this(new ObjectMetadata("<empty>"))
	{
	}

	public ObjectMetadataViewModel(ObjectMetadata model, Gui.ObjectServiceClient? objectServiceClient = null, ILogger? logger = null)
		: base(null, model)
	{
		Model = model;
		description = model.Description;
		createdDate = model.CreatedDate;
		modifiedDate = model.ModifiedDate;
		selectedLicence = model.Licence;
		this.logger = logger;

		// Initialize observable collections from metadata
		Authors = new ObservableCollection<DtoAuthorEntry>(model.Authors);
		Tags = new ObservableCollection<DtoTagEntry>(model.Tags);
		ObjectPacks = new ObservableCollection<DtoItemPackEntry>(model.ObjectPacks);

		this.objectServiceClient = objectServiceClient;

		// Initialize commands
		AddAuthorCommand = ReactiveCommand.Create<DtoAuthorEntry?>(author =>
		{
			if (author != null && !Authors.Contains(author))
			{
				Authors.Add(author);
				SyncAuthorsToMetadata();
			}
		});

		RemoveAuthorCommand = ReactiveCommand.Create<DtoAuthorEntry>(author =>
		{
			_ = Authors.Remove(author);
			SyncAuthorsToMetadata();
		});

		AddTagCommand = ReactiveCommand.Create<DtoTagEntry?>(tag =>
		{
			if (tag != null && !Tags.Contains(tag))
			{
				Tags.Add(tag);
				SyncTagsToMetadata();
			}
		});

		RemoveTagCommand = ReactiveCommand.Create<DtoTagEntry>(tag =>
		{
			_ = Tags.Remove(tag);
			SyncTagsToMetadata();
		});

		AddObjectPackCommand = ReactiveCommand.Create<DtoItemPackEntry?>(pack =>
		{
			if (pack != null && !ObjectPacks.Contains(pack))
			{
				ObjectPacks.Add(pack);
				SyncObjectPacksToMetadata();
			}
		});

		RemoveObjectPackCommand = ReactiveCommand.Create<DtoItemPackEntry>(pack =>
		{
			_ = ObjectPacks.Remove(pack);
			SyncObjectPacksToMetadata();
		});

		// Load data from server if we have a client
		if (objectServiceClient != null)
		{
			_ = LoadServerDataAsync().ContinueWith(t =>
			{
				// Log any exceptions that occur
				if (t.Exception != null)
				{
					logger?.Error("Failed to load server data for metadata editing", t.Exception);
				}
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}

	// InternalName is readonly (init-only in the model)
	public string InternalName
		=> Model.InternalName;

	// Availability is readonly (user cannot change this)
	public ObjectAvailability Availability
		=> Model.Availability;

	// Collections for editing
	public ObservableCollection<DtoAuthorEntry> Authors { get; }
	public ObservableCollection<DtoTagEntry> Tags { get; }
	public ObservableCollection<DtoItemPackEntry> ObjectPacks { get; }

	// Commands
	public ReactiveCommand<DtoAuthorEntry?, Unit> AddAuthorCommand { get; }
	public ReactiveCommand<DtoAuthorEntry, Unit> RemoveAuthorCommand { get; }
	public ReactiveCommand<DtoTagEntry?, Unit> AddTagCommand { get; }
	public ReactiveCommand<DtoTagEntry, Unit> RemoveTagCommand { get; }
	public ReactiveCommand<DtoItemPackEntry?, Unit> AddObjectPackCommand { get; }
	public ReactiveCommand<DtoItemPackEntry, Unit> RemoveObjectPackCommand { get; }

	// Available items for selection
	ObservableCollection<DtoAuthorEntry> availableAuthors = [];
	public ObservableCollection<DtoAuthorEntry> AvailableAuthors
	{
		get => availableAuthors;
		set => this.RaiseAndSetIfChanged(ref availableAuthors, value);
	}

	ObservableCollection<DtoTagEntry> availableTags = [];
	public ObservableCollection<DtoTagEntry> AvailableTags
	{
		get => availableTags;
		set => this.RaiseAndSetIfChanged(ref availableTags, value);
	}

	ObservableCollection<DtoItemPackEntry> availableObjectPacks = [];
	public ObservableCollection<DtoItemPackEntry> AvailableObjectPacks
	{
		get => availableObjectPacks;
		set => this.RaiseAndSetIfChanged(ref availableObjectPacks, value);
	}

	// Available licences
	ObservableCollection<DtoLicenceEntry?> availableLicences = [];
	public ObservableCollection<DtoLicenceEntry?> AvailableLicences
	{
		get => availableLicences;
		set => this.RaiseAndSetIfChanged(ref availableLicences, value);
	}

	async Task LoadServerDataAsync()
	{
		if (objectServiceClient == null)
		{
			return;
		}

		try
		{
			// Load licences
			var licences = await objectServiceClient.GetLicencesAsync();
			var licenceList = new List<DtoLicenceEntry?> { null }; // Add None option
			licenceList.AddRange(licences);
			AvailableLicences = new ObservableCollection<DtoLicenceEntry?>(licenceList.OrderBy(x => x?.Name));

			// Load authors, tags, and object packs
			var authors = await objectServiceClient.GetAuthorsAsync();
			AvailableAuthors = new ObservableCollection<DtoAuthorEntry>(authors.OrderBy(x => x.Name));

			var tags = await objectServiceClient.GetTagsAsync();
			AvailableTags = new ObservableCollection<DtoTagEntry>(tags.OrderBy(x => x.Name));

			var objectPacks = await objectServiceClient.GetObjectPacksAsync();
			AvailableObjectPacks = new ObservableCollection<DtoItemPackEntry>(objectPacks.OrderBy(x => x.Name));
		}
		catch (Exception ex)
		{
			// Log the exception so users know why data failed to load
			logger?.Error("Failed to load server data for metadata editing", ex);

			// If we can't load data (e.g., offline mode), just set empty lists
			AvailableLicences = [null];
			AvailableAuthors = [];
			AvailableTags = [];
			AvailableObjectPacks = [];
		}
	}

	// Commands for adding/removing items
	void SyncAuthorsToMetadata()
	{
		Model.Authors.Clear();
		foreach (var author in Authors)
		{
			Model.Authors.Add(author);
		}
	}

	void SyncTagsToMetadata()
	{
		Model.Tags.Clear();
		foreach (var tag in Tags)
		{
			Model.Tags.Add(tag);
		}
	}

	void SyncObjectPacksToMetadata()
	{
		Model.ObjectPacks.Clear();
		foreach (var pack in ObjectPacks)
		{
			Model.ObjectPacks.Add(pack);
		}
	}

	string? description;
	public string? Description
	{
		get => description;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref description, value);
			Model.Description = value;
		}
	}

	DtoLicenceEntry? selectedLicence;
	public DtoLicenceEntry? SelectedLicence
	{
		get => selectedLicence;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref selectedLicence, value);
			Model.Licence = value;
		}
	}

	DateTimeOffset? createdDate;
	public DateTimeOffset? CreatedDate
	{
		get => createdDate;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref createdDate, value);
			Model.CreatedDate = value;
		}
	}

	DateTimeOffset? modifiedDate;
	public DateTimeOffset? ModifiedDate
	{
		get => modifiedDate;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref modifiedDate, value);
			Model.ModifiedDate = value;
		}
	}

	// UploadedDate is readonly (server-managed)
	public DateTimeOffset UploadedDate
		=> Model.UploadedDate;
}
