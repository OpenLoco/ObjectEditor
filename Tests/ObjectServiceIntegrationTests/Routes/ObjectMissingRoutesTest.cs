using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ObjectService.Tests.Integration;

namespace Tests.ObjectServiceIntegrationTests.Routes;

[TestFixture]
public class ObjectMissingRoutesTest
	: BaseReferenceDataTableTestFixture<
		DtoObjectMissingEntry,
		DtoObjectMissingPost,
		DtoObjectMissingEntry,
		DtoObjectMissingEntry,
		DtoObjectMissingEntry,
		TblObjectMissing>
{
	public override string BaseRoute
	=> RoutesV2.Objects + RoutesV2.Missing;

	protected override DbSet<TblObjectMissing> GetTable(LocoDbContext db)
		=> db.ObjectsMissing;

	protected override TblObjectMissing ToRowFunc(DtoObjectMissingEntry request)
		=> request.ToTable();

	protected override DtoObjectMissingEntry ToDtoEntryFunc(TblObjectMissing row)
		=> row.ToDtoEntry();

	protected override IEnumerable<TblObjectMissing> DbSeedData =>
	[
		new() { Id = 1, DatName = "AIR1", DatChecksum = 123, ObjectType = ObjectType.Airport },
		new() { Id = 2, DatName = "WATER1", DatChecksum = 456, ObjectType = ObjectType.Water },
	];

	protected override DtoObjectMissingPost PostRequestDto
		=> new("TESTOBJ1", 123456789, ObjectType.Vehicle);

	protected override DtoObjectMissingEntry PostResponseDto
		=> new(3, "TESTOBJ1", 123456789, ObjectType.Vehicle);

	protected override DtoObjectMissingEntry PutRequestDto
		=> new(1, "TESTOBJ2", 123456788, ObjectType.StreetLight);

	protected override DtoObjectMissingEntry PutResponseDto
		=> new(1, "TESTOBJ2", 123456788, ObjectType.StreetLight);

	[Test]
	public async Task MissingObjectsRoute_IsNotMapped_WhenHostIsLocal()
	{
		using var localFactory = new TestWebApplicationFactory<Program>(isServer: false);
		_ = localFactory.CreateClient();

		var endpoints = localFactory.Services.GetRequiredService<EndpointDataSource>().Endpoints;
		var routePatterns = endpoints
			.OfType<RouteEndpoint>()
			.Select(x => x.RoutePattern.RawText);

		Assert.That(routePatterns, Does.Not.Contain(RoutesV2.Prefix + RoutesV2.Objects + RoutesV2.Missing));
		await Task.CompletedTask;
	}
}
