using Index;
using NUnit.Framework;

namespace ObjectService.Tests;

[TestFixture]
public class ServerFolderManagerTests
{
	static readonly string[] CategoryFolderNames =
	[
		ServerFolderManager.ObjectsFolderName,
		ServerFolderManager.LandscapesFolderName,
		ServerFolderManager.ScenariosFolderName,
		ServerFolderManager.TutorialsFolderName,
		ServerFolderManager.SoundEffectsFolderName,
		ServerFolderManager.MusicFolderName,
		ServerFolderManager.GraphicsFolderName,
	];

	static readonly string[] SourceFolderNames =
	[
		ServerFolderManager.OriginalFolderName,
		ServerFolderManager.CustomFolderName,
		ServerFolderManager.OpenLocoFolderName,
	];

	[Test]
	public void Constructor_CreatesGameDataFolderWithUniformSourceSubfolders()
	{
		var root = Path.Combine(Path.GetTempPath(), $"server-folder-manager-{Guid.NewGuid():N}");
		_ = Directory.CreateDirectory(root);

		try
		{
			var sfm = new ServerFolderManager(root);
			var gameDataFolder = Path.Combine(root, ServerFolderManager.GameDataFolderName);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(sfm.GameDataFolder, Is.EqualTo(gameDataFolder));

				foreach (var category in CategoryFolderNames)
				{
					var categoryFolder = Path.Combine(gameDataFolder, category);

					Assert.That(Directory.Exists(categoryFolder), Is.True, $"Missing category folder '{category}'.");

					foreach (var source in SourceFolderNames)
					{
						Assert.That(
							Directory.Exists(Path.Combine(categoryFolder, source)),
							Is.True,
							$"Missing source folder '{category}/{source}'.");
					}
				}
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public void CalculatedProperties_AllLiveUnderGameData()
	{
		var root = Path.Combine(Path.GetTempPath(), $"server-folder-manager-{Guid.NewGuid():N}");
		_ = Directory.CreateDirectory(root);

		try
		{
			var sfm = new ServerFolderManager(root);
			var gameDataFolder = Path.Combine(root, ServerFolderManager.GameDataFolderName);
			var objectsFolder = Path.Combine(gameDataFolder, ServerFolderManager.ObjectsFolderName);
			var scenariosFolder = Path.Combine(gameDataFolder, ServerFolderManager.ScenariosFolderName);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(sfm.ObjectsFolder, Is.EqualTo(objectsFolder));
				Assert.That(sfm.ObjectsOriginalFolder, Is.EqualTo(Path.Combine(objectsFolder, ServerFolderManager.OriginalFolderName)));
				Assert.That(sfm.ObjectsCustomFolder, Is.EqualTo(Path.Combine(objectsFolder, ServerFolderManager.CustomFolderName)));
				Assert.That(sfm.ObjectsOpenLocoFolder, Is.EqualTo(Path.Combine(objectsFolder, ServerFolderManager.OpenLocoFolderName)));

				Assert.That(sfm.ScenariosFolder, Is.EqualTo(scenariosFolder));
				Assert.That(sfm.ScenariosOriginalFolder, Is.EqualTo(Path.Combine(scenariosFolder, ServerFolderManager.OriginalFolderName)));
				Assert.That(sfm.ScenariosCustomFolder, Is.EqualTo(Path.Combine(scenariosFolder, ServerFolderManager.CustomFolderName)));
				Assert.That(sfm.ScenariosOpenLocoFolder, Is.EqualTo(Path.Combine(scenariosFolder, ServerFolderManager.OpenLocoFolderName)));

				Assert.That(sfm.LandscapesFolder, Is.EqualTo(Path.Combine(gameDataFolder, ServerFolderManager.LandscapesFolderName)));
				Assert.That(sfm.TutorialsFolder, Is.EqualTo(Path.Combine(gameDataFolder, ServerFolderManager.TutorialsFolderName)));
				Assert.That(sfm.SoundEffectsFolder, Is.EqualTo(Path.Combine(gameDataFolder, ServerFolderManager.SoundEffectsFolderName)));
				Assert.That(sfm.MusicFolder, Is.EqualTo(Path.Combine(gameDataFolder, ServerFolderManager.MusicFolderName)));
				Assert.That(sfm.GraphicsFolder, Is.EqualTo(Path.Combine(gameDataFolder, ServerFolderManager.GraphicsFolderName)));
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public void IndexFile_LivesInObjectsFolderAndIsCreated()
	{
		var root = Path.Combine(Path.GetTempPath(), $"server-folder-manager-{Guid.NewGuid():N}");
		_ = Directory.CreateDirectory(root);

		try
		{
			var sfm = new ServerFolderManager(root);
			var expectedIndexFile = Path.Combine(sfm.ObjectsFolder, ObjectIndex.DefaultIndexFileName);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(sfm.IndexFile, Is.EqualTo(expectedIndexFile));
				Assert.That(File.Exists(sfm.IndexFile), Is.True);
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public void GetCustomObjectRelativeFileName_IsRelativeAndUnderCustomFolder()
	{
		var uuid = Guid.NewGuid();

		var relative = ServerFolderManager.GetCustomObjectRelativeFileName(uuid);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(Path.IsPathRooted(relative), Is.False);
			Assert.That(Path.GetExtension(relative), Is.EqualTo(".dat"));
			Assert.That(Path.GetDirectoryName(relative), Is.EqualTo(ServerFolderManager.CustomFolderName));
		}
	}

	[Test]
	public void Constructor_ThrowsWhenRootDirectoryDoesNotExist()
	{
		var missingRoot = Path.Combine(Path.GetTempPath(), $"server-folder-manager-missing-{Guid.NewGuid():N}");

		_ = Assert.Throws<DirectoryNotFoundException>(() => _ = new ServerFolderManager(missingRoot));
	}
}