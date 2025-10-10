using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Common;

public static class PlatformSpecific
{
	// For Unix-like systems (Linux, macOS)
	static class UnixUserChecker
	{
		// Imports the geteuid() function from libc (the standard C library)
		// geteuid() returns the effective user ID of the calling process.
		// A value of 0 typically indicates the root user.
		[DllImport("libc", EntryPoint = "geteuid")]
		internal static extern uint geteuid();

		public static bool IsRoot()
			=> geteuid() == 0;
	}

	public static bool RunningAsAdmin()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return UnixUserChecker.IsRoot();
		}

		return false;
	}

	public static string EditorPlatformZipName(OSPlatform platform) =>
		platform == OSPlatform.Windows ? "win-x64.zip" :
		platform == OSPlatform.OSX ? "osx-x64.tar" :
		platform == OSPlatform.Linux ? "linux-x64.tar" :
		throw new PlatformNotSupportedException();

	public static string EditorPlatformBinaryName(OSPlatform platform) =>
		platform == OSPlatform.Windows ? $"{VersionHelpers.ObjectEditorName}.exe" :
		platform == OSPlatform.OSX ? VersionHelpers.ObjectEditorName :
		platform == OSPlatform.Linux ? VersionHelpers.ObjectEditorName :
		throw new PlatformNotSupportedException();

	public static OSPlatform GetPlatform =>
		RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OSPlatform.Windows :
		RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OSPlatform.OSX :
		RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OSPlatform.Linux :
		throw new PlatformNotSupportedException();
}
