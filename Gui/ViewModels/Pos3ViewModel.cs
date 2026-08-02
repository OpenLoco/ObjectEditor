using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel;

namespace Gui.ViewModels;

public class Pos3ViewModel : MiniReactiveObject
{
	public Pos3 Pos = new();

	public coord_t X
	{
		get => Pos?.X ?? 0;
		set => Pos.X = value;
	}

	public coord_t Y
	{
		get => Pos?.Y ?? 0;
		set => Pos.Y = value;
	}

	public coord_t Z
	{
		get => Pos?.Z ?? 0;
		set => Pos.Z = value;
	}
}
