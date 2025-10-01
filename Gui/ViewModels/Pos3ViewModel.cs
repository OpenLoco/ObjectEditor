using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel;

namespace Gui.ViewModels;

public class Pos3ViewModel : MiniReactiveObject
{
	public Pos3 Pos;

	public coord_t X
	{
		get => Pos.X;
		set => Pos.X = value;
	}

	public coord_t Y
	{
		get => Pos.Y;
		set => Pos.Y = value;
	}

	public coord_t Z
	{
		get => Pos.Z;
		set => Pos.Z = value;
	}
}
