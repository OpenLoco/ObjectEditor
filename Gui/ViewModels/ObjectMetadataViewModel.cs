using Definitions;
using Definitions.DTO;
using Definitions.ObjectModels;
using Gui.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Gui.ViewModels;

public class ObjectMetadataViewModel : BaseViewModelWithEditorContext<ObjectMetadata>, IViewModel
{
	public new string DisplayName
		=> "Metadata";

	public ObjectMetadataViewModel() : this(new ObjectMetadata("<empty>"), null!)
	{
	}

	public ObjectMetadataViewModel(ObjectMetadata model, ObjectEditorContext editorContext)
		: base(editorContext, model)
	{
		Model = model;
		description = model.Description;
		createdDate = model.CreatedDate;
		modifiedDate = model.ModifiedDate;
		selectedLicence = model.Licence;

		// Initialize observable collections from metadata
		Authors = [with(model.Authors)];
		Tags = [with(model.Tags)];
		ObjectPacks = [with(model.ObjectPacks)];

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

		// Server metadata (licences/authors/tags/object packs) is fetched once and cached on the
		// ObjectEditorContext; just point at those shared collections.
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

	// Available items for selection - sourced from the shared cache on ObjectEditorContext.ObjectServiceModel
	// so they are only fetched from the server once per editor session.
	public ObservableCollection<DtoAuthorEntry> AvailableAuthors => EditorContext.ObjectServiceModel.AvailableAuthors;

	public ObservableCollection<DtoTagEntry> AvailableTags => EditorContext.ObjectServiceModel.AvailableTags;

	public ObservableCollection<DtoItemPackEntry> AvailableObjectPacks => EditorContext.ObjectServiceModel.AvailableObjectPacks;

	public ObservableCollection<DtoLicenceEntry?> AvailableLicences => EditorContext.ObjectServiceModel.AvailableLicences;

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
