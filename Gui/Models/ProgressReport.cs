namespace Gui.Models;

/// <summary>
/// A progress update emitted by long-running operations.
/// </summary>
/// <param name="Value">Fractional progress in the range 0.0 .. 1.0. Use NaN or a negative value to leave the current value unchanged (useful when only updating <see cref="Message"/>).</param>
/// <param name="Message">Optional status message describing the current step.</param>
public readonly record struct ProgressReport(double Value, string? Message = null)
{
    public static ProgressReport WithMessage(string message) => new(double.NaN, message);
}
