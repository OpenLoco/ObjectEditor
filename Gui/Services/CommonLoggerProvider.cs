using Microsoft.Extensions.Logging;
using System;

namespace Gui.Services;

// Bridges ASP.NET Core / Kestrel logging from the embedded ObjectService into the GUI's
// Common.Logging.Logger so its output shows up in the local-server log window. Without
// this provider the embedded host has no logging sinks at all (the standard console
// providers are cleared on purpose) so request/response activity disappears silently.
sealed class CommonLoggerProvider : ILoggerProvider
{
	readonly Common.Logging.Logger target;

	public CommonLoggerProvider(Common.Logging.Logger target)
	{
		this.target = target;
	}

	public ILogger CreateLogger(string categoryName) => new CategoryLogger(target, categoryName);

	public void Dispose()
	{
	}

	sealed class CategoryLogger : ILogger
	{
		readonly Common.Logging.Logger target;
		readonly string category;

		public CategoryLogger(Common.Logging.Logger target, string category)
		{
			this.target = target;
			this.category = category;
		}

		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

		public bool IsEnabled(LogLevel logLevel) => target.IsEnabled(logLevel);

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			// Forward via the target Logger's standard Log path so it ends up in the queue
			// + LogAdded event + observable collection bound to the LogWindow. The category
			// name (e.g. "Microsoft.AspNetCore.Hosting.Diagnostics") is surfaced via EventId
			// so the existing "[name]" formatting picks it up.
			var forwardedEventId = string.IsNullOrEmpty(eventId.Name)
				? new EventId(eventId.Id, category)
				: new EventId(eventId.Id, $"{category}/{eventId.Name}");

			target.Log(logLevel, forwardedEventId, state, exception, formatter);
		}
	}
}
