using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using ObjectService.Services;
using static Definitions.ObjectAvailability;

namespace ObjectService.Pages.Manage.Objects;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	private readonly IObjectQueryService _objectService;
	private readonly LocoDbContext _db;
	private readonly ICrudService<DtoAuthorEntry, TblAuthor> _authorService;
	private readonly ICrudService<DtoTagEntry, TblTag> _tagService;
	private readonly ICrudService<DtoLicenceEntry, TblLicence> _licenceService;

	public EditModel(
		IObjectQueryService objectService,
		LocoDbContext db,
		ICrudService<DtoAuthorEntry, TblAuthor> authorService,
		ICrudService<DtoTagEntry, TblTag> tagService,
		ICrudService<DtoLicenceEntry, TblLicence> licenceService)
	{
		_objectService = objectService;
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
	public List<UniqueObjectId> SelectedObjectPackIds { get; set; } = [];

	[BindProperty]
	public ObjectAvailability Availability { get; set; }

	public DtoObjectPostResponse? Object { get; set; }
	public List<DtoAuthorEntry> AvailableAuthors { get; set; } = [];
	public List<DtoTagEntry> AvailableTags { get; set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; set; } = [];
	public List<DtoItemPackEntry> AvailableObjectPacks { get; set; } = [];

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id)
	{
		Object = await _objectService.GetByIdAsync(id, CancellationToken.None);
		if (Object == null)
		{
			return NotFound();
		}

		// Check if object is vanilla (not editable)
		if (Object.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
		{
			ErrorMessage = "Vanilla game objects cannot be edited.";
			return Page();
		}

		// Load current values
		Id = Object.Id;
		Name = Object.Name;
		Description = Object.Description;
		CreatedDate = Object.CreatedDate;
		ModifiedDate = Object.ModifiedDate;
		UploadedDate = Object.UploadedDate;
		LicenceId = Object.Licence?.Id;
		Availability = Object.Availability;
		SelectedAuthorIds = Object.Authors.Select(a => a.Id).ToList();
		SelectedTagIds = Object.Tags.Select(t => t.Id).ToList();
		SelectedObjectPackIds = Object.ObjectPacks.Select(p => p.Id).ToList();

		// Load available values for dropdowns
		AvailableAuthors = (await _authorService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(a => a.Name).ToList();
		AvailableTags = (await _tagService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(t => t.Name).ToList();
		AvailableLicences = (await _licenceService.ListAsync(HttpContext, CancellationToken.None)).OrderBy(l => l.Name).ToList();
		AvailableObjectPacks = await _db.ObjectPacks.Select(p => new DtoItemPackEntry(p.Id, p.Name, p.Description, p.CreatedDate, p.ModifiedDate, p.UploadedDate, null)).OrderBy(p => p.Name).ToListAsync();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var licence = LicenceId.HasValue && Object != null ? Object.Licence : null;
			var licenceEntry = licence != null ? new DtoLicenceEntry(licence.Id, licence.Name, licence.Text) : null;

			var authors = await _authorService.ListAsync(HttpContext, CancellationToken.None);
			var authorEntries = authors.Where(a => SelectedAuthorIds.Contains(a.Id)).ToList();

			var tags = await _tagService.ListAsync(HttpContext, CancellationToken.None);
			var tagEntries = tags.Where(t => SelectedTagIds.Contains(t.Id)).ToList();

			var packItems = await _db.ObjectPacks
				.Where(p => SelectedObjectPackIds.Contains(p.Id))
				.Select(p => new DtoItemPackEntry(p.Id, p.Name, p.Description, p.CreatedDate, p.ModifiedDate, p.UploadedDate, null))
				.ToListAsync();

			var updateRequest = new DtoObjectPostResponse(
				Id,
				Name,
				Name, // DisplayName
				Object?.DatChecksum,
				Description,
				Object?.ObjectSource ?? ObjectSource.Custom,
				Object?.ObjectType ?? ObjectType.Bridge,
				Object?.VehicleType,
				Availability,
				CreatedDate,
				ModifiedDate,
				UploadedDate,
				licenceEntry,
				authorEntries,
				tagEntries,
				packItems,
				Object?.DatObjects ?? [],
				Object?.StringTable ?? new DtoStringTableDescriptor([], 0));

			var updated = await _objectService.UpdateAsync(Id, updateRequest, CancellationToken.None);
			if (updated != null)
			{
				TempData["SuccessMessage"] = $"Object '{Name}' updated successfully.";
				return RedirectToPage("/manage/objects");
			}

			ErrorMessage = "Failed to update object. It may no longer exist.";
			return Page();
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating object: {ex.Message}";
			return Page();
		}
	}
}
