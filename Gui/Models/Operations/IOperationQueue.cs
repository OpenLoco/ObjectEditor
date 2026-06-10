namespace Gui.Models.Operations;

/// <summary>
/// Central queue that accepts <see cref="IOperation"/> instances and runs them, exposing
/// progress / cancellation through the returned <see cref="OperationHandle"/>. The queue
/// itself is responsible for surfacing operations in the UI; callers should not own any
/// progress-overlay UI of their own.
/// </summary>
public interface IOperationQueue
{
    /// <summary>
    /// Enqueue an operation. Operations run concurrently. The returned
    /// <see cref="OperationHandle"/> exposes a <see cref="OperationHandle.Completion"/>
    /// task that callers can await, a cancel command, and live progress/status that the
    /// UI binds to.
    /// </summary>
    OperationHandle Enqueue(IOperation operation);
}
