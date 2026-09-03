using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Licences;

[Authorize(Policy = "AdminOnly")]
public sealed class IndexModel : PageModel
{
	private readonly ICrudService<DtoLicenceEntry, TblLicence> _service;

	public IndexModel(ICrudService<DtoLicenceEntry, TblLicence> service)
	{
		_service = service;
	}

	public List<DtoLicenceEntry> Licences { get; set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public async Task OnGetAsync()
	{
		Licences = (await _service.ListAsync(HttpContext, CancellationToken.None)).ToList();
		Licences = Licences.OrderBy(l => l.Name).ToList();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		var deleted = await _service.DeleteAsync(id, CancellationToken.None);
		if (deleted)
		{
			SuccessMessage = "Licence deleted successfully.";
		}
		else
		{
			ErrorMessage = "Failed to delete licence.";
		}

		return RedirectToPage();
	}
}
