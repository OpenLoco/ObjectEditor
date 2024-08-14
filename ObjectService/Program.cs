using Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocoDb>(opt => opt.UseSqlite(LocoDb.GetDbPath()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	app.UseSwagger();
	app.UseSwaggerUI();
}

// eg: https://localhost:7230/objects/list
app.MapGet("/objects/list/", () =>
{
	const string objFolderPath = @"Q:\Games\Locomotion\OriginalObjects";
	var datFiles = Directory
		.GetFileSystemEntries(objFolderPath)
		.Where(x => Path.GetExtension(x.ToLower()).Equals(".dat", StringComparison.OrdinalIgnoreCase))
		.Select(x => Path.GetFileNameWithoutExtension(x));

	return datFiles;
});

// eg: https://localhost:7230/objects/originaldat/114
app.MapGet("/objects/originaldat/{uniqueObjectId}", (string uniqueObjectId) =>
{
	const string objFolderPath = @"Q:\Games\Locomotion\OriginalObjects";
	var objFilename = Path.Combine(objFolderPath, $"{uniqueObjectId}.dat");
	if (File.Exists(objFilename))
	{
		var bytes = File.ReadAllBytes(objFilename);
		return Convert.ToBase64String(bytes);
	}
	return null;
});

app.MapGet("/objects/newobjectformat/{uniqueObjectId}", (string uniqueObjectId) =>
{

});

app.MapGet("/objects/metadata/{uniqueObjectId}", (string uniqueObjectId) =>
{

});

app.MapGet("/todoitems", async (LocoDb db) =>
	await db.Objects.ToListAsync());

app.MapPost("/todoitems", async (TblLocoObject locoObject, LocoDb db) =>
{
	db.Objects.Add(locoObject);
	await db.SaveChangesAsync();

	return Results.Created($"/todoitems/{locoObject.Name}", locoObject);
});

app.Run();
