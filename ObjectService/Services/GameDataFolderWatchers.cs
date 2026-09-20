namespace ObjectService.Services;

/// <summary>Watches <c>GameData/Objects</c> using <see cref="ObjectsFolderService"/>.</summary>
public sealed class ObjectsFolderWatcher(IServiceScopeFactory scopeFactory, ServerFolderManager sfm, ILoggerFactory loggerFactory, GameDataWatcherLock operationLock)
	: GameDataFolderWatcher("Objects", sfm.ObjectsFolder, scopeFactory, loggerFactory, operationLock)
{
	protected override IGameDataFileService ResolveService(IServiceProvider services) => services.GetRequiredService<ObjectsFolderService>();
}

/// <summary>Watches <c>GameData/Scenarios</c> using <see cref="ScenariosFolderService"/>.</summary>
public sealed class ScenariosFolderWatcher(IServiceScopeFactory scopeFactory, ServerFolderManager sfm, ILoggerFactory loggerFactory, GameDataWatcherLock operationLock)
	: GameDataFolderWatcher("Scenarios", sfm.ScenariosFolder, scopeFactory, loggerFactory, operationLock)
{
	protected override IGameDataFileService ResolveService(IServiceProvider services) => services.GetRequiredService<ScenariosFolderService>();
}

/// <summary>Watches <c>GameData/Landscapes</c> using <see cref="LandscapesFolderService"/>.</summary>
public sealed class LandscapesFolderWatcher(IServiceScopeFactory scopeFactory, ServerFolderManager sfm, ILoggerFactory loggerFactory, GameDataWatcherLock operationLock)
	: GameDataFolderWatcher("Landscapes", sfm.LandscapesFolder, scopeFactory, loggerFactory, operationLock)
{
	protected override IGameDataFileService ResolveService(IServiceProvider services) => services.GetRequiredService<LandscapesFolderService>();
}

/// <summary>Watches <c>GameData/Tutorials</c> using <see cref="TutorialsFolderService"/>.</summary>
public sealed class TutorialsFolderWatcher(IServiceScopeFactory scopeFactory, ServerFolderManager sfm, ILoggerFactory loggerFactory, GameDataWatcherLock operationLock)
	: GameDataFolderWatcher("Tutorials", sfm.TutorialsFolder, scopeFactory, loggerFactory, operationLock)
{
	protected override IGameDataFileService ResolveService(IServiceProvider services) => services.GetRequiredService<TutorialsFolderService>();
}

/// <summary>Watches <c>GameData/SoundEffects</c> using <see cref="SoundEffectsFolderService"/>.</summary>
public sealed class SoundEffectsFolderWatcher(IServiceScopeFactory scopeFactory, ServerFolderManager sfm, ILoggerFactory loggerFactory, GameDataWatcherLock operationLock)
	: GameDataFolderWatcher("SoundEffects", sfm.SoundEffectsFolder, scopeFactory, loggerFactory, operationLock)
{
	protected override IGameDataFileService ResolveService(IServiceProvider services) => services.GetRequiredService<SoundEffectsFolderService>();
}

/// <summary>Watches <c>GameData/Music</c> using <see cref="MusicFolderService"/>.</summary>
public sealed class MusicFolderWatcher(IServiceScopeFactory scopeFactory, ServerFolderManager sfm, ILoggerFactory loggerFactory, GameDataWatcherLock operationLock)
	: GameDataFolderWatcher("Music", sfm.MusicFolder, scopeFactory, loggerFactory, operationLock)
{
	protected override IGameDataFileService ResolveService(IServiceProvider services) => services.GetRequiredService<MusicFolderService>();
}

/// <summary>Watches <c>GameData/Graphics</c> using <see cref="GraphicsFolderService"/>.</summary>
public sealed class GraphicsFolderWatcher(IServiceScopeFactory scopeFactory, ServerFolderManager sfm, ILoggerFactory loggerFactory, GameDataWatcherLock operationLock)
	: GameDataFolderWatcher("Graphics", sfm.GraphicsFolder, scopeFactory, loggerFactory, operationLock)
{
	protected override IGameDataFileService ResolveService(IServiceProvider services) => services.GetRequiredService<GraphicsFolderService>();
}
