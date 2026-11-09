using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.CliffEdge;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackExtra;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Tunnel;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Objects.Wall;
using Definitions.ObjectModels.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class DesignerObjectModelHeaderViewModel : ObjectModelHeaderViewModel
{
	public DesignerObjectModelHeaderViewModel()
		: base(new ObjectModelHeader("TestObj", ObjectType.Building, ObjectSource.Custom, 0))
	{ }
}

/// <summary>
/// A view model over an object reference (<see cref="ObjectModelHeader"/>). The base class can point
/// at any object type; the generic subclasses (<see cref="ObjectModelHeaderViewModel{T}"/> and
/// <see cref="ObjectModelHeaderViewModel{T1, T2}"/>) constrain the reference to a specific set of
/// object types, which is how reference type restrictions (eg Building.ProducedCargoType only being
/// able to reference Cargo objects) are enforced at the type level.
/// </summary>
public class ObjectModelHeaderViewModel : BaseViewModel<ObjectModelHeader>
{
	static readonly IReadOnlySet<ObjectType> AnyObjectType = new HashSet<ObjectType>(Enum.GetValues<ObjectType>());

	public ObjectModelHeaderViewModel()
		: this(new ObjectModelHeader())
	{ }

	public ObjectModelHeaderViewModel(ObjectModelHeader model)
		: base(model)
	{ }

	[Browsable(false)]
	public override string DisplayName
		=> "Object Header";

	/// <summary>
	/// The object types this reference is permitted to point at. The untyped view model allows any
	/// object type; constrained subclasses narrow this to the type(s) they are specialised for.
	/// </summary>
	[Browsable(false)]
	public virtual IReadOnlySet<ObjectType> AllowedObjectTypes => AnyObjectType;

	[MaxLength(8)]
	public string Name
	{
		get => Model.Name;
		set => Model.Name = value;
	}

	public ObjectSource ObjectSource
	{
		get => Model.ObjectSource;
		set => Model.ObjectSource = value;
	}

	/// <summary>
	/// The object type of the referenced object. Setting a type that is not in
	/// <see cref="AllowedObjectTypes"/> is rejected, so a constrained reference can never be pointed
	/// at an object type it does not allow.
	/// </summary>
	public virtual ObjectType ObjectType
	{
		get => Model.ObjectType;
		set
		{
			if (!AllowedObjectTypes.Contains(value))
			{
				// The reference type restriction forbids this object type. Ignore the change and raise a
				// notification so any bound editor restores the current (allowed) value.
				RaisePropertiesChanged(nameof(ObjectType));
				return;
			}

			Model.ObjectType = value;
		}
	}

	public uint DatChecksumInt
	{
		get => Model.DatChecksum;
		set => Model.DatChecksum = value;
	}

	public string DatChecksumHex
	{
		get => string.Format($"0x{DatChecksumInt:X}");
		set => DatChecksumInt = Convert.ToUInt32(value[2..], 16);
	}
}

/// <summary>
/// An <see cref="ObjectModelHeaderViewModel"/> constrained to a single object model type
/// (<typeparamref name="T"/>). The <c>where</c> constraint plus the derived
/// <see cref="AllowedObjectTypes"/> make it impossible to construct a reference to the wrong type.
/// </summary>
public class ObjectModelHeaderViewModel<T> : ObjectModelHeaderViewModel
	where T : class, ILocoStruct
{
	public ObjectModelHeaderViewModel()
		: base(new ObjectModelHeader("<obj>", ReferenceObjectTypes<T>.Value, ObjectSource.Custom, 0))
	{ }

	public ObjectModelHeaderViewModel(ObjectModelHeader model)
		: base(model)
	{ }

	public override IReadOnlySet<ObjectType> AllowedObjectTypes { get; } = new HashSet<ObjectType> { ReferenceObjectTypes<T>.Value };

	// The reference is pinned to a single object type, so it cannot be changed through the UI.
	[ReadOnly(true)]
	public override ObjectType ObjectType
	{
		get => base.ObjectType;
		set => base.ObjectType = value;
	}
}

/// <summary>
/// An <see cref="ObjectModelHeaderViewModel"/> constrained to one of two object model types
/// (<typeparamref name="T1"/> or <typeparamref name="T2"/>), eg Track-or-Road references.
/// </summary>
public class ObjectModelHeaderViewModel<T1, T2> : ObjectModelHeaderViewModel
	where T1 : class, ILocoStruct
	where T2 : class, ILocoStruct
{
	public ObjectModelHeaderViewModel()
		: base(new ObjectModelHeader("<obj>", ReferenceObjectTypes<T1>.Value, ObjectSource.Custom, 0))
	{ }

	public ObjectModelHeaderViewModel(ObjectModelHeader model)
		: base(model)
	{ }

	public override IReadOnlySet<ObjectType> AllowedObjectTypes { get; } =
		new HashSet<ObjectType> { ReferenceObjectTypes<T1>.Value, ReferenceObjectTypes<T2>.Value };
}

/// <summary>
/// Caches the <see cref="ObjectType"/> that a given object model type maps to, used to derive the
/// allowed object types of the constrained reference view models.
/// </summary>
static class ReferenceObjectTypes<T>
	where T : class, ILocoStruct
{
	public static readonly ObjectType Value = ObjectTypeMapping.StructTypeToObjectType(typeof(T));
}

#region Constrained reference view models
// A view model per object reference type restriction used across the object editors. Using these
// instead of the raw ObjectModelHeader makes the restriction part of the property's type: for
// example a BindingList<CargoReferenceViewModel> can only ever hold references to Cargo objects.
// Each exposes a parameterless constructor (used by the property grid when adding a new item) that
// creates a reference pointing at the correct object type.

public class CargoReferenceViewModel : ObjectModelHeaderViewModel<CargoObject>
{
	public CargoReferenceViewModel() { }
	public CargoReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class TrackReferenceViewModel : ObjectModelHeaderViewModel<TrackObject>
{
	public TrackReferenceViewModel() { }
	public TrackReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class RoadReferenceViewModel : ObjectModelHeaderViewModel<RoadObject>
{
	public RoadReferenceViewModel() { }
	public RoadReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class TrackOrRoadReferenceViewModel : ObjectModelHeaderViewModel<TrackObject, RoadObject>
{
	public TrackOrRoadReferenceViewModel() { }
	public TrackOrRoadReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class BridgeReferenceViewModel : ObjectModelHeaderViewModel<BridgeObject>
{
	public BridgeReferenceViewModel() { }
	public BridgeReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class TunnelReferenceViewModel : ObjectModelHeaderViewModel<TunnelObject>
{
	public TunnelReferenceViewModel() { }
	public TunnelReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class WallReferenceViewModel : ObjectModelHeaderViewModel<WallObject>
{
	public WallReferenceViewModel() { }
	public WallReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class SoundReferenceViewModel : ObjectModelHeaderViewModel<SoundObject>
{
	public SoundReferenceViewModel() { }
	public SoundReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class CliffEdgeReferenceViewModel : ObjectModelHeaderViewModel<CliffEdgeObject>
{
	public CliffEdgeReferenceViewModel() { }
	public CliffEdgeReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class LandReferenceViewModel : ObjectModelHeaderViewModel<LandObject>
{
	public LandReferenceViewModel() { }
	public LandReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class TrackStationReferenceViewModel : ObjectModelHeaderViewModel<TrackStationObject>
{
	public TrackStationReferenceViewModel() { }
	public TrackStationReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class RoadStationReferenceViewModel : ObjectModelHeaderViewModel<RoadStationObject>
{
	public RoadStationReferenceViewModel() { }
	public RoadStationReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class TrackExtraReferenceViewModel : ObjectModelHeaderViewModel<TrackExtraObject>
{
	public TrackExtraReferenceViewModel() { }
	public TrackExtraReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class TrackSignalReferenceViewModel : ObjectModelHeaderViewModel<TrackSignalObject>
{
	public TrackSignalReferenceViewModel() { }
	public TrackSignalReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class RoadExtraReferenceViewModel : ObjectModelHeaderViewModel<RoadExtraObject>
{
	public RoadExtraReferenceViewModel() { }
	public RoadExtraReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class VehicleReferenceViewModel : ObjectModelHeaderViewModel<VehicleObject>
{
	public VehicleReferenceViewModel() { }
	public VehicleReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

public class SteamReferenceViewModel : ObjectModelHeaderViewModel<SteamObject>
{
	public SteamReferenceViewModel() { }
	public SteamReferenceViewModel(ObjectModelHeader model) : base(model) { }
}

#endregion
