using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Definitions;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using FileLocationKind = Definitions.FileLocation;
using OnlineApiEndpointGroupKind = Gui.Models.OnlineApiEndpointGroup;

namespace Gui.Models;

public record FileSystemItem(
	string DisplayName,
	string? FileName, // only available in local mode
	UniqueObjectId? Id, // only available in remote mode
	DateOnly? CreatedDate = null,
	DateOnly? ModifiedDate = null,
	FileLocationKind? FileLocation = null,
	ObjectSource? ObjectSource = null,
	ObjectType? ObjectType = null,
	VehicleType? VehicleType = null,
	ObjectAvailability? Availability = null,
	ObservableCollection<FileSystemItem>? SubNodes = null)
{
	public uint? DatChecksum { get; init; }

	public ulong? xxHash3 { get; init; }

	[JsonIgnore]
	public bool CanOpen
		// Items delivered by a server (any FileLocation) have an Id and are openable via
		// the API; items without an Id are raw disk files from the file-open dialog and
		// only openable when present on disk.
		=> (Id != null && ObjectType != null && OnlineApiEndpointGroup == OnlineApiEndpointGroupKind.Objects)
			|| (FileLocation == FileLocationKind.Local && Id == null && !string.IsNullOrEmpty(FileName));

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

	[JsonIgnore]
	public OnlineApiEndpointGroupKind? OnlineApiEndpointGroup { get; init; }

	[JsonIgnore]
	public bool HasChildren
		=> SubNodes != null && SubNodes.Count > 0;

	[JsonIgnore]
	public bool IsLeafNode
		=> !HasChildren;

	[JsonIgnore]
	public string? DisplayIcon
		=> HasChildren ? DisplayName : ObjectSource.ToString();

	[JsonIgnore]
	public string NameComputed
		=> $"{DisplayName}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string...what have i become

	[JsonIgnore]
	public string NiceObjectSource
		=> ObjectSource switch
		{
			Definitions.ObjectModels.Types.ObjectSource.Custom => "Custom",
			Definitions.ObjectModels.Types.ObjectSource.LocomotionSteam => "Steam",
			Definitions.ObjectModels.Types.ObjectSource.LocomotionGoG => "GoG",
			Definitions.ObjectModels.Types.ObjectSource.OpenLoco => "OpenLoco",
			null => string.Empty,
			_ => throw new NotImplementedException($"Unhandled {nameof(Definitions.ObjectModels.Types.ObjectSource)} value: {ObjectSource}"),
		};

}
