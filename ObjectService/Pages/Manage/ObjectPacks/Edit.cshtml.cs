using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Manage.ObjectPacks;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	private readonly LocoDbContext _db;
	private readonly ICrudService<DtoAuthorEntry, TblAuthor> _authorService;
	private readonly ICrudService<DtoTagEntry, TblTag> _tagService;
	private readonly ICrudService<DtoLicenceEntry, TblLicence> _licenceService;

	public EditModel(
		LocoDbContext db,
		ICrudService<DtoAuthorEntry, TblAuthor> authorService,
		ICrudService<DtoTagEntry, TblTag> tagService,
		ICrudService<DtoLicenceEntry, TblLicence> licenceService)
	{
		_db = db;
		_authorService = authorService;
		_tagService = tagService;
		_licenceService = licenceService;
	}

	[BindProperty]
	public UniqueObjectId Id { get; set; }

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	[BindProperty]
	public string? Description { get; set; }

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
	public List<UniqueObjectId> SelectedObjectIds { get; set; } = [];

	public TblObjectPack? Pack { get; set; }
	public List<DtoAuthorEntry> AvailableAuthors { get; set; } = [];
	public List<DtoTagEntry> AvailableTags { get; set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; set; } = [];
	public List<(UniqueObjectId Id, string Name)> AvailableObjects { get; set; } = [];
	public List<(UniqueObjectId Id, string Name)> PackObjects { get; set; } = [];

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id)
	{
		Pack = await _db.ObjectPacks
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Licence)
			.Include(p => p.Objects)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (Pack == null)
		{
			return NotFound();
		}

		// Load current values
		Id = Pack.Id;
		Name = Pack.Name;
		Description = Pack.Description;
		CreatedDate = Pack.CreatedDate;
		ModifiedDate = Pack.ModifiedDate;
		UploadedDate = Pack.UploadedDate;
		LicenceId = Pack.Licence?.Id;
		SelectedAuthorIds = Pack.Authors.Select(a => a.Id).ToList();
		SelectedTagIds = Pack.Tags.Select(t => t.Id).ToList();
		SelectedObjectIds = Pack.Objects.Select(o => o.Id).ToList();
		PackObjects = Pack.Objects.OrderBy(o => o.Name).Select(o => (o.Id, o.Name)).ToList();

		// Load available values for dropdowns
		AvailableAuthors = (await _authorService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(a => a.Name).ToList();
		AvailableTags = (await _tagService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(t => t.Name).ToList();
		AvailableLicences = (await _licenceService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(l => l.Name).ToList();
		var objList = await _db.Objects.OrderBy(o => o.Name).Select(o => new { o.Id, o.Name }).ToListAsync();
		AvailableObjects = objList.Select(o => (o.Id, o.Name)).ToList();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var pack = await _db.ObjectPacks
				.Include(p => p.Authors)
				.Include(p => p.Tags)
				.Include(p => p.Licence)
				.Include(p => p.Objects)
				.FirstOrDefaultAsync(p => p.Id == Id);

			if (pack == null)
			{
				return NotFound();
			}

			// Update basic properties
			pack.Name = Name;
			pack.Description = Description;
			pack.CreatedDate = CreatedDate;
			pack.ModifiedDate = ModifiedDate;
			pack.UploadedDate = UploadedDate;
			pack.Licence = LicenceId.HasValue ? await _db.Licences.FindAsync(new object[] { (object)LicenceId.Value }, cancellationToken: CancellationToken.None) : null;

			// Update authors
			var selectedAuthors = await _db.Authors.Where(a => SelectedAuthorIds.Contains(a.Id)).ToListAsync();
			pack.Authors.Clear();
			foreach (var author in selectedAuthors)
			{
				pack.Authors.Add(author);
			}

			// Update tags
			var selectedTags = await _db.Tags.Where(t => SelectedTagIds.Contains(t.Id)).ToListAsync();
			pack.Tags.Clear();
			foreach (var tag in selectedTags)
			{
				pack.Tags.Add(tag);
			}

			// Update objects in pack
			var selectedObjects = await _db.Objects.Where(o => SelectedObjectIds.Contains(o.Id)).ToListAsync();
			pack.Objects.Clear();
			foreach (var obj in selectedObjects)
			{
				pack.Objects.Add(obj);
			}

			await _db.SaveChangesAsync();
			TempData["SuccessMessage"] = $"Object pack '{Name}' updated successfully.";
			return RedirectToPage("/manage/objectpacks");
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating object pack: {ex.Message}";
			return Page();
		}
	}
}
