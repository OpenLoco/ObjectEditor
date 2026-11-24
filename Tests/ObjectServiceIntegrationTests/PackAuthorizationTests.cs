using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Confirms the authorization policies that gate the pack write endpoints are registered with the
/// running application. Without these, pack POST/PUT/DELETE would only require an authenticated user.
/// </summary>
[TestFixture]
public class PackAuthorizationTests
{
	TestWebApplicationFactory<Program>? factory;

	[SetUp]
	public void SetUp() => factory = new TestWebApplicationFactory<Program>();

	[TearDown]
	public void TearDown() => factory?.Dispose();

	[TestCase("CanCreateObjectPacks")]
	[TestCase("CanModifyObjectPacks")]
	[TestCase("CanModifyScenarioPacks")]
	[TestCase("Curator")]
	[TestCase("CanEditObject")]
	public async Task Policy_IsRegistered(string policyName)
	{
		var policyProvider = factory!.Services.GetRequiredService<IAuthorizationPolicyProvider>();

		var policy = await policyProvider.GetPolicyAsync(policyName);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(policy, Is.Not.Null);
			Assert.That(policy!.Requirements, Is.Not.Empty);
		}
	}
}