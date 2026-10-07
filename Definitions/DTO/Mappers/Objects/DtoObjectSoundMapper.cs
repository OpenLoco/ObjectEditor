using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectSoundMapper
{
	public static DtoObjectSound ToDto(this TblObjectSound tblobjectsound) => new()
	{
		ShouldLoop = tblobjectsound.ShouldLoop,
		Volume = tblobjectsound.Volume,
		SoundObjectData = tblobjectsound.SoundObjectData,
		Id = tblobjectsound.Id,
	};

	public static TblObjectSound ToTblObjectSoundEntity(this DtoObjectSound model, TblObject parent) => new()
	{
		Parent = parent,
		ShouldLoop = model.ShouldLoop,
		Volume = model.Volume,
		SoundObjectData = model.SoundObjectData,
		Id = model.Id,
	};

}

