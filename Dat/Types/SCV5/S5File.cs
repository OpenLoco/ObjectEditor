using Dat.Data;
using Dat.FileParsing;
using System.ComponentModel;

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
			var ot = (ObjectType)i;
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

	public static int GetMaxObjectCount(ObjectType objectType)
		=> objectType switch
		{
			ObjectType.InterfaceSkin => 1,
			ObjectType.Sound => 128,
			ObjectType.Currency => 1,
			ObjectType.Steam => 32,
			ObjectType.CliffEdge => 8,
			ObjectType.Water => 1,
			ObjectType.Land => 32,
			ObjectType.TownNames => 1,
			ObjectType.Cargo => 32,
			ObjectType.Wall => 32,
			ObjectType.TrackSignal => 16,
			ObjectType.LevelCrossing => 4,
			ObjectType.StreetLight => 1,
			ObjectType.Tunnel => 16,
			ObjectType.Bridge => 8,
			ObjectType.TrackStation => 16,
			ObjectType.TrackExtra => 8,
			ObjectType.Track => 8,
			ObjectType.RoadStation => 16,
			ObjectType.RoadExtra => 4,
			ObjectType.Road => 8,
			ObjectType.Airport => 8,
			ObjectType.Dock => 8,
			ObjectType.Vehicle => 224,
			ObjectType.Tree => 64,
			ObjectType.Snow => 1,
			ObjectType.Climate => 1,
			ObjectType.HillShapes => 1,
			ObjectType.Building => 128,
			ObjectType.Scaffolding => 1,
			ObjectType.Industry => 16,
			ObjectType.Region => 1,
			ObjectType.Competitor => 32,
			ObjectType.ScenarioText => 1,
			_ => throw new NotImplementedException()
		};
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(StructLength)]
public record S5File(
	[property: LocoStructOffset(0x00)] S5FileHeader Header,
	[property: LocoStructOffset(0x20)] ScenarioOptions? ScenarioOptions,
	[property: LocoStructOffset(0x433A)] SaveDetails? SaveDetails,
	[property: LocoStructOffset(0x10952), LocoArrayLength(S5File.RequiredObjectsCount), Browsable(false)] List<S5Header> RequiredObjects,
	IGameState? GameState,
	[property: LocoStructOffset(0x4B4546)] List<TileElement>? TileElements,
	List<(S5Header, byte[])> PackedObjects,
	uint32_t Checksum
	)
	: ILocoStruct
{
	public bool Validate() => true;
	public const int StructLength = 0x20;
	public const int RequiredObjectsCount = 859;

	// convert the 1D TileElements into a more usable 2D array
	public List<TileElement>[,]? TileElementMap { get; set; }
	byte[] OriginalTileElementData { get; set; } = [];

	public byte[] Write()
	{
		var hdr = SawyerStreamWriter.WriteChunk(Header, SawyerEncoding.Rotate);
		ReadOnlySpan<byte> save = default;
		ReadOnlySpan<byte> scenario = default;
		if (Header.Flags.HasFlag(HeaderFlags.HasSaveDetails))
		{
			save = SawyerStreamWriter.WriteChunk(SaveDetails, SawyerEncoding.Rotate);
		}

		if (Header.Type == S5FileType.Scenario)
		{
			scenario = SawyerStreamWriter.WriteChunk(ScenarioOptions, SawyerEncoding.Rotate);
		}

		// packed
		ReadOnlySpan<byte> packed = [];
		if (Header.NumPackedObjects != 0)
		{
			// todo: add data here
			// todo: copy ObjectManager::writePackedObjects
			//packed = WritePackedObjects();
		}

		// required
		var structured = ObjectManager.GetStructuredHeaders(RequiredObjects);
		var reqData = structured.ConvertAll(x => x.Write().ToArray()).SelectMany(x => x);
		var required = SawyerStreamWriter.WriteChunkCore([.. reqData], SawyerEncoding.Rotate);

		// gamestate
		byte[] gameState = [];
		byte[] tiles = [];

		if (Header.Type == S5FileType.Scenario && GameState is GameStateScenario gsc)
		{
			var gA = SawyerStreamWriter.WriteChunk(gsc.StateA, SawyerEncoding.RunLengthSingle);
			var gB = SawyerStreamWriter.WriteChunk(gsc.StateB, SawyerEncoding.RunLengthSingle);
			var gC = SawyerStreamWriter.WriteChunk(gsc.StateC, SawyerEncoding.RunLengthSingle);
			gameState = [.. gA, .. gB, .. gC];
		}
		else
		{
			if (GameState is GameStateSave gsv)
			{
				gameState = [.. SawyerStreamWriter.WriteChunk(gsv, SawyerEncoding.RunLengthSingle)];
			}
		}

		if (Header.Flags.HasFlag(HeaderFlags.IsRaw))
		{
			throw new NotImplementedException();
		}

		tiles = SawyerStreamWriter.WriteChunkCore(OriginalTileElementData, SawyerEncoding.RunLengthMulti);

		byte[] data = [.. hdr, .. save, .. scenario, .. packed, .. required, .. gameState, .. tiles];
		var checksum = data.Sum(x => x);
		return [.. data, .. BitConverter.GetBytes((uint32_t)checksum)];
	}

	public static S5File Read(ReadOnlySpan<byte> data)
	{
		var header = SawyerStreamReader.ReadChunk<S5FileHeader>(ref data);

		SaveDetails? saveDetails = null;
		ScenarioOptions? scenarioOptions = null;

		if (header.Flags.HasFlag(HeaderFlags.HasSaveDetails))
		{
			saveDetails = SawyerStreamReader.ReadChunk<SaveDetails>(ref data);
		}

		if (header.Type == S5FileType.Scenario)
		{
			scenarioOptions = SawyerStreamReader.ReadChunk<ScenarioOptions>(ref data);
		}

		// packed objects
		List<(S5Header, byte[])> packedObjects = [];
		for (var i = 0; i < header.NumPackedObjects; ++i)
		{
			var obj = S5Header.Read(data[..S5Header.StructLength]);
			data = data[S5Header.StructLength..];

			var chunkData = SawyerStreamReader.ReadChunkCore(ref data);
			packedObjects.Add((obj, chunkData.ToArray()));
		}

		// read required objects
		List<S5Header> requiredObjects = [];
		var bytes = SawyerStreamReader.ReadChunkCore(ref data);
		for (var i = 0; i < RequiredObjectsCount; i++)
		{
			var obj = S5Header.Read(bytes[..S5Header.StructLength]);
			if (obj.Checksum != uint.MaxValue)
			{
				requiredObjects.Add(obj);
			}

			bytes = bytes[S5Header.StructLength..];
		}

		// load game state
		List<TileElement>? tileElements = null;
		List<TileElement>[,]? tileElementMap = null;
		byte[] tileElementData = [];
		IGameState gameState;

		if (header.Type == S5FileType.Scenario)
		{
			var gameStateA = SawyerStreamReader.ReadChunk<GameStateScenarioA>(ref data);
			var gameStateB = SawyerStreamReader.ReadChunk<GameStateScenarioB>(ref data);
			var gameStateC = SawyerStreamReader.ReadChunk<GameStateScenarioC>(ref data);
			var newFlags = gameStateA.FixFlags | S5FixFlags.FixFlag1;
			gameStateA = gameStateA with { FixFlags = newFlags };
			gameState = new GameStateScenario(gameStateA, gameStateB, gameStateC);
			FixState();

			if (gameStateA.GameStateFlags.HasFlag(GameStateFlags.TileManagerLoaded))
			{
				tileElementData = SawyerStreamReader.ReadChunkCore(ref data).ToArray();
				(tileElements, tileElementMap) = ParseTileElements(tileElementData);
			}
		}
		else
		{
			gameState = SawyerStreamReader.ReadChunk<GameStateSave>(ref data);
			FixState();

			tileElementData = SawyerStreamReader.ReadChunkCore(ref data).ToArray();
			(tileElements, tileElementMap) = ParseTileElements(tileElementData);
		}

		var checksum = BitConverter.ToUInt32(data[0..4]);
		data = data[4..];

		return new S5File(header, scenarioOptions, saveDetails, requiredObjects, gameState, tileElements, packedObjects, checksum) { TileElementMap = tileElementMap, OriginalTileElementData = tileElementData };
	}

	static void FixState()
	{ }

	static (List<TileElement>, List<TileElement>[,]) ParseTileElements(ReadOnlySpan<byte> tileElementData)
	{
		var numTileElements = tileElementData.Length / TileElement.StructLength;

		List<TileElement> tileElements = [];
		var tileElementMap = new List<TileElement>[Limits.kMapColumns, Limits.kMapRows];

		var x = 0;
		var y = 0;
		for (var i = 0; i < numTileElements; ++i)
		{
			var el = TileElement.Read(tileElementData[..TileElement.StructLength]);
			tileElementData = tileElementData[TileElement.StructLength..];
			tileElements.Add(el);

			if (tileElementMap[x, y] == null)
			{
				tileElementMap[x, y] = [el];
			}
			else
			{
				tileElementMap[x, y].Add(el);
			}

			if (el.IsLast())
			{
				if (x == Limits.kMapColumns - 1)
				{
					y = (y + 1) % Limits.kMapRows;
				}

				x = (x + 1) % Limits.kMapColumns;
			}

			// el.IsLast() indicates its the last element on that tile
			// tiles are set out in rows
			// see TileManager.cpp::updateTilePointers in OpenLoco
		}

		return (tileElements, tileElementMap);
	}

	//public ReadOnlySpan<byte> Write()
	//{
	//}
}
