using Definitions.Database;
using Definitions.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObjectService.Services;

namespace ObjectService.Pages.Licences;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;
	readonly ICrudService<DtoLicenceEntry, TblLicence> _licenceService;

	public DetailsModel(LocoDbContext db, ICrudService<DtoLicenceEntry, TblLicence> licenceService)
	{
		_db = db;
		_licenceService = licenceService;
	}

	public TblLicence? Licence { get; private set; }

	public List<ListItem> Objects { get; private set; } = [];
	public List<ListItem> ObjectPacks { get; private set; } = [];
	public List<ListItem> SC5Files { get; private set; } = [];
	public List<ListItem> SC5FilePacks { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	[BindProperty]
	public UniqueObjectId Id { get; set; }

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	[BindProperty]
	public string Text { get; set; } = string.Empty;

	public bool IsAdmin => User.IsInRole("Admin");

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

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
			return Forbid();

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Licence name is required.";
			await ReloadAsync(Id);
			return Page();
		}

		try
		{
			var entry = new DtoLicenceEntry(Id, Name.Trim(), Text?.Trim() ?? string.Empty);
			var updated = await _licenceService.UpdateAsync(Id, entry, CancellationToken.None);
			if (updated != null)
			{
				SuccessMessage = $"Licence '{Name.Trim()}' updated.";
			}
			else
			{
				ErrorMessage = "Licence not found.";
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating licence: {ex.Message}";
		}

		await ReloadAsync(Id);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!IsAdmin)
			return Forbid();

		var deleted = await _licenceService.DeleteAsync(id, CancellationToken.None);
		if (deleted)
		{
			SuccessMessage = "Licence deleted.";
			return RedirectToPage("/Index", new { category = "licences" });
		}

		await ReloadAsync(id);
		ErrorMessage = "Failed to delete licence.";
		return Page();
	}

	private async Task ReloadAsync(UniqueObjectId id)
	{
		Licence = await _db.Licences.FirstOrDefaultAsync(l => l.Id == id);
		if (Licence != null)
		{
			var licenceId = Licence.Id;
			Objects = await _db.Objects.Where(o => o.Licence != null && o.Licence.Id == licenceId).OrderBy(o => o.Name).Select(o => new ListItem(o.Id, o.Description ?? o.Name, "Objects", null)).ToListAsync();
			ObjectPacks = await _db.ObjectPacks.Where(p => p.Licence != null && p.Licence.Id == licenceId).OrderBy(p => p.Name).Select(p => new ListItem(p.Id, p.Name, "ObjectPacks", null)).ToListAsync();
			SC5Files = await _db.SC5Files.Where(f => f.Licence != null && f.Licence.Id == licenceId).OrderBy(f => f.Name).Select(f => new ListItem(f.Id, f.Name, "SC5Files", null)).ToListAsync();
			SC5FilePacks = await _db.SC5FilePacks.Where(p => p.Licence != null && p.Licence.Id == licenceId).OrderBy(p => p.Name).Select(p => new ListItem(p.Id, p.Name, "SC5FilePacks", null)).ToListAsync();
		}
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}