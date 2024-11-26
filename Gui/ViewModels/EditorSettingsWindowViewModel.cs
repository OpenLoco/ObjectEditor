using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class EditorSettingsWindowViewModel : ViewModelBase
	{
		public EditorSettingsWindowViewModel()
		{ }

		EditorSettings _Settings { get; }

		public EditorSettingsWindowViewModel(EditorSettings settings)
		{
			_Settings = settings;

			AllowSavingAsVanillaObject = settings.AllowSavingAsVanillaObject;
			AutoObjectDiscoveryAndUpload = settings.AutoObjectDiscoveryAndUpload;
			UseHttps = settings.UseHttps;
			ServerAddressHttp = settings.ServerAddressHttp;
			ServerAddressHttps = settings.ServerAddressHttps;
			DownloadFolder = settings.DownloadFolder;
			ObjDataDirectory = settings.ObjDataDirectory;
			ObjDataDirectories = settings.ObjDataDirectories.ToBindingList();
		}

		public void Commit()
		{
			_Settings.AllowSavingAsVanillaObject = AllowSavingAsVanillaObject;
			_Settings.AutoObjectDiscoveryAndUpload = AutoObjectDiscoveryAndUpload;
			_Settings.UseHttps = UseHttps;
			_Settings.ServerAddressHttp = ServerAddressHttp;
			_Settings.ServerAddressHttps = ServerAddressHttps;
			_Settings.DownloadFolder = DownloadFolder;
			_Settings.ObjDataDirectory = ObjDataDirectory;
			_Settings.ObjDataDirectories = ObjDataDirectories.ToHashSet();
		}

		[Reactive, Category("Misc"), DisplayName("Allow saving as vanilla object"), Description("If enabled, the editor will allow saving objects with \"Vanilla\" flag set. If disabled, the object will be forcefully saved as \"Custom\" instead.")]
		public bool AllowSavingAsVanillaObject { get; set; }

		#region Object Folders

		[Reactive, PathBrowsable(PathBrowsableType.Directory), Category("Object Folders"), DisplayName("Downloads"), Description("The folder to store downloaded objects")]
		public string DownloadFolder { get; set; } = string.Empty;

		[Reactive, ReadOnly(true), Category("Object Folders"), DisplayName("Current ObjectData folder"), Description("The currently-selected ObjectData folder. This is readonly and only used to remember the previous location when you start up the editor.")]
		public string ObjDataDirectory { get; set; }

		[Reactive, Category("Object Folders"), DisplayName("ObjectData folders"), Description("The list of all ObjectData folders.")]
		public BindingList<string> ObjDataDirectories { get; set; }

		#endregion

		#region Object Service

		[Reactive, Category("Object Service"), DisplayName("Automatic object discovery and upload"), Description("If enabled, the editor will scan the current object directory for objects and check if there are any that are not known to the object service. If any new objects are discovered they will be automatically uploaded to the service.")]
		public bool AutoObjectDiscoveryAndUpload { get; set; }

		[Reactive, ConditionTarget, Category("Object Service"), DisplayName("Use HTTPS"), Description("Will use the HTTPS address instead of the HTTP address for Object Service connections.")]
		public bool UseHttps { get; set; }

		[Reactive, Category("Object Service"), DisplayName("HTTP"), VisibilityPropertyCondition(nameof(UseHttps), false)]
		public string ServerAddressHttp { get; set; } = "http://openloco.leftofzen.dev/";

		[Reactive, Category("Object Service"), DisplayName("HTTPS"), VisibilityPropertyCondition(nameof(UseHttps), true)]
		public string ServerAddressHttps { get; set; } = "https://openloco.leftofzen.dev/";

		#endregion
	}
}
