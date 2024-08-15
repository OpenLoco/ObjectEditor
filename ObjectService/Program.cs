using Database;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using OpenLoco.ObjectEditor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocoDb>(opt => opt.UseSqlite(LocoDb.GetDbPath()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpLogging(logging => logging.LoggingFields = HttpLoggingFields.All);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();
app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	app.UseSwagger();
	app.UseSwaggerUI();
}

// eg: https://localhost:7230/objects/list
_ = app.MapGet("/objects/list/", () =>
{
	const string objFolderPath = @"Q:\Games\Locomotion\OriginalObjects";
	const string objIndex = @"objectIndex.json";
	var index = ObjectIndexManager.DeserialiseHeaderIndexFromFile(Path.Combine(objFolderPath, objIndex)) ?? []; // todo: currently this loads every time - lets cache it
	return index.Values.Select(x => x with { Filename = "<online>" }); // make sure we don't expose server filepaths to clients...
});

// eg: https://localhost:7230/objects/originaldat/114
_ = app.MapGet("/objects/originaldat/{uniqueObjectId}", (string uniqueObjectId) =>
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

_ = app.MapGet("/objects/newobjectformat/{uniqueObjectId}", (string uniqueObjectId) =>
{

});

_ = app.MapGet("/objects/metadata/{uniqueObjectId}", (string uniqueObjectId) =>
{

});

_ = app.MapGet("/todoitems", async (LocoDb db) =>
	await db.Objects.ToListAsync());

_ = app.MapPost("/todoitems", async (TblLocoObject locoObject, LocoDb db) =>
{
	db.Objects.Add(locoObject);
	await db.SaveChangesAsync();

	return Results.Created($"/todoitems/{locoObject.Name}", locoObject);
});

app.Run();
