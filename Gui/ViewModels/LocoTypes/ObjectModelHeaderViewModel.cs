using Definitions.ObjectModels.Types;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectModelHeaderViewModel : ReactiveObject, IObjectViewModel<ObjectModelHeader>
{
	public ObjectModelHeaderViewModel(string name, uint datChecksum, ObjectSource objectSource, ObjectType objectType)
	{
		Name = name;
		DatChecksum = datChecksum;
		ObjectSource = objectSource;
		ObjectType = objectType;
	}

	public ObjectModelHeaderViewModel(ObjectModelHeader header)
	{
		Name = header.Name;
		DatChecksum = header.DatChecksum;
		ObjectSource = header.ObjectSource;
		ObjectType = header.ObjectType;
	}

	[MaxLength(8)]
	public string Name { get; set; }

	uint32_t DatChecksum { get; set; }

	public string DatChecksumHex
	{
		get => string.Format($"0x{DatChecksum:X}");
		set => DatChecksum = Convert.ToUInt32(value[2..], 16);
	}

	public ObjectSource ObjectSource { get; set; }

	public ObjectType ObjectType { get; set; }

	public ObjectModelHeader GetAsModel()
		=> new(Name, ObjectType, ObjectSource, DatChecksum);
}
