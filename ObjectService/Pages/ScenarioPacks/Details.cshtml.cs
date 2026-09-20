using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using ObjectService.Identity;

namespace ObjectService.Pages.ScenarioPacks;

public sealed class DetailsModel : PageModel
{
	readonly FrontendApiClient _api;

	public DetailsModel(FrontendApiClient api)
	{
		_api = api;
	}

	public DtoScenarioPackDescriptor? Pack { get; private set; }

	public List<DtoAuthorEntry> AvailableAuthors { get; private set; } = [];
	public List<DtoTagEntry> AvailableTags { get; private set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; private set; } = [];
	public List<DtoItemRef> AvailableScenarios { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public bool CanEdit => User.IsInRole("Admin") || User.IsInRole("Curator")
		|| User.HasClaim(LocoPermissions.ClaimType, LocoPermissions.ScenarioPacksModify);

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		await LoadAsync(id, ct);
		return Pack is null ? NotFound() : Page();
	}

	public async Task<IActionResult> OnPostEditAsync(
		[FromForm] UniqueObjectId Id,
		[FromForm] string Name,
		[FromForm] string? Description,
		[FromForm] DateOnly? CreatedDate,
		[FromForm] DateOnly? ModifiedDate,
		[FromForm] UniqueObjectId? LicenceId,
		[FromForm] List<UniqueObjectId>? SelectedAuthorIds,
		[FromForm] List<UniqueObjectId>? SelectedTagIds,
		[FromForm] List<UniqueObjectId>? SelectedSC5FileIds)
	{
		if (!CanEdit)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Scenario pack name is required.";
			await LoadAsync(Id, CancellationToken.None);
			return Page();
		}

		SelectedAuthorIds ??= [];
		SelectedTagIds ??= [];
		SelectedSC5FileIds ??= [];

		var request = new DtoScenarioPackDescriptor(
			Id,
			Name.Trim(),
			Description?.Trim(),
			CreatedDate,
			ModifiedDate,
			DateOnly.FromDateTime(DateTime.UtcNow),
			LicenceId.HasValue ? new DtoLicenceEntry(LicenceId.Value, string.Empty, string.Empty) : null,
			[.. SelectedAuthorIds.Select(a => new DtoAuthorEntry(a, string.Empty))],
			[.. SelectedTagIds.Select(t => new DtoTagEntry(t, string.Empty))],
			[.. SelectedSC5FileIds.Select(f => new DtoItemRef(f, string.Empty))]);

		using var client = _api.CreateClient();
		var updated = await Client.UpdateScenarioPackAsync(client, request);
		if (updated != null)
		{
			SuccessMessage = $"Scenario pack '{Name.Trim()}' updated.";
		}
		else
		{
			ErrorMessage = "Scenario pack not found.";
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
		var deleted = await Client.DeleteScenarioPackAsync(client, id);
		if (deleted)
		{
			SuccessMessage = "Scenario pack deleted.";
			return RedirectToPage("/Index", new { category = "scenariopacks" });
		}

		await LoadAsync(id, CancellationToken.None);
		ErrorMessage = "Failed to delete scenario pack.";
		return Page();
	}

	async Task LoadAsync(UniqueObjectId id, CancellationToken ct)
	{
		using var client = _api.CreateClient();
		Pack = await Client.GetScenarioPackDescriptorAsync(client, id, cancellationToken: ct);
		AvailableAuthors = [.. (await Client.GetAuthorsAsync(client, cancellationToken: ct)).OrderBy(a => a.Name)];
		AvailableTags = [.. (await Client.GetTagsAsync(client, cancellationToken: ct)).OrderBy(t => t.Name)];
		AvailableLicences = [.. (await Client.GetLicencesAsync(client, cancellationToken: ct)).OrderBy(l => l.Name)];
		AvailableScenarios = [.. (await Client.GetScenariosAsync(client, cancellationToken: ct)).OrderBy(s => s.Name).Select(s => new DtoItemRef(s.Id, s.Name))];
	}
}
