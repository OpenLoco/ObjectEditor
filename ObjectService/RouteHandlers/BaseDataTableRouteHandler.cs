using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions;
using OpenLoco.Definitions.Database;

namespace ObjectService.RouteHandlers
{
	public abstract class BaseDataTableRouteHandler<THandler, TDto, TRow> : ITableRouteHandler
		where TDto : class, IHasId
		where TRow : class, IHasId
		where THandler : ITableRouteConfig<TDto, TRow>
	{
		public static string BaseRoute => THandler.GetBaseRoute();

		public static Delegate ListDelegate => ListAsync;

		public static Delegate CreateDelegate => CreateAsync;

		public static Delegate ReadDelegate => ReadAsync;

		public static Delegate UpdateDelegate => UpdateAsync;

		public static Delegate DeleteDelegate => DeleteAsync;

		public static async Task<IResult> CreateAsync(TDto request, LocoDbContext db)
			=> await BaseDataTableRouteHandlerImpl.CreateAsync(
				THandler.GetTable(db),
				THandler.ToDtoFunc,
				THandler.ToRowFunc,
				request,
				() => (THandler.TryValidateCreate(request, db, out var result), result),
				THandler.GetBaseRoute(),
				db);

		public static async Task<IResult> ReadAsync(UniqueObjectId id, LocoDbContext db)
			=> await BaseDataTableRouteHandlerImpl.ReadAsync(THandler.GetTable(db), THandler.ToDtoFunc, id, db);

		public static async Task<IResult> UpdateAsync(UniqueObjectId id, TDto request, LocoDbContext db)
			=> await BaseDataTableRouteHandlerImpl.UpdateAsync(
				THandler.GetTable(db),
				THandler.ToDtoFunc,
				THandler.ToRowFunc,
				request,
				() => (THandler.TryValidateCreate(request, db, out var result), result),
				THandler.GetBaseRoute(),
				id,
				db,
				THandler.UpdateFunc);

		public static async Task<IResult> DeleteAsync(UniqueObjectId id, LocoDbContext db)
			=> await BaseDataTableRouteHandlerImpl.DeleteAsync(THandler.GetTable(db), THandler.ToDtoFunc, id, db);

		public static async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
			=> await BaseDataTableRouteHandlerImpl.ListAsync(context, THandler.GetTable(db), THandler.ToDtoFunc);
	}

	public static class BaseDataTableRouteHandlerImpl
	{
		public static async Task<IResult> CreateAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, Func<TDto, TRow> rowConverter, TDto request, Func<(bool Success, IResult? ErrorMessage)> tryValidateFunc, string baseRoute, LocoDbContext db)
			where TDto : class, IHasId
			where TRow : class, IHasId
		{
			var (Success, ErrorMessage) = tryValidateFunc();
			if (!Success)
			{
				return ErrorMessage!;
			}

			var row = rowConverter(request);
			_ = await table.AddAsync(row);
			_ = await db.SaveChangesAsync();
			return Results.Created($"{baseRoute}/{row.Id}", dtoConverter(row));
		}

		public static async Task<IResult> ReadAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, UniqueObjectId id, LocoDbContext db)
			where TDto : class, IHasId
			where TRow : class, IHasId
			=> await table.FindAsync(id) is TRow row
				? Results.Ok(dtoConverter(row))
				: Results.NotFound();

		public static async Task<IResult> UpdateAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, Func<TDto, TRow> rowConverter, TDto request, Func<(bool Success, IResult? ErrorMessage)> tryValidateFunc, string baseRoute, UniqueObjectId id, LocoDbContext db, Action<TDto, TRow> updateFunc)
			where TDto : class, IHasId
			where TRow : class, IHasId
		{
			if (await table.FindAsync(id) is not TRow row) // do not use Request.Id here, use the route id
			{
				return await CreateAsync(table, dtoConverter, rowConverter, request, tryValidateFunc, baseRoute, db);
			}

			updateFunc(request, row);
			_ = await db.SaveChangesAsync();
			return Results.Accepted($"{baseRoute}/{row.Id}", dtoConverter(row));
		}

		public static async Task<IResult> DeleteAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, UniqueObjectId id, LocoDbContext db)
			where TDto : class, IHasId
			where TRow : class, IHasId
		{
			if (await table.FindAsync(id) is TRow row)
			{
				_ = table.Remove(row);
				_ = await db.SaveChangesAsync();
				return Results.Ok();
			}

			return Results.NotFound();
		}

		public static async Task<IResult> ListAsync<TDto, TRow>(HttpContext context, DbSet<TRow> table, Func<TRow, TDto> dtoConverter)
			where TDto : class, IHasId
			where TRow : class, IHasId
			=> Results.Ok(await table
				.Select(x => dtoConverter(x))
				.ToListAsync());
	}
}
