using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObjectService.Frontend;
using ObjectService.Services;

namespace ObjectService.Pages;

public sealed class IndexModel : PageModel
{
	readonly ObjectExplorerService _explorerService;
	readonly LocoDbContext _db;
	readonly ICrudService<DtoAuthorEntry, TblAuthor> _authorService;
	readonly ICrudService<DtoTagEntry, TblTag> _tagService;
	readonly ICrudService<DtoLicenceEntry, TblLicence> _licenceService;
	readonly ICrudService<DtoObjectMissingEntry, TblObjectMissing> _objectsMissingService;

	public IndexModel(
		ObjectExplorerService explorerService,
		LocoDbContext db,
		ICrudService<DtoAuthorEntry, TblAuthor> authorService,
		ICrudService<DtoTagEntry, TblTag> tagService,
		ICrudService<DtoLicenceEntry, TblLicence> licenceService,
		ICrudService<DtoObjectMissingEntry, TblObjectMissing> objectsMissingService)
	{
		_explorerService = explorerService;
		_db = db;
		_authorService = authorService;
		_tagService = tagService;
		_licenceService = licenceService;
		_objectsMissingService = objectsMissingService;
	}

	[BindProperty(SupportsGet = true)]
	public string? Search { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectType? ObjectType { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectSource? ObjectSource { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectAvailability? Availability { get; set; }

	[BindProperty(SupportsGet = true)]
	public VehicleType? VehicleType { get; set; }

	[BindProperty(Name = "p", SupportsGet = true)]
	public int PageNumber { get; set; } = 1;

	[BindProperty(SupportsGet = true)]
	public string Category { get; set; } = "objects";

	public ObjectBrowsePageViewModel Results { get; private set; } = new(0, 0, 1, 48, []);

	public IReadOnlyList<ObjectType> ObjectTypes { get; } = [.. Enum.GetValues<ObjectType>().OrderBy(t => t.ToString())];

	public IReadOnlyList<ObjectSource> ObjectSources { get; } = [.. Enum.GetValues<ObjectSource>()];

	public IReadOnlyList<ObjectAvailability> AvailabilityStates { get; } = [.. Enum.GetValues<ObjectAvailability>()];

	public IReadOnlyList<VehicleType> VehicleTypes { get; } = [.. Enum.GetValues<VehicleType>()];

	public sealed record TabGroup(string Name, IReadOnlyDictionary<string, string> Items);

	public IReadOnlyList<TabGroup> TabGroups { get; } =
	[
		new("Objects", new Dictionary<string, string>
		{
			["objects"] = "Objects",
			["objectpacks"] = "Object\u00A0Packs",
			["objectsmissing"] = "Missing Objects",
		}),
		new("Audio", new Dictionary<string, string>
		{
			["music"] = "Music",
			["sfx"] = "SFX",
		}),
		new("Scenarios", new Dictionary<string, string>
		{
			["sc5files"] = "Scenarios",
			["sc5filepacks"] = "Scenario\u00A0Packs",
		}),
		new("Data", new Dictionary<string, string>
		{
			["authors"] = "Authors",
			["tags"] = "Tags",
			["licences"] = "Licences",
		}),
	];
	
	public bool IsAdmin => User.IsInRole("Admin");

	// ── Data lists for database-view categories ──
	public List<AuthorListViewModel> AuthorList { get; private set; } = [];
	public List<TagListViewModel> TagList { get; private set; } = [];
	public List<LicenceListViewModel> LicenceList { get; private set; } = [];
	public List<ObjectPackListViewModel> ObjectPackList { get; private set; } = [];
	public List<SC5FileListViewModel> SC5FileList { get; private set; } = [];
	public List<SC5FilePackListViewModel> SC5FilePackList { get; private set; } = [];
	public List<ObjectsMissingListViewModel> ObjectsMissingList { get; private set; } = [];

	public async Task OnGetAsync(CancellationToken cancellationToken)
	{
		switch (Category)
		{
			case "objects":
				Results = await _explorerService.GetObjectsAsync(
					new ObjectBrowseQuery(Search, ObjectType, ObjectSource, Availability, VehicleType, PageNumber),
					cancellationToken);
				PageNumber = Results.Page;
				break;

			case "scenarios":
			case "music":
			case "sfx":
				Results = new(0, 0, 1, 48, []);
				break;

			case "objectpacks":
				await LoadObjectPacksAsync(cancellationToken);
				break;

			case "sc5files":
				await LoadSC5FilesAsync(cancellationToken);
				break;

			case "sc5filepacks":
				await LoadSC5FilePacksAsync(cancellationToken);
				break;

			case "authors":
				await LoadAuthorsAsync(cancellationToken);
				break;

			case "tags":
				await LoadTagsAsync(cancellationToken);
				break;

			case "licences":
				await LoadLicencesAsync(cancellationToken);
				break;

			case "objectsmissing":
				await LoadObjectsMissingAsync(cancellationToken);
				break;

			default:
				Results = new(0, 0, 1, 48, []);
				break;
		}
	}
async Task LoadObjectPacksAsync(CancellationToken ct)
	{
		var packs = await _db.ObjectPacks
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Licence)
			.Include(p => p.Objects)
			.OrderByDescending(p => p.UploadedDate)
			.ToListAsync(ct);

		ObjectPackList = packs.Select(p => new ObjectPackListViewModel(
			p.Id,
			p.Name,
			p.Description ?? "",
			p.UploadedDate,
			p.Authors.Count,
			p.Tags.Count,
			p.Licence?.Name ?? "None",
			p.Objects.Count)).ToList();
	}

	async Task LoadSC5FilesAsync(CancellationToken ct)
	{
		var files = await _db.SC5Files
			.Include(f => f.Authors)
			.Include(f => f.Tags)
			.Include(f => f.Licence)
			.Include(f => f.SC5FilePacks)
			.OrderByDescending(f => f.UploadedDate)
			.ToListAsync(ct);

		SC5FileList = files.Select(f => new SC5FileListViewModel(
			f.Id,
			f.Name,
			f.Description ?? "",
			f.UploadedDate,
			f.ObjectSource,
			f.Authors.Count,
			f.Tags.Count,
			f.Licence?.Name ?? "None",
			f.SC5FilePacks.Count)).ToList();
	}

	async Task LoadSC5FilePacksAsync(CancellationToken ct)
	{
		var packs = await _db.SC5FilePacks
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Licence)
			.Include(p => p.SC5Files)
			.OrderByDescending(p => p.UploadedDate)
			.ToListAsync(ct);

		SC5FilePackList = packs.Select(p => new SC5FilePackListViewModel(
			p.Id,
			p.Name,
			p.Description ?? "",
			p.UploadedDate,
			p.Authors.Count,
			p.Tags.Count,
			p.Licence?.Name ?? "None",
			p.SC5Files.Count)).ToList();
	}

	async Task LoadAuthorsAsync(CancellationToken ct)
	{
		var authors = await _db.Authors
			.OrderBy(a => a.Name)
			.ToListAsync(ct);

		AuthorList = authors.Select(a => new AuthorListViewModel(a.Id, a.Name)).ToList();
	}

	async Task LoadTagsAsync(CancellationToken ct)
	{
		var tags = await _db.Tags
			.OrderBy(t => t.Name)
			.ToListAsync(ct);

		TagList = tags.Select(t => new TagListViewModel(t.Id, t.Name)).ToList();
	}

	async Task LoadLicencesAsync(CancellationToken ct)
	{
		var licences = await _db.Licences
			.OrderBy(l => l.Name)
			.ToListAsync(ct);

		LicenceList = licences.Select(l => new LicenceListViewModel(l.Id, l.Name, l.Text)).ToList();
	}

	async Task LoadObjectsMissingAsync(CancellationToken ct)
	{
		var missing = await _db.ObjectsMissing
			.OrderBy(m => m.DatName)
			.ThenBy(m => m.DatChecksum)
			.ToListAsync(ct);

		ObjectsMissingList = missing.Select(m => new ObjectsMissingListViewModel(m.Id, m.DatName, m.DatChecksum, m.ObjectType)).ToList();
	}

	// ── CRUD form bindings ──

	[BindProperty]
	public UniqueObjectId CrudId { get; set; }

	[BindProperty]
	public string CrudName { get; set; } = string.Empty;

	[BindProperty]
	public string CrudText { get; set; } = string.Empty;

	[BindProperty]
	public string CrudDescription { get; set; } = string.Empty;

	[BindProperty]
	public uint32_t CrudChecksum { get; set; }

	[BindProperty]
	public ObjectType CrudObjectType { get; set; }

	[BindProperty]
	public string CrudCategory { get; set; } = string.Empty;

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	// ── POST: Create author/tag/licence ──

	public async Task<IActionResult> OnPostCreateAsync()
	{
		if (!IsAdmin)
			return Forbid();

		if (string.IsNullOrWhiteSpace(CrudName))
		{
			ErrorMessage = "Name is required.";
			return RedirectToPage(new { category = CrudCategory });
		}

		try
		{
			switch (CrudCategory)
			{
				case "authors":
				{
					var entry = new DtoAuthorEntry(0, CrudName.Trim());
					if (!_authorService.TryValidateCreate(entry, out var err))
					{
						ErrorMessage = err;
						return RedirectToPage(new { category = CrudCategory });
					}
					await _authorService.CreateAsync(entry, CancellationToken.None);
					SuccessMessage = $"Author '{CrudName.Trim()}' created.";
					break;
				}
				case "tags":
				{
					var entry = new DtoTagEntry(0, CrudName.Trim());
					if (!_tagService.TryValidateCreate(entry, out var err))
					{
						ErrorMessage = err;
						return RedirectToPage(new { category = CrudCategory });
					}
					await _tagService.CreateAsync(entry, CancellationToken.None);
					SuccessMessage = $"Tag '{CrudName.Trim()}' created.";
					break;
				}
				case "licences":
				{
					var entry = new DtoLicenceEntry(0, CrudName.Trim(), CrudText?.Trim() ?? string.Empty);
					if (!_licenceService.TryValidateCreate(entry, out var err))
					{
						ErrorMessage = err;
						return RedirectToPage(new { category = CrudCategory });
					}
					await _licenceService.CreateAsync(entry, CancellationToken.None);
					SuccessMessage = $"Licence '{CrudName.Trim()}' created.";
					break;
				}
				case "objectsmissing":
				{
					var entry = new DtoObjectMissingEntry(0, CrudName.Trim(), CrudChecksum, CrudObjectType);
					if (!_objectsMissingService.TryValidateCreate(entry, out var err))
					{
						ErrorMessage = err;
						return RedirectToPage(new { category = CrudCategory });
					}
					await _objectsMissingService.CreateAsync(entry, CancellationToken.None);
					SuccessMessage = $"Missing object '{CrudName.Trim()}' created.";
					break;
				}
				case "objectpacks":
				{
					var newPack = new TblObjectPack { Name = CrudName.Trim(), Description = CrudDescription?.Trim() };
					_ = _db.ObjectPacks.Add(newPack);
					_ = await _db.SaveChangesAsync();
					SuccessMessage = $"Object pack '{CrudName.Trim()}' created.";
					break;
				}
				case "sc5filepacks":
				{
					var newPack = new TblSC5FilePack { Name = CrudName.Trim(), Description = CrudDescription?.Trim() };
					_ = _db.SC5FilePacks.Add(newPack);
					_ = await _db.SaveChangesAsync();
					SuccessMessage = $"Scenario pack '{CrudName.Trim()}' created.";
					break;
				}
				default:
					break;
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error creating: {ex.Message}";
		}

		return RedirectToPage(new { category = CrudCategory });
	}

	// ── POST: Edit author/tag/licence ──

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
			return Forbid();

		if (string.IsNullOrWhiteSpace(CrudName))
		{
			ErrorMessage = "Name is required.";
			return RedirectToPage(new { category = CrudCategory });
		}

		try
		{
			switch (CrudCategory)
			{
				case "authors":
				{
					var entry = new DtoAuthorEntry(CrudId, CrudName.Trim());
					var updated = await _authorService.UpdateAsync(CrudId, entry, CancellationToken.None);
					SuccessMessage = updated != null ? $"Author '{CrudName.Trim()}' updated." : "Author not found.";
					if (updated == null) ErrorMessage = "Author not found.";
					break;
				}
				case "tags":
				{
					var entry = new DtoTagEntry(CrudId, CrudName.Trim());
					var updated = await _tagService.UpdateAsync(CrudId, entry, CancellationToken.None);
					SuccessMessage = updated != null ? $"Tag '{CrudName.Trim()}' updated." : "Tag not found.";
					if (updated == null) ErrorMessage = "Tag not found.";
					break;
				}
				case "licences":
				{
					var entry = new DtoLicenceEntry(CrudId, CrudName.Trim(), CrudText?.Trim() ?? string.Empty);
					var updated = await _licenceService.UpdateAsync(CrudId, entry, CancellationToken.None);
					SuccessMessage = updated != null ? $"Licence '{CrudName.Trim()}' updated." : "Licence not found.";
					if (updated == null) ErrorMessage = "Licence not found.";
					break;
				}
				case "objectsmissing":
				{
					var entry = new DtoObjectMissingEntry(CrudId, CrudName.Trim(), CrudChecksum, CrudObjectType);
					var updated = await _objectsMissingService.UpdateAsync(CrudId, entry, CancellationToken.None);
					SuccessMessage = updated != null ? $"Missing object '{CrudName.Trim()}' updated." : "Missing object not found.";
					if (updated == null) ErrorMessage = "Missing object not found.";
					break;
				}
				default:
					break;
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error updating: {ex.Message}";
		}

		return RedirectToPage(new { category = CrudCategory });
	}

	// ── POST: Delete author/tag/licence/objectpacks/sc5filepacks ──

	public async Task<IActionResult> OnPostDeleteAsync()
	{
		if (!IsAdmin)
			return Forbid();

		try
		{
			switch (CrudCategory)
			{
				case "authors":
				{
					var deleted = await _authorService.DeleteAsync(CrudId, CancellationToken.None);
					SuccessMessage = deleted ? "Author deleted." : null;
					ErrorMessage = deleted ? null : "Failed to delete author.";
					break;
				}
				case "tags":
				{
					var deleted = await _tagService.DeleteAsync(CrudId, CancellationToken.None);
					SuccessMessage = deleted ? "Tag deleted." : null;
					ErrorMessage = deleted ? null : "Failed to delete tag.";
					break;
				}
				case "licences":
				{
					var deleted = await _licenceService.DeleteAsync(CrudId, CancellationToken.None);
					SuccessMessage = deleted ? "Licence deleted." : null;
					ErrorMessage = deleted ? null : "Failed to delete licence.";
					break;
				}
				case "objectsmissing":
				{
					var deleted = await _objectsMissingService.DeleteAsync(CrudId, CancellationToken.None);
					SuccessMessage = deleted ? "Missing object deleted." : null;
					ErrorMessage = deleted ? null : "Failed to delete missing object.";
					break;
				}
				case "objectpacks":
				{
					var pack = await _db.ObjectPacks.FindAsync(new object[] { (object)CrudId }, CancellationToken.None);
					if (pack != null)
					{
						_db.ObjectPacks.Remove(pack);
						await _db.SaveChangesAsync();
						SuccessMessage = $"Object pack '{pack.Name}' deleted.";
					}
					else
						ErrorMessage = "Pack not found.";
					break;
				}
				case "sc5filepacks":
				{
					var pack = await _db.SC5FilePacks.FindAsync(new object[] { (object)CrudId }, CancellationToken.None);
					if (pack != null)
					{
						_db.SC5FilePacks.Remove(pack);
						await _db.SaveChangesAsync();
						SuccessMessage = $"SC5 file pack '{pack.Name}' deleted.";
					}
					else
						ErrorMessage = "Pack not found.";
					break;
				}
				default:
					ErrorMessage = "Unknown category.";
					break;
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error deleting: {ex.Message}";
		}

		return RedirectToPage(new { category = CrudCategory });
	}

	// ── View models ──
	public record AuthorListViewModel(UniqueObjectId Id, string Name);
	public record TagListViewModel(UniqueObjectId Id, string Name);
	public record LicenceListViewModel(UniqueObjectId Id, string Name, string Text);
	public record ObjectPackListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, int AuthorCount, int TagCount, string Licence, int ObjectCount);
	public record SC5FileListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, ObjectSource ObjectSource, int AuthorCount, int TagCount, string Licence, int PackCount);
	public record SC5FilePackListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, int AuthorCount, int TagCount, string Licence, int FileCount);
	public record ObjectsMissingListViewModel(UniqueObjectId Id, string DatName, uint32_t DatChecksum, ObjectType ObjectType);
}
