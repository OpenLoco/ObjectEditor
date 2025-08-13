using Dat.Data;
using Dat.Objects;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Gui.Models;

[Flags]
public enum FileLocation
{
	Local,
	Online,
}

public record FileSystemItem(
	string DisplayName,
	string? FileName, // only available in local mode
	UniqueObjectId? Id, // only available in online-mode
	DateOnly? CreatedDate = null,
	DateOnly? ModifiedDate = null,
	FileLocation? FileLocation = null,
	ObjectSource? ObjectSource = null,
	DatObjectType? ObjectType = null,
	DatVehicleType? VehicleType = null,
	ObservableCollection<FileSystemItem>? SubNodes = null)
{
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
