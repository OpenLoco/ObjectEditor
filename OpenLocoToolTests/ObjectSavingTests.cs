using NUnit.Framework;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;

namespace OpenLocoToolTests
{
	[TestFixture]
	public class ObjectSavingTests
	{
		[Test]
		public void WriteLocoStruct()
		{
			// arrange
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SIGC3.DAT";
			var fileSize = new FileInfo(testFile).Length;
			var logger = new Logger();
			var ssr = new SawyerStreamReader(logger);
			var ssw = new SawyerStreamWriter(logger);
			var loaded = ssr.LoadFull(testFile);

			// load data in raw bytes for test
			ReadOnlySpan<byte> fullData = ssr.LoadBytesFromFile(testFile);

			// make openlocotool useful objects
			var s5Header = S5Header.Read(fullData[0..S5Header.StructLength]);
			var remainingData = fullData[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
			remainingData = remainingData[ObjectHeader.StructLength..];

			var originalEncodedData = remainingData.ToArray();
			var decodedData = ssr.Decode(objectHeader.Encoding, originalEncodedData);
			remainingData = decodedData;

			var originalObjectData = decodedData[..TrainSignalObject.StructSize];

			// act
			var bytes = ByteWriter.WriteLocoStruct(loaded.Object);

			// assert
			CollectionAssert.AreEqual(originalObjectData, bytes.ToArray());
		}
	}
}