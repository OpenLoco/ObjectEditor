using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Db.Schema;
using OpenLoco.ObjectService;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocoDb>(opt => opt.UseSqlite(LocoDb.GetDbPath()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpLogging(logging => logging.LoggingFields = HttpLoggingFields.All);

var tokenPolicy = "token";
var myOptions = new ObjectServiceRateLimitOptions();
builder.Configuration.GetSection(ObjectServiceRateLimitOptions.MyRateLimit).Bind(myOptions);

builder.Services.AddRateLimiter(rlOptions => rlOptions
	.AddTokenBucketLimiter(policyName: tokenPolicy, options =>
	{
		options.TokenLimit = myOptions.TokenLimit;
		options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		options.QueueLimit = myOptions.QueueLimit;
		options.ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod);
		options.TokensPerPeriod = myOptions.TokensReplenishedPerPeriod;
		options.AutoReplenishment = myOptions.AutoReplenishment;
		rlOptions.OnRejected = (context, cancellationToken) =>
		{
			if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
			{
				context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
			}

			context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
			context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

			return new ValueTask();
		};
	}));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();
app.UseHttpLogging();
app.UseRateLimiter();

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
	=> await db.Objects.FindAsync(uniqueObjectId))
	.RequireRateLimiting(tokenPolicy);

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
