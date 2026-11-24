using Definitions.ObjectModels.Graphics.Dithering;
using System;
using System.Linq;

namespace Gui.ViewModels;

public static class DitheringMethods
{
	public static DitheringMethod[] AllMethods { get; } = Enum.GetValues<DitheringMethod>();

	/// <summary>All dithering methods except <see cref="DitheringMethod.None"/> (used for the popup combo).</summary>
	public static DitheringMethod[] WithoutNone { get; } = AllMethods.Where(m => m != DitheringMethod.None).ToArray();
}