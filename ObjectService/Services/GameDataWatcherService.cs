namespace ObjectService.Services;

/// <summary>
/// <para>
/// The single file-watching service for the server. It owns one watcher per GameData category
/// folder (<c>Objects</c>, <c>Scenarios</c>, <c>Landscapes</c>, <c>Tutorials</c>,
/// <c>SoundEffects</c>, <c>Music</c>, <c>Graphics</c>) and runs a one-off reconciliation of every
/// folder at startup so files added or removed while the server was offline are picked up.
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
	private readonly GameDataWatcherLock _operationLock;
	private readonly ServerFolderManager _sfm;
	private readonly ILogger<GameDataWatcherService> _logger;

	public GameDataWatcherService(
		IEnumerable<GameDataFolderWatcher> watchers,
		GameDataWatcherLock operationLock,
		ServerFolderManager sfm,
		ILogger<GameDataWatcherService> logger)
	{
		_watchers = [.. watchers];
		_operationLock = operationLock;
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

		await ReconcileAsync(stoppingToken).ConfigureAwait(false);

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

	/// <summary>
	/// Reassesses every watched folder in turn. Live events raised while this is running are queued
	/// by each folder watcher and processed once the shared lock is released.
	/// </summary>
	private async Task ReconcileAsync(CancellationToken ct)
	{
		_logger.LogInformation("Reconciling the GameData folder structure with the index and database...");

		try
		{
			await _operationLock.Semaphore.WaitAsync(ct).ConfigureAwait(false);
			try
			{
				foreach (var watcher in _watchers)
				{
					try
					{
						await watcher.ReconcileAsync(ct).ConfigureAwait(false);
					}
					catch (OperationCanceledException) when (ct.IsCancellationRequested)
					{
						// Normal shutdown - stop reconciling the remaining folders.
						throw;
					}
					catch (Exception ex)
					{
						// One folder failing must not stop the others from being reconciled.
						_logger.LogError(ex, "Reconciliation of the {Category} folder failed; continuing with the remaining folders", watcher.Category);
					}
				}
			}
			finally
			{
				_ = _operationLock.Semaphore.Release();
			}

			_logger.LogInformation("Reconciliation complete");
		}
		catch (OperationCanceledException) when (ct.IsCancellationRequested)
		{
			// Normal shutdown.
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Startup reconciliation failed; the watchers will still process live changes");
		}
	}
}
