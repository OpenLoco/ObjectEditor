using Avalonia;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using System.Collections.ObjectModel;

namespace AvaGui.Models
{
	public abstract record FileSystemItemBase(string Path, string Name, ObservableCollection<FileSystemItemBase>? SubNodes = null, PixelRect? SourceRect = null)
	{
		public string NameComputed
			=> $"{Name}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string...what have i become
	}

	public record FileSystemItem(string Path, string Name, SourceGame SourceGame) : FileSystemItemBase(Path, Name, null, new PixelRect((int)SourceGame * 16, 0, 16, 16));

	public record FileSystemItemGroup(string Path, ObjectType ObjectType, ObservableCollection<FileSystemItemBase> SubNodes, int SpriteOffsetIndex)
		: FileSystemItemBase(Path, ObjectType.ToString(), SubNodes, new PixelRect(32 * SpriteOffsetIndex, 0, 32, 32));

	public record FileSystemVehicleGroup(string Path, VehicleType VehicleType, ObservableCollection<FileSystemItemBase> SubNodes, int SpriteOffsetIndex)
		: FileSystemItemBase(Path, VehicleType.ToString(), SubNodes, new PixelRect(32 * SpriteOffsetIndex, 0, 32, 32));
}
