using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;

namespace ObjectService.Pages.Manage.ObjectPacks;

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

	public List<ObjectPackListViewModel> ObjectPacks { get; set; } = [];

	public async Task OnGetAsync()
	{
		var query = _db.ObjectPacks
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Licence)
			.Include(p => p.Objects)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(Search))
		{
			var s = Search.Trim();
			query = query.Where(p => p.Name.Contains(s) || (p.Description != null && p.Description.Contains(s)));
		}

		var packList = await query
			.OrderByDescending(p => p.UploadedDate)
			.ToListAsync();

		ObjectPacks = packList.Select(p => new ObjectPackListViewModel(
				p.Id,
				p.Name,
				p.Description ?? "",
				p.UploadedDate,
				p.Authors.Count,
				p.Tags.Count,
				p.Licence != null ? p.Licence.Name : "None",
				p.Objects.Count)).ToList();
	}

	public record ObjectPackListViewModel(
		UniqueObjectId Id,
		string Name,
		string Description,
		DateOnly UploadedDate,
		int AuthorCount,
		int TagCount,
		string Licence,
		int ObjectCount);
}
