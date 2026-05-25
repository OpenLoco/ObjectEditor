using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Models.Operations;

/// <summary>
/// Live, bindable handle for a single enqueued operation. Created by the queue when an
/// <see cref="IOperation"/> is enqueued; the queue mutates the reactive properties as the
/// operation progresses so the UI updates without callers ever touching UI code.
/// </summary>
public sealed class OperationHandle : ReactiveObject
{
    readonly CancellationTokenSource cts;

    public OperationHandle(IOperation operation)
    {
        Operation = operation;
        Title = operation.Title;
        StatusMessage = operation.InitialStatus ?? string.Empty;
        cts = new CancellationTokenSource();

        // SupportsCancellation is fixed for the lifetime of the operation, so we don't observe it.
        IObservable<bool> canCancel = operation.SupportsCancellation
            ? this.WhenAnyValue(x => x.Status).Select(s => s is OperationStatus.Pending or OperationStatus.Running)
            : Observable.Return(false);
        CancelCommand = ReactiveCommand.Create(Cancel, canCancel);
    }

    public IOperation Operation { get; }
    public string Title { get; }
    public CancellationToken CancellationToken => cts.Token;
    public Task Completion { get; internal set; } = Task.CompletedTask;

    [Reactive] public OperationStatus Status { get; internal set; } = OperationStatus.Pending;
    [Reactive] public double Progress { get; internal set; }            // 0..100
    [Reactive] public bool IsIndeterminate { get; internal set; } = true;
    [Reactive] public string StatusMessage { get; internal set; }
    [Reactive] public string? ErrorMessage { get; internal set; }

    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    void Cancel()
    {
        try { cts.Cancel(); }
        catch (ObjectDisposedException) { /* already finished */ }
    }

    internal void DisposeCts()
    {
        try { cts.Dispose(); }
        catch { /* ignore */ }
    }
}
