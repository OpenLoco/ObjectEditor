using Definitions.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Pages.Licences;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;

	public DetailsModel(LocoDbContext db)
	{
		_db = db;
	}

	public TblLicence? Licence { get; private set; }

	public List<ListItem> Objects { get; private set; } = [];
	public List<ListItem> ObjectPacks { get; private set; } = [];
	public List<ListItem> SC5Files { get; private set; } = [];
	public List<ListItem> SC5FilePacks { get; private set; } = [];

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		Licence = await _db.Licences
			.FirstOrDefaultAsync(l => l.Id == id, ct);

		if (Licence is null)
		{
			return NotFound();
		}

		var licenceId = Licence.Id;

		Objects = await _db.Objects
			.Where(o => o.Licence != null && o.Licence.Id == licenceId)
			.OrderBy(o => o.Name)
			.Select(o => new ListItem(o.Id, o.Description ?? o.Name, "Objects", null))
			.ToListAsync(ct);

		ObjectPacks = await _db.ObjectPacks
			.Where(p => p.Licence != null && p.Licence.Id == licenceId)
			.OrderBy(p => p.Name)
			.Select(p => new ListItem(p.Id, p.Name, "ObjectPacks", null))
			.ToListAsync(ct);

		SC5Files = await _db.SC5Files
			.Where(f => f.Licence != null && f.Licence.Id == licenceId)
			.OrderBy(f => f.Name)
			.Select(f => new ListItem(f.Id, f.Name, "SC5Files", null))
			.ToListAsync(ct);

		SC5FilePacks = await _db.SC5FilePacks
			.Where(p => p.Licence != null && p.Licence.Id == licenceId)
			.OrderBy(p => p.Name)
			.Select(p => new ListItem(p.Id, p.Name, "SC5FilePacks", null))
			.ToListAsync(ct);

		return Page();
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}