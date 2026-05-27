using Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.RouteHandlers;

public abstract class BaseDataTableRouteHandler<THandler, TContext, TDto, TRow> : ITableRouteHandler
	where TContext : DbContext
	where TDto : class, IHasId
	where TRow : class, IHasId
	where THandler : ITableRouteConfig<TContext, TDto, TRow>
{
	public static string BaseRoute => THandler.GetBaseRoute();

	public static Delegate ListDelegate => ListAsync;

	public static Delegate CreateDelegate => CreateAsync;

	public static Delegate ReadDelegate => ReadAsync;

	public static Delegate UpdateDelegate => UpdateAsync;

	public static Delegate DeleteDelegate => DeleteAsync;

	public static async Task<IResult> CreateAsync(TDto request, [FromServices] TContext db, CancellationToken cancellationToken)
		=> await BaseDataTableRouteHandlerImpl.CreateAsync(
			THandler.GetTable(db),
			THandler.ToDtoFunc,
			THandler.ToRowFunc,
			request,
			() => (THandler.TryValidateCreate(request, db, out var result), result),
			THandler.GetBaseRoute(),
			db,
			cancellationToken);

	public static async Task<IResult> ReadAsync(UniqueObjectId id, [FromServices] TContext db, CancellationToken cancellationToken)
		=> await BaseDataTableRouteHandlerImpl.ReadAsync(THandler.GetTable(db), THandler.ToDtoFunc, id, db, cancellationToken);

	public static async Task<IResult> UpdateAsync(UniqueObjectId id, TDto request, [FromServices] TContext db, CancellationToken cancellationToken)
		=> await BaseDataTableRouteHandlerImpl.UpdateAsync(
			THandler.GetTable(db),
			THandler.ToDtoFunc,
			THandler.ToRowFunc,
			request,
			() => (THandler.TryValidateCreate(request, db, out var result), result),
			THandler.GetBaseRoute(),
			id,
			db,
			THandler.UpdateFunc,
			cancellationToken);

	public static async Task<IResult> DeleteAsync(UniqueObjectId id, [FromServices] TContext db, CancellationToken cancellationToken)
		=> await BaseDataTableRouteHandlerImpl.DeleteAsync(THandler.GetTable(db), THandler.ToDtoFunc, id, db, cancellationToken);

	public static async Task<IResult> ListAsync(HttpContext context, [FromServices] TContext db, CancellationToken cancellationToken)
		=> await BaseDataTableRouteHandlerImpl.ListAsync(context, THandler.GetTable(db), THandler.ToDtoFunc, cancellationToken);
}

public static class BaseDataTableRouteHandlerImpl
{
	public static async Task<IResult> CreateAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, Func<TDto, TRow> rowConverter, TDto request, Func<(bool Success, IResult? ErrorMessage)> tryValidateFunc, string baseRoute, DbContext db, CancellationToken cancellationToken)
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		var (Success, ErrorMessage) = tryValidateFunc();
		if (!Success)
		{
			return ErrorMessage!;
		}

		var row = rowConverter(request);
		_ = await table.AddAsync(row, cancellationToken);
		_ = await db.SaveChangesAsync(cancellationToken);
		return Results.Created($"{baseRoute}/{row.Id}", dtoConverter(row));
	}

	public static async Task<IResult> ReadAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, UniqueObjectId id, DbContext db, CancellationToken cancellationToken)
		where TDto : class, IHasId
		where TRow : class, IHasId
		=> await table.FindAsync([id], cancellationToken) is TRow row
			? Results.Ok(dtoConverter(row))
			: Results.NotFound();

	public static async Task<IResult> UpdateAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, Func<TDto, TRow> rowConverter, TDto request, Func<(bool Success, IResult? ErrorMessage)> tryValidateFunc, string baseRoute, UniqueObjectId id, DbContext db, Action<TDto, TRow> updateFunc, CancellationToken cancellationToken)
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		if (await table.FindAsync([id], cancellationToken) is not TRow row) // do not use Request.Id here, use the route id
		{
			return await CreateAsync(table, dtoConverter, rowConverter, request, tryValidateFunc, baseRoute, db, cancellationToken);
		}

		updateFunc(request, row);
		_ = await db.SaveChangesAsync(cancellationToken);
		return Results.Accepted($"{baseRoute}/{row.Id}", dtoConverter(row));
	}

	public static async Task<IResult> DeleteAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, UniqueObjectId id, DbContext db, CancellationToken cancellationToken)
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		if (await table.FindAsync([id], cancellationToken) is TRow row)
		{
			_ = table.Remove(row);
			_ = await db.SaveChangesAsync(cancellationToken);
			return Results.Ok();
		}

		return Results.NotFound();
	}

	public static async Task<IResult> ListAsync<TDto, TRow>(HttpContext context, DbSet<TRow> table, Func<TRow, TDto> dtoConverter, CancellationToken cancellationToken)
		where TDto : class, IHasId
		where TRow : class, IHasId
		=> Results.Ok(await table
			.Select(x => dtoConverter(x))
			.ToListAsync(cancellationToken));
}
