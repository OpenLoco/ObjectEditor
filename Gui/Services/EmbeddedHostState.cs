namespace Gui.Services;

public enum EmbeddedHostState
{
	// Initial state before StartAsync has been called, or after construction with no
	// runnable configuration. The host is not yet trying to start.
	Disabled,

	// Bootstrap / Kestrel startup is in progress.
	Starting,

	// Kestrel is bound and the health probe succeeded.
	Running,

	// Startup or runtime error - see StatusMessage for details.
	Failed,

	// Host was running and has been stopped (cleanly).
	Stopped,
}
