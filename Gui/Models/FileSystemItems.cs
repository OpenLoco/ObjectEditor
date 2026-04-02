using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using FileLocationKind = Gui.Models.FileLocation;
using OnlineApiEndpointGroupKind = Gui.Models.OnlineApiEndpointGroup;

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
		=> FileLocation == FileLocationKind.Local
			|| (FileLocation == FileLocationKind.Online
				&& OnlineApiEndpointGroup == OnlineApiEndpointGroupKind.Objects
				&& Id != null
				&& ObjectType != null);

	[JsonIgnore]
	public bool CanDownload
		=> FileLocation == FileLocationKind.Online
			&& Id != null
			&& OnlineApiEndpointGroup is OnlineApiEndpointGroupKind.Objects or OnlineApiEndpointGroupKind.Scenarios;

	[JsonIgnore]
	public bool CanOpenFolder
		=> FileLocation == FileLocationKind.Local && IsLeafNode && !string.IsNullOrEmpty(FileName);

	[JsonIgnore]
	public bool HasContextActions
		=> CanOpenFolder || CanDownload;

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
