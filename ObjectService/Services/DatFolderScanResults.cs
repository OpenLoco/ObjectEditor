namespace ObjectService.Services;

public record DatFolderScanResults(
	IReadOnlyList<DatFileScanResult> Succeeded,
	IReadOnlyList<string> Failed);
