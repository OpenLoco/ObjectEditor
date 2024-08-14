using Database;
using Microsoft.EntityFrameworkCore;

var builder = new DbContextOptionsBuilder<LocoDb>();
builder.UseSqlite(LocoDb.GetDbPath());
using var db = new LocoDb(builder.Options);

// Note: This sample requires the database to be created before running.
Console.WriteLine($"Database path: {LocoDb.GetDbPath()}");

// Create
Console.WriteLine("Seeding");

if (!db.Tags.Any())
{
	var t1 = new TblTag() { Name = "Engine" };
	var t2 = new TblTag() { Name = "Tree" };

	db.Add(t1);
	db.Add(t2);
	db.SaveChanges();
}

if (!db.Authors.Any())
{
	var a1 = new TblAuthor() { Name = "Bob" };
	var a2 = new TblAuthor() { Name = "Jane" };

	db.Add(a1);
	db.Add(a2);
	db.SaveChanges();
}

//db.Objects.ExecuteDelete();
//db.SaveChanges();

if (!db.Objects.Any())
{
	var o1 = new TblLocoObject()
	{
		Name = "bob1",
		Author = db.Authors.First(),
		OriginalName = "bob1.dat",
		OriginalChecksum = 12345,
		OriginalBytes = [],
		OriginalObjectType = OpenLoco.ObjectEditor.Data.ObjectType.InterfaceSkin,
		OriginalSourceGame = OpenLoco.ObjectEditor.Data.SourceGame.Custom,
	};

	db.Add(o1);
	db.SaveChanges();

	//if (!db.ObjectTagLinks.Any())
	//{
	//	//var otl1 = new TblObjectTagLink() { Object = o1, Tag = db.Tags.Single(x => x.Name == "Engine") };
	//	//var otl2 = new TblObjectTagLink() { Object = o1, Tag = db.Tags.Single(x => x.Name == "Tree") };

	//	db.Add(otl1);
	//	db.Add(otl2);
	//	db.SaveChanges();


	//	//o1.Tags.Add(db.ObjectTagLinks.OrderBy(x => x.Tag.Name).First());
	//	//o1.Tags.Add(db.ObjectTagLinks.OrderBy(x => x.Tag.Name).Last());
	//	db.SaveChanges();
	//}

}

// Read
Console.WriteLine("Querying for an Author");
var author = db.Authors
	.OrderBy(b => b.Name)
	.First();
Console.WriteLine(author.Name);

Console.WriteLine("Querying for a Tag");
var tag = db.Tags
	.OrderBy(b => b.Name)
	.First();
Console.WriteLine(tag.Name);

Console.WriteLine("Querying for an Object");
var obj = db.Objects
	.OrderBy(b => b.Name)
	.First();
Console.WriteLine(obj.Name);
Console.WriteLine(obj.Author?.Name);
//Console.WriteLine(obj.Tags.Count);
//foreach (var t in obj.Tags)
//{
//	Console.WriteLine(t.Tag.Name);
//}

Console.WriteLine("done");
Console.ReadLine();

// Update
//Console.WriteLine("Updating the blog and adding a post");
//blog.Url = "https://devblogs.microsoft.com/dotnet";
//blog.Posts.Add(new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
//db.SaveChanges();

// Delete
//Console.WriteLine("Delete the blog");
//db.Remove(blog);
//db.SaveChanges();
