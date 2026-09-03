using Definitions.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Pages.Authors;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;

	public DetailsModel(LocoDbContext db)
	{
		_db = db;
	}

	public TblAuthor? Author { get; private set; }

	public List<ListItem> Objects { get; private set; } = [];
	public List<ListItem> ObjectPacks { get; private set; } = [];
	public List<ListItem> SC5Files { get; private set; } = [];
	public List<ListItem> SC5FilePacks { get; private set; } = [];

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		Author = await _db.Authors
			.Include(a => a.Objects)
			.Include(a => a.ObjectPacks)
			.Include(a => a.SC5Files)
			.Include(a => a.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(a => a.Id == id, ct);

		if (Author is null)
		{
			return NotFound();
		}

		Objects = Author.Objects
			.OrderBy(o => o.Name)
			.Select(o => new ListItem(o.Id, o.Description ?? o.Name, "Objects", null))
			.ToList();

		ObjectPacks = Author.ObjectPacks
			.OrderBy(p => p.Name)
			.Select(p => new ListItem(p.Id, p.Name, "ObjectPacks", null))
			.ToList();

		SC5Files = Author.SC5Files
			.OrderBy(f => f.Name)
			.Select(f => new ListItem(f.Id, f.Name, "SC5Files", null))
			.ToList();

		SC5FilePacks = Author.SC5FilePacks
			.OrderBy(p => p.Name)
			.Select(p => new ListItem(p.Id, p.Name, "SC5FilePacks", null))
			.ToList();

		return Page();
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}