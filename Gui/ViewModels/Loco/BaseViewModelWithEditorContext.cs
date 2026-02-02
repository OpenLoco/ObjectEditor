using Common.Logging;
using Gui.Models;
using System.ComponentModel;

namespace Gui.ViewModels;

public abstract class BaseViewModelWithEditorContext<T> : BaseViewModel<T> where T : class
{
	protected BaseViewModelWithEditorContext(ObjectEditorContext editorContext, T? model = default)
		: base(model)
		=> EditorContext = editorContext;

	[Browsable(false)]
	public ObjectEditorContext EditorContext { get; init; }

	protected ILogger logger
		=> EditorContext.Logger;
}
