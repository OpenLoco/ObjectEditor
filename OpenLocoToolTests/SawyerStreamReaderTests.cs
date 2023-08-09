using OpenLocoTool;
using OpenLocoTool.DatFileParsing;
using OpenLocoToolCommon;

namespace OpenLocoToolTests
{
	public class Tests
	{
		[Test]
		public void DecodeEncodeIdempotent()
		{
			//const string path = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\STEAM.dat";
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SIGC3.DAT";
			var fileSize = new FileInfo(testFile).Length;
			var logger = new Logger();
			var ssr = new SawyerStreamReader(logger);
			var loaded = ssr.LoadFromFile(testFile);

			Assert.AreEqual(SawyerEncoding.runLengthSingle, loaded.ObjHdr2.Encoding);
			Assert.AreEqual(fileSize, Constants.DatFileHeaderSize + Constants.ObjHeaderSize + loaded.ObjHdr2.Length);

			var decoded = ssr.Decode(loaded.ObjHdr2.Encoding, loaded.RawData);

			var ssw = new SawyerStreamWriter(logger);
			var encoded = ssw.Encode(loaded.ObjHdr2.Encoding, decoded);

			CollectionAssert.AreEqual(loaded.RawData, encoded.ToArray());
			//var temp = Path.GetTempFileName();
			//ssw.WriteToFile(temp, loaded.ObjHdr1, loaded.ObjHDr2, loaded.Data);
		}
	}
}