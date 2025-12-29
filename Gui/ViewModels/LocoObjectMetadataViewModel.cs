using Definitions.ObjectModels;
using ReactiveUI;
using System;

namespace Gui.ViewModels;

public class LocoObjectMetadataViewModel : ReactiveObject
{
	public LocoObjectMetadata Metadata { get; }

	string? description;
	public string? Description
	{
		get => description;
		set
		{
			this.RaiseAndSetIfChanged(ref description, value);
			Metadata.Description = value;
		}
	}

	DateTimeOffset? createdDate;
	public DateTimeOffset? CreatedDate
	{
		get => createdDate;
		set
		{
			this.RaiseAndSetIfChanged(ref createdDate, value);
			Metadata.CreatedDate = value;
		}
	}

	DateTimeOffset? modifiedDate;
	public DateTimeOffset? ModifiedDate
	{
		get => modifiedDate;
		set
		{
			this.RaiseAndSetIfChanged(ref modifiedDate, value);
			Metadata.ModifiedDate = value;
		}
	}

	public LocoObjectMetadataViewModel(LocoObjectMetadata metadata)
	{
		Metadata = metadata;
		description = metadata.Description;
		createdDate = metadata.CreatedDate;
		modifiedDate = metadata.ModifiedDate;
	}

	public LocoObjectMetadataViewModel() : this(new LocoObjectMetadata("<empty>"))
	{
	}
}
