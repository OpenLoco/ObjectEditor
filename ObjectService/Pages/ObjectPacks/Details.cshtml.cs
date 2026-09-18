using Definitions;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using ObjectService.Identity;

namespace ObjectService.Pages.ObjectPacks;

public sealed class DetailsModel : PageModel
{
	readonly FrontendApiClient _api;

	public DetailsModel(FrontendApiClient api)
	{
		_api = api;
	}

	public DtoObjectPackDescriptor? ObjectPack { get; private set; }

	public List<DtoAuthorEntry> AvailableAuthors { get; private set; } = [];
	public List<DtoTagEntry> AvailableTags { get; private set; } = [];
	public List<DtoLicenceEntry> AvailableLicences { get; private set; } = [];
	public List<DtoObjectEntry> AvailableObjects { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public bool CanEdit => User.IsInRole("Admin") || User.IsInRole("Curator")
		|| User.HasClaim(LocoPermissions.ClaimType, LocoPermissions.ObjectPacksModify);

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		await LoadAsync(id, ct);
		return ObjectPack is null ? NotFound() : Page();
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
		[FromForm] List<UniqueObjectId>? SelectedObjectIds)
	{
		if (!CanEdit)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Object pack name is required.";
			await LoadAsync(Id, CancellationToken.None);
			return Page();
		}

		SelectedAuthorIds ??= [];
		SelectedTagIds ??= [];
		SelectedObjectIds ??= [];

		var request = new DtoObjectPackDescriptor(
			Id,
			Name.Trim(),
			Description?.Trim(),
			CreatedDate,
			ModifiedDate,
			DateOnly.FromDateTime(DateTime.UtcNow),
			LicenceId.HasValue ? new DtoLicenceEntry(LicenceId.Value, string.Empty, string.Empty) : null,
			[.. SelectedAuthorIds.Select(a => new DtoAuthorEntry(a, string.Empty))],
			[.. SelectedTagIds.Select(t => new DtoTagEntry(t, string.Empty))],
			[.. SelectedObjectIds.Select(o => new DtoItemRef(o, string.Empty))]);

		using var client = _api.CreateClient();
		var updated = await Client.UpdateObjectPackAsync(client, request);
		if (updated != null)
		{
			SuccessMessage = $"Object pack '{Name.Trim()}' updated.";
		}
		else
		{
			ErrorMessage = "Object pack not found.";
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
		var deleted = await Client.DeleteObjectPackAsync(client, id);
		if (deleted)
		{
			SuccessMessage = "Object pack deleted.";
			return RedirectToPage("/Index", new { category = "objectpacks" });
		}

		await LoadAsync(id, CancellationToken.None);
		ErrorMessage = "Failed to delete object pack.";
		return Page();
	}

	async Task LoadAsync(UniqueObjectId id, CancellationToken ct)
	{
		using var client = _api.CreateClient();
		ObjectPack = await Client.GetObjectPackDescriptorAsync(client, id, cancellationToken: ct);
		AvailableAuthors = [.. (await Client.GetAuthorsAsync(client, cancellationToken: ct)).OrderBy(a => a.Name)];
		AvailableTags = [.. (await Client.GetTagsAsync(client, cancellationToken: ct)).OrderBy(t => t.Name)];
		AvailableLicences = [.. (await Client.GetLicencesAsync(client, cancellationToken: ct)).OrderBy(l => l.Name)];
		AvailableObjects = [.. (await Client.GetObjectListAsync(client, cancellationToken: ct)).OrderBy(o => o.DisplayName)];
	}
}
