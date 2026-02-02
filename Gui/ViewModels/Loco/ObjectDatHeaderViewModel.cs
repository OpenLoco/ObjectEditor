using Dat.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;

namespace Gui.ViewModels;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectDatHeaderViewModel(uint32_t checksum, SawyerEncoding encoding, uint datDataLength) : ReactiveObject, IViewModel
{
	[Browsable(false)]
	public string DisplayName
		=> "Dat Header";

	public ObjectDatHeaderViewModel()
		: this(0, SawyerEncoding.Uncompressed, 0)
	{ }

	[Reactive]
	public SawyerEncoding Encoding { get; set; } = encoding;

	[ReadOnly(true)]
	public uint32_t DataLength { get; } = datDataLength;

	public string ChecksumHex
	{
		get => string.Format($"0x{_checksum:X}");
		set => _checksum = Convert.ToUInt32(value[2..], 16);
	}
	uint32_t _checksum { get; set; } = checksum;
}
