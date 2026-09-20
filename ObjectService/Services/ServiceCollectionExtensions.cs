using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Identity;
using Definitions.DTO.Mappers;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers.TableHandlers;

namespace ObjectService.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddObjectEditorServices(this IServiceCollection services)
	{
		// CRUD services for Category A handlers
		_ = services.AddScoped(provider =>
		{
			var db = provider.GetRequiredService<LocoDbContext>();
			return new CrudService<DtoAuthorEntry, TblAuthor>(
				db, d => d.Authors, r => r.ToDtoEntry(), d => d.ToTable(),
				(d, r) => r.Name = d.Name,
				d => string.IsNullOrWhiteSpace(d.Name) ? "Name required" : null);
		});
		_ = services.AddScoped<ICrudService<DtoAuthorEntry, TblAuthor>>(p => p.GetRequiredService<CrudService<DtoAuthorEntry, TblAuthor>>());

		_ = services.AddScoped(provider =>
		{
			var db = provider.GetRequiredService<LocoDbContext>();
			return new CrudService<DtoTagEntry, TblTag>(
				db, d => d.Tags, r => r.ToDtoEntry(), d => d.ToTable(),
				(d, r) => r.Name = d.Name,
				d => string.IsNullOrWhiteSpace(d.Name) ? "Name required" : null);
		});
		_ = services.AddScoped<ICrudService<DtoTagEntry, TblTag>>(p => p.GetRequiredService<CrudService<DtoTagEntry, TblTag>>());

		_ = services.AddScoped(provider =>
		{
			var db = provider.GetRequiredService<LocoDbContext>();
			return new CrudService<DtoLicenceEntry, TblLicence>(
				db, d => d.Licences, r => r.ToDtoEntry(), d => d.ToTable(),
				(d, r) =>
				{
					r.Name = d.Name;
					r.Text = d.Text;
				},
				d => string.IsNullOrWhiteSpace(d.Name) ? "Name required" : null);
		});
		_ = services.AddScoped<ICrudService<DtoLicenceEntry, TblLicence>>(p => p.GetRequiredService<CrudService<DtoLicenceEntry, TblLicence>>());

		_ = services.AddScoped(provider =>
		{
			var db = provider.GetRequiredService<LocoDbContext>();
			return new CrudService<DtoUserEntry, TblUser>(
				db, d => d.Users, r => r.ToDtoEntry(), d => d.ToTable(),
				(d, r) => r.UserName = d.UserName,
				d => string.IsNullOrWhiteSpace(d.UserName) ? "UserName required" : null);
		});
		_ = services.AddScoped<ICrudService<DtoUserEntry, TblUser>>(p => p.GetRequiredService<CrudService<DtoUserEntry, TblUser>>());

		_ = services.AddScoped(provider =>
		{
			var db = provider.GetRequiredService<LocoDbContext>();
			return new CrudService<DtoRoleEntry, TblUserRole>(
				db, d => d.Roles, r => r.ToDtoEntry(), d => d.ToTable(),
				(d, r) => r.Name = d.Name,
				d => string.IsNullOrWhiteSpace(d.Name) ? "Name required" : null);
		});
		_ = services.AddScoped<ICrudService<DtoRoleEntry, TblUserRole>>(p => p.GetRequiredService<CrudService<DtoRoleEntry, TblUserRole>>());

		_ = services.AddScoped(provider =>
		{
			var db = provider.GetRequiredService<LocoDbContext>();
			return new CrudService<DtoObjectMissingEntry, TblObjectMissing>(
				db, d => d.ObjectsMissing, r => r.ToDtoEntry(), d => d.ToTable(),
				(d, r) =>
				{
					r.DatName = d.DatName;
					r.DatChecksum = d.DatChecksum;
					r.ObjectType = d.ObjectType;
				},
				d => string.IsNullOrWhiteSpace(d.DatName) ? "DatName required" : d.DatChecksum == 0 ? "DatChecksum cannot be 0" : !Enum.IsDefined(d.ObjectType) ? $"Invalid ObjectType: {d.ObjectType}" : null);
		});
		_ = services.AddScoped<ICrudService<DtoObjectMissingEntry, TblObjectMissing>>(p => p.GetRequiredService<CrudService<DtoObjectMissingEntry, TblObjectMissing>>());

		// Domain services for Category B handlers
		_ = services.AddScoped<IObjectQueryService, ObjectQueryService>();
		_ = services.AddScoped<IScenarioService, ScenarioService>();
		_ = services.AddScoped<IObjectPackService, ObjectPackService>();
		_ = services.AddScoped<IScenarioPackService, ScenarioPackService>();
		_ = services.AddScoped<IMusicService, MusicService>();
		_ = services.AddScoped<ISoundEffectsService, SoundEffectsService>();
		_ = services.AddScoped<ITutorialsService, TutorialsService>();
		_ = services.AddScoped<IGraphicsService, GraphicsService>();
		_ = services.AddScoped<IReferenceDataService, ReferenceDataService>();
		_ = services.AddScoped<IUserService, UserService>();

		// Route handlers
		_ = services.AddScoped<AuthorRouteHandler>();
		_ = services.AddScoped<TagRouteHandler>();
		_ = services.AddScoped<LicenceRouteHandler>();
		_ = services.AddScoped<UserRouteHandler>();
		_ = services.AddScoped<RoleRouteHandler>();
		_ = services.AddScoped<ObjectMissingRouteHandler>();
		_ = services.AddScoped<ObjectRouteHandler>();
		_ = services.AddScoped<ScenarioRouteHandler>();
		_ = services.AddScoped<ObjectPackRouteHandler>();
		_ = services.AddScoped<ScenarioPackRouteHandler>();
		_ = services.AddScoped<MusicRouteHandler>();
		_ = services.AddScoped<SoundEffectsRouteHandler>();
		_ = services.AddScoped<TutorialsRouteHandler>();
		_ = services.AddScoped<GraphicsRouteHandler>();

		return services;
	}

	/// <summary>
	/// Registers the single GameData file-watching service plus the seven per-folder watchers it
	/// owns (see <see cref="GameDataWatcherService"/>).
	/// </summary>
	public static IServiceCollection AddGameDataFileWatchers(this IServiceCollection services)
	{
		_ = services.AddSingleton<GameDataWatcherLock>();

		// One entity-specific import service per GameData folder - a game object, scenario,
		// landscape, tutorial, sound effect, music file and graphics file are all distinct entities.
		_ = services.AddScoped<ObjectsFolderService>();
		_ = services.AddScoped<ScenariosFolderService>();
		_ = services.AddScoped<LandscapesFolderService>();
		_ = services.AddScoped<TutorialsFolderService>();
		_ = services.AddScoped<SoundEffectsFolderService>();
		_ = services.AddScoped<MusicFolderService>();
		_ = services.AddScoped<GraphicsFolderService>();

		// One watcher per folder, owned by the single hosted service.
		_ = services.AddSingleton<GameDataFolderWatcher, ObjectsFolderWatcher>();
		_ = services.AddSingleton<GameDataFolderWatcher, ScenariosFolderWatcher>();
		_ = services.AddSingleton<GameDataFolderWatcher, LandscapesFolderWatcher>();
		_ = services.AddSingleton<GameDataFolderWatcher, TutorialsFolderWatcher>();
		_ = services.AddSingleton<GameDataFolderWatcher, SoundEffectsFolderWatcher>();
		_ = services.AddSingleton<GameDataFolderWatcher, MusicFolderWatcher>();
		_ = services.AddSingleton<GameDataFolderWatcher, GraphicsFolderWatcher>();

		_ = services.AddHostedService<GameDataWatcherService>();

		return services;
	}
}
