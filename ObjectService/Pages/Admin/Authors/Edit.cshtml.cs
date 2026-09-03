using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Authors;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	private readonly ICrudService<DtoAuthorEntry, TblAuthor> _service;

	public EditModel(ICrudService<DtoAuthorEntry, TblAuthor> service)
	{
		_service = service;
	}

	[BindProperty]
	public UniqueObjectId Id { get; set; }

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id)
	{
		var author = await _service.ReadAsync(id, CancellationToken.None);
		if (author == null)
		{
			return NotFound();
		}

		Id = author.Id;
		Name = author.Name;
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Author name is required.";
			return Page();
		}

		var author = new DtoAuthorEntry(Id, Name.Trim());
		
		try
		{
			var updated = await _service.UpdateAsync(Id, author, CancellationToken.None);
			if (updated != null)
			{
				return RedirectToPage("/Admin/Authors/Index");
			}

			ErrorMessage = "Author not found.";
			return Page();
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating author: {ex.Message}";
			return Page();
		}
	}
}
