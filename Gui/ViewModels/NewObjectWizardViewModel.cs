using Dat.Data;
using Dat.Templates;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Gui.ViewModels;

/// <summary>
/// Backing view-model for the "New Object" wizard. The wizard walks the user through three steps -
/// picking an object type, filling in the object details, and reviewing - and then reports the chosen
/// values via <see cref="Confirmed"/> so the caller can build a template object.
/// </summary>
public class NewObjectWizardViewModel : ViewModelBase
{
	public const int TypeStepIndex = 0;
	public const int DetailsStepIndex = 1;
	public const int ReviewStepIndex = 2;
	public const int LastStepIndex = ReviewStepIndex;

	/// <summary>All object types a template can be created for.</summary>
	public IReadOnlyList<ObjectType> ObjectTypes { get; } = NewObjectTemplateFactory.SupportedTypes;

	/// <summary>Sources the user can pick. Vanilla sources are intentionally excluded - users create custom objects.</summary>
	public IReadOnlyList<ObjectSource> ObjectSources { get; } = [ObjectSource.Custom, ObjectSource.OpenLoco];

	/// <summary>The DAT encodings a new object can be saved with.</summary>
	public IReadOnlyList<SawyerEncoding> Encodings { get; } = Enum.GetValues<SawyerEncoding>();

	[Reactive] public int StepIndex { get; set; }

	[Reactive] public ObjectType SelectedObjectType { get; set; } = ObjectType.Vehicle;

	[Reactive] public string ObjectName { get; set; } = string.Empty;

	[Reactive] public string DisplayName { get; set; } = string.Empty;

	[Reactive] public ObjectSource SelectedObjectSource { get; set; } = ObjectSource.Custom;

	[Reactive] public SawyerEncoding SelectedEncoding { get; set; } = SawyerEncoding.RunLengthSingle;

	/// <summary>True when the user pressed "Create"; false (or never set) if the wizard was cancelled.</summary>
	[Reactive] public bool Confirmed { get; private set; }

	readonly ObservableAsPropertyHelper<string?> nameError;
	readonly ObservableAsPropertyHelper<bool> hasNameError;
	readonly ObservableAsPropertyHelper<bool> isTypeStep;
	readonly ObservableAsPropertyHelper<bool> isDetailsStep;
	readonly ObservableAsPropertyHelper<bool> isReviewStep;
	readonly ObservableAsPropertyHelper<bool> isFirstStep;
	readonly ObservableAsPropertyHelper<bool> isLastStep;
	readonly ObservableAsPropertyHelper<string> stepTitle;
	readonly ObservableAsPropertyHelper<string> effectiveDisplayName;
	readonly ObservableAsPropertyHelper<string> selectedObjectTypeText;
	readonly ObservableAsPropertyHelper<string> selectedObjectSourceText;
	readonly ObservableAsPropertyHelper<string> selectedEncodingText;

	/// <summary>Human-readable reason the current name is not acceptable, or null when it is fine.</summary>
	public string? NameError => nameError.Value;

	/// <summary>True when <see cref="NameError"/> is set; convenience for binding IsVisible.</summary>
	public bool HasNameError => hasNameError.Value;

	public bool IsTypeStep => isTypeStep.Value;
	public bool IsDetailsStep => isDetailsStep.Value;
	public bool IsReviewStep => isReviewStep.Value;
	public bool IsFirstStep => isFirstStep.Value;
	public bool IsLastStep => isLastStep.Value;

	/// <summary>A short title describing the current step.</summary>
	public string StepTitle => stepTitle.Value;

	/// <summary>The display name the new object will use, falling back to the internal name.</summary>
	public string EffectiveDisplayName => effectiveDisplayName.Value;

	/// <summary>Summaries of the current selections for the review step (enums bound to text).</summary>
	public string SelectedObjectTypeText => selectedObjectTypeText.Value;
	public string SelectedObjectSourceText => selectedObjectSourceText.Value;
	public string SelectedEncodingText => selectedEncodingText.Value;

	public ReactiveCommand<Unit, Unit> NextCommand { get; }
	public ReactiveCommand<Unit, Unit> BackCommand { get; }
	public ReactiveCommand<Unit, Unit> CreateCommand { get; }
	public ReactiveCommand<Unit, Unit> CancelCommand { get; }

	public NewObjectWizardViewModel()
	{
		nameError = this.WhenAnyValue(o => o.ObjectName)
			.Select(ValidateName)
			.ToProperty(this, o => o.NameError);

		hasNameError = this.WhenAnyValue(o => o.NameError)
			.Select(error => error is not null)
			.ToProperty(this, o => o.HasNameError);

		var stepIndex = this.WhenAnyValue(o => o.StepIndex);
		isTypeStep = stepIndex.Select(step => step == TypeStepIndex).ToProperty(this, o => o.IsTypeStep);
		isDetailsStep = stepIndex.Select(step => step == DetailsStepIndex).ToProperty(this, o => o.IsDetailsStep);
		isReviewStep = stepIndex.Select(step => step == ReviewStepIndex).ToProperty(this, o => o.IsReviewStep);
		isFirstStep = stepIndex.Select(step => step == TypeStepIndex).ToProperty(this, o => o.IsFirstStep);
		isLastStep = stepIndex.Select(step => step == LastStepIndex).ToProperty(this, o => o.IsLastStep);
		stepTitle = stepIndex.Select(step => step switch
		{
			TypeStepIndex => "Step 1 of 3: Object type",
			DetailsStepIndex => "Step 2 of 3: Details",
			_ => "Step 3 of 3: Review",
		}).ToProperty(this, o => o.StepTitle);

		selectedObjectTypeText = this.WhenAnyValue(o => o.SelectedObjectType)
			.Select(type => type.ToString())
			.ToProperty(this, o => o.SelectedObjectTypeText);
		selectedObjectSourceText = this.WhenAnyValue(o => o.SelectedObjectSource)
			.Select(source => source.ToString())
			.ToProperty(this, o => o.SelectedObjectSourceText);
		selectedEncodingText = this.WhenAnyValue(o => o.SelectedEncoding)
			.Select(encoding => encoding.ToString())
			.ToProperty(this, o => o.SelectedEncodingText);
		effectiveDisplayName = this.WhenAnyValue(o => o.ObjectName, o => o.DisplayName,
				(name, display) => string.IsNullOrWhiteSpace(display) ? name.Trim() : display.Trim())
			.ToProperty(this, o => o.EffectiveDisplayName);

		var canGoNext = this.WhenAnyValue(o => o.StepIndex, o => o.NameError, (step, error)
			=> step == TypeStepIndex || (step == DetailsStepIndex && error is null));
		NextCommand = ReactiveCommand.Create(Next, canGoNext);

		var canGoBack = this.WhenAnyValue(o => o.StepIndex, step => step > TypeStepIndex);
		BackCommand = ReactiveCommand.Create(Back, canGoBack);

		var canCreate = this.WhenAnyValue(o => o.StepIndex, o => o.NameError, (step, error)
			=> step == ReviewStepIndex && error is null);
		CreateCommand = ReactiveCommand.Create(() => { Confirmed = true; }, canCreate);

		CancelCommand = ReactiveCommand.Create(() => { Confirmed = false; });
	}

	void Next()
		=> StepIndex = Math.Min(LastStepIndex, StepIndex + 1);

	void Back()
		=> StepIndex = Math.Max(TypeStepIndex, StepIndex - 1);

	/// <summary>A DAT object name is at most 8 ASCII characters and must not be empty.</summary>
	internal static string? ValidateName(string? name)
	{
		var trimmed = name?.Trim();
		if (string.IsNullOrEmpty(trimmed))
		{
			return "An object name is required.";
		}

		if (trimmed.Length > 8)
		{
			return "The object name must be 8 characters or fewer.";
		}

		if (trimmed.Any(c => c > 127))
		{
			return "The object name must contain only ASCII characters.";
		}

		return null;
	}
}
