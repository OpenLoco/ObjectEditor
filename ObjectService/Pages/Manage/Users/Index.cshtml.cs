using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;

namespace ObjectService.Pages.Manage.Users;

[Authorize(Policy = "AdminOnly")]
public sealed class IndexModel : PageModel
{
	private readonly LocoDbContext _db;

	public IndexModel(LocoDbContext db)
	{
		_db = db;
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
		var allRoles = await _db.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
		var userRolesData = await _db.UserRoles.ToListAsync();
		var userQuery = _db.Users.AsQueryable();

		if (!string.IsNullOrWhiteSpace(Search))
		{
			var s = Search.Trim();
			userQuery = userQuery.Where(u => (u.UserName != null && u.UserName.Contains(s)) || (u.Email != null && u.Email.Contains(s)));
		}

		var userList = await userQuery.OrderBy(u => u.UserName).ToListAsync();

		Users = userList.Select(u =>
		{
			var userRoles = userRolesData.Where(ur => ur.UserId == u.Id);
			var roleNames = userRoles.Where(ur => allRoles.ContainsKey(ur.RoleId)).Select(ur => allRoles[ur.RoleId] ?? "").ToList();
			return new UserViewModel(
				u.Id,
				u.UserName ?? "Unknown",
				u.Email ?? "",
				string.Join(", ", roleNames));
		}).ToList();
	}

	public record UserViewModel(
		UniqueObjectId Id,
		string UserName,
		string Email,
		string Roles);
}
