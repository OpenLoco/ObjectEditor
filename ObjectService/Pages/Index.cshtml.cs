using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ObjectService.Frontend;

namespace ObjectService.Pages;

public sealed class IndexModel : PageModel
{
	readonly ObjectExplorerService _explorerService;
	readonly LocoDbContext _db;

	public IndexModel(ObjectExplorerService explorerService, LocoDbContext db)
	{
		_explorerService = explorerService;
		_db = db;
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

	public IReadOnlyList<ObjectType> ObjectTypes { get; } = [.. Enum.GetValues<ObjectType>()];

	public IReadOnlyList<ObjectSource> ObjectSources { get; } = [.. Enum.GetValues<ObjectSource>()];

	public IReadOnlyList<ObjectAvailability> AvailabilityStates { get; } = [.. Enum.GetValues<ObjectAvailability>()];

	public IReadOnlyList<VehicleType> VehicleTypes { get; } = [.. Enum.GetValues<VehicleType>()];

	public IReadOnlyDictionary<string, string> Categories { get; } = new Dictionary<string, string>
	{
		["objects"] = "Objects",
		["music"] = "Music",
		["sfx"] = "SFX",
		["objectpacks"] = "Object Packs",
		["sc5files"] = "Scenarios",
		["sc5filepacks"] = "Scenario Packs",
	};

	public IReadOnlyDictionary<string, string> DataCategories { get; } = new Dictionary<string, string>
	{
		["authors"] = "Authors",
		["tags"] = "Tags",
		["licences"] = "Licences",
	};

	public bool IsAdmin => User.IsInRole("Admin");

	// ── Data lists for database-view categories ──
	public List<AuthorListViewModel> AuthorList { get; private set; } = [];
	public List<TagListViewModel> TagList { get; private set; } = [];
	public List<LicenceListViewModel> LicenceList { get; private set; } = [];
	public List<ObjectPackListViewModel> ObjectPackList { get; private set; } = [];
	public List<SC5FileListViewModel> SC5FileList { get; private set; } = [];
	public List<SC5FilePackListViewModel> SC5FilePackList { get; private set; } = [];

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

	// ── View models ──
	public record AuthorListViewModel(UniqueObjectId Id, string Name);
	public record TagListViewModel(UniqueObjectId Id, string Name);
	public record LicenceListViewModel(UniqueObjectId Id, string Name, string Text);
	public record ObjectPackListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, int AuthorCount, int TagCount, string Licence, int ObjectCount);
	public record SC5FileListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, ObjectSource ObjectSource, int AuthorCount, int TagCount, string Licence, int PackCount);
	public record SC5FilePackListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, int AuthorCount, int TagCount, string Licence, int FileCount);
}
