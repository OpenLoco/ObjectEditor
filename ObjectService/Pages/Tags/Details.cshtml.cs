using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages.Tags;

public sealed class DetailsModel : PageModel
{
	readonly FrontendApiClient _api;

	public DetailsModel(FrontendApiClient api)
	{
		_api = api;
	}

	public DtoTagDescriptor? Tag { get; private set; }

	public IReadOnlyList<DtoItemRef> Objects { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> ObjectPacks { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> Scenarios { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> ScenarioPacks { get; private set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	[BindProperty]
	public UniqueObjectId Id { get; set; }

	[BindProperty]
	public string Name { get; set; } = string.Empty;

	public bool IsAdmin => User.IsInRole("Admin");

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id, CancellationToken ct)
	{
		await LoadAsync(id, ct);
		return Tag is null ? NotFound() : Page();
	}

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Tag name is required.";
			await LoadAsync(Id, CancellationToken.None);
			return Page();
		}

		using var client = _api.CreateClient();
		var updated = await Client.UpdateResourceAsync<DtoTagEntry, DtoTagEntry>(
			client, Client.TagsEndpointGroup, Id, new DtoTagEntry(Id, Name.Trim()));

		if (updated != null)
		{
			SuccessMessage = $"Tag '{Name.Trim()}' updated.";
		}
		else
		{
			ErrorMessage = "Tag not found.";
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
		var deleted = await Client.DeleteResourceAsync(client, Client.TagsEndpointGroup, id);
		if (deleted)
		{
			SuccessMessage = "Tag deleted.";
			return RedirectToPage("/Index", new { category = "tags" });
		}

		await LoadAsync(id, CancellationToken.None);
		ErrorMessage = "Failed to delete tag.";
		return Page();
	}

	async Task LoadAsync(UniqueObjectId id, CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var tag = await Client.GetTagDescriptorAsync(client, id, cancellationToken: ct);
		Tag = tag;

		if (tag != null)
		{
			Objects = [.. tag.Objects];
			ObjectPacks = [.. tag.ObjectPacks];
			Scenarios = [.. tag.Scenarios];
			ScenarioPacks = [.. tag.ScenarioPacks];
		}
	}
}
