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

public class ObjectMetadataViewModel : ReactiveObject
{
	readonly Gui.ObjectServiceClient? objectServiceClient;
	readonly ILogger? logger;

	public ObjectMetadataViewModel(ObjectMetadata metadata, Gui.ObjectServiceClient? objectServiceClient = null, ILogger? logger = null)
	{
		Metadata = metadata;
		description = metadata.Description;
		createdDate = metadata.CreatedDate;
		modifiedDate = metadata.ModifiedDate;
		selectedLicence = metadata.Licence;
		this.logger = logger;
		
		// Initialize observable collections from metadata
		Authors = new ObservableCollection<DtoAuthorEntry>(metadata.Authors);
		Tags = new ObservableCollection<DtoTagEntry>(metadata.Tags);
		ObjectPacks = new ObservableCollection<DtoItemPackEntry>(metadata.ObjectPacks);
		
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
			Authors.Remove(author);
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
			Tags.Remove(tag);
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
			ObjectPacks.Remove(pack);
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

	public ObjectMetadataViewModel() : this(new ObjectMetadata("<empty>"))
	{
	}

	public ObjectMetadata Metadata { get; }

	// InternalName is readonly (init-only in the model)
	public string InternalName => Metadata.InternalName;

	// Availability is readonly (user cannot change this)
	public ObjectAvailability Availability => Metadata.Availability;

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
			AvailableLicences = new ObservableCollection<DtoLicenceEntry?>(licenceList);

			// Load authors, tags, and object packs
			var authors = await objectServiceClient.GetAuthorsAsync();
			AvailableAuthors = new ObservableCollection<DtoAuthorEntry>(authors);

			var tags = await objectServiceClient.GetTagsAsync();
			AvailableTags = new ObservableCollection<DtoTagEntry>(tags);

			var objectPacks = await objectServiceClient.GetObjectPacksAsync();
			AvailableObjectPacks = new ObservableCollection<DtoItemPackEntry>(objectPacks);
		}
		catch (Exception ex)
		{
			// Log the exception so users know why data failed to load
			logger?.Warning($"Failed to load server data for metadata editing: {ex.Message}");
			
			// If we can't load data (e.g., offline mode), just set empty lists
			AvailableLicences = new ObservableCollection<DtoLicenceEntry?> { null };
			AvailableAuthors = new ObservableCollection<DtoAuthorEntry>();
			AvailableTags = new ObservableCollection<DtoTagEntry>();
			AvailableObjectPacks = new ObservableCollection<DtoItemPackEntry>();
		}
	}

	// Commands for adding/removing items
	void SyncAuthorsToMetadata()
	{
		Metadata.Authors.Clear();
		foreach (var author in Authors)
		{
			Metadata.Authors.Add(author);
		}
	}

	void SyncTagsToMetadata()
	{
		Metadata.Tags.Clear();
		foreach (var tag in Tags)
		{
			Metadata.Tags.Add(tag);
		}
	}

	void SyncObjectPacksToMetadata()
	{
		Metadata.ObjectPacks.Clear();
		foreach (var pack in ObjectPacks)
		{
			Metadata.ObjectPacks.Add(pack);
		}
	}

	string? description;
	public string? Description
	{
		get => description;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref description, value);
			Metadata.Description = value;
		}
	}

	DtoLicenceEntry? selectedLicence;
	public DtoLicenceEntry? SelectedLicence
	{
		get => selectedLicence;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref selectedLicence, value);
			Metadata.Licence = value;
		}
	}

	DateTimeOffset? createdDate;
	public DateTimeOffset? CreatedDate
	{
		get => createdDate;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref createdDate, value);
			Metadata.CreatedDate = value;
		}
	}

	DateTimeOffset? modifiedDate;
	public DateTimeOffset? ModifiedDate
	{
		get => modifiedDate;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref modifiedDate, value);
			Metadata.ModifiedDate = value;
		}
	}

	// UploadedDate is readonly (server-managed)
	public DateTimeOffset UploadedDate => Metadata.UploadedDate;
}
