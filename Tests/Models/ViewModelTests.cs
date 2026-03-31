using Definitions.ObjectModels.Types;
using Gui.Models;
using Gui.ViewModels;
using Index;
using NUnit.Framework;

namespace Tests.Models;

[TestFixture]
public class ViewModelTests
{
	[Test]
	public void IndexEntryToFileSystemItem_MarksOnlineObjectsAsOpenableObjectEntries()
	{
		var entry = new ObjectIndexEntry(
			"Test object",
			null,
			1,
			1234,
			null,
			ObjectType.Building,
			ObjectSource.Custom,
			DateOnly.FromDateTime(DateTime.UtcNow),
			DateOnly.FromDateTime(DateTime.UtcNow));

		var item = FolderTreeViewModel.IndexEntryToFileSystemItem(entry, "/tmp/downloads", FileLocation.Online);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(item.OnlineApiEndpointGroup, Is.EqualTo(OnlineApiEndpointGroup.Objects));
			Assert.That(item.CanOpen, Is.True);
			Assert.That(item.DatChecksum, Is.EqualTo((uint)1234));
		}
	}

	[Test]
	public void FileSystemItem_CanOpen_IsFalseForNonObjectOnlineEntries()
	{
		var pack = new FileSystemItem("Pack", null, 10, FileLocation: FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.ObjectPacks,
		};
		var scenario = new FileSystemItem("Scenario", null, 11, FileLocation: FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.Scenarios,
		};

		using (Assert.EnterMultipleScope())
		{
			Assert.That(pack.CanOpen, Is.False);
			Assert.That(scenario.CanOpen, Is.False);
		}
	}
}
