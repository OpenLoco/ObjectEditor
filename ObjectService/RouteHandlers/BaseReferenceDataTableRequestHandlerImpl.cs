using Definitions;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;

namespace ObjectService.RouteHandlers
{
	public static class BaseReferenceDataTableRequestHandlerImpl
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

		public static async Task<IResult> ReadAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, int id, LocoDbContext db)
			where TDto : class, IHasId
			where TRow : class, IHasId
			=> await table.FindAsync(id) is TRow row
				? Results.Ok(dtoConverter(row))
				: Results.NotFound();

		public static async Task<IResult> UpdateAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, Func<TDto, TRow> rowConverter, TDto request, Func<(bool Success, IResult? ErrorMessage)> tryValidateFunc, string baseRoute, int id, LocoDbContext db, Action<TDto, TRow> updateFunc)
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

		public static async Task<IResult> DeleteAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, int id, LocoDbContext db)
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
