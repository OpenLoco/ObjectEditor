using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaGui.Models
{
	public enum FileSystemItemType
	{
		Item,
		Group,
		VehicleGroup
	}

	public abstract record FileSystemItemBase(string Path, string Name, FileSystemItemType Type, ObservableCollection<FileSystemItemBase>? SubNodes = null, PixelRect? SourceRect = null)
	{
		public string NameComputed
			=> $"{Name}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string..what have i become
	}

	public record FileSystemItem(string Path, string Name) : FileSystemItemBase(Path, Name, FileSystemItemType.Item);

	public record FileSystemItemGroup(string Path, ObjectType ObjectType, ObservableCollection<FileSystemItemBase> SubNodes, int SpriteOffsetIndex)
		: FileSystemItemBase(Path, ObjectType.ToString(), FileSystemItemType.Group, SubNodes, new PixelRect(32 * SpriteOffsetIndex, 0, 32, 32));

	public record FileSystemVehicleGroup(string Path, VehicleType VehicleType, ObservableCollection<FileSystemItemBase> SubNodes, int SpriteOffsetIndex)
		: FileSystemItemBase(Path, VehicleType.ToString(), FileSystemItemType.VehicleGroup, SubNodes, new PixelRect(0, 0, 1, 1));
}
