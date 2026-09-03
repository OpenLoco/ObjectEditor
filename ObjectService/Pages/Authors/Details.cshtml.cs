using Definitions.Database;
using Definitions.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObjectService.Services;

namespace ObjectService.Pages.Authors;

public sealed class DetailsModel : PageModel
{
	readonly LocoDbContext _db;
	readonly ICrudService<DtoAuthorEntry, TblAuthor> _authorService;

	public DetailsModel(LocoDbContext db, ICrudService<DtoAuthorEntry, TblAuthor> authorService)
	{
		_db = db;
		_authorService = authorService;
	}

	public TblAuthor? Author { get; private set; }

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

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
			return Forbid();

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Author name is required.";
			await ReloadAsync(Id);
			return Page();
		}

		try
		{
			var entry = new DtoAuthorEntry(Id, Name.Trim());
			var updated = await _authorService.UpdateAsync(Id, entry, CancellationToken.None);
			if (updated != null)
			{
				SuccessMessage = $"Author '{Name.Trim()}' updated.";
			}
			else
			{
				ErrorMessage = "Author not found.";
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating author: {ex.Message}";
		}

		await ReloadAsync(Id);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!IsAdmin)
			return Forbid();

		var deleted = await _authorService.DeleteAsync(id, CancellationToken.None);
		if (deleted)
		{
			SuccessMessage = "Author deleted.";
			return RedirectToPage("/Index", new { category = "authors" });
		}

		await ReloadAsync(id);
		ErrorMessage = "Failed to delete author.";
		return Page();
	}

	private async Task ReloadAsync(UniqueObjectId id)
	{
		Author = await _db.Authors
			.Include(a => a.Objects)
			.Include(a => a.ObjectPacks)
			.Include(a => a.SC5Files)
			.Include(a => a.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(a => a.Id == id);

		if (Author != null)
		{
			Objects = Author.Objects.OrderBy(o => o.Name).Select(o => new ListItem(o.Id, o.Description ?? o.Name, "Objects", null)).ToList();
			ObjectPacks = Author.ObjectPacks.OrderBy(p => p.Name).Select(p => new ListItem(p.Id, p.Name, "ObjectPacks", null)).ToList();
			SC5Files = Author.SC5Files.OrderBy(f => f.Name).Select(f => new ListItem(f.Id, f.Name, "SC5Files", null)).ToList();
			SC5FilePacks = Author.SC5FilePacks.OrderBy(p => p.Name).Select(p => new ListItem(p.Id, p.Name, "SC5FilePacks", null)).ToList();
		}
	}

	public record ListItem(UniqueObjectId Id, string Name, string Kind, string? Extra);
}