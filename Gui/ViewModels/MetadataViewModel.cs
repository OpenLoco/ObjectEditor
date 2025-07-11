using Gui.Models;
using ReactiveUI;

namespace Gui.ViewModels
{
	public class MetadataViewModel(MetadataModel metadata) : ReactiveObject
	{
		public MetadataModel Metadata { get; } = metadata;

	}
}
