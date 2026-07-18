using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using System.Collections.Generic;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

public class DesignVehicleViewModel : VehicleViewModel
{
	public DesignVehicleViewModel()
		: base(new VehicleObject
		{
			Mode = TransportMode.Rail,
			Type = VehicleType.Train,
			CostIndex = 4,
			CostFactor = 100,
			RunCostIndex = 8,
			RunCostFactor = 50,
			Reliability = 80,
			Weight = 50,
			Power = 500,
			Speed = 80,
			RackSpeed = 40,
			DesignedYear = 1950,
			ObsoleteYear = 1980,
			NumCarComponents = 1,
			Flags = VehicleObjectFlags.None,
			CompanyColourSchemeIndex = CompanyColourType.SteamLoco,
			ShipWakeSpacing = 0,
			DrivingSoundType = DrivingSoundType.None,
			CompatibleCargoCategories =
			[
				[CargoCategory.Passengers, CargoCategory.Mail],
				[],
			],
			MaxCargo = [200, 0],
			CarComponents =
			[
				new VehicleObjectCar
				{
					FrontBogiePosition = 20,
					BackBogiePosition = 20,
					FrontBogieSpriteIndex = 0,
					BackBogieSpriteIndex = 0,
					BodySpriteIndex = 0,
					EmitterHorizontalOffset = 0,
				},
			],
			BodySprites =
			[
				new BodySprite
				{
					NumFlatRotationFrames = 8,
					NumSlopedRotationFrames = 4,
					NumAnimationFrames = 1,
					NumCargoLoadFrames = 1,
					NumCargoFrames = 1,
					NumRollFrames = 1,
					HalfLength = 20,
					Flags = BodySpriteFlags.HasSprites,
				},
			],
			BogieSprites =
			[
				new BogieSprite
				{
					NumAnimationFrames = 1,
					Flags = BogieSpriteFlags.HasSprites,
					Width = 8,
					HeightNegative = 4,
					HeightPositive = 4,
				},
			],
			ParticleEmitters =
			[
				new EmitterAnimation
				{
					SteamObject = new ObjectModelHeader("LOCOLOCO", ObjectType.Steam, ObjectSource.LocomotionSteam, 0),
					EmitterVerticalPos = 10,
					Type = SimpleAnimationType.SteamPuff1,
				},
			],
			CargoTypeSpriteOffsets = new Dictionary<CargoCategory, uint8_t>
			{
				[CargoCategory.Passengers] = 0,
				[CargoCategory.Mail] = 1,
			},
			StartSounds = [],
			var_135 = [],
		})
	{
	}
}