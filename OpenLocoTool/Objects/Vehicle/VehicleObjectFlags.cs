namespace OpenLocoTool.Objects
{
    [Flags]
    enum VehicleObjectFlags : uint16_t
    {
        none = 0,
        flag_02 = 1 << 2, // rollable? APT Passenger carriage
        flag_03 = 1 << 3, // rollable? APT Driving carriage
        rackRail = 1 << 6,
        unk_08 = 1 << 8,
        unk_09 = 1 << 9, // anytrack??
        speedControl = 1 << 10,
        canCouple = 1 << 11,
        unk_12 = 1 << 12, // dualhead??
        isHelicopter = 1 << 13,
        refittable = 1 << 14,
        unk_15 = 1 << 15, // noannounce??
    };
}
