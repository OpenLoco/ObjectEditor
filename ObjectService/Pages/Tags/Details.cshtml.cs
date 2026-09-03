using Definitions.Database;
using Definitions.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObjectService.Services;

namespace ObjectService.Pages.Tags;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;
	readonly ICrudService<DtoTagEntry, TblTag> _tagService;

	public DetailsModel(LocoDbContext db, ICrudService<DtoTagEntry, TblTag> tagService)
	{
		_db = db;
		_tagService = tagService;
	}

	public TblTag? Tag { get; private set; }

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

	public bool IsAdmin => User.IsInRole("Admin");

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

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
			return Forbid();

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Tag name is required.";
			await ReloadAsync(Id);
			return Page();
		}

		try
		{
			var entry = new DtoTagEntry(Id, Name.Trim());
			var updated = await _tagService.UpdateAsync(Id, entry, CancellationToken.None);
			if (updated != null)
			{
				SuccessMessage = $"Tag '{Name.Trim()}' updated.";
			}
			else
			{
				ErrorMessage = "Tag not found.";
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating tag: {ex.Message}";
		}

		await ReloadAsync(Id);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!IsAdmin)
			return Forbid();

		var deleted = await _tagService.DeleteAsync(id, CancellationToken.None);
		if (deleted)
		{
			SuccessMessage = "Tag deleted.";
			return RedirectToPage("/Index", new { category = "tags" });
		}

		await ReloadAsync(id);
		ErrorMessage = "Failed to delete tag.";
		return Page();
	}

	private async Task ReloadAsync(UniqueObjectId id)
	{
		Tag = await _db.Tags
			.Include(t => t.Objects)
			.Include(t => t.ObjectPacks)
			.Include(t => t.SC5Files)
			.Include(t => t.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(t => t.Id == id);

		if (Tag != null)
		{
			Objects = Tag.Objects.OrderBy(o => o.Name).Select(o => new ListItem(o.Id, o.Description ?? o.Name, "Objects", null)).ToList();
			ObjectPacks = Tag.ObjectPacks.OrderBy(p => p.Name).Select(p => new ListItem(p.Id, p.Name, "ObjectPacks", null)).ToList();
			SC5Files = Tag.SC5Files.OrderBy(f => f.Name).Select(f => new ListItem(f.Id, f.Name, "SC5Files", null)).ToList();
			SC5FilePacks = Tag.SC5FilePacks.OrderBy(p => p.Name).Select(p => new ListItem(p.Id, p.Name, "SC5FilePacks", null)).ToList();
		}
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}