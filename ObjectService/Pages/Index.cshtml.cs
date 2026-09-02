using Definitions;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages;

public sealed class IndexModel : PageModel
{
	readonly ObjectExplorerService _explorerService;

	public IndexModel(ObjectExplorerService explorerService)
	{
		_explorerService = explorerService;
	}

	[BindProperty(SupportsGet = true)]
	public string? Search { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectType? ObjectType { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectSource? ObjectSource { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectAvailability? Availability { get; set; }

	[BindProperty(SupportsGet = true)]
	public VehicleType? VehicleType { get; set; }

	[BindProperty(Name = "p", SupportsGet = true)]
	public int PageNumber { get; set; } = 1;

	[BindProperty(SupportsGet = true)]
	public string Category { get; set; } = "objects";

	public ObjectBrowsePageViewModel Results { get; private set; } = new(0, 0, 1, 48, []);

	public IReadOnlyList<ObjectType> ObjectTypes { get; } = [.. Enum.GetValues<ObjectType>()];

	public IReadOnlyList<ObjectSource> ObjectSources { get; } = [.. Enum.GetValues<ObjectSource>()];

	public IReadOnlyList<ObjectAvailability> AvailabilityStates { get; } = [.. Enum.GetValues<ObjectAvailability>()];

	public IReadOnlyList<VehicleType> VehicleTypes { get; } = [.. Enum.GetValues<VehicleType>()];

	public IReadOnlyDictionary<string, string> Categories { get; } = new Dictionary<string, string>
	{
		["objects"] = "Objects",
		["scenarios"] = "Scenarios",
		["music"] = "Music",
		["sfx"] = "SFX",
		["ai"] = "AI",
	};

	public async Task OnGetAsync(CancellationToken cancellationToken)
	{
		if (Category != "objects")
		{
			// Only objects browsing is implemented for now; other categories show placeholders.
			Results = new(0, 0, 1, 48, []);
			return;
		}

		Results = await _explorerService.GetObjectsAsync(
			new ObjectBrowseQuery(Search, ObjectType, ObjectSource, Availability, VehicleType, PageNumber),
			cancellationToken);

		PageNumber = Results.Page;
	}
}
