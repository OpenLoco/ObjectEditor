namespace Definitions.ObjectModels.Types;

public record ObjectModelHeader(string Name, uint Checksum, ObjectType ObjectType, ObjectSource ObjectSource); // mimics S5Header from DAT
