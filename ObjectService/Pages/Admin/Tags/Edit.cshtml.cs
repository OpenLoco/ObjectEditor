using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Admin.Tags;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	private readonly ICrudService<DtoTagEntry, TblTag> _service;

	public EditModel(ICrudService<DtoTagEntry, TblTag> service)
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
		var tag = await _service.ReadAsync(id, CancellationToken.None);
		if (tag == null)
		{
			return NotFound();
		}

		Id = tag.Id;
		Name = tag.Name;
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Tag name is required.";
			return Page();
		}

		var tag = new DtoTagEntry(Id, Name.Trim());
		
		try
		{
			var updated = await _service.UpdateAsync(Id, tag, CancellationToken.None);
			if (updated != null)
			{
				return RedirectToPage("/Admin/Tags/Index");
			}

			ErrorMessage = "Tag not found.";
			return Page();
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating tag: {ex.Message}";
			return Page();
		}
	}
}
