using Dat.Data;
using Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace Gui.ViewModels;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectHeaderViewModel(SawyerEncoding encoding, uint datDataLength) : ReactiveObject
{
	public ObjectHeaderViewModel()
		: this(SawyerEncoding.Uncompressed, 0)
	{ }

	public ObjectHeaderViewModel(ObjectHeader objectHeader)
		: this(objectHeader.Encoding, objectHeader.DataLength)
	{ }

	[Reactive]
	public SawyerEncoding DatEncoding { get; set; } = encoding;

	[ReadOnly(true)]
	public uint32_t DatDataLength { get; } = datDataLength;
}
