using Avalonia.Threading;
using Gui.Models;
using Gui.Models.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Operations;

/// <summary>
/// Runs a synchronous action on the UI thread at <see cref="DispatcherPriority.Background"/>.
/// Used for work that mutates UI-bound observable collections (e.g. <c>BaseFileViewModel.Load</c>),
/// where running on a background thread isn't safe but we still want the queue panel to render
/// before the dispatcher gets tied up.
/// </summary>
public sealed class UiThreadOperation(string title, Action action, string? initialStatus = null) : IOperation
{
    public string Title { get; } = title;
    public string? InitialStatus { get; } = initialStatus;

    /// <summary>The wrapped action runs synchronously on the UI thread, so it cannot be cancelled mid-flight.</summary>
    public bool SupportsCancellation => false;

    public Task ExecuteAsync(IProgress<ProgressReport> progress, CancellationToken ct)
        => Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background).GetTask();
}
