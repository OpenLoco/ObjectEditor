using Definitions.Web;

namespace Gui.Models;

public enum OnlineApiEndpointGroup
{
	Objects,
	ObjectPacks,
	Scenarios,
	ScenarioPacks,
	Music,
	SoundEffects,
	Tutorials,
	Graphics,
	Tags,
	Authors,
	Licences,
	MissingObjects,
}

public record OnlineBrowseTargetOption(
	OnlineApiEndpointGroup Group,
	string DisplayName,
	string ItemLabelPlural,
	ApiEndpointGroup EndpointGroup);
