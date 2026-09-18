using Definitions;
using Definitions.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;

namespace ObjectService.Pages.Manage.Users;

[Authorize(Policy = "AdminOnly")]
public sealed class IndexModel : PageModel
{
	readonly FrontendApiClient _api;

	public IndexModel(FrontendApiClient api)
	{
		_api = api;
	}

	public List<UserViewModel> Users { get; set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	[BindProperty(SupportsGet = true)]
	public string? Search { get; set; }

	public async Task OnGetAsync()
	{
		using var client = _api.CreateClient();
		var users = (await Client.GetUsersAsync(client)).AsEnumerable();

		if (!string.IsNullOrWhiteSpace(Search))
		{
			var s = Search.Trim();
			users = users.Where(u =>
				(u.UserName?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false)
				|| (u.Email?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
		}

		Users = [.. users
			.OrderBy(u => u.UserName)
			.Select(u => new UserViewModel(
				u.Id,
				u.UserName,
				u.Email,
				string.Join(", ", u.Roles)))];
	}

	public record UserViewModel(
		UniqueObjectId Id,
		string UserName,
		string Email,
		string Roles);
}
