using HarfBuzzSharp;
using System.Collections.ObjectModel;

namespace AvaGui.Models
{
	public record FileSystemItemBase(string Path, string Name, ObservableCollection<FileSystemItemBase>? SubNodes = null)
	{
		public string NameComputed
			=> $"{Name}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string..what have i become
	}

	public record FileSystemItemGroup(string Path, string Name, ObservableCollection<FileSystemItemBase> SubNodes) : FileSystemItemBase(Path, Name, SubNodes);
	public record FileSystemItemVehicle(string Path, string Name, string ObjectType, string VehicleType) : FileSystemItemBase(Path, Name);
}
