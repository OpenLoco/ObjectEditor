using Gui.ViewModels;
using NUnit.Framework;

namespace Dat.Tests;

[TestFixture]
public class NewObjectWizardViewModelTests
{
	[Test]
	public void Wizard_StartsOnTheTypeStep()
	{
		var wizard = new NewObjectWizardViewModel();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(wizard.StepIndex, Is.EqualTo(NewObjectWizardViewModel.TypeStepIndex));
			Assert.That(wizard.IsTypeStep, Is.True);
			Assert.That(wizard.IsFirstStep, Is.True);
			Assert.That(wizard.IsLastStep, Is.False);
			Assert.That(wizard.Confirmed, Is.False);
		}
	}

	[Test]
	public void Wizard_NameValidationRejectsEmptyLongAndNonAsciiNames()
	{
		var wizard = new NewObjectWizardViewModel { ObjectName = string.Empty };
		Assert.That(wizard.HasNameError, Is.True);

		wizard.ObjectName = "TOOLONGNAME";
		Assert.That(wizard.NameError, Is.Not.Null);

		wizard.ObjectName = "BADDÉ";
		Assert.That(wizard.NameError, Is.Not.Null);

		wizard.ObjectName = "NEWOBJ";
		Assert.That(wizard.NameError, Is.Null);
		Assert.That(wizard.HasNameError, Is.False);
	}

	[Test]
	public void Wizard_NextAndBackMoveThroughTheSteps()
	{
		var wizard = new NewObjectWizardViewModel { ObjectName = "NEWOBJ" };

		wizard.NextCommand.Execute().Subscribe(_ => { });
		Assert.That(wizard.IsDetailsStep, Is.True);
		Assert.That(wizard.IsFirstStep, Is.False);

		wizard.NextCommand.Execute().Subscribe(_ => { });
		Assert.That(wizard.IsReviewStep, Is.True);
		Assert.That(wizard.IsLastStep, Is.True);

		wizard.BackCommand.Execute().Subscribe(_ => { });
		Assert.That(wizard.IsDetailsStep, Is.True);
	}

	[Test]
	public void Wizard_CreateConfirmsTheWizard()
	{
		var wizard = new NewObjectWizardViewModel { ObjectName = "NEWOBJ" };

		wizard.CreateCommand.Execute().Subscribe(_ => { });

		Assert.That(wizard.Confirmed, Is.True);
	}

	[Test]
	public void Wizard_DisplayNameFallsBackToTheInternalName()
	{
		var wizard = new NewObjectWizardViewModel { ObjectName = "  NEWOBJ  " };
		Assert.That(wizard.EffectiveDisplayName, Is.EqualTo("NEWOBJ"));

		wizard.DisplayName = "My New Object";
		Assert.That(wizard.EffectiveDisplayName, Is.EqualTo("My New Object"));
	}
}
