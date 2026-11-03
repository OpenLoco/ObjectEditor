using Dat.Data;
using Dat.Templates;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace Dat.Tests;

[TestFixture]
public class NewObjectTemplateFactoryTests
{
	static IEnumerable<TestCaseData> AllTypes()
		=> NewObjectTemplateFactory.SupportedTypes.Select(t => new TestCaseData(t).SetName($"{{m}}({t})"));

	[TestCaseSource(nameof(AllTypes))]
	public void CreateModel_ReturnsLocoStructForEverySupportedType(ObjectType objectType)
	{
		var model = NewObjectTemplateFactory.CreateModel(objectType);

		Assert.That(model, Is.Not.Null);
		Assert.That(model, Is.AssignableTo<ILocoStruct>());
	}

	[TestCaseSource(nameof(AllTypes))]
	public void CreateModel_DefaultsPassObjectValidation(ObjectType objectType)
	{
		var model = NewObjectTemplateFactory.CreateModel(objectType);

		var errors = model.Validate(new ValidationContext(model)).Select(x => x.ToString()).ToList();

		Assert.That(errors, Is.Empty, $"Default template for {objectType} failed validation: {string.Join("; ", errors)}");
	}

	[TestCaseSource(nameof(AllTypes))]
	public void CreateModel_DefaultsPassDataAnnotations(ObjectType objectType)
	{
		// Writing a DAT runs Validator.TryValidateObject, so the defaults must satisfy the attributes.
		var model = NewObjectTemplateFactory.CreateModel(objectType);
		var results = new List<ValidationResult>();

		var isValid = Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);

		Assert.That(isValid, Is.True, $"Default template for {objectType} failed DataAnnotations: {string.Join("; ", results.Select(r => r.ErrorMessage))}");
	}

	[Test]
	public void Create_BuildsHeadersAndMediaTables()
	{
		var template = NewObjectTemplateFactory.Create("NEWTRACK", ObjectType.Track, ObjectSource.Custom, SawyerEncoding.RunLengthSingle);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(template.DatInfo.S5Header.Name, Is.EqualTo("NEWTRACK"));
			Assert.That(template.DatInfo.S5Header.ObjectType, Is.EqualTo(DatObjectType.Track));
			Assert.That(template.DatInfo.S5Header.ObjectSource, Is.EqualTo(DatObjectSource.Custom));
			Assert.That(template.DatInfo.ObjectHeader.Encoding, Is.EqualTo(SawyerEncoding.RunLengthSingle));

			Assert.That(template.LocoObject.ObjectType, Is.EqualTo(ObjectType.Track));
			Assert.That(template.LocoObject.Object, Is.InstanceOf<TrackObject>());

			Assert.That(template.LocoObject.StringTable.Table.Keys, Does.Contain("Name"));

			Assert.That(template.LocoObject.ImageTable, Is.Not.Null);
			Assert.That(template.LocoObject.ImageTable!.Groups, Has.Count.EqualTo(1));
			Assert.That(template.LocoObject.ImageTable.Groups[0].GraphicsElements, Is.Empty);
		}
	}

	[Test]
	public void Create_UsesEveryRequiredStringTableKeyForTheType()
	{
		var template = NewObjectTemplateFactory.Create("NEWCARGO", ObjectType.Cargo, ObjectSource.Custom, SawyerEncoding.Uncompressed);

		Assert.That(
			template.LocoObject.StringTable.Table.Keys,
			Is.EquivalentTo(new[] { "Name", "UnitsAndCargoName", "UnitNameSingular", "UnitNamePlural" }));
	}

	[Test]
	public void Create_EmptyNameIsRejected()
		=> Assert.That(() => NewObjectTemplateFactory.Create("   ", ObjectType.Track, ObjectSource.Custom, SawyerEncoding.Uncompressed), Throws.ArgumentException);

	[Test]
	public void SupportedTypes_CoversTheWholeObjectTypeEnum()
		=> Assert.That(NewObjectTemplateFactory.SupportedTypes, Is.EquivalentTo(Enum.GetValues<ObjectType>()));
}
