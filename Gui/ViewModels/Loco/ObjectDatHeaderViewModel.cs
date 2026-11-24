using Dat.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace Gui.ViewModels;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectDatHeaderViewModel(SawyerEncoding encoding, uint datDataLength) : ReactiveObject, IViewModel
{
	[Browsable(false)]
	public string DisplayName
		=> "Dat Header";

	public ObjectDatHeaderViewModel()
		: this(SawyerEncoding.Uncompressed, 0)
	{ }

	[Reactive]
	public SawyerEncoding Encoding { get; set; } = encoding;

	[ReadOnly(true)]
	public uint32_t DataLength { get; } = datDataLength;
}
