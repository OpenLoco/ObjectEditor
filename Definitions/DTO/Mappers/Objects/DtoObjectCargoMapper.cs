using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectCargoMapper
	{
		public static DtoObjectCargo ToDto(this TblObjectCargo tblobjectcargo)
		{
			return new DtoObjectCargo
			{
				CargoTransferTime = tblobjectcargo.CargoTransferTime,
				CargoCategory = tblobjectcargo.CargoCategory,
				Flags = tblobjectcargo.Flags,
				NumPlatformVariations = tblobjectcargo.NumPlatformVariations,
				StationCargoDensity = tblobjectcargo.StationCargoDensity,
				PremiumDays = tblobjectcargo.PremiumDays,
				MaxNonPremiumDays = tblobjectcargo.MaxNonPremiumDays,
				MaxPremiumRate = tblobjectcargo.MaxPremiumRate,
				PenaltyRate = tblobjectcargo.PenaltyRate,
				PaymentFactor = tblobjectcargo.PaymentFactor,
				PaymentIndex = tblobjectcargo.PaymentIndex,
				UnitSize = tblobjectcargo.UnitSize,
				Id = tblobjectcargo.Id,
			};
		}

		public static TblObjectCargo ToTblObjectCargoEntity(this DtoObjectCargo model)
		{
			return new TblObjectCargo
			{
				CargoTransferTime = model.CargoTransferTime,
				CargoCategory = model.CargoCategory,
				Flags = model.Flags,
				NumPlatformVariations = model.NumPlatformVariations,
				StationCargoDensity = model.StationCargoDensity,
				PremiumDays = model.PremiumDays,
				MaxNonPremiumDays = model.MaxNonPremiumDays,
				MaxPremiumRate = model.MaxPremiumRate,
				PenaltyRate = model.PenaltyRate,
				PaymentFactor = model.PaymentFactor,
				PaymentIndex = model.PaymentIndex,
				UnitSize = model.UnitSize,
				Id = model.Id,
			};
		}

	}
}

