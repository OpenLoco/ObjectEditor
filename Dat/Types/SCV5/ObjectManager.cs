// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

using Dat.Data;

namespace Dat.Types.SCV5;

public static class ObjectManager
{
	public static readonly S5Header FillHeader = new(uint.MaxValue, "ÿÿÿÿÿÿÿÿ", uint.MaxValue);

	public static List<S5Header> GetStructuredHeaders(List<S5Header> allHeaders)
	{
		var structuredList = new List<S5Header>(S5File.RequiredObjectsCount);
		var grouped = allHeaders.GroupBy(x => x.ObjectType).ToDictionary(x => x.Key, x => x.Select(y => y).ToList());

		for (var i = 0; i < Limits.kMaxObjectTypes; ++i)
		{
			var ot = (DatObjectType)i;
			var count = GetMaxObjectCount(ot);

			for (var hdr = 0; hdr < count; ++hdr)
			{
				if (grouped.TryGetValue(ot, out var hdrs))
				{
					var item = hdr < hdrs.Count ? hdrs[hdr] : FillHeader;
					structuredList.Add(item);
				}
				else
				{
					structuredList.Add(FillHeader);
				}
			}
		}

		if (structuredList.Count != S5File.RequiredObjectsCount)
		{
			throw new ArgumentOutOfRangeException(nameof(allHeaders), $"The constructed list didn't have exactly {S5File.RequiredObjectsCount} objects, so it is invalid.");
		}

		return structuredList;
	}

	public static int GetMaxObjectCount(DatObjectType objectType)
		=> objectType switch
		{
			DatObjectType.InterfaceSkin => 1,
			DatObjectType.Sound => 128,
			DatObjectType.Currency => 1,
			DatObjectType.Steam => 32,
			DatObjectType.CliffEdge => 8,
			DatObjectType.Water => 1,
			DatObjectType.Land => 32,
			DatObjectType.TownNames => 1,
			DatObjectType.Cargo => 32,
			DatObjectType.Wall => 32,
			DatObjectType.TrackSignal => 16,
			DatObjectType.LevelCrossing => 4,
			DatObjectType.StreetLight => 1,
			DatObjectType.Tunnel => 16,
			DatObjectType.Bridge => 8,
			DatObjectType.TrackStation => 16,
			DatObjectType.TrackExtra => 8,
			DatObjectType.Track => 8,
			DatObjectType.RoadStation => 16,
			DatObjectType.RoadExtra => 4,
			DatObjectType.Road => 8,
			DatObjectType.Airport => 8,
			DatObjectType.Dock => 8,
			DatObjectType.Vehicle => 224,
			DatObjectType.Tree => 64,
			DatObjectType.Snow => 1,
			DatObjectType.Climate => 1,
			DatObjectType.HillShapes => 1,
			DatObjectType.Building => 128,
			DatObjectType.Scaffolding => 1,
			DatObjectType.Industry => 16,
			DatObjectType.Region => 1,
			DatObjectType.Competitor => 32,
			DatObjectType.ScenarioText => 1,
			_ => throw new NotImplementedException()
		};
}
