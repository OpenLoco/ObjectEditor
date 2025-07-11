using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

[Index(nameof(Name), IsUnique = true)]
public class TblTag : DbReferenceObject
{
	public ICollection<TblObject> Objects { get; set; } = [];
	public ICollection<TblObjectPack> ObjectPacks { get; set; } = [];
	public ICollection<TblSC5File> SC5Files { get; set; } = [];
	public ICollection<TblSC5FilePack> SC5FilePacks { get; set; } = [];
}
