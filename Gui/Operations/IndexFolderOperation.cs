using Gui.Models;
using Gui.Models.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Operations;

/// <summary>
/// Indexes a local folder of .dat objects by delegating to
/// <see cref="ObjectEditorContext.LoadObjDirectoryAsync"/>. Reports fractional progress
/// from the underlying indexer (which uses <see cref="IProgress{Single}"/>) through the
/// queue's progress channel.
/// </summary>
public sealed class IndexFolderOperation(
    ObjectEditorContext context,
    string directory,
    bool useExistingIndex,
    IProgress<float>? extraReceiver = null) : IOperation
{
    public string Title => $"Indexing folder";
    public string? InitialStatus => directory;
    public bool SupportsCancellation => false;

    public Task ExecuteAsync(IProgress<ProgressReport> progress, CancellationToken ct)
    {
        // The indexer expects IProgress<float>; the queue's reporter implements both interfaces,
        // but we receive the strongly-typed IProgress<ProgressReport>. Adapt and optionally tee
        // to the caller's existing receiver (e.g. an inline ProgressBar binding).
        IProgress<float> primary = new FloatAdapter(progress);
        IProgress<float> sink = extraReceiver == null
            ? primary
            : new TeeProgress(primary, extraReceiver);
        return context.LoadObjDirectoryAsync(directory, sink, useExistingIndex);
    }

    sealed class FloatAdapter(IProgress<ProgressReport> inner) : IProgress<float>
    {
        public void Report(float value) => inner.Report(new ProgressReport(value));
    }
}
