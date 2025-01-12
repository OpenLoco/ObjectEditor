using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using System.ComponentModel;

namespace OpenLoco.Dat.Types.SCV5
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(StructLength)]
	public record S5File(
		[property: LocoStructOffset(0x00)] S5FileHeader Header,
		[property: LocoStructOffset(0x20)] ScenarioOptions? LandscapeOptions,
		[property: LocoStructOffset(0x433A)] SaveDetails? SaveDetails,
		[property: LocoStructOffset(0x10952), LocoArrayLength(S5File.RequiredObjectsCount), Browsable(false)] List<S5Header> RequiredObjects,
		IGameState? GameState,
		[property: LocoStructOffset(0x4B4546)] List<TileElement>? TileElements,
		List<(S5Header, byte[])> PackedObjects
		)
		: ILocoStruct
	{
		public bool Validate() => true;
		public const int StructLength = 0x20;
		public const int RequiredObjectsCount = 859;

		// convert the 1D TileElements into a more usable 2D array
		public List<TileElement>[,]? TileElementMap { get; set; }

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
			IGameState gameState;

			if (header.Type == S5FileType.Scenario)
			{
				var gameStateA = SawyerStreamReader.ReadChunk<GameStateScenarioA>(ref data);
				var gameStateB = SawyerStreamReader.ReadChunk<GameStateScenarioB>(ref data);
				var gameStateC = SawyerStreamReader.ReadChunk<GameStateScenarioC>(ref data);
				gameState = new GameStateScenario(gameStateA, gameStateB, gameStateC);

				if (gameStateA.GameStateFlags.HasFlag(GameStateFlags.TileManagerLoaded))
				{
					var tileElementData = SawyerStreamReader.ReadChunkCore(ref data);
					(tileElements, tileElementMap) = ParseTileElements(tileElementData);
				}
			}
			else
			{
				gameState = SawyerStreamReader.ReadChunk<GameStateSave>(ref data);

				var tileElementData = SawyerStreamReader.ReadChunkCore(ref data);
				(tileElements, tileElementMap) = ParseTileElements(tileElementData);

			}

			return new S5File(header, scenarioOptions, saveDetails, requiredObjects, gameState, tileElements, packedObjects) { TileElementMap = tileElementMap };
		}

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
}
