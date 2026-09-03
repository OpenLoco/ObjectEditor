using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Manage.Authors;

[Authorize(Policy = "AdminOnly")]
public sealed class AuthorsIndexModel : PageModel
{
	private readonly ICrudService<DtoAuthorEntry, TblAuthor> _service;

	public AuthorsIndexModel(ICrudService<DtoAuthorEntry, TblAuthor> service)
	{
		_service = service;
	}

	[BindProperty(SupportsGet = true)]
	public string? Search { get; set; }

	public List<DtoAuthorEntry> Authors { get; set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public async Task OnGetAsync()
	{
		Authors = (await _service.ListAsync(HttpContext, CancellationToken.None)).ToList();
		Authors = Authors.OrderBy(a => a.Name).ToList();

		if (!string.IsNullOrWhiteSpace(Search))
		{
			var s = Search.Trim();
			Authors = Authors.Where(a => a.Name.Contains(s, StringComparison.OrdinalIgnoreCase)).ToList();
		}
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		var deleted = await _service.DeleteAsync(id, CancellationToken.None);
		if (deleted)
		{
			SuccessMessage = "Author deleted successfully.";
		}
		else
		{
			ErrorMessage = "Failed to delete author.";
		}

		return RedirectToPage();
	}
}
