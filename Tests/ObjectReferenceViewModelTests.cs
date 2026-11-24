using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Types;
using Gui.ViewModels;
using Gui.ViewModels.Loco.Objects.Building;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ObjectReferenceViewModelTests
{
	static ObjectModelHeader MakeHeader(string name, ObjectType type, uint checksum = 0)
		=> new(name, type, ObjectSource.Custom, checksum);

	[Test]
	public void ConstrainedReference_DerivesAllowedObjectTypeFromModelType()
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(new CargoReferenceViewModel().AllowedObjectTypes, Is.EquivalentTo(new[] { ObjectType.Cargo }));
			Assert.That(new TrackReferenceViewModel().AllowedObjectTypes, Is.EquivalentTo(new[] { ObjectType.Track }));
			Assert.That(new RoadReferenceViewModel().AllowedObjectTypes, Is.EquivalentTo(new[] { ObjectType.Road }));
			Assert.That(new WallReferenceViewModel().AllowedObjectTypes, Is.EquivalentTo(new[] { ObjectType.Wall }));
			Assert.That(new TrackOrRoadReferenceViewModel().AllowedObjectTypes, Is.EquivalentTo(new[] { ObjectType.Track, ObjectType.Road }));
		}
	}

	[Test]
	public void ConstrainedReference_ParameterlessCtor_DefaultsToAllowedObjectType()
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(new CargoReferenceViewModel().ObjectType, Is.EqualTo(ObjectType.Cargo));
			Assert.That(new SoundReferenceViewModel().ObjectType, Is.EqualTo(ObjectType.Sound));
			Assert.That(new VehicleReferenceViewModel().ObjectType, Is.EqualTo(ObjectType.Vehicle));
		}
	}

	[Test]
	public void ConstrainedReference_RejectsAnObjectTypeItDoesNotAllow()
	{
		var viewModel = new CargoReferenceViewModel();

		viewModel.ObjectType = ObjectType.Building;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(viewModel.ObjectType, Is.EqualTo(ObjectType.Cargo));
			Assert.That(viewModel.Model.ObjectType, Is.EqualTo(ObjectType.Cargo));
		}
	}

	[Test]
	public void ConstrainedReference_AllowsItsOwnObjectType()
	{
		var viewModel = new CargoReferenceViewModel();

		viewModel.ObjectType = ObjectType.Cargo;

		Assert.That(viewModel.ObjectType, Is.EqualTo(ObjectType.Cargo));
	}

	[Test]
	public void MultiTypeReference_AllowsEachOfItsObjectTypesButNoOthers()
	{
		var viewModel = new TrackOrRoadReferenceViewModel();

		viewModel.ObjectType = ObjectType.Road;
		Assert.That(viewModel.ObjectType, Is.EqualTo(ObjectType.Road));

		viewModel.ObjectType = ObjectType.Track;
		Assert.That(viewModel.ObjectType, Is.EqualTo(ObjectType.Track));

		viewModel.ObjectType = ObjectType.Building;
		Assert.That(viewModel.ObjectType, Is.EqualTo(ObjectType.Track));
	}

	[Test]
	public void ReferenceViewModel_WrapsTheSameUnderlyingHeader()
	{
		var header = MakeHeader("WOOD", ObjectType.Cargo, 0x20);
		var viewModel = new CargoReferenceViewModel(header);

		viewModel.Name = "STEEL";

		using (Assert.EnterMultipleScope())
		{
			Assert.That(viewModel.Model, Is.SameAs(header));
			Assert.That(header.Name, Is.EqualTo("STEEL"));
		}
	}

	[Test]
	public void ObjectReferenceList_AddPropagatesToTheObjectModel()
	{
		var model = new BuildingObject();
		var viewModel = new BuildingViewModel(model);

		viewModel.ProducedCargoType.Add(new CargoReferenceViewModel(MakeHeader("COAL", ObjectType.Cargo)));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(model.ProducedCargoType, Has.Count.EqualTo(1));
			Assert.That(model.ProducedCargoType[0].Name, Is.EqualTo("COAL"));
		}
	}

	[Test]
	public void ObjectReferenceList_RemovePropagatesToTheObjectModel()
	{
		var model = new BuildingObject { ProducedCargoType = [MakeHeader("COAL", ObjectType.Cargo)] };
		var viewModel = new BuildingViewModel(model);

		viewModel.ProducedCargoType.RemoveAt(0);

		Assert.That(model.ProducedCargoType, Is.Empty);
	}

	[Test]
	public void ObjectReferenceList_WrapsExistingHeadersWithoutCopying()
	{
		var header = MakeHeader("COAL", ObjectType.Cargo);
		var model = new BuildingObject { ProducedCargoType = [header] };

		var viewModel = new BuildingViewModel(model);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(viewModel.ProducedCargoType, Has.Count.EqualTo(1));
			Assert.That(viewModel.ProducedCargoType[0].Model, Is.SameAs(header));
		}
	}

	[Test]
	public void SynchronizeToModel_WritesReferenceCollectionsBackToTheModel()
	{
		var model = new BuildingObject();
		var viewModel = new BuildingViewModel(model);
		viewModel.ProducedCargoType.Add(new CargoReferenceViewModel(MakeHeader("COAL", ObjectType.Cargo)));
		viewModel.ConsumedCargoType.Add(new CargoReferenceViewModel(MakeHeader("WOOD", ObjectType.Cargo)));

		viewModel.SynchronizeToModel();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(model.ProducedCargoType, Has.Count.EqualTo(1));
			Assert.That(model.ProducedCargoType[0].Name, Is.EqualTo("COAL"));
			Assert.That(model.ConsumedCargoType, Has.Count.EqualTo(1));
			Assert.That(model.ConsumedCargoType[0].Name, Is.EqualTo("WOOD"));
		}
	}

	[Test]
	public void EntryViewModel_ReferenceReflectsTheUnderlyingReference()
	{
		var reference = new CargoReferenceViewModel(MakeHeader("COAL", ObjectType.Cargo));
		var entryViewModel = new ObjectReferenceEntryViewModel(() => reference, typeof(CargoReferenceViewModel), _ => { });

		using (Assert.EnterMultipleScope())
		{
			Assert.That(entryViewModel.Reference, Is.SameAs(reference));
			Assert.That(entryViewModel.HasReference, Is.True);
			Assert.That(entryViewModel.ReferenceName, Is.EqualTo("COAL"));
		}
	}

	[Test]
	public void EntryViewModel_EnablingManualEditorWithoutAReference_CreatesABlankReference()
	{
		ObjectModelHeaderViewModel? committed = null;
		var entryViewModel = new ObjectReferenceEntryViewModel(() => null, typeof(CargoReferenceViewModel), vm => committed = vm);

		entryViewModel.IsManualEditorVisible = true;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(entryViewModel.IsManualEditorVisible, Is.True);
			Assert.That(committed, Is.InstanceOf<CargoReferenceViewModel>());
			Assert.That(committed!.ObjectType, Is.EqualTo(ObjectType.Cargo));
			Assert.That(committed.Name, Is.Empty);
		}
	}

	[Test]
	public void EntryViewModel_EnablingManualEditorWithAReference_KeepsTheExistingReference()
	{
		var reference = new CargoReferenceViewModel(MakeHeader("COAL", ObjectType.Cargo));
		ObjectModelHeaderViewModel? committed = null;
		var entryViewModel = new ObjectReferenceEntryViewModel(() => reference, typeof(CargoReferenceViewModel), vm => committed = vm);

		entryViewModel.IsManualEditorVisible = true;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(committed, Is.Null);
			Assert.That(entryViewModel.Reference, Is.SameAs(reference));
		}
	}
}
