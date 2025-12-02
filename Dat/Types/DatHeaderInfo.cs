using System.ComponentModel;

namespace Dat.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public record DatHeaderInfo(S5Header S5Header, ObjectHeader ObjectHeader);
