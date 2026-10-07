namespace ObjectService.Services;

/// <summary>
/// <para>
/// The single file-watching service for the server. It owns one watcher per GameData category
/// folder (<c>Objects</c>, <c>Scenarios</c>, <c>Landscapes</c>, <c>Tutorials</c>,
/// <c>SoundEffects</c>, <c>Music</c>, <c>Graphics</c>). The one-off startup reconciliation of every
/// folder lives in <see cref="GameDataSyncService"/> so that it also runs when this service is not
/// registered.
/// </para>
/// <para>
/// Each folder is watched by its own <see cref="FileSystemWatcher"/> (owned by a
/// <see cref="GameDataFolderWatcher"/> subclass) so a busy folder can never overflow a shared OS
/// buffer and starve the others, and so events arrive already scoped to the correct folder. Each
/// watcher delegates to that folder's own entity-specific service, because a game object is a
/// different entity from a music file, scenario, sound effect, tutorial or graphics file.
/// </para>
/// </summary>
public sealed class GameDataWatcherService : BackgroundService
{
	private readonly IReadOnlyList<GameDataFolderWatcher> _watchers;
	private readonly ServerFolderManager _sfm;
	private readonly ILogger<GameDataWatcherService> _logger;

	public GameDataWatcherService(
		IEnumerable<GameDataFolderWatcher> watchers,
		ServerFolderManager sfm,
		ILogger<GameDataWatcherService> logger)
	{
		_watchers = [.. watchers];
		_sfm = sfm;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_watchers.Count == 0)
		{
			_logger.LogWarning("No GameData folder watchers are registered");
			return;
		}

		foreach (var watcher in _watchers)
		{
			watcher.Start();
		}

		_logger.LogInformation(
			"GameData watcher service started for \"{Root}\" ({Count} folders: {Folders})",
			_sfm.GameDataFolder, _watchers.Count, string.Join(", ", _watchers.Select(w => w.Category)));

		// The one-off startup synchronisation is owned by GameDataSyncService so that it also runs when
		// the file watcher is disabled; from here on the watchers only handle live changes.

		try
		{
			await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
		}
		catch (OperationCanceledException)
		{
			// Normal shutdown.
		}

		foreach (var watcher in _watchers)
		{
			await watcher.StopAsync().ConfigureAwait(false);
		}
	}
}
