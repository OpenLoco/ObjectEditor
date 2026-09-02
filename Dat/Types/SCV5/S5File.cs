// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dat.Types.SCV5;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(StructLength)]
public record S5File(
	[property: LocoStructOffset(0x00)] S5FileHeader Header,
	[property: LocoStructOffset(0x20)] ScenarioOptions? ScenarioOptions,
	[property: LocoStructOffset(0x433A)] SaveDetails? SaveDetails,
	[property: LocoStructOffset(0x10952), LocoArrayLength(S5File.RequiredObjectsCount), Browsable(false)] List<S5Header> RequiredObjects,
	IGameState? GameState,
	[property: LocoStructOffset(0x4B4546)] List<TileElement>? TileElements,
	List<PackedObject> PackedObjects,
	uint32_t Checksum
	)
	: ILocoStruct
{
	public const int StructLength = 0x20;
	public const int RequiredObjectsCount = 859;

	public sealed record PackedObject(S5Header Header, SawyerEncoding Encoding, byte[] Data);

	// convert the 1D TileElements into a more usable 2D array
	public List<TileElement>[,]? TileElementMap { get; set; }
	byte[] OriginalTileElementData { get; set; } = [];

	public (int Width, int Height) GetMapSize()
		=> GetMapSize(SaveDetails, ScenarioOptions);

	public static (int Width, int Height) GetMapSize(SaveDetails saveDetails, ScenarioOptions scenarioOptions)
	{
		var (x, y) = (0, 0);
		if (saveDetails != null)
		{
			(x, y) = (saveDetails.MapSizeX, saveDetails.MapSizeY);
		}
		else if (scenarioOptions != null)
		{
			(x, y) = (scenarioOptions.MapSizeX, scenarioOptions.MapSizeY);
		}

		if (x == 0 || y == 0)
		{
			(x, y) = (Limits.kMapColumnsVanilla, Limits.kMapRowsVanilla);
		}

		return (x, y);
	}

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];

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
			packed = WritePackedObjects();
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
			if (GameState is GeneralStateSave gsv)
			{
				gameState = [.. SawyerStreamWriter.WriteChunk(gsv, SawyerEncoding.RunLengthSingle)];
			}
		}

		if (Header.Flags.HasFlag(HeaderFlags.IsRaw))
		{
			throw new NotImplementedException();
		}

		var tileData = TileElements is { Count: > 0 }
			? SerializeTileElements(TileElements)
			: OriginalTileElementData;
		tiles = SawyerStreamWriter.WriteChunkCore(tileData, SawyerEncoding.RunLengthMulti);

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
		List<PackedObject> packedObjects = [];
		for (var i = 0; i < header.NumPackedObjects; ++i)
		{
			var obj = S5Header.Read(data[..S5Header.StructLength]);
			data = data[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(data[..ObjectHeader.StructLength]);
			data = data[ObjectHeader.StructLength..];

			var encodedData = data[..(int)objectHeader.DataLength];
			data = data[(int)objectHeader.DataLength..];

			var decodedData = SawyerStreamReader.Decode(objectHeader.Encoding, encodedData);
			packedObjects.Add(new PackedObject(obj, objectHeader.Encoding, decodedData));
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

		var mapSize = GetMapSize(saveDetails, scenarioOptions);

		if (header.Type == S5FileType.Scenario)
		{
			var gameStateA = SawyerStreamReader.ReadChunk<GameStateScenarioA>(ref data);
			var gameStateB = SawyerStreamReader.ReadChunk<GameStateScenarioB>(ref data);
			var gameStateC = SawyerStreamReader.ReadChunk<GameStateScenarioC>(ref data);
			var newFlags = gameStateA.FixFlags | S5FixFlags.FixFlag0; // fixState
			gameStateA = gameStateA with { FixFlags = newFlags }; // fixState
			gameState = new GameStateScenario(gameStateA, gameStateB, gameStateC);

			if (gameStateA.GameStateFlags.HasFlag(GameStateFlags.TileManagerLoaded))
			{
				tileElementData = SawyerStreamReader.ReadChunkCore(ref data).ToArray();
				(tileElements, tileElementMap) = ParseTileElements(tileElementData, mapSize.Width, mapSize.Height);
			}
		}
		else
		{
			var chunkData = SawyerStreamReader.ReadChunkCore(ref data);
			var fixFlags = (S5FixFlags)BitConverter.ToUInt16(chunkData[0x434..(0x434 + 2)]);
			if (!fixFlags.HasFlag(S5FixFlags.FixFlag0) && !fixFlags.HasFlag(S5FixFlags.FixFlag1))
			{
				var gs2 = ByteReader.ReadLocoStruct<GameStateSave2>(chunkData);
				var newFlags = gs2.GeneralState.FixFlags | S5FixFlags.FixFlag0; // fixState
				gameState = gs2 with { GeneralState = gs2.GeneralState with { FixFlags = newFlags } }; // fixState
			}
			else
			{
				var gs1 = ByteReader.ReadLocoStruct<GameStateSave1>(chunkData);
				var newFlags = gs1.GeneralState.FixFlags | S5FixFlags.FixFlag0; // fixState
				gameState = gs1 with { GeneralState = gs1.GeneralState with { FixFlags = newFlags } }; // fixState
			}

			tileElementData = SawyerStreamReader.ReadChunkCore(ref data).ToArray();
			(tileElements, tileElementMap) = ParseTileElements(tileElementData, mapSize.Width, mapSize.Height);
		}

		var checksum = BitConverter.ToUInt32(data[0..4]);
		data = data[4..];

		return new S5File(header, scenarioOptions, saveDetails, requiredObjects, gameState, tileElements, packedObjects, checksum) { TileElementMap = tileElementMap, OriginalTileElementData = tileElementData };
	}

	ReadOnlySpan<byte> WritePackedObjects()
	{
		var bytes = new List<byte>();
		foreach (var (header, encoding, data) in PackedObjects)
		{
			bytes.AddRange(header.Write().ToArray());
			bytes.AddRange(SawyerStreamWriter.WriteChunkCore(data, encoding));
		}

		return [.. bytes];
	}

	static byte[] SerializeTileElements(IEnumerable<TileElement> tileElements)
		=> tileElements.SelectMany(x => x.Write()).ToArray();

	static (List<TileElement>, List<TileElement>[,]) ParseTileElements(ReadOnlySpan<byte> tileElementData, int mapWidth, int mapHeight)
	{
		var numTileElements = tileElementData.Length / TileElement.StructLength;

		List<TileElement> tileElements = [];
		var tileElementMap = new List<TileElement>[mapWidth, mapHeight];

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
				if (x == mapWidth - 1)
				{
					y = (y + 1) % mapHeight;
				}

				x = (x + 1) % mapWidth;
			}

			// el.IsLast() indicates its the last element on that tile
			// tiles are set out in rows
			// see TileManager.cpp::updateTilePointers in OpenLoco
		}

		return (tileElements, tileElementMap);
	}
}
