using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Tags;

[Authorize(Policy = "AdminOnly")]
public sealed class IndexModel : PageModel
{
	private readonly ICrudService<DtoTagEntry, TblTag> _service;

	public IndexModel(ICrudService<DtoTagEntry, TblTag> service)
	{
		_service = service;
	}

	public List<DtoTagEntry> Tags { get; set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public async Task OnGetAsync()
	{
		Tags = (await _service.ListAsync(HttpContext, CancellationToken.None)).ToList();
		Tags = Tags.OrderBy(t => t.Name).ToList();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		var deleted = await _service.DeleteAsync(id, CancellationToken.None);
		if (deleted)
		{
			SuccessMessage = "Tag deleted successfully.";
		}
		else
		{
			ErrorMessage = "Failed to delete tag.";
		}

		return RedirectToPage();
	}
}
