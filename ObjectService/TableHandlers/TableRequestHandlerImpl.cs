using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using SixLabors.ImageSharp;

namespace ObjectService.TableHandlers
{
	public static class TableRequestHandlerImpl
	{
		public static async Task<IResult> CreateAsync<TDto, TRow>(TDto request, DbSet<TRow> table, string route, Func<TDto, TRow> ToRowFunc, Func<TRow, TDto> ToDtoFunc, LocoDb db)
			where TDto : class, IHasId
			where TRow : class, IHasId
		{
			var row = ToRowFunc(request);
			_ = await table.AddAsync(row);
			_ = await db.SaveChangesAsync();
			return Results.Created($"{route}/{row.Id}", ToDtoFunc(row));
		}

		public static async Task<IResult> ReadById<TDto, TRow>(int id, DbSet<TRow> table, Func<TRow, TDto> ToDtoFunc, LocoDb db)
			where TDto : IHasId
			where TRow : class
			=> await table.FindAsync(id) is TRow row
				? Results.Ok(ToDtoFunc(row))
				: Results.NotFound();

		public static async Task<IResult> UpdateById<TDto, TRow>(TDto request, DbSet<TRow> table, Action<TDto, TRow> updateMethod, LocoDb db)
			where TDto : IHasId
			where TRow : class
		{
			if (await table.FindAsync(request.Id) is not TRow row)
			{
				return Results.NotFound();
			}

			updateMethod(request, row);

			_ = await db.SaveChangesAsync();
			return Results.NoContent();
		}

		public static async Task<IResult> DeleteById<TRow>(int id, DbSet<TRow> table, LocoDb db) where TRow : class
		{
			if (await table.FindAsync(id) is TRow row)
			{
				_ = table.Remove(row);
				_ = await db.SaveChangesAsync();
				return Results.Ok();
			}

			return Results.NotFound();
		}

		public static async Task<IResult> List<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> ToDtoFunc, LocoDb db) where TRow : class
			=> Results.Ok(await table
				.Select(x => ToDtoFunc(x))
				.ToListAsync());
	}
}
