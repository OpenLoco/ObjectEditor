using Definitions.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ObjectService.Services;

public sealed class ObjectServiceHealthCheck : IHealthCheck
{
	readonly LocoDbContext _dbContext;
	readonly ServerFolderManager _serverFolderManager;

	public ObjectServiceHealthCheck(LocoDbContext dbContext, ServerFolderManager serverFolderManager)
	{
		_dbContext = dbContext;
		_serverFolderManager = serverFolderManager;
	}

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		var canConnectToDatabase = await _dbContext.Database.CanConnectAsync(cancellationToken);
		var objectsFolderExists = Directory.Exists(_serverFolderManager.ObjectsFolder);
		var indexFileExists = File.Exists(_serverFolderManager.IndexFile);

		var data = new Dictionary<string, object>
		{
			["databaseConnected"] = canConnectToDatabase,
			["objectsFolder"] = _serverFolderManager.ObjectsFolder,
			["objectsFolderExists"] = objectsFolderExists,
			["indexFile"] = _serverFolderManager.IndexFile,
			["indexFileExists"] = indexFileExists,
		};

		if (!canConnectToDatabase)
		{
			return HealthCheckResult.Unhealthy("The SQLite database is not reachable.", data: data);
		}

		if (!objectsFolderExists)
		{
			return HealthCheckResult.Unhealthy("The configured Objects folder is missing.", data: data);
		}

		if (!indexFileExists)
		{
			return HealthCheckResult.Unhealthy("The object index file is missing.", data: data);
		}

		return HealthCheckResult.Healthy("ObjectService is tracking perfectly and has a full head of steam.", data);
	}
}
