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
			.Subscribe(Observer.Create<IChangeSet<ObjectModelHeader>>(_ => SyncRequiredObjectsToModel()))
			.DisposeWith(modelSyncSubscriptions);

		CargoInfluenceObjects = new(model.CargoInfluenceObjects, h => new CargoReferenceViewModel(h), list => model.CargoInfluenceObjects = [.. list]);
		CargoInfluenceTownFilter = [with(model.CargoInfluenceTownFilter)];
	}

	public DrivingSide VehicleDrivingSide
	{
		get => Model.VehicleDrivingSide;
		set => Model.VehicleDrivingSide = value;
	}

	[Browsable(false)]
	public RequiredObjectsListViewModel RequiredObjects { get; init; }

	[Category("Cargo")]
	public ObjectReferenceList<CargoReferenceViewModel> CargoInfluenceObjects { get; init; }

	[Category("Cargo")]
	public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; init; }

	void SyncRequiredObjectsToModel()
	{
		Model.DependentObjects.Clear();
		Model.DependentObjects.AddRange(RequiredObjects.Items);
	}

	public override void SynchronizeToModel()
	{
		CargoInfluenceObjects.Synchronize();
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
