using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;

namespace ObjectService.Pages.Manage.SC5FilePacks;

[Authorize(Policy = "AdminOnly")]
public sealed class IndexModel : PageModel
{
	private readonly LocoDbContext _db;

	public IndexModel(LocoDbContext db)
	{
		_db = db;
	}

	[BindProperty(SupportsGet = true)]
	public string? Search { get; set; }

	public List<SC5FilePackListViewModel> Packs { get; set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public async Task OnGetAsync()
	{
		var query = _db.SC5FilePacks
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Licence)
			.Include(p => p.SC5Files)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(Search))
		{
			var s = Search.Trim();
			query = query.Where(p => p.Name.Contains(s) || (p.Description != null && p.Description.Contains(s)));
		}

		var packList = await query
			.OrderByDescending(p => p.UploadedDate)
			.ToListAsync();

		Packs = packList.Select(p => new SC5FilePackListViewModel(
				p.Id,
				p.Name,
				p.Description ?? "",
				p.UploadedDate,
				p.Authors.Count,
				p.Tags.Count,
				p.Licence?.Name ?? "None",
				p.SC5Files.Count)).ToList();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		var pack = await _db.SC5FilePacks.FindAsync(id);
		if (pack == null)
		{
			ErrorMessage = "Pack not found.";
			return RedirectToPage();
		}

		try
		{
			_db.SC5FilePacks.Remove(pack);
			await _db.SaveChangesAsync();
			SuccessMessage = $"SC5 file pack '{pack.Name}' deleted successfully.";
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Failed to delete pack: {ex.Message}";
		}

		return RedirectToPage();
	}

	public record SC5FilePackListViewModel(
		UniqueObjectId Id,
		string Name,
		string Description,
		DateOnly UploadedDate,
		int AuthorCount,
		int TagCount,
		string Licence,
		int SC5FileCount);
}