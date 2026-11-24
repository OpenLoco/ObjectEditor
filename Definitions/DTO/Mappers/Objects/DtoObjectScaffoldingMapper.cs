using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectScaffoldingMapper
{
	public static DtoObjectScaffolding ToDto(this TblObjectScaffolding tblobjectscaffolding) => new()
	{
		SegmentHeights = tblobjectscaffolding.SegmentHeights,
		RoofHeights = tblobjectscaffolding.RoofHeights,
		Id = tblobjectscaffolding.Id,
	};

	public static TblObjectScaffolding ToTblObjectScaffoldingEntity(this DtoObjectScaffolding model, TblObject parent) => new()
	{
		Parent = parent,
		SegmentHeights = model.SegmentHeights,
		RoofHeights = model.RoofHeights,
		Id = model.Id,
	};

}

