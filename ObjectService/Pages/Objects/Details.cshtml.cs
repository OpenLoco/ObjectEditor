using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages.Objects;

public sealed class DetailsModel : PageModel
{
	readonly ObjectExplorerService _explorerService;

	public DetailsModel(ObjectExplorerService explorerService)
	{
		_explorerService = explorerService;
	}

	public ObjectDetailViewModel? ObjectDetails { get; private set; }

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken cancellationToken)
	{
		ObjectDetails = await _explorerService.GetObjectAsync(id, cancellationToken);
		return ObjectDetails == null ? NotFound() : Page();
	}
}