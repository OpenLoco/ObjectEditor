using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.FileParsing;
using OpenLoco.ObjectService;
using OpenLoco.Schema.Database;
using OpenLoco.Schema.Server;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocoDb>(opt => opt.UseSqlite(LocoDb.GetDbPath()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpLogging(logging => logging.LoggingFields = HttpLoggingFields.All);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
//builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

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
			_ = context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);

			return new ValueTask();
		};
	}));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();
//app.UseAuthentication();
app.UseHttpLogging();
app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
}

// eg: https://localhost:7230/objects/list
_ = app.MapGet("/objects/list", (LocoDb db)
	=> db.Objects.Select(x => new ObjectIndexEntry(x.TblLocoObjectId.ToString(), x.OriginalName, x.ObjectType, x.SourceGame, x.OriginalChecksum, x.VehicleType)))
	.RequireRateLimiting(tokenPolicy);

// using db id
// eg: https://localhost:7230/objects/originaldat?uniqueObjectId=246263256
_ = app.MapGet("/objects/getobject", async (int uniqueObjectId, LocoDb db) =>
	{
		Console.WriteLine($"Object [{uniqueObjectId}] requested");
		var obj = await db.Objects.FindAsync(uniqueObjectId);
		return obj == null
			? Results.NotFound()
			: new TblLocoObjectDTO()
			{
				TblLocoObjectId = obj.TblLocoObjectId,
				Name = obj.Name,

				// OriginalDatdata
				OriginalName = obj.OriginalName,
				OriginalChecksum = obj.OriginalChecksum,
				OriginalBytes = null, //File.Exists(obj.PathOnDisk) ? await File.ReadAllBytesAsync(obj.PathOnDisk) : null,

				SourceGame = obj.SourceGame,
				ObjectType = obj.ObjectType,
				VehicleType = obj.VehicleType,

				// Metadata
				Description = obj.Description,
				Author = obj.Author,
				CreationDate = obj.CreationDate,
				LastEditDate = obj.LastEditDate,
				Tags = obj.Tags,
				Modpacks = obj.Modpacks,
				Availability = obj.Availability,
				Licence = obj.Licence
			};
	})
	.RequireRateLimiting(tokenPolicy);

// using objectname+checksum
// eg: https://localhost:7230/objects/originaldat?objectName=114&checksum=123
_ = app.MapGet("/objects/getdat", async (string objectName, uint checksum, LocoDb db)
	=> await db.Objects.SingleAsync(x => x.OriginalName == objectName && x.OriginalChecksum == checksum))
	.RequireRateLimiting(tokenPolicy);

// default
app.MapGet("/", () => "This is a GET");
app.MapPost("/", () => "This is a POST");
app.MapPut("/", () => "This is a PUT");
app.MapDelete("/", () => "This is a DELETE");

//_ = app.MapPost("/todoitems", async (TblLocoObject locoObject, LocoDb db) =>
//{
//	_ = db.Objects.Add(locoObject);
//	_ = await db.SaveChangesAsync();

//	return Results.Created($"/todoitems/{locoObject.Name}", locoObject);
//});

app.Run();
