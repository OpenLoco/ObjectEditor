using Gui.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Gui.ViewModels;

public abstract class BaseViewModelWithEditorContext<T> : BaseViewModel<T> where T : class
{
	protected BaseViewModelWithEditorContext(ObjectEditorContext editorContext, T? model)
		: base(model)
		=> EditorContext = editorContext;

	[Browsable(false)]
	public ObjectEditorContext EditorContext { get; init; }

	protected ILogger Logger
		=> EditorContext.Logger;
}
