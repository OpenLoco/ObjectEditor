using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Manage.Licences;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	private readonly ICrudService<DtoLicenceEntry, TblLicence> _service;

	public EditModel(ICrudService<DtoLicenceEntry, TblLicence> service)
	{
		_service = service;
	}

	[BindProperty]
	public UniqueObjectId Id { get; set; }

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	[BindProperty]
	public string Text { get; set; } = string.Empty;

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id)
	{
		var licence = await _service.ReadAsync(id, CancellationToken.None);
		if (licence == null)
		{
			return NotFound();
		}

		Id = licence.Id;
		Name = licence.Name;
		Text = licence.Text;
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Licence name is required.";
			return Page();
		}

		var licence = new DtoLicenceEntry(Id, Name.Trim(), Text?.Trim() ?? string.Empty);
		
		try
		{
			var updated = await _service.UpdateAsync(Id, licence, CancellationToken.None);
			if (updated != null)
			{
				return RedirectToPage("/manage/licences");
			}

			ErrorMessage = "Licence not found.";
			return Page();
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating licence: {ex.Message}";
			return Page();
		}
	}
}
