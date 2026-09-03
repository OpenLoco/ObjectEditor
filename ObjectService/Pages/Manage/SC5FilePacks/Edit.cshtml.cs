using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO;
using ObjectService.Services;

namespace ObjectService.Pages.Manage.SC5FilePacks;

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
	public List<UniqueObjectId> SelectedSC5FileIds { get; set; } = [];

	public TblSC5FilePack? Pack { get; set; }
	public List<DtoAuthorEntry> AvailableAuthors { get; set; } = [];
	public List<DtoTagEntry> AvailableTags { get; set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; set; } = [];
	public List<(UniqueObjectId Id, string Name)> AvailableSC5Files { get; set; } = [];

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id)
	{
		Pack = await _db.SC5FilePacks
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Licence)
			.Include(p => p.SC5Files)
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
		SelectedSC5FileIds = Pack.SC5Files.Select(f => f.Id).ToList();

		// Load available values for dropdowns
		AvailableAuthors = (await _authorService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(a => a.Name).ToList();
		AvailableTags = (await _tagService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(t => t.Name).ToList();
		AvailableLicences = (await _licenceService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(l => l.Name).ToList();
		var fileList = await _db.SC5Files.OrderBy(f => f.Name).Select(f => new { f.Id, f.Name }).ToListAsync();
		AvailableSC5Files = fileList.Select(f => (f.Id, f.Name)).ToList();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var pack = await _db.SC5FilePacks
				.Include(p => p.Authors)
				.Include(p => p.Tags)
				.Include(p => p.Licence)
				.Include(p => p.SC5Files)
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

			// Update SC5 files in pack
			var selectedFiles = await _db.SC5Files.Where(f => SelectedSC5FileIds.Contains(f.Id)).ToListAsync();
			pack.SC5Files.Clear();
			foreach (var file in selectedFiles)
			{
				pack.SC5Files.Add(file);
			}

			await _db.SaveChangesAsync();
			TempData["SuccessMessage"] = $"SC5 file pack '{Name}' updated successfully.";
			return RedirectToPage("/Manage/SC5FilePacks/Index");
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating SC5 file pack: {ex.Message}";
			return Page();
		}
	}
}