namespace Gui.Services;

public enum RemoteServerState
{
	// Initial state before the first probe completes.
	Unknown,

	// A probe is currently in flight.
	Checking,

	// The most recent probe succeeded.
	Reachable,

	// The most recent probe failed (timeout, non-success status, DNS error, etc).
	Unreachable,
}
