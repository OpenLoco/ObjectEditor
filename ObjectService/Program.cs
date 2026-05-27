using ObjectService.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

ObjectServiceHost.ConfigureBuilder(builder);

var app = builder.Build();

ObjectServiceHost.Configure(app);

app.Run();

#pragma warning disable CA1050 // Declare types in namespaces

// this is to enable unit testing in a top-level statement program
public partial class Program;

#pragma warning restore CA1050 // Declare types in namespaces
