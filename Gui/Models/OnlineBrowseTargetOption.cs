using Definitions.Web;

namespace Gui.Models;

public enum OnlineApiEndpointGroup
{
	Objects,
	ObjectPacks,
	Scenarios,
}

public record OnlineBrowseTargetOption(
	OnlineApiEndpointGroup Group,
	string DisplayName,
	string ItemLabelPlural,
	ApiEndpointGroup EndpointGroup);
