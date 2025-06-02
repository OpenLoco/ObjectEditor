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

	public record FileSystemItemBase(
		string Filename,
		string DisplayName,
		string UniqueName,
		DateTimeOffset? CreatedDate = null,
		DateTimeOffset? ModifiedDate = null,
		FileLocation? FileLocation = null,
		ObjectSource? ObjectSource = null,
		ObjectType? ObjectType = null,
		VehicleType? VehicleType = null,
		ObservableCollection<FileSystemItemBase>? SubNodes = null)
	{
		public string NameComputed
			=> $"{DisplayName}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string...what have i become
	}
}
