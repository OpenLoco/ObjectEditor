using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Authors;

[Authorize(Policy = "AdminOnly")]
public sealed class CreateModel : PageModel
{
	private readonly ICrudService<DtoAuthorEntry, TblAuthor> _service;

	public CreateModel(ICrudService<DtoAuthorEntry, TblAuthor> service)
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
			ErrorMessage = "Author name is required.";
			return Page();
		}

		var author = new DtoAuthorEntry(0, Name.Trim());
		
		if (!_service.TryValidateCreate(author, out var validationError))
		{
			ErrorMessage = validationError;
			return Page();
		}

		try
		{
			await _service.CreateAsync(author, CancellationToken.None);
			return RedirectToPage("/Admin/Authors/Index");
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error creating author: {ex.Message}";
			return Page();
		}
	}
}
