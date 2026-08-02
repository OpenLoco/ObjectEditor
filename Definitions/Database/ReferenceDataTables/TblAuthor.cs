using Definitions.Database.Base;
using Definitions.Database.DataTables;
using Microsoft.EntityFrameworkCore;

namespace Definitions.Database.ReferenceDataTables;

[Index(nameof(Name), IsUnique = true)]
public class TblAuthor : DbReferenceObject
{
	public ICollection<TblObject> Objects { get; set; } = [];
	public ICollection<TblObjectPack> ObjectPacks { get; set; } = [];
	public ICollection<TblSC5File> SC5Files { get; set; } = [];
	public ICollection<TblSC5FilePack> SC5FilePacks { get; set; } = [];
}
