using NUnit.Framework;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;
using OpenLocoObjectEditor.Logging;
using OpenLocoObjectEditor.Objects;

namespace OpenLocoObjectEditor.Tests
{
	[TestFixture]
	public class ObjectSavingTests
	{
		[TestCase("AIRPORT1.DAT")]
		public void LoadSaveAirportObject(string objectName)
		{
			var obj = new ObjectLoadingTests();
			obj.LoadAirportObject(objectName);

		}

		[Test]
		public void WriteLocoStruct()
		{
			// arrange
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SIGC3.DAT";
			_ = new FileInfo(testFile).Length;
			_ = new Logger();
			var loaded = SawyerStreamReader.LoadFullObjectFromFile(testFile);

			// load data in raw bytes for test
			ReadOnlySpan<byte> fullData = SawyerStreamReader.LoadBytesFromFile(testFile);

			// make openlocotool useful objects
			_ = S5Header.Read(fullData[0..S5Header.StructLength]);
			var remainingData = fullData[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
			remainingData = remainingData[ObjectHeader.StructLength..];

			var originalEncodedData = remainingData.ToArray();
			var decodedData = SawyerStreamReader.Decode(objectHeader.Encoding, originalEncodedData);
			_ = decodedData;

			var originalObjectData = decodedData[..ObjectAttributes.StructSize<TrainSignalObject>()];

			// act
			var bytes = ByteWriter.WriteLocoStruct(loaded.LocoObject.Object);

			// assert
			CollectionAssert.AreEqual(originalObjectData, bytes.ToArray());
		}

		[Test]
		public void Industry()
		{
			// arrange
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\BREWERY.DAT";
			_ = new FileInfo(testFile).Length;
			_ = new Logger();
			var loaded = SawyerStreamReader.LoadFullObjectFromFile(testFile);

			// load data in raw bytes for test
			ReadOnlySpan<byte> fullData = SawyerStreamReader.LoadBytesFromFile(testFile);

			// make openlocotool useful objects
			_ = S5Header.Read(fullData[0..S5Header.StructLength]);
			var remainingData = fullData[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
			remainingData = remainingData[ObjectHeader.StructLength..];

			var originalEncodedData = remainingData.ToArray();
			var decodedData = SawyerStreamReader.Decode(objectHeader.Encoding, originalEncodedData);
			_ = decodedData;

			var originalObjectData = decodedData[..ObjectAttributes.StructSize<TrainSignalObject>()];

			// act
			var bytes = ByteWriter.WriteLocoStruct(loaded.LocoObject.Object);

			// assert
			CollectionAssert.AreEqual(originalObjectData, bytes.ToArray());
		}
	}
}