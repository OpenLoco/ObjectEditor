namespace Definitions.DTO;

/// <summary>One rendered frame of an object image table: its index and trimmed pixel dimensions.</summary>
public record DtoObjectImageFrame(int Index, int Width, int Height);

/// <summary>The rendered image table metadata for an object, as returned by the metadata endpoint.</summary>
public record DtoObjectImageMetadata(int Count, IReadOnlyList<DtoObjectImageFrame> Frames);
