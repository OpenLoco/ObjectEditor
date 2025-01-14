using OpenLoco.Common;
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
		byte[] OriginalTileElementData { get; set; }

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
				scenario = SawyerStreamWriter.WriteChunk(LandscapeOptions, SawyerEncoding.Rotate);
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
			var reqData = RequiredObjects.Fill(RequiredObjectsCount, S5Header.NullHeader).Select(x => x.Write().ToArray()).ToList();
			ReadOnlySpan<byte> req = [.. reqData.SelectMany(x => x)];
			var required = SawyerStreamWriter.WriteChunkCore(req, SawyerEncoding.Rotate);

			// gamestate
			byte[] gameState;
			byte[] tiles = [];

			if (Header.Type == S5FileType.Scenario)
			{
				var gs = (GameStateScenario)GameState;
				var gA = SawyerStreamWriter.WriteChunk(gs.StateA, SawyerEncoding.RunLengthSingle);
				var gB = SawyerStreamWriter.WriteChunk(gs.StateB, SawyerEncoding.RunLengthSingle);
				var gC = SawyerStreamWriter.WriteChunk(gs.StateC, SawyerEncoding.RunLengthSingle);
				gameState = [.. gA, .. gB, .. gC];
			}
			else
			{
				var gs = (GameStateSave)GameState;
				gameState = [.. SawyerStreamWriter.WriteChunk(gs, SawyerEncoding.RunLengthSingle)];
			}

			if (Header.Flags.HasFlag(HeaderFlags.IsRaw))
			{
				throw new NotImplementedException();
			}

			tiles = SawyerStreamWriter.WriteChunkCore(OriginalTileElementData, SawyerEncoding.RunLengthMulti);

			var checksum = BitConverter.GetBytes((uint32_t)0);

			return [.. hdr, .. save, .. scenario, .. packed, .. required, .. gameState, .. tiles, .. checksum];
		}

		public static S5File Read(ReadOnlySpan<byte> data)
		{
			List<(string, int)> dataLengths = [];
			var curr = data.Length;

			var header = SawyerStreamReader.ReadChunk<S5FileHeader>(ref data);
			dataLengths.Add(("header", curr - data.Length));
			curr = data.Length;

			SaveDetails? saveDetails = null;
			ScenarioOptions? scenarioOptions = null;

			if (header.Flags.HasFlag(HeaderFlags.HasSaveDetails))
			{
				saveDetails = SawyerStreamReader.ReadChunk<SaveDetails>(ref data);

				dataLengths.Add(("save", curr - data.Length));
				curr = data.Length;
			}

			if (header.Type == S5FileType.Scenario)
			{
				scenarioOptions = SawyerStreamReader.ReadChunk<ScenarioOptions>(ref data);

				dataLengths.Add(("scenario", curr - data.Length));
				curr = data.Length;
			}

			// packed objects
			List<(S5Header, byte[])> packedObjects = [];
			for (var i = 0; i < header.NumPackedObjects; ++i)
			{
				var obj = S5Header.Read(data[..S5Header.StructLength]);
				data = data[S5Header.StructLength..];

				var chunkData = SawyerStreamReader.ReadChunkCore(ref data);

				dataLengths.Add(("packed", curr - data.Length));
				curr = data.Length;

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

			dataLengths.Add(("required", curr - data.Length));
			curr = data.Length;

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
				gameState = new GameStateScenario(gameStateA, gameStateB, gameStateC);

				dataLengths.Add(("gamestate", curr - data.Length));
				curr = data.Length;

				if (gameStateA.GameStateFlags.HasFlag(GameStateFlags.TileManagerLoaded))
				{
					tileElementData = SawyerStreamReader.ReadChunkCore(ref data).ToArray();

					dataLengths.Add(("tileelements", curr - data.Length));
					curr = data.Length;

					(tileElements, tileElementMap) = ParseTileElements(tileElementData);

				}
			}
			else
			{
				gameState = SawyerStreamReader.ReadChunk<GameStateSave>(ref data);

				dataLengths.Add(("gamestate", curr - data.Length));
				curr = data.Length;

				tileElementData = SawyerStreamReader.ReadChunkCore(ref data).ToArray();

				dataLengths.Add(("tileelements", curr - data.Length));
				curr = data.Length;

				(tileElements, tileElementMap) = ParseTileElements(tileElementData);
			}

			var checksum = BitConverter.ToUInt32(data[0..4]);
			data = data[4..];

			dataLengths.Add(("checksum", curr - data.Length));
			_ = data.Length;

			return new S5File(header, scenarioOptions, saveDetails, requiredObjects, gameState, tileElements, packedObjects, checksum) { TileElementMap = tileElementMap, OriginalTileElementData = tileElementData };
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
