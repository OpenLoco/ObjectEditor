using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Gui.Models;

public record FileSystemItem(
	string DisplayName,
	string? FileName, // only available in local mode
	UniqueObjectId? Id, // only available in online-mode
	DateOnly? CreatedDate = null,
	DateOnly? ModifiedDate = null,
	FileLocation? FileLocation = null,
	ObjectSource? ObjectSource = null,
	ObjectType? ObjectType = null,
	VehicleType? VehicleType = null,
	ObservableCollection<FileSystemItem>? SubNodes = null)
{
	[JsonIgnore]
	public bool CanOpen
		=> FileLocation == global::Gui.Models.FileLocation.Local
			|| (FileLocation == global::Gui.Models.FileLocation.Online
				&& OnlineApiEndpointGroup == global::Gui.Models.OnlineApiEndpointGroup.Objects
				&& Id != null
				&& ObjectType != null);

	[JsonIgnore]
	public bool CanOpenFolder
		=> FileLocation == global::Gui.Models.FileLocation.Local && IsLeafNode && !string.IsNullOrEmpty(FileName);

	public uint? DatChecksum { get; init; }

	public ulong? xxHash3 { get; init; }

	public OnlineApiEndpointGroup? OnlineApiEndpointGroup { get; init; }

	[JsonIgnore]
	public bool HasChildren
		=> SubNodes != null && SubNodes.Count > 0;

	[JsonIgnore]
	public bool IsLeafNode
		=> !HasChildren;

	[JsonIgnore]
	public string NameComputed
		=> $"{DisplayName}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string...what have i become
}
