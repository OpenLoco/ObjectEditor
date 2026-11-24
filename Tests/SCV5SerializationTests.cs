using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Dat.Types.SCV5;
using NUnit.Framework;
using Logger = Common.Logging.Logger;

namespace Dat.Tests;

[TestFixture]
public class SCV5SerializationTests
{
	[TestCase(S5FileType.Scenario, ".sc5")]
	[TestCase(S5FileType.SavedGame, ".sv5")]
	public void LoadThenSave_PreservesBytes(S5FileType fileType, string extension)
	{
		var inputBytes = CreateS5FileBytes(fileType);
		var inputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}{extension}");
		var outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}{extension}");
		try
		{
			File.WriteAllBytes(inputPath, inputBytes);

			var loaded = SawyerStreamReader.LoadSave(inputPath, new Logger());
			Assert.That(loaded, Is.Not.Null);

			File.WriteAllBytes(outputPath, loaded!.Write());

			var outputBytes = File.ReadAllBytes(outputPath);
			Assert.That(outputBytes, Is.EqualTo(inputBytes).AsCollection);
		}
		finally
		{
			if (File.Exists(inputPath))
			{
				File.Delete(inputPath);
			}

			if (File.Exists(outputPath))
			{
				File.Delete(outputPath);
			}
		}
	}

	static byte[] CreateS5FileBytes(S5FileType fileType)
	{
		var hasSaveDetails = fileType == S5FileType.SavedGame;
		List<(S5Header Header, SawyerEncoding Encoding, byte[] Data)> packedObjects = fileType == S5FileType.Scenario
			? [(new S5Header(0x13, "PACKOBJ1", 0x12345678), SawyerEncoding.RunLengthSingle, new byte[] { 1, 1, 2, 2, 3, 4, 4, 4 })]
			: [];

		var header = new S5FileHeader(
			fileType,
			hasSaveDetails ? HeaderFlags.HasSaveDetails : HeaderFlags.None,
			(ushort)packedObjects.Count,
			0x62262,
			0x62300,
			new byte[20]);

		var data = new List<byte>();
		data.AddRange(SawyerStreamWriter.WriteChunk(header, SawyerEncoding.Rotate).ToArray());

		if (hasSaveDetails)
		{
			data.AddRange(SawyerStreamWriter.WriteChunkCore(CreateSaveDetailsRaw(), SawyerEncoding.Rotate));
		}

		if (fileType == S5FileType.Scenario)
		{
			data.AddRange(SawyerStreamWriter.WriteChunkCore(CreateScenarioOptionsRaw(), SawyerEncoding.Rotate));
		}

		foreach (var (packedHeader, encoding, packedData) in packedObjects)
		{
			data.AddRange(packedHeader.Write().ToArray());
			data.AddRange(SawyerStreamWriter.WriteChunkCore(packedData, encoding));
		}

		data.AddRange(SawyerStreamWriter.WriteChunkCore(CreateRequiredObjectsRaw(), SawyerEncoding.Rotate));

		if (fileType == S5FileType.Scenario)
		{
			data.AddRange(SawyerStreamWriter.WriteChunkCore(CreateScenarioGameStateA(), SawyerEncoding.RunLengthSingle));
			data.AddRange(SawyerStreamWriter.WriteChunkCore(new byte[0x123480], SawyerEncoding.RunLengthSingle));
			data.AddRange(SawyerStreamWriter.WriteChunkCore(new byte[0x79D80], SawyerEncoding.RunLengthSingle));
		}
		else
		{
			data.AddRange(SawyerStreamWriter.WriteChunkCore(CreateSaveGameStateRaw(), SawyerEncoding.RunLengthSingle));
		}

		data.AddRange(SawyerStreamWriter.WriteChunkCore(CreateTileElementsRaw(), SawyerEncoding.RunLengthMulti));

		var checksum = (uint)data.Sum(x => x);
		data.AddRange(BitConverter.GetBytes(checksum));
		return [.. data];
	}

	static byte[] CreateSaveDetailsRaw()
	{
		var saveDetails = new byte[SaveDetails.StructLength];
		BitConverter.GetBytes((ushort)2).CopyTo(saveDetails, 0xC59C);
		BitConverter.GetBytes((ushort)1).CopyTo(saveDetails, 0xC59E);
		return saveDetails;
	}

	static byte[] CreateScenarioOptionsRaw()
	{
		var scenarioOptions = new byte[ScenarioOptions.StructLength];
		BitConverter.GetBytes((ushort)2).CopyTo(scenarioOptions, 0x41C4);
		BitConverter.GetBytes((ushort)1).CopyTo(scenarioOptions, 0x41C6);
		return scenarioOptions;
	}

	static byte[] CreateScenarioGameStateA()
	{
		var stateA = new byte[0xB96C];
		BitConverter.GetBytes((uint)GameStateFlags.TileManagerLoaded).CopyTo(stateA, 0x10);
		return stateA;
	}

	static byte[] CreateSaveGameStateRaw()
	{
		var saveState = new byte[0x4A0644];
		BitConverter.GetBytes((ushort)S5FixFlags.FixFlag0).CopyTo(saveState, 0x434);
		return saveState;
	}

	static byte[] CreateRequiredObjectsRaw()
	{
		var required = new byte[S5File.RequiredObjectsCount * S5Header.StructLength];
		var emptyHeader = ObjectManager.FillHeader.Write().ToArray();
		for (var i = 0; i < S5File.RequiredObjectsCount; i++)
		{
			emptyHeader.CopyTo(required, i * S5Header.StructLength);
		}
		return required;
	}

	static byte[] CreateTileElementsRaw()
		=> [
			0x00, 0x80, 0x05, 0x05, 0x03, 0x00, 0x02, 0x00,
			0x06, 0x80, 0x06, 0x06, 0x01, 0x12, 0x30, 0x40
		];
}
