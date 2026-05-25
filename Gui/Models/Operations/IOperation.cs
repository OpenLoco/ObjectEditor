using System.Threading;
using System.Threading.Tasks;

namespace Gui.Models.Operations;

/// <summary>
/// A unit of work that can be enqueued onto the central <see cref="IOperationQueue"/>.
/// Concrete implementations encapsulate everything needed to perform the work plus a
/// human-readable <see cref="Title"/> for display in the operation panel.
/// </summary>
public interface IOperation
{
    /// <summary>Short, user-visible label describing what is happening (e.g. "Indexing folder").</summary>
    string Title { get; }

    /// <summary>Optional secondary line (path, item id, etc).</summary>
    string? InitialStatus => null;

    /// <summary>True if the operation honors <paramref name="ct"/> and can be cancelled mid-flight.</summary>
    bool SupportsCancellation => true;

    /// <summary>
    /// Run the work. Implementations should periodically report progress via
    /// <paramref name="progress"/> and observe <paramref name="ct"/> for cancellation.
    /// </summary>
    Task ExecuteAsync(System.IProgress<ProgressReport> progress, CancellationToken ct);
}
