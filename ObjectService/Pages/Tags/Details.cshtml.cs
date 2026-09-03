using Definitions.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Pages.Tags;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;

	public DetailsModel(LocoDbContext db)
	{
		_db = db;
	}

	public TblTag? Tag { get; private set; }

	public List<ListItem> Objects { get; private set; } = [];
	public List<ListItem> ObjectPacks { get; private set; } = [];
	public List<ListItem> SC5Files { get; private set; } = [];
	public List<ListItem> SC5FilePacks { get; private set; } = [];

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		Tag = await _db.Tags
			.Include(t => t.Objects)
			.Include(t => t.ObjectPacks)
			.Include(t => t.SC5Files)
			.Include(t => t.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(t => t.Id == id, ct);

		if (Tag is null)
		{
			return NotFound();
		}

		Objects = Tag.Objects
			.OrderBy(o => o.Name)
			.Select(o => new ListItem(o.Id, o.Description ?? o.Name, "Objects", null))
			.ToList();

		ObjectPacks = Tag.ObjectPacks
			.OrderBy(p => p.Name)
			.Select(p => new ListItem(p.Id, p.Name, "ObjectPacks", null))
			.ToList();

		SC5Files = Tag.SC5Files
			.OrderBy(f => f.Name)
			.Select(f => new ListItem(f.Id, f.Name, "SC5Files", null))
			.ToList();

		SC5FilePacks = Tag.SC5FilePacks
			.OrderBy(p => p.Name)
			.Select(p => new ListItem(p.Id, p.Name, "SC5FilePacks", null))
			.ToList();

		return Page();
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}