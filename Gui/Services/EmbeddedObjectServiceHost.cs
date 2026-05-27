using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectService.Hosting;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Services;

// Owns the lifetime of an in-process ObjectService web application. This is the GUI's
// "local backend": rather than talking to a remote server, the editor can spin up its own
// instance pointing at the user's local objects folder + database, and the existing
// ObjectServiceClient can target it via the resolved BaseAddress.
public sealed class EmbeddedObjectServiceHost : ReactiveObject, IAsyncDisposable
{
	readonly ILogger logger;
	readonly SemaphoreSlim lifecycleLock = new(1, 1);
	WebApplication? app;

	[Reactive]
	public EmbeddedHostState State { get; private set; } = EmbeddedHostState.Disabled;

	[Reactive]
	public string? StatusMessage { get; private set; }

	[Reactive]
	public Uri? BaseAddress { get; private set; }

	public bool IsRunning => State == EmbeddedHostState.Running;

	public EmbeddedObjectServiceHost(ILogger logger)
	{
		this.logger = logger;
	}

	public async Task StartAsync(ObjectServiceHostOptions options, CancellationToken cancellationToken = default)
	{
		await lifecycleLock.WaitAsync(cancellationToken);
		try
		{
			if (app is not null)
			{
				throw new InvalidOperationException("Embedded ObjectService host is already running.");
			}

			State = EmbeddedHostState.Starting;
			StatusMessage = "Bootstrapping folders and database...";

			try
			{
				await ObjectServiceHost.BootstrapAsync(options, cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Embedded ObjectService bootstrap failed.");
				State = EmbeddedHostState.Failed;
				StatusMessage = $"Bootstrap failed: {ex.Message}";
				throw;
			}

			var builder = WebApplication.CreateBuilder();

			// Suppress noisy console output from the embedded host - the GUI owns presentation.
			builder.Logging.ClearProviders();

			ObjectServiceHost.ApplyOptionsToConfiguration(builder, options);
			ObjectServiceHost.ConfigureBuilder(builder);

			app = builder.Build();
			ObjectServiceHost.Configure(app);

			StatusMessage = "Starting Kestrel...";

			try
			{
				await app.StartAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Embedded ObjectService failed to start Kestrel.");
				State = EmbeddedHostState.Failed;
				StatusMessage = $"Kestrel start failed: {ex.Message}";
				await app.DisposeAsync();
				app = null;
				throw;
			}

			// Resolve the actual bound address (handles the ":0" ephemeral-port case).
			var addresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses;
			var resolved = addresses?.FirstOrDefault();
			if (!string.IsNullOrWhiteSpace(resolved) && Uri.TryCreate(resolved, UriKind.Absolute, out var uri))
			{
				BaseAddress = uri;
				logger.LogInformation("Embedded ObjectService listening on {BaseAddress}", uri);

				StatusMessage = "Waiting for health check...";
				await WaitForReadyAsync(uri, cancellationToken);

				State = EmbeddedHostState.Running;
				StatusMessage = $"Running at {uri}";
			}
			else
			{
				logger.LogWarning("Embedded ObjectService started but no bound address could be resolved.");
				State = EmbeddedHostState.Failed;
				StatusMessage = "Started but could not resolve bound address.";
			}
		}
		finally
		{
			_ = lifecycleLock.Release();
		}
	}

	async Task WaitForReadyAsync(Uri baseAddress, CancellationToken cancellationToken)
	{
		using var probe = new HttpClient { BaseAddress = baseAddress, Timeout = TimeSpan.FromSeconds(2) };
		var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
		while (DateTime.UtcNow < deadline)
		{
			try
			{
				using var response = await probe.GetAsync("/health", cancellationToken);
				if (response.IsSuccessStatusCode)
				{
					return;
				}
			}
			catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
			{
				// keep polling
			}

			await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
		}

		logger.LogWarning("Embedded ObjectService did not report healthy within the readiness window; continuing anyway.");
	}

	public async Task StopAsync(CancellationToken cancellationToken = default)
	{
		await lifecycleLock.WaitAsync(cancellationToken);
		try
		{
			if (app is null)
			{
				return;
			}

			await app.StopAsync(cancellationToken);
			await app.DisposeAsync();
			app = null;
			BaseAddress = null;
			State = EmbeddedHostState.Stopped;
			StatusMessage = "Stopped.";
		}
		finally
		{
			_ = lifecycleLock.Release();
		}
	}

	public async ValueTask DisposeAsync()
	{
		await StopAsync();
		lifecycleLock.Dispose();
	}

	// Helper for callers that don't already have a JWT signing key set up. Generates a
	// cryptographically random key sized for HS256 (>= 32 bytes). The key only ever lives
	// in memory for the lifetime of this process.
	public static string GenerateEphemeralJwtKey()
	{
		Span<byte> bytes = stackalloc byte[48];
		RandomNumberGenerator.Fill(bytes);
		return Convert.ToBase64String(bytes);
	}
}
