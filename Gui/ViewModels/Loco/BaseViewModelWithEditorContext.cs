using Gui.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Gui.ViewModels;

public abstract class BaseViewModelWithEditorContext<T>(ObjectEditorContext editorContext, T? model) : BaseViewModel<T>(model) where T : class
{
	[Browsable(false)]
	public ObjectEditorContext EditorContext { get; init; } = editorContext;

	protected ILogger Logger
		=> EditorContext.Logger;
}
