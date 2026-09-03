using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.ObjectModels.Types;

namespace ObjectService.Pages.Admin.Objects;

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

	[BindProperty(SupportsGet = true)]
	public ObjectType? ObjectType { get; set; }

	[BindProperty(Name = "p", SupportsGet = true)]
	public int PageNumber { get; set; } = 1;

	public List<ObjectListViewModel> Objects { get; set; } = [];
	public int TotalCount { get; set; }
	public int PageSize { get; } = 50;

	[TempData]
	public string? SuccessMessage { get; set; }

	public IReadOnlyList<ObjectType> ObjectTypes { get; } = [.. Enum.GetValues<ObjectType>()];

	public async Task OnGetAsync()
	{
		var query = _db.Objects.AsQueryable();

		if (!string.IsNullOrWhiteSpace(Search))
		{
			var searchLower = Search.ToLower();
			query = query.Where(o => o.Name.ToLower().Contains(searchLower));
		}

		if (ObjectType.HasValue)
		{
			query = query.Where(o => o.ObjectType == ObjectType.Value);
		}

		TotalCount = await query.CountAsync();

			var objectsQuery = query.OrderByDescending(o => o.UploadedDate)
				.Skip((PageNumber - 1) * PageSize)
				.Take(PageSize)
				.Include(o => o.Authors)
				.Include(o => o.Tags)
				.Include(o => o.Licence)
				.Include(o => o.DatObjects);

			var objectList = await objectsQuery.ToListAsync();
			Objects = objectList.Select(o => new ObjectListViewModel(
				o.Id,
				o.Name,
				o.ObjectType,
				o.UploadedDate,
				o.Authors.Count,
				o.Tags.Count,
				o.Licence != null ? o.Licence.Name : "None",
				o.DatObjects.Count)).ToList();
	}

	public record ObjectListViewModel(
		UniqueObjectId Id,
		string Name,
		ObjectType ObjectType,
		DateOnly UploadedDate,
		int AuthorCount,
		int TagCount,
		string Licence,
		int DatObjectCount);
}
