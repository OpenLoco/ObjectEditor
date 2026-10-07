using Definitions;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages.Objects;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	readonly FrontendApiClient _api;

	public EditModel(FrontendApiClient api)
	{
		_api = api;
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

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		using var client = _api.CreateClient();
		Object = await Client.GetObjectAsync(client, id, cancellationToken: ct);
		if (Object == null)
		{
			return NotFound();
		}

		if (Object.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
		{
			ErrorMessage = "Vanilla game objects cannot be edited.";
			return Page();
		}

		Id = Object.Id;
		Name = Object.Name;
		Description = Object.Description;
		CreatedDate = Object.CreatedDate;
		ModifiedDate = Object.ModifiedDate;
		UploadedDate = Object.UploadedDate;
		LicenceId = Object.Licence?.Id;
		Availability = Object.Availability;
		SelectedAuthorIds = [.. Object.Authors.Select(a => a.Id)];
		SelectedTagIds = [.. Object.Tags.Select(t => t.Id)];
		SelectedObjectPackIds = [.. Object.ObjectPacks.Select(p => p.Id)];

		await LoadListsAsync(client, ct);
		return Page();
	}

	public async Task<IActionResult> OnPostAsync(CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var existing = await Client.GetObjectAsync(client, Id, cancellationToken: ct);
		if (existing == null)
		{
			ErrorMessage = "Object not found.";
			await LoadListsAsync(client, ct);
			return Page();
		}

		Object = existing;

		var licenceEntry = LicenceId.HasValue && existing.Licence != null
			? new DtoLicenceEntry(existing.Licence.Id, existing.Licence.Name, existing.Licence.Text)
			: null;

		var updateRequest = new DtoObjectPostResponse(
			Id,
			Name,
			Name, // DisplayName
			existing.DatChecksum,
			Description,
			existing.ObjectSource,
			existing.ObjectType,
			existing.VehicleType,
			Availability,
			CreatedDate,
			ModifiedDate,
			UploadedDate,
			licenceEntry,
			[.. SelectedAuthorIds.Select(a => new DtoAuthorEntry(a, string.Empty))],
			[.. SelectedTagIds.Select(t => new DtoTagEntry(t, string.Empty))],
			[.. SelectedObjectPackIds.Select(p => new DtoItemPackEntry(p, string.Empty, null, null, null, UploadedDate, null))],
			existing.DatObjects,
			existing.StringTable,
			existing.SubObject);

		var updated = await Client.UpdateObjectAsync(client, Id, updateRequest, cancellationToken: ct);
		if (updated != null)
		{
			return RedirectToPage("/Objects/Details", new { id = Id.ToString() });
		}

		ErrorMessage = "Failed to update object. It may no longer exist.";
		await LoadListsAsync(client, ct);
		return Page();
	}

	async Task LoadListsAsync(HttpClient client, CancellationToken ct)
	{
		AvailableAuthors = [.. await Client.GetAuthorsAsync(client, cancellationToken: ct)];
		AvailableTags = [.. await Client.GetTagsAsync(client, cancellationToken: ct)];
		AvailableLicences = [.. await Client.GetLicencesAsync(client, cancellationToken: ct)];
		AvailableObjectPacks = [.. await Client.GetObjectPacksAsync(client, cancellationToken: ct)];
	}
}
