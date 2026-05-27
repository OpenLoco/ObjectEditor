namespace Definitions.DTO;

public record DtoIndexFolderResponse(
	string Path,
	int ScannedCount,
	int AddedCount,
	int SkippedCount,
	int FailedCount);
