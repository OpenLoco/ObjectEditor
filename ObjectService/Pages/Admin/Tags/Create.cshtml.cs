using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Tags;

[Authorize(Policy = "AdminOnly")]
public sealed class CreateModel : PageModel
{
	private readonly ICrudService<DtoTagEntry, TblTag> _service;

	public CreateModel(ICrudService<DtoTagEntry, TblTag> service)
	{
		_service = service;
	}

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Tag name is required.";
			return Page();
		}

		var tag = new DtoTagEntry(0, Name.Trim());
		
		if (!_service.TryValidateCreate(tag, out var validationError))
		{
			ErrorMessage = validationError;
			return Page();
		}

		try
		{
			await _service.CreateAsync(tag, CancellationToken.None);
			return RedirectToPage("/Admin/Tags/Index");
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error creating tag: {ex.Message}";
			return Page();
		}
	}
}
