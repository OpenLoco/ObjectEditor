using Definitions.Database;

namespace Definitions.DTO.Mappers
{
	public static class DtoObjectTrackSignalMapper
	{
		public static DtoObjectTrackSignal ToDto(this TblObjectTrackSignal tblobjecttracksignal) => new()
		{
			Flags = tblobjecttracksignal.Flags,
			AnimationSpeed = tblobjecttracksignal.AnimationSpeed,
			NumFrames = tblobjecttracksignal.NumFrames,
			BuildCostFactor = tblobjecttracksignal.BuildCostFactor,
			SellCostFactor = tblobjecttracksignal.SellCostFactor,
			CostIndex = tblobjecttracksignal.CostIndex,
			DesignedYear = tblobjecttracksignal.DesignedYear,
			ObsoleteYear = tblobjecttracksignal.ObsoleteYear,
			Id = tblobjecttracksignal.Id,
		};

		public static TblObjectTrackSignal ToTblObjectTrackSignalEntity(this DtoObjectTrackSignal model, TblObject parent) => new()
		{
			Parent = parent,
			Flags = model.Flags,
			AnimationSpeed = model.AnimationSpeed,
			NumFrames = model.NumFrames,
			BuildCostFactor = model.BuildCostFactor,
			SellCostFactor = model.SellCostFactor,
			CostIndex = model.CostIndex,
			DesignedYear = model.DesignedYear,
			ObsoleteYear = model.ObsoleteYear,
			Id = model.Id,
		};

	}
}

