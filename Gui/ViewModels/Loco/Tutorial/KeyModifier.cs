namespace Gui.ViewModels.Loco.Tutorial;

public enum KeyModifier : uint8_t
{
	None = 0,
	Shift = 1 << 0,
	Control = 1 << 1,
	Unknown = 1 << 2,
	Cheat = 1 << 7,
	Invalid = 0xFF,
}
