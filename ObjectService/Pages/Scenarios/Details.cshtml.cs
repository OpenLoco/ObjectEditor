using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages.Scenarios;

public sealed class DetailsModel : PageModel
{
	readonly FrontendApiClient _api;

	public DetailsModel(FrontendApiClient api)
	{
		_api = api;
	}

	public DtoScenarioDescriptor? Scenario { get; private set; }

	public List<DtoAuthorEntry> AvailableAuthors { get; private set; } = [];
	public List<DtoTagEntry> AvailableTags { get; private set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; private set; } = [];
	public List<DtoItemRef> AvailablePacks { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

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
		await LoadAsync(id, ct);
		return Scenario is null ? NotFound() : Page();
	}

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!CanEdit)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Scenario name is required.";
			await LoadAsync(Id, CancellationToken.None);
			return Page();
		}

		SelectedAuthorIds ??= [];
		SelectedTagIds ??= [];
		SelectedPackIds ??= [];

		var request = new DtoScenarioDescriptor(
			Id,
			Name.Trim(),
			Description?.Trim(),
			ObjectSource,
			CreatedDate,
			ModifiedDate,
			UploadedDate,
			LicenceId.HasValue ? new DtoLicenceEntry(LicenceId.Value, string.Empty, string.Empty) : null,
			[.. SelectedAuthorIds.Select(a => new DtoAuthorEntry(a, string.Empty))],
			[.. SelectedTagIds.Select(t => new DtoTagEntry(t, string.Empty))],
			[.. SelectedPackIds.Select(p => new DtoItemRef(p, string.Empty))]);

		using var client = _api.CreateClient();
		var updated = await Client.UpdateScenarioAsync(client, request);
		if (updated != null)
		{
			SuccessMessage = $"Scenario '{Name.Trim()}' updated.";
		}
		else
		{
			ErrorMessage = "Scenario not found.";
		}

		await LoadAsync(Id, CancellationToken.None);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!CanEdit)
		{
			return Forbid();
		}

		using var client = _api.CreateClient();
		var deleted = await Client.DeleteScenarioAsync(client, id);
		if (deleted)
		{
			SuccessMessage = "Scenario deleted.";
			return RedirectToPage("/Index", new { category = "scenarios" });
		}

		await LoadAsync(id, CancellationToken.None);
		ErrorMessage = "Failed to delete scenario.";
		return Page();
	}

	async Task LoadAsync(UniqueObjectId id, CancellationToken ct)
	{
		using var client = _api.CreateClient();
		Scenario = await Client.GetScenarioAsync(client, id, cancellationToken: ct);
		AvailableAuthors = [.. (await Client.GetAuthorsAsync(client, cancellationToken: ct)).OrderBy(a => a.Name)];
		AvailableTags = [.. (await Client.GetTagsAsync(client, cancellationToken: ct)).OrderBy(t => t.Name)];
		AvailableLicences = [.. (await Client.GetLicencesAsync(client, cancellationToken: ct)).OrderBy(l => l.Name)];
		AvailablePacks = [.. (await Client.GetScenarioPackListEntriesAsync(client, cancellationToken: ct)).OrderBy(p => p.Name).Select(p => new DtoItemRef(p.Id, p.Name))];
	}
}
