namespace OpenLocoTool
{
	public record LocoObject(DatFileHeader datHdr, ObjHeader objHdr, object? obj);
}
