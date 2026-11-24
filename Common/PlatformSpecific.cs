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
		EditorPlatformZipName(platform, RuntimeInformation.ProcessArchitecture);

	// mac:   object-editor-5.3.5-osx-x64.zip / object-editor-5.3.5-osx-arm64.zip
	// linux: object-editor-5.3.5-linux-x64.tar.gz
	// win:   object-editor-5.3.5-win-x64.zip
	public static string EditorPlatformZipName(OSPlatform platform, Architecture architecture)
	{
		var rid = RuntimeIdentifierFor(platform, architecture);

		return platform == OSPlatform.Windows ? $"{rid}.zip"
			: platform == OSPlatform.OSX ? $"{rid}.zip"
			: platform == OSPlatform.Linux ? $"{rid}.tar.gz"
			: throw new PlatformNotSupportedException();
	}

	public static string EditorPlatformBinaryName(OSPlatform platform) =>
		platform == OSPlatform.Windows ? $"{VersionHelpers.ObjectEditorName}.exe"
			: platform == OSPlatform.OSX || platform == OSPlatform.Linux ? VersionHelpers.ObjectEditorName
			: throw new PlatformNotSupportedException();

	public static string EditorPlatformUpdaterName(OSPlatform platform) =>
		platform == OSPlatform.Windows ? $"{VersionHelpers.ObjectEditorUpdaterName}.exe"
			: platform == OSPlatform.OSX || platform == OSPlatform.Linux ? VersionHelpers.ObjectEditorUpdaterName
			: throw new PlatformNotSupportedException();

	// the runtime identifier used by build.sh, eg win-x64, osx-arm64, linux-x64
	public static string RuntimeIdentifierFor(OSPlatform platform, Architecture architecture)
	{
		var arch = architecture switch
		{
			Architecture.X64 => "x64",
			Architecture.Arm64 => "arm64",
			_ => throw new PlatformNotSupportedException($"Architecture {architecture} is not supported."),
		};

		return platform == OSPlatform.Windows ? $"win-{arch}"
			: platform == OSPlatform.OSX ? $"osx-{arch}"
			: platform == OSPlatform.Linux ? $"linux-{arch}"
			: throw new PlatformNotSupportedException();
	}

	public static OSPlatform GetPlatform =>
		RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OSPlatform.Windows :
		RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OSPlatform.OSX :
		RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OSPlatform.Linux :
		throw new PlatformNotSupportedException();
}
