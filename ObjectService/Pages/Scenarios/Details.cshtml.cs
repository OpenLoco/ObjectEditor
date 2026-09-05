using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Pages.Scenarios;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;

	public DetailsModel(LocoDbContext db)
	{
		_db = db;
	}

	public TblSC5File? Scenario { get; private set; }

	public List<ListItem> SC5FilePacks { get; private set; } = [];

	// ── Edit form available values ──
	public List<DtoAuthorEntry> AvailableAuthors { get; private set; } = [];
	public List<DtoTagEntry> AvailableTags { get; private set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; private set; } = [];
	public List<DtoScenarioEntry> AvailablePacks { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	// ── Edit form bindings ──

	[BindProperty]
	public UniqueObjectId Id { get; set; }

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	[BindProperty]
	public string? Description { get; set; }

	[BindProperty]
	public ObjectSource ObjectSource { get; set; }

	[BindProperty]
	public DateOnly? CreatedDate { get; set; }

	[BindProperty]
	public DateOnly? ModifiedDate { get; set; }

	[BindProperty]
	public DateOnly UploadedDate { get; set; }

	[BindProperty]
	public UniqueObjectId? LicenceId { get; set; }

	[BindProperty]
	public List<UniqueObjectId> SelectedAuthorIds { get; set; } = [];

	[BindProperty]
	public List<UniqueObjectId> SelectedTagIds { get; set; } = [];

	[BindProperty]
	public List<UniqueObjectId> SelectedPackIds { get; set; } = [];

	public bool CanEdit => User.IsInRole("Admin") || User.IsInRole("Curator");

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		Scenario = await _db.SC5Files
			.Include(s => s.Licence)
			.Include(s => s.Authors)
			.Include(s => s.Tags)
			.Include(s => s.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(s => s.Id == id, ct);

		if (Scenario is null)
		{
			return NotFound();
		}

		await LoadRelatedDataAsync(ct);

		return Page();
	}

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!CanEdit)
			return Forbid();

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Scenario name is required.";
			await ReloadAsync(Id);
			return Page();
		}

		try
		{
			var scenario = await _db.SC5Files
				.Include(s => s.Licence)
				.Include(s => s.Authors)
				.Include(s => s.Tags)
				.Include(s => s.SC5FilePacks)
				.AsSplitQuery()
				.FirstOrDefaultAsync(s => s.Id == Id);

			if (scenario is null)
			{
				ErrorMessage = "Scenario not found.";
				return Page();
			}

			// Basic properties
			scenario.Name = Name.Trim();
			scenario.Description = Description?.Trim();
			scenario.ObjectSource = ObjectSource;
			scenario.CreatedDate = CreatedDate;
			scenario.ModifiedDate = ModifiedDate;

			// Licence
			if (LicenceId.HasValue)
			{
				var licence = await _db.Licences.FindAsync(new object[] { (object)LicenceId.Value });
				scenario.Licence = licence;
			}
			else
			{
				scenario.Licence = null;
			}

			// Authors
			scenario.Authors.Clear();
			if (SelectedAuthorIds.Count > 0)
			{
				var authors = await _db.Authors
					.Where(a => SelectedAuthorIds.Contains(a.Id))
					.ToListAsync();
				foreach (var author in authors)
					scenario.Authors.Add(author);
			}

			// Tags
			scenario.Tags.Clear();
			if (SelectedTagIds.Count > 0)
			{
				var tags = await _db.Tags
					.Where(t => SelectedTagIds.Contains(t.Id))
					.ToListAsync();
				foreach (var tag in tags)
					scenario.Tags.Add(tag);
			}

			// SC5 File Packs
			scenario.SC5FilePacks.Clear();
			if (SelectedPackIds.Count > 0)
			{
				var packs = await _db.SC5FilePacks
					.Where(p => SelectedPackIds.Contains(p.Id))
					.ToListAsync();
				foreach (var pack in packs)
					scenario.SC5FilePacks.Add(pack);
			}

			await _db.SaveChangesAsync();
			SuccessMessage = $"Scenario '{Name.Trim()}' updated.";
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating scenario: {ex.Message}";
		}

		await ReloadAsync(Id);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!CanEdit)
			return Forbid();

		var scenario = await _db.SC5Files.FindAsync(new object[] { (object)id });
		if (scenario is null)
		{
			await ReloadAsync(id);
			ErrorMessage = "Failed to delete scenario.";
			return Page();
		}

		_db.SC5Files.Remove(scenario);
		await _db.SaveChangesAsync();

		SuccessMessage = "Scenario deleted.";
		return RedirectToPage("/Index", new { category = "sc5files" });
	}

	private async Task ReloadAsync(UniqueObjectId id)
	{
		Scenario = await _db.SC5Files
			.Include(s => s.Licence)
			.Include(s => s.Authors)
			.Include(s => s.Tags)
			.Include(s => s.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(s => s.Id == id);

		await LoadRelatedDataAsync(CancellationToken.None);
	}

	private async Task LoadRelatedDataAsync(CancellationToken ct)
	{
		if (Scenario is not null)
		{
			SC5FilePacks = Scenario.SC5FilePacks
				.OrderBy(p => p.Name)
				.Select(p => new ListItem(p.Id, p.Name, "SC5FilePacks", null))
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

		AvailablePacks = await _db.SC5FilePacks
			.OrderBy(p => p.Name)
			.Select(p => new DtoScenarioEntry(p.Id, p.Name))
			.ToListAsync(ct);
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}