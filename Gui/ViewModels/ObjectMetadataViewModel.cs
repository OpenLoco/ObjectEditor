using Definitions;
using Definitions.ObjectModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gui.ViewModels;

public class ObjectMetadataViewModel : ReactiveObject
{
	public ObjectMetadataViewModel(ObjectMetadata metadata)
	{
		Metadata = metadata;
		description = metadata.Description;
		availability = metadata.Availability;
		createdDate = metadata.CreatedDate;
		modifiedDate = metadata.ModifiedDate;
	}

	public ObjectMetadataViewModel() : this(new ObjectMetadata("<empty>"))
	{
	}

	public ObjectMetadata Metadata { get; }

	// InternalName is readonly (init-only in the model)
	public string InternalName => Metadata.InternalName;

	// Available values for Availability enum
	public IEnumerable<ObjectAvailability> AvailabilityValues => Enum.GetValues<ObjectAvailability>();

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

	ObjectAvailability availability;
	public ObjectAvailability Availability
	{
		get => availability;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref availability, value);
			Metadata.Availability = value;
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
