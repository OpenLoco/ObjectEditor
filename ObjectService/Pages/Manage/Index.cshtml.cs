using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;

namespace ObjectService.Pages.Manage;

[Authorize(Policy = "AdminOnly")]
public sealed class ManageIndexModel : PageModel
{
	private readonly LocoDbContext _db;

	public ManageIndexModel(LocoDbContext db)
	{
		_db = db;
	}

	public int TotalObjects { get; set; }
	public int TotalAuthors { get; set; }
	public int TotalTags { get; set; }
	public int TotalLicences { get; set; }
	public int TotalObjectPacks { get; set; }
	public int TotalUsers { get; set; }

	public async Task OnGetAsync()
	{
		TotalObjects = await _db.Objects.CountAsync();
		TotalAuthors = await _db.Authors.CountAsync();
		TotalTags = await _db.Tags.CountAsync();
		TotalLicences = await _db.Licences.CountAsync();
		TotalObjectPacks = await _db.ObjectPacks.CountAsync();
		TotalUsers = await _db.Users.CountAsync();
	}
}
