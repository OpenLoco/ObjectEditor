namespace ObjectService.Services;

/// <summary>
/// <para>
/// Runs the one-off GameData synchronisation at startup, independently of the file watchers, so the
/// files on disk and the database are always brought back in sync when the server starts - even when
/// <c>ObjectService:EnableFileWatcher</c> is disabled.
/// </para>
/// <para>
/// Each folder's service reconciles its own entity type, using the same index/disk comparison the
/// editor performs when it reloads its index (see
/// <see cref="Index.ObjectIndex.LoadOrCreateAndSyncAsync"/>), so both behave identically.
/// </para>
/// </summary>
public sealed class GameDataSyncService : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly GameDataWatcherLock _operationLock;
	private readonly ServerFolderManager _sfm;
	private readonly ILogger<GameDataSyncService> _logger;

	public GameDataSyncService(
		IServiceScopeFactory scopeFactory,
		GameDataWatcherLock operationLock,
		ServerFolderManager sfm,
		ILogger<GameDataSyncService> logger)
	{
		_scopeFactory = scopeFactory;
		_operationLock = operationLock;
		_sfm = sfm;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Synchronising the GameData folders with the index and database...");

		try
		{
			await _operationLock.Semaphore.WaitAsync(stoppingToken).ConfigureAwait(false);
			try
			{
				using var scope = _scopeFactory.CreateScope();
				foreach (var service in scope.ServiceProvider.GetServices<IGameDataFileService>())
				{
					try
					{
						await service.ReconcileAsync(stoppingToken).ConfigureAwait(false);
					}
					catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
					{
						// Normal shutdown - stop syncing the remaining folders.
						throw;
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Startup sync failed for {Service}; continuing with the other folders", service.GetType().Name);
					}
				}
			}
			finally
			{
				_ = _operationLock.Semaphore.Release();
			}

			_logger.LogInformation("GameData startup sync complete for \"{Root}\"", _sfm.GameDataFolder);
		}
		catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
		{
			// Normal shutdown.
		}
	}
}