using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using System;
using System.Collections.ObjectModel;

namespace AvaGui.Models
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

	public record FileSystemItem(string Filename, string DisplayName, bool IsVanilla, FileLocation FileLocation)
		: FileSystemItemBase(Filename, DisplayName, null);

	//public record FileSystemDatGroup(string Path, DatFileType DatFileType, ObservableCollection<FileSystemItemBase> SubNodes)
	//	: FileSystemItemBase(Path, DatFileType.ToString(), SubNodes);

	public record FileSystemItemGroup(string Filename, ObjectType ObjectType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Filename, ObjectType.ToString(), SubNodes);

	public record FileSystemVehicleGroup(string Filename, VehicleType VehicleType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Filename, VehicleType.ToString(), SubNodes);
}
