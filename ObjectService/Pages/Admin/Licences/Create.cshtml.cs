using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Licences;

[Authorize(Policy = "AdminOnly")]
public sealed class CreateModel : PageModel
{
	private readonly ICrudService<DtoLicenceEntry, TblLicence> _service;

	public CreateModel(ICrudService<DtoLicenceEntry, TblLicence> service)
	{
		_service = service;
	}

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	[BindProperty]
	public string Text { get; set; } = string.Empty;

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Licence name is required.";
			return Page();
		}

		var licence = new DtoLicenceEntry(0, Name.Trim(), Text?.Trim() ?? string.Empty);
		
		if (!_service.TryValidateCreate(licence, out var validationError))
		{
			ErrorMessage = validationError;
			return Page();
		}

		try
		{
			await _service.CreateAsync(licence, CancellationToken.None);
			return RedirectToPage("/Admin/Licences/Index");
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error creating licence: {ex.Message}";
			return Page();
		}
	}
}
