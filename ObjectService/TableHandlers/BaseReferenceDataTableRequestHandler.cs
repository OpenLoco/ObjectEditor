using Definitions;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;

namespace ObjectService.TableHandlers
{
	public interface ITableRequestHandler<TDto> where TDto : class, IHasId
	{
		string BaseRoute { get; }

		void MapRoutes(IEndpointRouteBuilder parentRoute);
		void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute);

		Task<IResult> CreateAsync(TDto request, LocoDbContext db);

		Task<IResult> ReadAsync(int id, LocoDbContext db);

		Task<IResult> UpdateAsync(TDto request, LocoDbContext db);

		Task<IResult> DeleteAsync(int id, LocoDbContext db);

		Task<IResult> ListAsync(HttpContext context, LocoDbContext db);
	}

	public abstract class BaseTableRequestHandler<TDto> : ITableRequestHandler<TDto>
		where TDto : class, IHasId
	{
		public abstract string BaseRoute { get; }

		public void MapRoutes(IEndpointRouteBuilder parentRoute)
		{
			var baseRoute = parentRoute
				.MapGroup(BaseRoute)
				.WithTags(MakeNicePlural(GetType().Name));

			_ = baseRoute.MapGet(string.Empty, ListAsync);

			var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
			_ = resourceRoute.MapGet(string.Empty, ReadAsync);

			// todo: do not enable until user permissions are implemented. for now, enable for testing
#if DEBUG
			_ = baseRoute.MapPost(string.Empty, CreateAsync);
			_ = resourceRoute.MapPut(string.Empty, UpdateAsync);
			_ = resourceRoute.MapDelete(string.Empty, DeleteAsync);
#endif

			MapAdditionalRoutes(baseRoute);

			static string MakeNicePlural(string name)
				=> $"{name.Replace("RequestHandler", string.Empty)}s";
		}

		public virtual void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute) { }
		public abstract Task<IResult> CreateAsync(TDto request, LocoDbContext db);
		public abstract Task<IResult> ReadAsync(int id, LocoDbContext db);
		public abstract Task<IResult> UpdateAsync(TDto request, LocoDbContext db);
		public abstract Task<IResult> DeleteAsync(int id, LocoDbContext db);
		public abstract Task<IResult> ListAsync(HttpContext context, LocoDbContext db);
	}

	public abstract class BaseReferenceDataTableRequestHandler<TDto, TRow> : BaseTableRequestHandler<TDto>
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		protected abstract DbSet<TRow> GetTable(LocoDbContext db);

		protected abstract void UpdateFunc(TDto request, TRow row);

		protected abstract TRow ToRowFunc(TDto request);
		protected abstract TDto ToDtoFunc(TRow request);

		protected abstract bool TryValidateCreate(TDto request, LocoDbContext db, out IResult? result);

		public override async Task<IResult> CreateAsync(TDto request, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.CreateAsync(
				GetTable(db),
				ToDtoFunc,
				ToRowFunc,
				request,
				() => (TryValidateCreate(request, db, out var result), result),
				BaseRoute,
				db);

		public override async Task<IResult> ReadAsync(int id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ReadAsync(GetTable(db), ToDtoFunc, id, db);

		public override async Task<IResult> UpdateAsync(TDto request, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.UpdateAsync(GetTable(db), ToDtoFunc, UpdateFunc, request, db);

		public override async Task<IResult> DeleteAsync(int id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.DeleteAsync(GetTable(db), ToDtoFunc, id, db);

		public override async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ListAsync(context, GetTable(db), ToDtoFunc);
	}

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

		public static async Task<IResult> UpdateAsync<TDto, TRow>(DbSet<TRow> table, Func<TRow, TDto> dtoConverter, Action<TDto, TRow> updateFunc, TDto request, LocoDbContext db)
			where TDto : class, IHasId
			where TRow : class, IHasId
		{
			if (await table.FindAsync(request.Id) is not TRow row)
			{
				return Results.NotFound();
			}

			updateFunc(request, row);

			_ = await db.SaveChangesAsync();
			return Results.NoContent();
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
