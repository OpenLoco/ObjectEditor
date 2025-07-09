using OpenLoco.Gui.Models;
using ReactiveUI;

namespace OpenLoco.Gui.ViewModels
{
	public class MetadataViewModel(MetadataModel metadata) : ReactiveObject
	{
		public MetadataModel Metadata { get; } = metadata;

	}
}
