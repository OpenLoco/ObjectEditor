using Definitions.ObjectModels.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class DesignerObjectModelHeaderViewModel : ObjectModelHeaderViewModel
{
	public DesignerObjectModelHeaderViewModel()
		: base(new ObjectModelHeader("TestObj", ObjectType.Building, ObjectSource.Custom, 0))
	{ }
}

public class ObjectModelHeaderViewModel(ObjectModelHeader model)
	: LocoObjectViewModel<ObjectModelHeader>(model)
{
	[MaxLength(8)]
	public string Name
	{
		get => Model.Name;
		set => Model.Name = value;
	}

	public ObjectSource ObjectSource
	{
		get => Model.ObjectSource;
		set => Model.ObjectSource = value;
	}

	public ObjectType ObjectType
	{
		get => Model.ObjectType;
		set => Model.ObjectType = value;
	}

	public string DatChecksumHex
	{
		get => string.Format($"0x{Model.DatChecksum:X}");
		set => Model.DatChecksum = Convert.ToUInt32(value[2..], 16);
	}
}
