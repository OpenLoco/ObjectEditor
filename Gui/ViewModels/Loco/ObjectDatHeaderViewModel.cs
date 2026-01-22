using Dat.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;

namespace Gui.ViewModels;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectDatHeaderViewModel(uint32_t checksum, SawyerEncoding encoding, uint datDataLength) : ReactiveObject
{
	public ObjectDatHeaderViewModel()
		: this(0, SawyerEncoding.Uncompressed, 0)
	{ }

	[Reactive]
	public SawyerEncoding DatEncoding { get; set; } = encoding;

	[ReadOnly(true)]
	public uint32_t DatDataLength { get; } = datDataLength;

	public string DatChecksumHex
	{
		get => string.Format($"0x{datChecksum:X}");
		set => datChecksum = Convert.ToUInt32(value[2..], 16);
	}
	uint32_t datChecksum { get; set; } = checksum;
}
