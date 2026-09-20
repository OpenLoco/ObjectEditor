using System.Reflection;
using NUnit.Framework;
using ObjectService.Identity;

namespace Tests;

/// <summary>
/// Guards the permission model invariant: a permission is only useful if it is both defined as a
/// constant and listed in <see cref="LocoPermissions.All"/> (so the user-management UI can toggle it)
/// and <em>enforced</em> by a policy or page check.
/// </summary>
[TestFixture]
public class LocoPermissionsTests
{
	static IReadOnlyList<string> PermissionConstants => [.. typeof(LocoPermissions)
		.GetFields(BindingFlags.Public | BindingFlags.Static)
		.Where(f => f.IsLiteral && f.FieldType == typeof(string))
		.Select(f => (string)f.GetRawConstantValue()!)
		.Where(value => value != LocoPermissions.ClaimType)];

	[Test]
	public void All_ContainsEveryPermissionConstant()
	{
		Assert.That(PermissionConstants, Is.Not.Empty);
		Assert.That(LocoPermissions.All, Is.SupersetOf(PermissionConstants));
	}

	[Test]
	public void All_HasNoDuplicates()
	{
		Assert.That(LocoPermissions.All, Is.Unique);
	}

	[Test]
	public void Curator_IsSubsetOfAll()
	{
		Assert.That(LocoPermissions.All, Is.SupersetOf(LocoPermissions.Curator));
	}

	[Test]
	public void ClaimType_IsNotAPermission()
	{
		Assert.That(LocoPermissions.All, Does.Not.Contain(LocoPermissions.ClaimType));
	}
}