using Definitions;
using Definitions.DTO.Identity;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using System.Net.Http.Json;

namespace ObjectService.Pages.Account;

[Authorize]
public sealed class ManageModel : PageModel
{
	private readonly FrontendApiClient _api;

	public ManageModel(FrontendApiClient api)
	{
		_api = api;
	}

	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public bool AccountDeleted { get; set; }
	public List<OwnedObjectViewModel> OwnedObjects { get; set; } = [];

	public async Task<IActionResult> OnGetAsync()
	{
		using var client = _api.CreateClient();
		var info = await GetInfoAsync(client);
		if (info == null)
		{
			return RedirectToPage("/Account/Login");
		}

		Username = info.UserName;
		Email = info.Email;

		OwnedObjects = [.. (await Client.GetMyObjectsAsync(client))
			.OrderByDescending(x => x.UploadedDate)
			.Select(x => new OwnedObjectViewModel(
				x.Id,
				x.InternalName,
				x.ObjectType,
				x.ObjectSource,
				x.UploadedDate,
				string.IsNullOrWhiteSpace(x.DisplayName) ? x.InternalName : x.DisplayName))];

		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAccountAsync()
	{
		using var client = _api.CreateClient();
		_ = await Client.DeleteCurrentUserAsync(client);
		await LogoutAsync(client);

		AccountDeleted = true;
		return Page();
	}

	public async Task<IActionResult> OnPostLogoutAsync()
	{
		using var client = _api.CreateClient();
		await LogoutAsync(client);
		return RedirectToPage("/Index");
	}

	static async Task<DtoInfoResponse?> GetInfoAsync(HttpClient client)
	{
		try
		{
			return await client.GetFromJsonAsync<DtoInfoResponse>("/manage/info");
		}
		catch (HttpRequestException)
		{
			return null;
		}
	}

	async Task LogoutAsync(HttpClient client)
	{
		try
		{
			using var response = await client.PostAsync("/logout", null);
		}
		catch (HttpRequestException)
		{
			// ignore - we clear local state regardless
		}

		Response.Cookies.Delete("access_token");
	}

	public sealed record OwnedObjectViewModel(
		UniqueObjectId Id,
		string InternalName,
		ObjectType ObjectType,
		ObjectSource ObjectSource,
		DateOnly UploadedDate,
		string DisplayName);
}
