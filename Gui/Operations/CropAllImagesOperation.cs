using Gui.Models;
using Gui.Models.Operations;
using Gui.ViewModels.Graphics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Operations;

/// <summary>
/// Crops every image in the supplied list, reporting per-image progress. The operation is
/// CPU-bound and runs entirely on a worker thread via <see cref="Task.Run"/>.
/// </summary>
public sealed class CropAllImagesOperation(IReadOnlyList<ImageViewModel> images) : IOperation
{
    public string Title => "Cropping images";
    public string? InitialStatus => $"0 / {images.Count}";

    public Task ExecuteAsync(System.IProgress<ProgressReport> progress, CancellationToken ct)
        => Task.Run(() =>
        {
            for (var i = 0; i < images.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                images[i].CropImage();
                progress.Report(new ProgressReport((i + 1) / (double)images.Count, $"{i + 1} / {images.Count}"));
            }
        }, ct);
}
