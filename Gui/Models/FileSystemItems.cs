using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using System;
using System.Collections.ObjectModel;

namespace OpenLoco.Gui.Models
{
	[Flags]
	public enum FileLocation
	{
		Local,
		Online,
	}

	public abstract record FileSystemItemBase(string Filename, string DisplayName, ObservableCollection<FileSystemItemBase>? SubNodes = null)
	{
		public string NameComputed
			=> $"{DisplayName}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string...what have i become
	}

	public record FileSystemItem(string Filename, string DisplayName, DateTimeOffset? CreatedDate, DateTimeOffset? ModifiedDate, FileLocation FileLocation)
		: FileSystemItemBase(Filename, DisplayName, null);

	public record FileSystemItemObject(string Filename, string DisplayName, DateTimeOffset? CreatedTime, DateTimeOffset? ModifiedTime, FileLocation FileLocation, ObjectSource ObjectSource)
		: FileSystemItem(Filename, DisplayName, CreatedTime, ModifiedTime, FileLocation);

	public record FileSystemItemGroup(string Filename, ObjectType ObjectType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Filename, ObjectType.ToString(), SubNodes);

	public record FileSystemVehicleGroup(string Filename, VehicleType VehicleType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Filename, VehicleType.ToString(), SubNodes);
}
