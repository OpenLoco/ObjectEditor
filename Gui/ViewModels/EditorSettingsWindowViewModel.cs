using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gui.ViewModels;

public class EditorSettingsWindowViewModel : ViewModelBase
{
	EditorSettings Model { get; }

	public EditorSettingsWindowViewModel()
	{ }

	public EditorSettingsWindowViewModel(EditorSettings settings)
	{
		Model = settings;
		ObjDataDirectories = new(settings.ObjDataDirectories);
	}

	public void Commit()
	{
		Model.ObjDataDirectories = [.. ObjDataDirectories];
	}

	[Category("Misc"), DisplayName("Allow saving as vanilla object"), Description("If enabled, the editor will allow saving objects with \"Vanilla\" flag set. If disabled, the object will be forcefully saved as \"Custom\" instead.")]
	public bool AllowSavingAsVanillaObject
	{
		get => Model.AllowSavingAsVanillaObject;
		set => Model.AllowSavingAsVanillaObject = value;
	}

	[Category("Misc"), DisplayName("Enable OG Validation"), Description("If enabled, a button is displayed for objects to run validation checks to ensure the object meets OpenGraphics requirements.")]
	public bool EnableOGValidation
	{
		get => Model.EnableOGValidation;
		set => Model.EnableOGValidation = value;
	}

	[Category("Misc"), DisplayName("Show Logs window on Error"), Description("When an error occurs, display the Logs window automatically")]
	public bool ShowLogsOnError
	{
		get => Model.ShowLogsOnError;
		set => Model.ShowLogsOnError = value;
	}

	#region Object Folders

	const string GameObjectFolderCategory = "Folders OpenLoco can use objects from";
	const string UserObjectFolderCategory = "Folders where you store custom objects";

	[PathBrowsable(PathBrowsableType.Directory), Category(GameObjectFolderCategory), DisplayName("Locomotion (Steam) ObjData Folder"), Description("The ObjData folder in your Steam Locomotion installation.")]
	public string LocomotionSteamObjDataFolder
	{
		get => Model.LocomotionSteamObjDataFolder;
		set => Model.LocomotionSteamObjDataFolder = value;
	}

	[PathBrowsable(PathBrowsableType.Directory), Category(GameObjectFolderCategory), DisplayName("Locomotion (GoG) ObjData Folder"), Description("The ObjData folder in your GoG Locomotion installation.")]
	public string LocomotionGoGObjDataFolder
	{
		get => Model.LocomotionGoGObjDataFolder;
		set => Model.LocomotionGoGObjDataFolder = value;
	}

	[PathBrowsable(PathBrowsableType.Directory), Category(GameObjectFolderCategory), DisplayName("AppData ObjData Folder"), Description("The ObjData folder in %AppData%\\OpenLoco\\objects.")]
	public string AppDataObjDataFolder
	{
		get => Model.AppDataObjDataFolder;
		set => Model.AppDataObjDataFolder = value;
	}

	[PathBrowsable(PathBrowsableType.Directory), Category(GameObjectFolderCategory), DisplayName("OpenLoco ObjData Folder"), Description("The ObjData folder in the OpenLoco\\Objects directory.")]
	public string OpenLocoObjDataFolder
	{
		get => Model.OpenLocoObjDataFolder;
		set => Model.OpenLocoObjDataFolder = value;
	}

	[PathBrowsable(PathBrowsableType.Directory), Category(UserObjectFolderCategory), DisplayName("Cache"), Description("The folder to store cached downloaded objects.")]
	public string CacheFolder
	{
		get => Model.CacheFolder;
		set => Model.CacheFolder = value;
	}

	[PathBrowsable(PathBrowsableType.Directory), Category(UserObjectFolderCategory), DisplayName("Downloads"), Description("The folder to store user-downloaded objects.")]
	public string DownloadFolder
	{
		get => Model.DownloadFolder;
		set => Model.DownloadFolder = value;
	}

	[ReadOnly(true), Category(UserObjectFolderCategory), DisplayName("Current ObjectData folder"), Description("The currently-selected ObjectData folder. This is readonly and only used to remember the previous location when you start up the editor.")]
	public string CurrentObjDataFolder
	{
		get => Model.ObjDataDirectory;
		set => Model.ObjDataDirectory = value;
	}

	[Category(UserObjectFolderCategory), DisplayName("ObjectData folders"), Description("The list of all ObjectData folders.")]
	public ObservableCollection<string> ObjDataDirectories { get; set; }

	#endregion

	#region Object Service

	[Category("Object Service"), DisplayName("Automatic object discovery and upload"), Description("If enabled, the editor will scan the current object directory for objects and check if there are any that are not known to the object service. If any new objects are discovered they will be automatically uploaded to the service.")]
	public bool AutoObjectDiscoveryAndUpload
	{
		get => Model.AutoObjectDiscoveryAndUpload;
		set => Model.AutoObjectDiscoveryAndUpload = value;
	}

	[ConditionTarget, Category("Object Service"), DisplayName("Use HTTPS"), Description("Will use the HTTPS address instead of the HTTP address for Object Service connections.")]
	public bool UseHttps
	{
		get => Model.UseHttps;
		set => Model.UseHttps = value;
	}

	[Category("Object Service"), DisplayName("HTTP"), PropertyVisibilityCondition(nameof(UseHttps), false)]
	public string ServerAddressHttp
	{
		get => Model.ServerAddressHttp;
		set => Model.ServerAddressHttp = value;
	}

	[Category("Object Service"), DisplayName("HTTPS"), PropertyVisibilityCondition(nameof(UseHttps), true)]
	public string ServerAddressHttps
	{
		get;
		set;
	}

	#endregion
}
