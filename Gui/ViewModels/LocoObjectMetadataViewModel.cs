using Definitions.ObjectModels;
using ReactiveUI;

namespace Gui.ViewModels;

public class LocoObjectMetadataViewModel(LocoObjectMetadata metadata) : ReactiveObject
{
	public LocoObjectMetadata Metadata { get; } = metadata;

}
