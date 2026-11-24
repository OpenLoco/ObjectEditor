using System.Security.Claims;
using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ObjectService.Identity;
using UniqueObjectId = System.UInt64;

namespace Tests;

/// <summary>
/// Verifies the object ownership authorization rule: a user may edit an object they own or one that
/// credits an author they are linked to, Admins may edit anything, and vanilla (Locomotion) objects
/// stay read-only for everyone.
/// </summary>
[TestFixture]
public class ObjectOwnershipHandlerTests
{
	const UniqueObjectId UserOwnerId = 1;
	const UniqueObjectId UserAuthorId = 2;
	const UniqueObjectId UserOtherId = 3;
	const UniqueObjectId UserAdminId = 4;
	const UniqueObjectId SeedObjectId = 100;

	static async Task<(LocoDbContext Db, SqliteConnection Connection)> CreateSeededDbAsync()
	{
		var connection = new SqliteConnection("DataSource=:memory:");
		await connection.OpenAsync();
		var options = new DbContextOptionsBuilder<LocoDbContext>().UseSqlite(connection).Options;
		var db = new LocoDbContext(options);
		_ = await db.Database.EnsureCreatedAsync();

		var author = new TblAuthor { Id = 1, Name = "Credited Author" };
		_ = await db.Authors.AddAsync(author);

		// Owned, custom object.
		_ = await db.Objects.AddAsync(new TblObject
		{
			Id = SeedObjectId,
			Name = "owned-object",
			ObjectType = ObjectType.Vehicle,
			ObjectSource = ObjectSource.Custom,
			Availability = ObjectAvailability.Available,
			OwnerUserId = UserOwnerId,
			Authors = [author],
		});

		// Unowned object credited to an author.
		_ = await db.Objects.AddAsync(new TblObject
		{
			Id = SeedObjectId + 1,
			Name = "author-object",
			ObjectType = ObjectType.Vehicle,
			ObjectSource = ObjectSource.Custom,
			Availability = ObjectAvailability.Available,
			Authors = [author],
		});

		// Vanilla object explicitly owned by a user - must still be locked.
		_ = await db.Objects.AddAsync(new TblObject
		{
			Id = SeedObjectId + 2,
			Name = "vanilla-object",
			ObjectType = ObjectType.Vehicle,
			ObjectSource = ObjectSource.LocomotionSteam,
			Availability = ObjectAvailability.Available,
			OwnerUserId = UserOwnerId,
		});

		await db.Users.AddRangeAsync(
			new TblUser { Id = UserOwnerId, UserName = "owner" },
			new TblUser { Id = UserAuthorId, UserName = "author-user", AssociatedAuthorId = author.Id },
			new TblUser { Id = UserOtherId, UserName = "other" },
			new TblUser { Id = UserAdminId, UserName = "admin" });

		_ = await db.SaveChangesAsync();
		return (db, connection);
	}

	static async Task<bool> CanEditAsync(LocoDbContext db, UniqueObjectId userId, UniqueObjectId objectId, bool isAdmin = false)
	{
		var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
		if (isAdmin)
		{
			claims.Add(new Claim(ClaimTypes.Role, "Admin"));
		}

		var identity = new ClaimsIdentity(claims, "TestAuth", ClaimTypes.Name, ClaimTypes.Role);
		var httpContext = new DefaultHttpContext();
		httpContext.Request.RouteValues["id"] = objectId.ToString();

		var requirement = new ObjectOwnershipRequirement();
		var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(identity), httpContext);

		await new ObjectOwnershipHandler(db).HandleAsync(context);

		return context.HasSucceeded;
	}

	[Test]
	public async Task Owner_CanEditOwnObject()
	{
		var (db, connection) = await CreateSeededDbAsync();
		await using (db)
		using (connection)
		{
			Assert.That(await CanEditAsync(db, UserOwnerId, SeedObjectId), Is.True);
		}
	}

	[Test]
	public async Task UserLinkedToCreditedAuthor_CanEditObject()
	{
		var (db, connection) = await CreateSeededDbAsync();
		await using (db)
		using (connection)
		{
			Assert.That(await CanEditAsync(db, UserAuthorId, SeedObjectId + 1), Is.True);
		}
	}

	[Test]
	public async Task UnrelatedUser_CannotEditObject()
	{
		var (db, connection) = await CreateSeededDbAsync();
		await using (db)
		using (connection)
		{
			Assert.That(await CanEditAsync(db, UserOtherId, SeedObjectId), Is.False);
		}
	}

	[Test]
	public async Task Admin_CanEditAnyObject()
	{
		var (db, connection) = await CreateSeededDbAsync();
		await using (db)
		using (connection)
		{
			Assert.That(await CanEditAsync(db, UserAdminId, SeedObjectId + 1, isAdmin: true), Is.True);
		}
	}

	[Test]
	public async Task VanillaObject_CannotBeEdited()
	{
		var (db, connection) = await CreateSeededDbAsync();
		await using (db)
		using (connection)
		{
			Assert.That(await CanEditAsync(db, UserOwnerId, SeedObjectId + 2), Is.False);
		}
	}
}