using Definitions;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages.Authors;

public sealed class DetailsModel : PageModel
{
	readonly FrontendApiClient _api;

	public DetailsModel(FrontendApiClient api)
	{
		_api = api;
	}

	public DtoAuthorDescriptor? Author { get; private set; }

	public IReadOnlyList<DtoItemRef> Objects { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> ObjectPacks { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> SC5Files { get; private set; } = [];
	public IReadOnlyList<DtoItemRef> SC5FilePacks { get; private set; } = [];

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
		return Author is null ? NotFound() : Page();
	}

	public async Task<IActionResult> OnPostEditAsync()
	{
		if (!IsAdmin)
		{
			return Forbid();
		}

		if (string.IsNullOrWhiteSpace(Name))
		{
			ErrorMessage = "Author name is required.";
			await LoadAsync(Id, CancellationToken.None);
			return Page();
		}

		using var client = _api.CreateClient();
		var updated = await Client.UpdateResourceAsync<DtoAuthorEntry, DtoAuthorEntry>(
			client, Client.AuthorsEndpointGroup, Id, new DtoAuthorEntry(Id, Name.Trim()));

		if (updated != null)
		{
			SuccessMessage = $"Author '{Name.Trim()}' updated.";
		}
		else
		{
			ErrorMessage = "Author not found.";
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
		var deleted = await Client.DeleteResourceAsync(client, Client.AuthorsEndpointGroup, id);
		if (deleted)
		{
			SuccessMessage = "Author deleted.";
			return RedirectToPage("/Index", new { category = "authors" });
		}

		await LoadAsync(id, CancellationToken.None);
		ErrorMessage = "Failed to delete author.";
		return Page();
	}

	async Task LoadAsync(UniqueObjectId id, CancellationToken ct)
	{
		using var client = _api.CreateClient();
		var author = await Client.GetAuthorDescriptorAsync(client, id, cancellationToken: ct);
		Author = author;

		if (author != null)
		{
			Objects = [.. author.Objects];
			ObjectPacks = [.. author.ObjectPacks];
			SC5Files = [.. author.SC5Files];
			SC5FilePacks = [.. author.SC5FilePacks];
		}
	}
}
