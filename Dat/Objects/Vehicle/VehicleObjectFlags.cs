namespace OpenLoco.Dat.Objects
{
	[Flags]
	public enum VehicleObjectFlags : uint16_t
	{
		None = 0,
		unk_00 = 1 << 0,
		unk_01 = 1 << 1,
		unk_02 = 1 << 2, // rollable? APT Passenger carriage
		unk_03 = 1 << 3, // rollable? APT Driving carriage
		unk_04 = 1 << 4,
		unk_05 = 1 << 5,
		RackRail = 1 << 6,
		unk_07 = 1 << 7,
		unk_08 = 1 << 8,
		unk_09 = 1 << 9, // any-track??
		SpeedControl = 1 << 10,
		CanCouple = 1 << 11,
		unk_12 = 1 << 12, // dual-head??
		IsHelicopter = 1 << 13,
		Refittable = 1 << 14,
		unk_15 = 1 << 15, // no-announce??
	};
}
