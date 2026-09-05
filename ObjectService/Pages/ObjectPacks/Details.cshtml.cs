using Definitions.Database;
using Definitions.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObjectService.Identity;

namespace ObjectService.Pages.ObjectPacks;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;

	public DetailsModel(LocoDbContext db)
	{
		_db = db;
	}

	public TblObjectPack? ObjectPack { get; private set; }

	public List<ListItem> Objects { get; private set; } = [];

	// ── Edit form available values ──
	public List<DtoAuthorEntry> AvailableAuthors { get; private set; } = [];
	public List<DtoTagEntry> AvailableTags { get; private set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; private set; } = [];
	public List<DtoObjectEntry> AvailableObjects { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public bool CanEdit => User.IsInRole("Admin") || User.IsInRole("Curator")
		|| User.HasClaim(LocoPermissions.ClaimType, LocoPermissions.ObjectPacksModify);

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		ObjectPack = await _db.ObjectPacks
			.Include(p => p.Licence)
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Objects)
			.AsSplitQuery()
			.FirstOrDefaultAsync(p => p.Id == id, ct);

		if (ObjectPack is null)
		{
			return NotFound();
		}

		await LoadRelatedDataAsync(ct);

		return Page();
	}

	public async Task<IActionResult> OnPostEditAsync(
		[FromForm] UniqueObjectId Id,
		[FromForm] string Name,
		[FromForm] string? Description,
		[FromForm] DateOnly? CreatedDate,
		[FromForm] DateOnly? ModifiedDate,
		[FromForm] UniqueObjectId? LicenceId,
		[FromForm] List<UniqueObjectId>? SelectedAuthorIds,
		[FromForm] List<UniqueObjectId>? SelectedTagIds,
		[FromForm] List<UniqueObjectId>? SelectedObjectIds)
	{
		if (!CanEdit)
			return Forbid();

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Object pack name is required.";
			await ReloadAsync(Id);
			return Page();
		}

		SelectedAuthorIds ??= [];
		SelectedTagIds ??= [];
		SelectedObjectIds ??= [];

		try
		{
			var pack = await _db.ObjectPacks
				.Include(p => p.Licence)
				.Include(p => p.Authors)
				.Include(p => p.Tags)
				.Include(p => p.Objects)
				.AsSplitQuery()
				.FirstOrDefaultAsync(p => p.Id == Id);

			if (pack is null)
			{
				ErrorMessage = "Object pack not found.";
				return Page();
			}

			// Basic properties
			pack.Name = Name.Trim();
			pack.Description = Description?.Trim();
			pack.CreatedDate = CreatedDate;
			pack.ModifiedDate = ModifiedDate;
			// UploadedDate is a database-generated computed column — do not set it

			// Licence
			if (LicenceId.HasValue)
			{
				var licence = await _db.Licences.FindAsync(new object[] { (object)LicenceId.Value });
				pack.Licence = licence;
			}
			else
			{
				pack.Licence = null;
			}

			// Authors
			pack.Authors.Clear();
			foreach (var authorId in SelectedAuthorIds)
			{
				var author = await _db.Authors.FindAsync(new object[] { (object)authorId });
				if (author != null)
					pack.Authors.Add(author);
			}

			// Tags
			pack.Tags.Clear();
			foreach (var tagId in SelectedTagIds)
			{
				var tag = await _db.Tags.FindAsync(new object[] { (object)tagId });
				if (tag != null)
					pack.Tags.Add(tag);
			}

			// Objects
			pack.Objects.Clear();
			foreach (var objId in SelectedObjectIds)
			{
				var obj = await _db.Objects.FindAsync(new object[] { (object)objId });
				if (obj != null)
					pack.Objects.Add(obj);
			}

			await _db.SaveChangesAsync();
			SuccessMessage = $"Object pack '{Name.Trim()}' updated.";
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating object pack: {ex.Message}";
		}

		await ReloadAsync(Id);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!CanEdit)
			return Forbid();

		var pack = await _db.ObjectPacks.FindAsync(new object[] { (object)id });
		if (pack is null)
		{
			await ReloadAsync(id);
			ErrorMessage = "Failed to delete object pack.";
			return Page();
		}

		_db.ObjectPacks.Remove(pack);
		await _db.SaveChangesAsync();

		SuccessMessage = "Object pack deleted.";
		return RedirectToPage("/Index", new { category = "objectpacks" });
	}

	private async Task ReloadAsync(UniqueObjectId id)
	{
		ObjectPack = await _db.ObjectPacks
			.Include(p => p.Licence)
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Objects)
			.AsSplitQuery()
			.FirstOrDefaultAsync(p => p.Id == id);

		await LoadRelatedDataAsync(CancellationToken.None);
	}

	private async Task LoadRelatedDataAsync(CancellationToken ct)
	{
		if (ObjectPack is not null)
		{
			Objects = ObjectPack.Objects
				.OrderBy(o => o.Name)
				.Select(o => new ListItem(o.Id, o.Name, "Objects", null))
				.ToList();
		}

		AvailableAuthors = await _db.Authors
			.OrderBy(a => a.Name)
			.Select(a => new DtoAuthorEntry(a.Id, a.Name))
			.ToListAsync(ct);

		AvailableTags = await _db.Tags
			.OrderBy(t => t.Name)
			.Select(t => new DtoTagEntry(t.Id, t.Name))
			.ToListAsync(ct);

		AvailableLicences = await _db.Licences
			.OrderBy(l => l.Name)
			.Select(l => new DtoLicenceEntry(l.Id, l.Name, l.Text))
			.ToListAsync(ct);

		AvailableObjects = await _db.Objects
			.OrderBy(o => o.Name)
			.Select(o => new DtoObjectEntry(o.Id, o.Name, o.Name, null, null,
				o.ObjectSource,
				o.ObjectType,
				null,
				o.Availability,
				null, null, o.UploadedDate))
			.ToListAsync(ct);
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}