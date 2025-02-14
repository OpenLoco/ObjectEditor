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

		Task<IResult> CreateAsync(TDto request, LocoDb db);

		Task<IResult> ReadAsync(int id, LocoDb db);

		Task<IResult> UpdateAsync(TDto request, LocoDb db);

		Task<IResult> DeleteAsync(int id, LocoDb db);

		Task<IResult> ListAsync(LocoDb db);
	}

	public abstract class BaseTableRequestHandler<TDto> : ITableRequestHandler<TDto>
		where TDto : class, IHasId
	{
		public abstract string BaseRoute { get; }

		public void MapRoutes(IEndpointRouteBuilder parentRoute)
		{
			var baseRoute = parentRoute.MapGroup(BaseRoute);

			_ = baseRoute.MapGet(string.Empty, ListAsync);
			_ = baseRoute.MapPost(string.Empty, CreateAsync);

			var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
			_ = resourceRoute.MapGet(string.Empty, ReadAsync);
			_ = resourceRoute.MapPut(string.Empty, UpdateAsync);
			_ = resourceRoute.MapDelete(string.Empty, DeleteAsync);

			MapAdditionalRoutes(baseRoute);
		}

		public virtual void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute) { }
		public abstract Task<IResult> CreateAsync(TDto request, LocoDb db);
		public abstract Task<IResult> ReadAsync(int id, LocoDb db);
		public abstract Task<IResult> UpdateAsync(TDto request, LocoDb db);
		public abstract Task<IResult> DeleteAsync(int id, LocoDb db);
		public abstract Task<IResult> ListAsync(LocoDb db);
	}

	public abstract class BaseReferenceDataTableRequestHandler<TDto, TRow> : BaseTableRequestHandler<TDto>
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		protected abstract DbSet<TRow> GetTable(LocoDb db);

		protected abstract void UpdateFunc(TDto request, TRow row);

		protected abstract TRow ToRowFunc(TDto request);
		protected abstract TDto ToDtoFunc(TRow request);

		protected abstract bool TryValidateCreate(TDto request, LocoDb db, out IResult result);

		public override async Task<IResult> CreateAsync(TDto request, LocoDb db)
		{
			if (!TryValidateCreate(request, db, out var result))
			{
				return result;
			}
			var row = ToRowFunc(request);
			_ = await GetTable(db).AddAsync(row);
			_ = await db.SaveChangesAsync();
			return Results.Created($"{BaseRoute}/{row.Id}", ToDtoFunc(row));
		}

		public override async Task<IResult> ReadAsync(int id, LocoDb db)
			=> await GetTable(db).FindAsync(id) is TRow row
				? Results.Ok(ToDtoFunc(row))
				: Results.NotFound();

		public override async Task<IResult> UpdateAsync(TDto request, LocoDb db)
		{
			var table = GetTable(db);
			if (await table.FindAsync(request.Id) is not TRow row)
			{
				return Results.NotFound();
			}

			UpdateFunc(request, row);

			_ = await db.SaveChangesAsync();
			return Results.NoContent();
		}

		public override async Task<IResult> DeleteAsync(int id, LocoDb db)
		{
			var table = GetTable(db);
			if (await table.FindAsync(id) is TRow row)
			{
				_ = table.Remove(row);
				_ = await db.SaveChangesAsync();
				return Results.Ok();
			}

			return Results.NotFound();
		}

		public override async Task<IResult> ListAsync(LocoDb db)
			=> Results.Ok(await GetTable(db)
				.Select(x => ToDtoFunc(x))
				.ToListAsync());
	}
}
