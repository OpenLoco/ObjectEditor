using Definitions.ObjectModels.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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

		_ = this.WhenAnyValue(o => o.Checksum)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(ChecksumHex)));
	}

	[Reactive, MaxLength(8)]
	public string Name { get; set; }

	[Reactive]
	public uint32_t Checksum { get; set; }

	public string ChecksumHex
		=> string.Format($"{Checksum:X}");

	[Reactive]
	public ObjectSource ObjectSource { get; set; }

	[Reactive]
	public ObjectType ObjectType { get; set; }

	public ObjectModelHeader GetAsModel()
		=> new(Name, Checksum, ObjectType, ObjectSource);
}
