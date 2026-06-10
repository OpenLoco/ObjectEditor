using System.Diagnostics;

namespace ObjectService.Hosting;

// Watches a parent process. If it disappears (the GUI crashes, is task-killed, or
// otherwise exits without cleanly stopping us) we trigger application shutdown so the
// embedded ObjectService cannot outlive its owner.
//
// When the configured parent PID matches the current process (the in-process embedded
// host case) the watchdog effectively becomes a no-op: we still poll, but the parent
// is always alive because it is us.
sealed class ParentProcessWatchdog : BackgroundService
{
	readonly int parentProcessId;
	readonly IHostApplicationLifetime lifetime;
	readonly ILogger<ParentProcessWatchdog> logger;

	public ParentProcessWatchdog(int parentProcessId, IHostApplicationLifetime lifetime, ILogger<ParentProcessWatchdog> logger)
	{
		this.parentProcessId = parentProcessId;
		this.lifetime = lifetime;
		this.logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		// PID == current means in-process embedded host; no parent to monitor.
		if (parentProcessId == Environment.ProcessId)
		{
			return;
		}

		Process? parent;
		try
		{
			parent = Process.GetProcessById(parentProcessId);
		}
		catch (ArgumentException)
		{
			logger.LogWarning("Parent process {Pid} not found at startup; shutting down.", parentProcessId);
			lifetime.StopApplication();
			return;
		}

		try
		{
			parent.EnableRaisingEvents = true;
			logger.LogInformation("Watching parent process {Pid} ({Name}). Will shut down when it exits.", parentProcessId, parent.ProcessName);

			// Race: explicit shutdown vs parent exit. Whichever wins, we exit.
			using var parentExited = new CancellationTokenSource();
			parent.Exited += (_, _) => parentExited.Cancel();

			// Edge: parent already exited between GetProcessById and EnableRaisingEvents.
			if (parent.HasExited)
			{
				parentExited.Cancel();
			}

			using var linked = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, parentExited.Token);
			try
			{
				await Task.Delay(Timeout.Infinite, linked.Token);
			}
			catch (OperationCanceledException)
			{
				// fall through
			}

			if (parentExited.IsCancellationRequested)
			{
				logger.LogWarning("Parent process {Pid} exited; stopping embedded ObjectService.", parentProcessId);
				lifetime.StopApplication();
			}
		}
		finally
		{
			parent.Dispose();
		}
	}
}
