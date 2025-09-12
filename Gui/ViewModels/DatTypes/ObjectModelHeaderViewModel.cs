using Definitions.ObjectModels.Types;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectModelHeaderViewModel : ReactiveObject, IObjectViewModel<ObjectModelHeader>
{
	// todo: instead of setting ObjectType to a default here, instead prompt user to select a type if appropriate
	public ObjectModelHeaderViewModel()
		: this(string.Empty, 0, ObjectSource.Custom, ObjectType.InterfaceSkin)
	{ }

	public ObjectModelHeaderViewModel(string name, uint checksum, ObjectSource objectSource, ObjectType objectType)
	{
		Name = name;
		Checksum = checksum;
		ObjectSource = objectSource;
		ObjectType = objectType;
	}

	public ObjectModelHeaderViewModel(ObjectModelHeader header)
	{
		Name = header.Name;
		Checksum = header.Checksum;
		ObjectSource = header.ObjectSource;
		ObjectType = header.ObjectType;
	}

	[MaxLength(8)]
	public string Name { get; set; }

	uint32_t Checksum { get; set; }

	public string ChecksumHex
	{
		get => string.Format($"0x{Checksum:X}");
		set => Checksum = Convert.ToUInt32(value[2..], 16);
	}

	public ObjectSource ObjectSource { get; set; }

	public ObjectType ObjectType { get; set; }

	public ObjectModelHeader GetAsModel()
		=> new(Name, Checksum, ObjectType, ObjectSource);
}
