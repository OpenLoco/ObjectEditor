using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace Core.Types.SCV5
{
	// todo: make a list? is this hardcoded?
	public record S5File(
		[property: LocoStructOffset(0x00)] Header Header,
		[property: LocoStructOffset(0x20)] LandscapeDetails? LandscapeOptions,
		[property: LocoStructOffset(0x433A)] SaveDetails? SaveDetails,
		[property: LocoStructOffset(0x10952), LocoArrayLength(859)] S5Header[] RequiredObjects,
		[property: LocoStructOffset(0x13F02)] GameState GameState
		//[property: LocoStructOffset(0x4B4546)] List<TileElement> TileElements,
		//List<(ObjectHeader, byte[])> PackedObjects
		)
		: ILocoStruct
	{
		public bool Validate() => true;

		public static S5File Read(ReadOnlySpan<byte> data)
		{
			var header = SawyerStreamReader.ReadChunk<Header>(ref data);

			SaveDetails? saveDetails = null;
			LandscapeDetails? landscapeDetails = null;

			if (header.Flags.HasFlag(HeaderFlags.HasSaveDetails))
			{
				saveDetails = SawyerStreamReader.ReadChunk<SaveDetails>(ref data);
			}

			if (header.Type == S5Type.Scenario)
			{
				landscapeDetails = SawyerStreamReader.ReadChunk<LandscapeDetails>(ref data);
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
			while (bytes.Length > 0)
			{
				var obj = S5Header.Read(bytes[..S5Header.StructLength]);
				requiredObjects.Add(obj);
				bytes = bytes[S5Header.StructLength..];
			}

			//var obj = ObjectHeader.Read(data[..ObjectHeader.StructLength]);
			//data = data[ObjectHeader.StructLength..];

			//List<(S5Header, byte[])> requiredObjects = [];
			//for (var i = 0; i < obj.DataLength; ++i)
			//{ }
			//var chunkData = SawyerStreamReader.ReadChunkCore(ref data);
			//packedObjects.Add((obj, chunkData));

			// load game state
			var gameState = SawyerStreamReader.ReadChunk<GameState>(ref data);

			//if (header.Type == S5Type.Scenario)
			//{
			//	//
			//}
			//else
			//{
			//}

			//List<S5Header> requiredObjects = [];
			//for (var i = 0; i < 859; ++i)
			//{
			//	requiredObjects.Add(S5Header.Read(data[..S5Header.StructLength]));
			//	data = data[S5Header.StructLength..];
			//}

			// game state
			//var gameState = ByteReader.ReadLocoStruct<GameState>(data);
			//data = data[..GameState.StructLength];

			// tile elements

			// packed objects
			return new S5File(header, landscapeDetails, saveDetails, [.. requiredObjects], gameState);
		}

		//public ReadOnlySpan<byte> Write()
		//{
		//}
	}
}
