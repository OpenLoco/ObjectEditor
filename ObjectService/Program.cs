using Database;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using OpenLoco.ObjectEditor;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Types;
using Schema;
using Shared;

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
_ = app.MapGet("/objects/list/", (LocoDb db)
	=> db.Objects.Select(x => new ObjectIndex(x.Name, x.OriginalName, x.ObjectType, x.SourceGame, x.OriginalChecksum, x.VehicleType)));

// eg: https://localhost:7230/objects/originaldat/114
_ = app.MapGet("/objects/originaldat/{uniqueObjectId}", async (string uniqueObjectId, LocoDb db)
	=> await db.Objects.FindAsync(uniqueObjectId));

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
