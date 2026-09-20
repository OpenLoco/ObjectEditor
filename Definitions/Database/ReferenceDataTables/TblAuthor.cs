using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

[Index(nameof(Name), IsUnique = true)]
public class TblAuthor : DbReferenceObject
{
	public ICollection<TblObject> Objects { get; set; } = [];
	public ICollection<TblObjectPack> ObjectPacks { get; set; } = [];
	public ICollection<TblScenario> SC5Files { get; set; } = [];
	public ICollection<TblScenarioPack> ScenarioPacks { get; set; } = [];
}
