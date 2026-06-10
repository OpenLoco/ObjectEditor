using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Services;

// Background poller that periodically pings the configured remote master ObjectService
// to surface its reachability in the GUI. Independent of the embedded local host; both
// indicators are expected to be visible simultaneously.
public sealed class RemoteServerMonitor : ReactiveObject, IAsyncDisposable
{
	readonly ILogger logger;
	readonly HttpClient probe;
	readonly TimeSpan pollInterval;
	CancellationTokenSource? loopCts;
	Task? loopTask;

	[Reactive]
	public RemoteServerState State { get; private set; } = RemoteServerState.Unknown;

	[Reactive]
	public string? StatusMessage { get; private set; }

	[Reactive]
	public Uri? Address { get; private set; }

	public RemoteServerMonitor(ILogger logger, TimeSpan? pollInterval = null)
	{
		this.logger = logger;
		this.pollInterval = pollInterval ?? TimeSpan.FromSeconds(60);
		probe = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
	}

	// Updates the target address and (re)starts the probe loop. Safe to call from any thread.
	public void Configure(Uri? address)
	{
		Address = address;
		StatusMessage = address is null ? "No remote address configured." : null;

		if (address is null)
		{
			State = RemoteServerState.Unreachable;
			return;
		}

		Stop();
		loopCts = new CancellationTokenSource();
		loopTask = Task.Run(() => PollLoopAsync(loopCts.Token));
	}

	async Task PollLoopAsync(CancellationToken cancellationToken)
	{
		// Probe once immediately so the UI doesn't sit on "Unknown" for the full interval.
		await ProbeOnceAsync(cancellationToken);

		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				await Task.Delay(pollInterval, cancellationToken);
			}
			catch (TaskCanceledException)
			{
				return;
			}

			await ProbeOnceAsync(cancellationToken);
		}
	}

	async Task ProbeOnceAsync(CancellationToken cancellationToken)
	{
		var address = Address;
		if (address is null)
		{
			return;
		}

		State = RemoteServerState.Checking;
		StatusMessage = $"Checking {address}...";

		try
		{
			var healthUri = new Uri(address, "/health");
			using var response = await probe.GetAsync(healthUri, cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				State = RemoteServerState.Reachable;
				StatusMessage = $"Reachable: {address}";
			}
			else
			{
				State = RemoteServerState.Unreachable;
				StatusMessage = $"HTTP {(int)response.StatusCode} from {address}";
			}
		}
		catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			// shutdown - swallow
		}
		catch (Exception ex)
		{
			State = RemoteServerState.Unreachable;
			StatusMessage = $"Unreachable: {ex.Message}";
			logger.LogDebug(ex, "Remote master server probe failed for {Address}", address);
		}
	}

	void Stop()
	{
		var cts = loopCts;
		loopCts = null;
		cts?.Cancel();
		cts?.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		Stop();
		if (loopTask is not null)
		{
			try
			{
				await loopTask;
			}
			catch
			{
				// best-effort shutdown
			}
		}

		probe.Dispose();
	}
}
