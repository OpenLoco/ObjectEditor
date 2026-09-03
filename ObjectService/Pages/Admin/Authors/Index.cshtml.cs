using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Authors;

[Authorize(Policy = "AdminOnly")]
public sealed class AuthorsIndexModel : PageModel
{
	private readonly ICrudService<DtoAuthorEntry, TblAuthor> _service;

	public AuthorsIndexModel(ICrudService<DtoAuthorEntry, TblAuthor> service)
	{
		_service = service;
	}

	public List<DtoAuthorEntry> Authors { get; set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public async Task OnGetAsync()
	{
		Authors = (await _service.ListAsync(HttpContext, CancellationToken.None)).ToList();
		Authors = Authors.OrderBy(a => a.Name).ToList();
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
