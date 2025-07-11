using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace Gui.ViewModels
{
	public class EditorSettingsWindowViewModel : ViewModelBase
	{
		EditorSettings _Settings { get; }

		public EditorSettingsWindowViewModel()
		{ }

		public EditorSettingsWindowViewModel(EditorSettings settings)
		{
			_Settings = settings;

			AllowSavingAsVanillaObject = settings.AllowSavingAsVanillaObject;
			AutoObjectDiscoveryAndUpload = settings.AutoObjectDiscoveryAndUpload;
			UseHttps = settings.UseHttps;
			ServerAddressHttp = settings.ServerAddressHttp;
			ServerAddressHttps = settings.ServerAddressHttps;
			DownloadFolder = settings.DownloadFolder;
			CurrentObjDataFolder = settings.ObjDataDirectory;
			ObjDataDirectories = settings.ObjDataDirectories.ToBindingList();

			AppDataObjDataFolder = settings.AppDataObjDataFolder;
			LocomotionObjDataFolder = settings.LocomotionObjDataFolder;
			OpenLocoObjDataFolder = settings.OpenLocoObjDataFolder;
		}

		public void Commit()
		{
			_Settings.AllowSavingAsVanillaObject = AllowSavingAsVanillaObject;
			_Settings.AutoObjectDiscoveryAndUpload = AutoObjectDiscoveryAndUpload;
			_Settings.UseHttps = UseHttps;
			_Settings.ServerAddressHttp = ServerAddressHttp;
			_Settings.ServerAddressHttps = ServerAddressHttps;
			_Settings.DownloadFolder = DownloadFolder;
			_Settings.ObjDataDirectory = CurrentObjDataFolder;
			_Settings.ObjDataDirectories = [.. ObjDataDirectories];

			_Settings.AppDataObjDataFolder = AppDataObjDataFolder;
			_Settings.LocomotionObjDataFolder = LocomotionObjDataFolder;
			_Settings.OpenLocoObjDataFolder = OpenLocoObjDataFolder;
		}

		[Reactive, Category("Misc"), DisplayName("Allow saving as vanilla object"), Description("If enabled, the editor will allow saving objects with \"Vanilla\" flag set. If disabled, the object will be forcefully saved as \"Custom\" instead.")]
		public bool AllowSavingAsVanillaObject { get; set; }

		#region Object Folders

		const string GameObjectFolderCategory = "Folders OpenLoco can use objects from";
		const string UserObjectFolderCategory = "Folders where you store custom objects";

		[Reactive, PathBrowsable(PathBrowsableType.Directory), Category(GameObjectFolderCategory), DisplayName("AppData ObjData Folder"), Description("The ObjData folder in %AppData%\\OpenLoco\\objects.")]
		public string AppDataObjDataFolder { get; set; } = string.Empty;

		[Reactive, PathBrowsable(PathBrowsableType.Directory), Category(GameObjectFolderCategory), DisplayName("Locomotion ObjData Folder"), Description("The ObjData folder in your Locomotion installation.")]
		public string LocomotionObjDataFolder { get; set; } = string.Empty;
		[Reactive, PathBrowsable(PathBrowsableType.Directory), Category(GameObjectFolderCategory), DisplayName("OpenLoco ObjData Folder"), Description("The ObjData folder in the OpenLoco\\Objects directory.")]
		public string OpenLocoObjDataFolder { get; set; } = string.Empty;

		[Reactive, PathBrowsable(PathBrowsableType.Directory), Category(UserObjectFolderCategory), DisplayName("Downloads"), Description("The folder to store downloaded objects.")]
		public string DownloadFolder { get; set; } = string.Empty;

		[Reactive, ReadOnly(true), Category(UserObjectFolderCategory), DisplayName("Current ObjectData folder"), Description("The currently-selected ObjectData folder. This is readonly and only used to remember the previous location when you start up the editor.")]
		public string CurrentObjDataFolder { get; set; }

		[Reactive, Category(UserObjectFolderCategory), DisplayName("ObjectData folders"), Description("The list of all ObjectData folders.")]
		public BindingList<string> ObjDataDirectories { get; set; }

		#endregion

		#region Object Service

		[Reactive, Category("Object Service"), DisplayName("Automatic object discovery and upload"), Description("If enabled, the editor will scan the current object directory for objects and check if there are any that are not known to the object service. If any new objects are discovered they will be automatically uploaded to the service.")]
		public bool AutoObjectDiscoveryAndUpload { get; set; }

		[Reactive, ConditionTarget, Category("Object Service"), DisplayName("Use HTTPS"), Description("Will use the HTTPS address instead of the HTTP address for Object Service connections.")]
		public bool UseHttps { get; set; }

		[Reactive, Category("Object Service"), DisplayName("HTTP"), PropertyVisibilityCondition(nameof(UseHttps), false)]
		public string ServerAddressHttp { get; set; } = "http://openloco.leftofzen.dev/";

		[Reactive, Category("Object Service"), DisplayName("HTTPS"), PropertyVisibilityCondition(nameof(UseHttps), true)]
		public string ServerAddressHttps { get; set; } = "https://openloco.leftofzen.dev/";

		#endregion
	}
}
