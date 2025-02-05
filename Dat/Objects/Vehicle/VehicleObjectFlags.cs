namespace OpenLoco.Dat.Objects
{
	[Flags]
	public enum VehicleObjectFlags : uint16_t
	{
		// See github issue https://github.com/OpenLoco/OpenLoco/issues/2877 for discussion on unnamed flags
		None = 0,
		AlternatingDirection = 1 << 0, // sequential vehicles face alternating directions
		TopAndTailPosition = 1 << 1,   // vehicle is forced to the rear of the train
		JacobsBogieFront = 1 << 2,
		JacobsBogieRear = 1 << 3,
		unk_04 = 1 << 4,
		CentrePosition = 1 << 5, // vehicle is forced to the middle of train
		RackRail = 1 << 6,
		// Alternates between sprite 0 and sprite 1 for each vehicle of this type in a train
		// NOTE: This is for vehicles and not vehicle components (which can also do similar)
		AlternatingCarSprite = 1 << 7,
		AircraftIsTailDragger = 1 << 8,
		AnyRoadType = 1 << 9, // set on all road vehicles except trams
		SpeedControl = 1 << 10,
		CannotCoupleToSelf = 1 << 11,
		AircraftFlaresLanding = 1 << 11, // set only on Concorde
		MustHavePair = 1 << 12,          // train requires two or more of this vehicle
		CanWheelSlip = 1 << 13,          // set on all steam locomotives
		AircraftIsHelicopter = 1 << 13,
		Refittable = 1 << 14,
		QuietInvention = 1 << 15, // no newspaper announcement
	};
}
