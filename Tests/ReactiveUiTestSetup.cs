using NUnit.Framework;
using ReactiveUI.Builder;

/// <summary>
/// Initialises ReactiveUI once for the whole test assembly. Needed by any test that constructs a
/// ReactiveUI view-model (for example the New Object wizard), because <c>WhenAnyValue</c> requires
/// ReactiveUI's services to be registered first.
/// </summary>
[SetUpFixture]
public class ReactiveUiTestSetup
{
	[OneTimeSetUp]
	public void InitialiseReactiveUi()
		=> RxAppBuilder.CreateReactiveUIBuilder()
			.WithCoreServices()
			.BuildApp();
}
