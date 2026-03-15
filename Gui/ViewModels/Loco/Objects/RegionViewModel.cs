using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;
using DynamicData;
using Gui.Models;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace Gui.ViewModels;

public class RegionViewModel : BaseViewModel<RegionObject>
{
	readonly CompositeDisposable modelSyncSubscriptions = [];

	public RegionViewModel(RegionObject model, ObjectEditorContext? editorContext = null)
		: base(model)
	{
		RequiredObjects = new RequiredObjectsListViewModel(editorContext);
		RequiredObjects.AddOrUpdate(model.DependentObjects);

		_ = RequiredObjects.Connect()
			.Subscribe(Observer.Create<IChangeSet<ObjectModelHeader, uint>>(_ => SyncRequiredObjectsToModel()))
			.DisposeWith(modelSyncSubscriptions);

		CargoInfluenceObjects = new(model.CargoInfluenceObjects);
		CargoInfluenceTownFilter = new(model.CargoInfluenceTownFilter);
	}

	public DrivingSide VehiclesDriveOnThe
	{
		get => Model.VehiclesDriveOnThe;
		set => Model.VehiclesDriveOnThe = value;
	}

	public uint8_t pad_07
	{
		get => Model.pad_07;
		set => Model.pad_07 = value;
	}

	[Browsable(false)]
	public RequiredObjectsListViewModel RequiredObjects { get; }

	[Category("Cargo")]
	public BindingList<ObjectModelHeader> CargoInfluenceObjects { get; }

	[Category("Cargo")]
	public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; }

	void SyncRequiredObjectsToModel()
	{
		Model.DependentObjects.Clear();
		Model.DependentObjects.AddRange(RequiredObjects.Items);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			modelSyncSubscriptions.Dispose();
			RequiredObjects.Dispose();
		}

		base.Dispose(disposing);
	}
}
