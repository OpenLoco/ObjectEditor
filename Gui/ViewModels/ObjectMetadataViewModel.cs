using Definitions.ObjectModels;
using ReactiveUI;
using System;

namespace Gui.ViewModels;

public class ObjectMetadataViewModel : ReactiveObject
{
	public ObjectMetadataViewModel(ObjectMetadata metadata)
	{
		Metadata = metadata;
		description = metadata.Description;
		createdDate = metadata.CreatedDate;
		modifiedDate = metadata.ModifiedDate;
	}

	public ObjectMetadataViewModel() : this(new ObjectMetadata("<empty>"))
	{
	}

	public ObjectMetadata Metadata { get; }

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
}
