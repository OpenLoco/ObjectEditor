using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectSteamMapper
{
	public static DtoObjectSteam ToDto(this TblObjectSteam tblobjectsteam) => new()
	{
		NumStationaryTicks = tblobjectsteam.NumStationaryTicks,
		SpriteWidth = tblobjectsteam.SpriteWidth,
		SpriteHeightNegative = tblobjectsteam.SpriteHeightNegative,
		SpriteHeightPositive = tblobjectsteam.SpriteHeightPositive,
		Flags = tblobjectsteam.Flags,
		ImageOffset = tblobjectsteam.ImageOffset,
		FrameInfoType0 = tblobjectsteam.FrameInfoType0,
		FrameInfoType1 = tblobjectsteam.FrameInfoType1,
		SoundEffects = tblobjectsteam.SoundEffects,
		Id = tblobjectsteam.Id,
	};

	public static TblObjectSteam ToTblObjectSteamEntity(this DtoObjectSteam model, TblObject parent) => new()
	{
		Parent = parent,
		NumStationaryTicks = model.NumStationaryTicks,
		SpriteWidth = model.SpriteWidth,
		SpriteHeightNegative = model.SpriteHeightNegative,
		SpriteHeightPositive = model.SpriteHeightPositive,
		Flags = model.Flags,
		ImageOffset = model.ImageOffset,
		FrameInfoType0 = model.FrameInfoType0,
		FrameInfoType1 = model.FrameInfoType1,
		SoundEffects = model.SoundEffects,
		Id = model.Id,
	};

}

