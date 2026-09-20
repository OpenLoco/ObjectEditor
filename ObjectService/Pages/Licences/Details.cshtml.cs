using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages.Licences;

public sealed class DetailsModel : PageModel
{
	readonly FrontendApiClient _api;

	public DetailsModel(FrontendApiClient api)
	{
		_api = api;
	}

	public DtoLicenceDescriptor? Licence { get; private set; }

	public IReadOnlyList<DtoItemRef> Objects { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> ObjectPacks { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> SC5Files { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> ScenarioPacks { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	[BindProperty]
	public UniqueObjectId Id { get; set; }

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	[BindProperty]
	public string Text { get; set; } = string.Empty;

	public bool IsAdmin => User.IsInRole("Admin");

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		await LoadAsync(id, ct);
		return Licence is null ? NotFound() : Page();
	}

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Licence name is required.";
			await LoadAsync(Id, CancellationToken.None);
			return Page();
		}

		using var client = _api.CreateClient();
		var updated = await Client.UpdateResourceAsync<DtoLicenceEntry, DtoLicenceEntry>(
			client, Client.LicencesEndpointGroup, Id, new DtoLicenceEntry(Id, Name.Trim(), Text?.Trim() ?? string.Empty));

		if (updated != null)
		{
			SuccessMessage = $"Licence '{Name.Trim()}' updated.";
		}
		else
		{
			ErrorMessage = "Licence not found.";
		}

		await LoadAsync(Id, CancellationToken.None);
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(UniqueObjectId id)
	{
		if (!IsAdmin)
		{
			return Forbid();
		}

		using var client = _api.CreateClient();
		var deleted = await Client.DeleteResourceAsync(client, Client.LicencesEndpointGroup, id);
		if (deleted)
		{
			SuccessMessage = "Licence deleted.";
			return RedirectToPage("/Index", new { category = "licences" });
		}

		await LoadAsync(id, CancellationToken.None);
		ErrorMessage = "Failed to delete licence.";
		return Page();
	}

	async Task LoadAsync(UniqueObjectId id, CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var licence = await Client.GetLicenceDescriptorAsync(client, id, cancellationToken: ct);
		Licence = licence;

		if (licence != null)
		{
			Objects = [.. licence.Objects];
			ObjectPacks = [.. licence.ObjectPacks];
			SC5Files = [.. licence.SC5Files];
			ScenarioPacks = [.. licence.ScenarioPacks];
		}
	}
}
