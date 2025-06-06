using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	public abstract class BaseServiceTestFixture
	{
		protected HttpClient? HttpClient;
		protected TestWebApplicationFactory<Program> _factory;

		[SetUp]
		public void SetUp()
		{
			_factory = new TestWebApplicationFactory<Program>();
			HttpClient = _factory.CreateClient();
		}

		[TearDown]
		public void TearDown()
		{
			HttpClient?.Dispose();
			_factory?.Dispose();
		}

		protected async Task SeedTestData(Action<LocoDbContext> seedAction)
		{
			using (var scope = _factory.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<LocoDbContext>();

				// Seed the specific data for the current test
				seedAction(dbContext);
				_ = await dbContext.SaveChangesAsync();
			}
		}
	}
}
