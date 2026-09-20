using Definitions.DTO;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages;

public sealed class IndexModel : PageModel
{
	readonly FrontendApiClient _api;
	readonly ObjectExplorerService _explorerService;

	public IndexModel(FrontendApiClient api, ObjectExplorerService explorerService)
	{
		_api = api;
		_explorerService = explorerService;
	}

	[BindProperty(SupportsGet = true)]
	public string? Search { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectType? ObjectType { get; set; }

	[BindProperty(SupportsGet = true)]
	public ObjectSource? ObjectSource { get; set; }

	[BindProperty(SupportsGet = true)]
	public bool? CanDownload { get; set; }

	[BindProperty(SupportsGet = true)]
	public VehicleType? VehicleType { get; set; }

	[BindProperty(Name = "p", SupportsGet = true)]
	public int PageNumber { get; set; } = 1;

	[BindProperty(SupportsGet = true)]
	public string Category { get; set; } = "objects";

	public ObjectBrowsePageViewModel Results { get; private set; } = new(0, 0, 1, 48, []);

	public IReadOnlyList<ObjectType> ObjectTypes { get; } = [.. Enum.GetValues<ObjectType>().OrderBy(t => t.ToString())];

	public IReadOnlyList<ObjectSource> ObjectSources { get; } = [.. Enum.GetValues<ObjectSource>()];

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
			["scenarios"] = "Scenarios",
			["scenariopacks"] = "Scenario\u00A0Packs",
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
	public List<ScenarioListViewModel> ScenarioList { get; private set; } = [];
	public List<ScenarioPackListViewModel> ScenarioPackList { get; private set; } = [];
	public List<ObjectsMissingListViewModel> ObjectsMissingList { get; private set; } = [];

	public async Task OnGetAsync(CancellationToken cancellationToken)
	{
		switch (Category)
		{
			case "objects":
				Results = await _explorerService.GetObjectsAsync(
					new ObjectBrowseQuery(Search, ObjectType, ObjectSource, CanDownload, VehicleType, PageNumber),
					cancellationToken);
				PageNumber = Results.Page;
				break;

			case "music":
			case "sfx":
				Results = new(0, 0, 1, 48, []);
				break;

			case "objectpacks":
				await LoadObjectPacksAsync(cancellationToken);
				break;

			case "scenarios":
				await LoadScenariosAsync(cancellationToken);
				break;

			case "scenariopacks":
				await LoadScenarioPacksAsync(cancellationToken);
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
		using var client = _api.CreateClient();
		var packs = await Client.GetObjectPackListEntriesAsync(client, cancellationToken: ct);

		ObjectPackList = [.. packs
			.OrderByDescending(p => p.UploadedDate)
			.Select(p => new ObjectPackListViewModel(
				p.Id,
				p.Name,
				p.Description ?? string.Empty,
				p.UploadedDate,
				p.AuthorCount,
				p.TagCount,
				p.Licence?.Name ?? "None",
				p.ObjectCount))];
	}

	async Task LoadScenariosAsync(CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var files = await Client.GetScenariosAsync(client, cancellationToken: ct);

		ScenarioList = [.. files
			.OrderByDescending(f => f.UploadedDate)
			.Select(f => new ScenarioListViewModel(
				f.Id,
				f.Name,
				f.Description ?? string.Empty,
				f.UploadedDate,
				f.ObjectSource,
				f.AuthorCount,
				f.TagCount,
				f.Licence?.Name ?? "None",
				f.PackCount))];
	}

	async Task LoadScenarioPacksAsync(CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var packs = await Client.GetScenarioPackListEntriesAsync(client, cancellationToken: ct);

		ScenarioPackList = [.. packs
			.OrderByDescending(p => p.UploadedDate)
			.Select(p => new ScenarioPackListViewModel(
				p.Id,
				p.Name,
				p.Description ?? string.Empty,
				p.UploadedDate,
				p.AuthorCount,
				p.TagCount,
				p.Licence?.Name ?? "None",
				p.FileCount))];
	}

	async Task LoadAuthorsAsync(CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var authors = await Client.GetAuthorsAsync(client, cancellationToken: ct);
		AuthorList = [.. authors.OrderBy(a => a.Name).Select(a => new AuthorListViewModel(a.Id, a.Name))];
	}

	async Task LoadTagsAsync(CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var tags = await Client.GetTagsAsync(client, cancellationToken: ct);
		TagList = [.. tags.OrderBy(t => t.Name).Select(t => new TagListViewModel(t.Id, t.Name))];
	}

	async Task LoadLicencesAsync(CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var licences = await Client.GetLicencesAsync(client, cancellationToken: ct);
		LicenceList = [.. licences.OrderBy(l => l.Name).Select(l => new LicenceListViewModel(l.Id, l.Name, l.Text))];
	}

	async Task LoadObjectsMissingAsync(CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var missing = await Client.GetMissingObjectsAsync(client);
		ObjectsMissingList = [.. missing
			.OrderBy(m => m.DatName)
			.ThenBy(m => m.DatChecksum)
			.Select(m => new ObjectsMissingListViewModel(m.Id, m.DatName, m.DatChecksum, m.ObjectType))];
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

	// ── POST: Create author/tag/licence/objectpack/scenariopack/missing object ──

	public async Task<IActionResult> OnPostCreateAsync()
	{
		if (!IsAdmin)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(CrudName))
		{
			ErrorMessage = "Name is required.";
			return RedirectToPage(new { category = CrudCategory });
		}

		using var client = _api.CreateClient();
		switch (CrudCategory)
		{
			case "authors":
			{
				var author = await Client.CreateResourceAsync<DtoAuthorEntry, DtoAuthorEntry>(client, Client.AuthorsEndpointGroup, new DtoAuthorEntry(0, CrudName.Trim()));
				SuccessMessage = author != null ? $"Author '{CrudName.Trim()}' created." : null;
				ErrorMessage = author != null ? null : "Failed to create author.";
				break;
			}
			case "tags":
			{
				var tag = await Client.CreateResourceAsync<DtoTagEntry, DtoTagEntry>(client, Client.TagsEndpointGroup, new DtoTagEntry(0, CrudName.Trim()));
				SuccessMessage = tag != null ? $"Tag '{CrudName.Trim()}' created." : null;
				ErrorMessage = tag != null ? null : "Failed to create tag.";
				break;
			}
			case "licences":
			{
				var licence = await Client.CreateResourceAsync<DtoLicenceEntry, DtoLicenceEntry>(client, Client.LicencesEndpointGroup, new DtoLicenceEntry(0, CrudName.Trim(), CrudText?.Trim() ?? string.Empty));
				SuccessMessage = licence != null ? $"Licence '{CrudName.Trim()}' created." : null;
				ErrorMessage = licence != null ? null : "Failed to create licence.";
				break;
			}
			case "objectsmissing":
			{
				var missing = await Client.AddMissingObjectAsync(client, new DtoObjectMissingPost(CrudName.Trim(), CrudChecksum, CrudObjectType));
				SuccessMessage = missing != null ? $"Missing object '{CrudName.Trim()}' created." : null;
				ErrorMessage = missing != null ? null : "Failed to create missing object.";
				break;
			}
			case "objectpacks":
			{
				var request = new DtoItemPackDescriptor<DtoObjectEntry>(0, CrudName.Trim(), CrudDescription?.Trim(), null, null, DateOnly.FromDateTime(DateTime.UtcNow), [], [], [], null);
				var pack = await Client.CreateResourceAsync<DtoItemPackDescriptor<DtoObjectEntry>, DtoItemPackEntry>(client, Client.ObjectPacksEndpointGroup, request);
				SuccessMessage = pack != null ? $"Object pack '{CrudName.Trim()}' created." : null;
				ErrorMessage = pack != null ? null : "Failed to create object pack.";
				break;
			}
			case "scenariopacks":
			{
				var request = new DtoItemPackDescriptor<DtoScenarioEntry>(0, CrudName.Trim(), CrudDescription?.Trim(), null, null, DateOnly.FromDateTime(DateTime.UtcNow), [], [], [], null);
				var pack = await Client.CreateResourceAsync<DtoItemPackDescriptor<DtoScenarioEntry>, DtoItemPackDescriptor<DtoScenarioEntry>>(client, Client.ScenarioPacksEndpointGroup, request);
				SuccessMessage = pack != null ? $"Scenario pack '{CrudName.Trim()}' created." : null;
				ErrorMessage = pack != null ? null : "Failed to create scenario pack.";
				break;
			}
			default:
				ErrorMessage = "Unknown category.";
				break;
		}

		return RedirectToPage(new { category = CrudCategory });
	}

	// ── POST: Edit author/tag/licence/missing object ──

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(CrudName))
		{
			ErrorMessage = "Name is required.";
			return RedirectToPage(new { category = CrudCategory });
		}

		using var client = _api.CreateClient();
		switch (CrudCategory)
		{
			case "authors":
			{
				var updated = await Client.UpdateResourceAsync<DtoAuthorEntry, DtoAuthorEntry>(client, Client.AuthorsEndpointGroup, CrudId, new DtoAuthorEntry(CrudId, CrudName.Trim()));
				SuccessMessage = updated != null ? $"Author '{CrudName.Trim()}' updated." : null;
				ErrorMessage = updated != null ? null : "Author not found.";
				break;
			}
			case "tags":
			{
				var updated = await Client.UpdateResourceAsync<DtoTagEntry, DtoTagEntry>(client, Client.TagsEndpointGroup, CrudId, new DtoTagEntry(CrudId, CrudName.Trim()));
				SuccessMessage = updated != null ? $"Tag '{CrudName.Trim()}' updated." : null;
				ErrorMessage = updated != null ? null : "Tag not found.";
				break;
			}
			case "licences":
			{
				var updated = await Client.UpdateResourceAsync<DtoLicenceEntry, DtoLicenceEntry>(client, Client.LicencesEndpointGroup, CrudId, new DtoLicenceEntry(CrudId, CrudName.Trim(), CrudText?.Trim() ?? string.Empty));
				SuccessMessage = updated != null ? $"Licence '{CrudName.Trim()}' updated." : null;
				ErrorMessage = updated != null ? null : "Licence not found.";
				break;
			}
			case "objectsmissing":
			{
				var updated = await Client.UpdateResourceAsync<DtoObjectMissingEntry, DtoObjectMissingEntry>(client, Client.MissingObjectsEndpointGroup, CrudId, new DtoObjectMissingEntry(CrudId, CrudName.Trim(), CrudChecksum, CrudObjectType));
				SuccessMessage = updated != null ? $"Missing object '{CrudName.Trim()}' updated." : null;
				ErrorMessage = updated != null ? null : "Missing object not found.";
				break;
			}
			default:
				ErrorMessage = "Unknown category.";
				break;
		}

		return RedirectToPage(new { category = CrudCategory });
	}

	// ── POST: Delete author/tag/licence/objectpack/scenariopack/missing object ──

	public async Task<IActionResult> OnPostDeleteAsync()
	{
		if (!IsAdmin)
		{
			return Forbid();
		}

		using var client = _api.CreateClient();
		bool deleted;
		switch (CrudCategory)
		{
			case "authors":
				deleted = await Client.DeleteResourceAsync(client, Client.AuthorsEndpointGroup, CrudId);
				SuccessMessage = deleted ? "Author deleted." : null;
				ErrorMessage = deleted ? null : "Failed to delete author.";
				break;
			case "tags":
				deleted = await Client.DeleteResourceAsync(client, Client.TagsEndpointGroup, CrudId);
				SuccessMessage = deleted ? "Tag deleted." : null;
				ErrorMessage = deleted ? null : "Failed to delete tag.";
				break;
			case "licences":
				deleted = await Client.DeleteResourceAsync(client, Client.LicencesEndpointGroup, CrudId);
				SuccessMessage = deleted ? "Licence deleted." : null;
				ErrorMessage = deleted ? null : "Failed to delete licence.";
				break;
			case "objectsmissing":
				deleted = await Client.DeleteResourceAsync(client, Client.MissingObjectsEndpointGroup, CrudId);
				SuccessMessage = deleted ? "Missing object deleted." : null;
				ErrorMessage = deleted ? null : "Failed to delete missing object.";
				break;
			case "objectpacks":
				deleted = await Client.DeleteObjectPackAsync(client, CrudId);
				SuccessMessage = deleted ? "Object pack deleted." : null;
				ErrorMessage = deleted ? null : "Failed to delete object pack.";
				break;
			case "scenariopacks":
				deleted = await Client.DeleteScenarioPackAsync(client, CrudId);
				SuccessMessage = deleted ? "Scenario pack deleted." : null;
				ErrorMessage = deleted ? null : "Failed to delete scenario pack.";
				break;
			default:
				ErrorMessage = "Unknown category.";
				break;
		}

		return RedirectToPage(new { category = CrudCategory });
	}

	// ── View models ──
	public record AuthorListViewModel(UniqueObjectId Id, string Name);
	public record TagListViewModel(UniqueObjectId Id, string Name);
	public record LicenceListViewModel(UniqueObjectId Id, string Name, string Text);
	public record ObjectPackListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, int AuthorCount, int TagCount, string Licence, int ObjectCount);
	public record ScenarioListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, ObjectSource ObjectSource, int AuthorCount, int TagCount, string Licence, int PackCount);
	public record ScenarioPackListViewModel(UniqueObjectId Id, string Name, string Description, DateOnly UploadedDate, int AuthorCount, int TagCount, string Licence, int FileCount);
	public record ObjectsMissingListViewModel(UniqueObjectId Id, string DatName, uint32_t DatChecksum, ObjectType ObjectType);
}
