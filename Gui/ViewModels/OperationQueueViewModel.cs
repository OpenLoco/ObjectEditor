using Avalonia.Threading;
using Gui.Models;
using Gui.Models.Operations;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Gui.ViewModels;

/// <summary>
/// Central operation queue exposed to the UI. Runs every enqueued operation concurrently
/// (callers serialize work explicitly when needed) and surfaces them in a non-modal panel
/// at the bottom of the main window. Completed operations briefly remain visible so the
/// user can see they finished, then auto-disappear; cancelled or failed ones stay until
/// dismissed.
/// </summary>
public sealed class OperationQueueViewModel : ViewModelBase, IOperationQueue
{
    /// <summary>How long a successful operation stays visible after completing.</summary>
    public TimeSpan CompletedRetentionDuration { get; init; } = TimeSpan.FromSeconds(2);

    public ObservableCollection<OperationHandle> Operations { get; } = [];

    public OperationHandle Enqueue(IOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var handle = new OperationHandle(operation);
        Dispatcher.UIThread.Post(() => Operations.Add(handle));

        var progress = new HandleProgressReporter(handle);
        handle.Completion = RunAsync(handle, progress);
        return handle;
    }

    async Task RunAsync(OperationHandle handle, HandleProgressReporter progress)
    {
        // Wait until the dispatcher has drained everything down to Background priority — that
        // guarantees the Operations.Add Post enqueued by Enqueue has been applied AND a render
        // pass has happened, so the row is visible before any (potentially long-running)
        // synchronous work in ExecuteAsync starts pinning the UI thread.
        await Dispatcher.UIThread.InvokeAsync(static () => { }, DispatcherPriority.Background);

        OperationStatus finalStatus;
        SetStatus(handle, OperationStatus.Running);
        try
        {
            await handle.Operation.ExecuteAsync(progress, handle.CancellationToken).ConfigureAwait(false);
            finalStatus = handle.CancellationToken.IsCancellationRequested ? OperationStatus.Cancelled : OperationStatus.Completed;
        }
        catch (OperationCanceledException)
        {
            finalStatus = OperationStatus.Cancelled;
        }
        catch (Exception ex)
        {
            Dispatcher.UIThread.Post(() => handle.ErrorMessage = ex.Message);
            finalStatus = OperationStatus.Failed;
        }

        SetStatus(handle, finalStatus);
        ScheduleAutoRemoval(handle, finalStatus);
    }

    void ScheduleAutoRemoval(OperationHandle handle, OperationStatus finalStatus)
    {
        if (finalStatus != OperationStatus.Completed)
        {
            // keep failed/cancelled visible so the user can dismiss manually
            return;
        }

        _ = Task.Delay(CompletedRetentionDuration).ContinueWith(t =>
            Dispatcher.UIThread.Post(() =>
            {
                _ = Operations.Remove(handle);
                handle.DisposeCts();
            }));
    }

    /// <summary>Manually dismiss a finished operation (used by the X button on cancelled/failed entries).</summary>
    public void Remove(OperationHandle handle)
    {
        if (handle.Status is OperationStatus.Pending or OperationStatus.Running)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            _ = Operations.Remove(handle);
            handle.DisposeCts();
        });
    }

    static void SetStatus(OperationHandle handle, OperationStatus status)
        => Dispatcher.UIThread.Post(() => handle.Status = status);

    sealed class HandleProgressReporter(OperationHandle handle) : IProgress<ProgressReport>, IProgress<float>
    {
        public void Report(ProgressReport value)
            => Dispatcher.UIThread.Post(() =>
            {
                if (!double.IsNaN(value.Value) && value.Value >= 0)
                {
                    handle.Progress = Math.Clamp(value.Value * 100.0, 0, 100);
                    handle.IsIndeterminate = false;
                }

                if (value.Message != null)
                {
                    handle.StatusMessage = value.Message;
                }
            });

        public void Report(float value) => Report(new ProgressReport(value));
    }
}
