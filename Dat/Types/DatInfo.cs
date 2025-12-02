using System.ComponentModel;

namespace Dat.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public record DatInfo(S5Header S5Header, ObjectHeader ObjectHeader);
