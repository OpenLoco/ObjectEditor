using System;
using System.Collections.Generic;

namespace Gui.Models;

/// <summary>
/// An <see cref="IProgress{T}"/> implementation that forwards each report to multiple
/// downstream progress receivers. Used to drive both the overlay and existing
/// inline progress indicators (e.g. <c>FolderTreeViewModel.IndexOrDownloadProgress</c>).
/// </summary>
public sealed class TeeProgress<T>(params IProgress<T>[] receivers) : IProgress<T>
{
    readonly IReadOnlyList<IProgress<T>> receivers = receivers;

    public void Report(T value)
    {
        foreach (var r in receivers)
        {
            r.Report(value);
        }
    }
}

/// <summary>Non-generic alias for the common <see cref="float"/> case.</summary>
public sealed class TeeProgress(params IProgress<float>[] receivers) : IProgress<float>
{
    readonly IReadOnlyList<IProgress<float>> receivers = receivers;

    public void Report(float value)
    {
        foreach (var r in receivers)
        {
            r.Report(value);
        }
    }
}
