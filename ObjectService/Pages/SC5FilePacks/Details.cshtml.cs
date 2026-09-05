using Definitions.Database;
using Definitions.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObjectService.Identity;

namespace ObjectService.Pages.SC5FilePacks;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;

	public DetailsModel(LocoDbContext db)
	{
		_db = db;
	}

	public TblSC5FilePack? Pack { get; private set; }

	public List<ListItem> SC5Files { get; private set; } = [];

	// ── Edit form available values ──
	public List<DtoAuthorEntry> AvailableAuthors { get; private set; } = [];
	public List<DtoTagEntry> AvailableTags { get; private set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; private set; } = [];
	public List<DtoScenarioEntry> AvailableScenarios { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public bool CanEdit => User.IsInRole("Admin") || User.IsInRole("Curator")
		|| User.HasClaim(LocoPermissions.ClaimType, LocoPermissions.SC5FilePacksModify);

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		Pack = await _db.SC5FilePacks
			.Include(p => p.Licence)
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.SC5Files)
			.AsSplitQuery()
			.FirstOrDefaultAsync(p => p.Id == id, ct);

		if (Pack is null)
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
		[FromForm] List<UniqueObjectId>? SelectedSC5FileIds)
	{
		if (!CanEdit)
			return Forbid();

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Scenario pack name is required.";
			await ReloadAsync(Id);
			return Page();
		}

		SelectedAuthorIds ??= [];
		SelectedTagIds ??= [];
		SelectedSC5FileIds ??= [];

		try
		{
			var pack = await _db.SC5FilePacks
				.Include(p => p.Licence)
				.Include(p => p.Authors)
				.Include(p => p.Tags)
				.Include(p => p.SC5Files)
				.AsSplitQuery()
				.FirstOrDefaultAsync(p => p.Id == Id);

			if (pack is null)
			{
				ErrorMessage = "Scenario pack not found.";
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

			// SC5 Files (scenarios)
			pack.SC5Files.Clear();
			foreach (var fileId in SelectedSC5FileIds)
			{
				var file = await _db.SC5Files.FindAsync(new object[] { (object)fileId });
				if (file != null)
					pack.SC5Files.Add(file);
			}

			await _db.SaveChangesAsync();
			SuccessMessage = $"Scenario pack '{Name.Trim()}' updated.";
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating scenario pack: {ex.Message}";
		}

		await ReloadAsync(Id);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!CanEdit)
			return Forbid();

		var pack = await _db.SC5FilePacks.FindAsync(new object[] { (object)id });
		if (pack is null)
		{
			await ReloadAsync(id);
			ErrorMessage = "Failed to delete scenario pack.";
			return Page();
		}

		_db.SC5FilePacks.Remove(pack);
		await _db.SaveChangesAsync();

		SuccessMessage = "Scenario pack deleted.";
		return RedirectToPage("/Index", new { category = "sc5filepacks" });
	}

	private async Task ReloadAsync(UniqueObjectId id)
	{
		Pack = await _db.SC5FilePacks
			.Include(p => p.Licence)
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.SC5Files)
			.AsSplitQuery()
			.FirstOrDefaultAsync(p => p.Id == id);

		await LoadRelatedDataAsync(CancellationToken.None);
	}

	private async Task LoadRelatedDataAsync(CancellationToken ct)
	{
		if (Pack is not null)
		{
			SC5Files = Pack.SC5Files
				.OrderBy(f => f.Name)
				.Select(f => new ListItem(f.Id, f.Name, "SC5Files", null))
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

		AvailableScenarios = await _db.SC5Files
			.OrderBy(s => s.Name)
			.Select(s => new DtoScenarioEntry(s.Id, s.Name))
			.ToListAsync(ct);
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}