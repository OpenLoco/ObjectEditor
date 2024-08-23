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

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
	options.SerializerOptions.WriteIndented = false;
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	options.JsonSerializerOptions.WriteIndented = false;
});

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
	=> db.Objects.Select(x => new DtoObjectIndexEntry(x.TblLocoObjectId.ToString(), x.OriginalName, x.ObjectType, x.IsVanilla, x.OriginalChecksum, x.VehicleType)))
	.RequireRateLimiting(tokenPolicy);

// using db id
// eg: https://localhost:7230/objects/originaldat?uniqueObjectId=246263256
_ = app.MapGet("/objects/getobject", async (int uniqueObjectId, LocoDb db) =>
	{
		Console.WriteLine($"Object [{uniqueObjectId}] requested");
		var obj = await db.Objects.FindAsync(uniqueObjectId);
		if (obj == null)
		{
			return Results.NotFound();
		}

		var bytes = !obj.IsVanilla && File.Exists(obj.PathOnDisk) ? await File.ReadAllBytesAsync(obj.PathOnDisk) : null;

		return Results.Ok(new DtoLocoObject(
				obj.TblLocoObjectId,
				obj.Name,
				obj.OriginalName,
				obj.OriginalChecksum,
				bytes,
				obj.IsVanilla,
				obj.ObjectType,
				obj.VehicleType,
				obj.Description,
				obj.Author,
				obj.CreationDate,
				obj.LastEditDate,
				obj.UploadDate,
				obj.Tags,
				obj.Modpacks,
				obj.Availability,
				obj.Licence));
	})
	.RequireRateLimiting(tokenPolicy);

// using objectname+checksum
// eg: https://localhost:7230/objects/originaldat?objectName=114&checksum=123
_ = app.MapGet("/objects/getdat", async (string objectName, uint checksum, LocoDb db) =>
	{
		var obj = await db.Objects.SingleOrDefaultAsync(x => x.OriginalName == objectName && x.OriginalChecksum == checksum);
		return obj == null
		? Results.NotFound()
		: Results.Ok(obj);
	})
	.RequireRateLimiting(tokenPolicy);

_ = app.MapPost("/objects/adddat", async (DtoLocoObject locoObject, LocoDb db) =>
{
	if (locoObject.OriginalBytes == null)
	{
		return Results.BadRequest("OriginalBytes cannot be null - it must contain the valid bytes of a loco dat object.");
	}

	var obj = SawyerStreamReader.LoadAndDecodeFromStream(locoObject.OriginalBytes);
	if (obj == null || obj.Value.decodedData.Length == 0)
	{
		return Results.BadRequest("Provided byte data was unable to be decoded into a real loco dat object.");
	}

	if (db.DoesObjectExist(locoObject.OriginalName, locoObject.OriginalChecksum))
	{
		return Results.Accepted("Provided object already exists in the database.");
	}

	const string UploadFolder = "Q:\\Games\\Locomotion\\Server\\CustomObjects\\Uploaded";
	var uuid = Guid.NewGuid();
	var saveFileName = Path.Combine(UploadFolder, uuid.ToString(), ".dat");
	Console.WriteLine($"File accepted OriginalName={locoObject.OriginalName} OriginalChecksum={locoObject.OriginalChecksum} PathOnDisk={saveFileName}");

	var s5Header = obj.Value.s5Header;
	var objectHeader = obj.Value.objHeader;
	//var decodedData = obj.Value.decodedData;

	//ObjectIndex.AddObject(await SawyerStreamReader.GetDatFileInfoFromBytesAsync((saveFileName, locoObject.OriginalBytes)));

	var locoTbl = new TblLocoObject()
	{
		// for now, trust DtoLocoObj, but we could full-parse here
		Name = locoObject.Name,
		PathOnDisk = saveFileName,
		OriginalName = locoObject.OriginalName,
		OriginalChecksum = locoObject.OriginalChecksum,
		IsVanilla = locoObject.IsVanilla,
		ObjectType = locoObject.ObjectType,
		VehicleType = locoObject.VehicleType,
		Description = locoObject.Description,
		Author = locoObject.Author,
		CreationDate = locoObject.CreationDate,
		LastEditDate = locoObject.LastEditDate,
		UploadDate = DateTimeOffset.UtcNow,
		Tags = locoObject.Tags,
		Modpacks = locoObject.Modpacks,
		Availability = locoObject.Availability,
		Licence = locoObject.Licence
	};

	_ = db.Objects.Add(locoTbl);
	_ = await db.SaveChangesAsync();

	return Results.Created($"Successfully added {locoObject.Name} with unique id {locoTbl.TblLocoObjectId}", locoTbl.TblLocoObjectId);

}).RequireRateLimiting(tokenPolicy);

app.Run();
