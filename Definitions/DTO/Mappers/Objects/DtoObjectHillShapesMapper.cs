using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectHillShapesMapper
{
	public static DtoObjectHillShapes ToDto(this TblObjectHillShapes tblobjecthillshapes) => new()
	{
		HillHeightMapCount = tblobjecthillshapes.HillHeightMapCount,
		MountainHeightMapCount = tblobjecthillshapes.MountainHeightMapCount,
		IsHeightMap = tblobjecthillshapes.IsHeightMap,
		Id = tblobjecthillshapes.Id,
	};

	public static TblObjectHillShapes ToTblObjectHillShapesEntity(this DtoObjectHillShapes model, TblObject parent) => new()
	{
		Parent = parent,
		HillHeightMapCount = model.HillHeightMapCount,
		MountainHeightMapCount = model.MountainHeightMapCount,
		IsHeightMap = model.IsHeightMap,
		Id = model.Id,
	};

}

