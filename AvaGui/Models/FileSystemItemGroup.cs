using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using System.Collections.ObjectModel;

namespace AvaGui.Models
{
	public abstract record FileSystemItemBase(string Path, string Name, ObservableCollection<FileSystemItemBase>? SubNodes = null)
	{
		public string NameComputed
			=> $"{Name}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string...what have i become
	}

	public record FileSystemItem(string Path, string Name, SourceGame SourceGame)
		: FileSystemItemBase(Path, Name, null);

	public record FileSystemDatGroup(string Path, DatFileType DatFileType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Path, DatFileType.ToString(), SubNodes);

	public record FileSystemItemGroup(string Path, ObjectType ObjectType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Path, ObjectType.ToString(), SubNodes);

	public record FileSystemVehicleGroup(string Path, VehicleType VehicleType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Path, VehicleType.ToString(), SubNodes);

}
