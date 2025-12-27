namespace Gui;

/// <summary>
/// Provides global access to editor settings for use in ViewModels.
/// This is primarily used for read-only access to settings like the inflation year.
/// </summary>
public static class GlobalSettings
{
	public static EditorSettings? CurrentSettings { get; set; }
}
