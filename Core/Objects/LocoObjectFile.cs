using Dat.Types;
using Definitions.ObjectModels;

namespace Core.Objects;

public sealed record LocoObjectFile(string FileName, DatHeaderInfo DatInfo, LocoObject LocoObject);
