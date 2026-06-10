using Gui.Models;
using Gui.Models.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Operations;

/// <summary>
/// Generic <see cref="IOperation"/> backed by a delegate. Useful when the body of the
/// operation is tightly coupled to existing view-model state and extracting a dedicated
/// class would add boilerplate without making the intent any clearer. Still a real class
/// (constructed at the call site with all required dependencies captured) so the central
/// queue can run, track, cancel and surface it like any other operation.
/// </summary>
public sealed class DelegateOperation(
    string title,
    Func<IProgress<ProgressReport>, CancellationToken, Task> work,
    string? initialStatus = null,
    bool supportsCancellation = true) : IOperation
{
    public string Title { get; } = title;
    public string? InitialStatus { get; } = initialStatus;
    public bool SupportsCancellation { get; } = supportsCancellation;

    public Task ExecuteAsync(IProgress<ProgressReport> progress, CancellationToken ct) => work(progress, ct);
}
