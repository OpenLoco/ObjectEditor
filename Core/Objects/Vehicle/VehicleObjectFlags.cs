namespace OpenLoco.ObjectEditor.Objects
{
	[Flags]
	public enum VehicleObjectFlags : uint16_t
	{
		None = 0,
		flag_02 = 1 << 2, // rollable? APT Passenger carriage
		flag_03 = 1 << 3, // rollable? APT Driving carriage
		RackRail = 1 << 6,
		unk08 = 1 << 8,
		unk09 = 1 << 9, // anytrack??
		SpeedControl = 1 << 10,
		CanCouple = 1 << 11,
		unk12 = 1 << 12, // dualhead??
		IsHelicopter = 1 << 13,
		Refittable = 1 << 14,
		unk15 = 1 << 15, // noannounce??
	};
}
