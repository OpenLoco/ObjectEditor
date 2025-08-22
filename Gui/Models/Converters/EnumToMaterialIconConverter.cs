using Avalonia.Data.Converters;
using Dat.Data;
using Dat.Loaders;
using Definitions.ObjectModels.Types;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gui.Models.Converters;

public class EnumToMaterialIconConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return TryGetIcon(value, DatTypeMapping)
			?? TryGetIcon(value, ObjectMapping)
			?? TryGetIcon(value, VehicleMapping)
			?? TryGetIcon(value, SourceGameMapping)
			?? "CircleSmall";

		static string? TryGetIcon<T>(object? value, Dictionary<T, string> mapping) where T : struct
		{
			if (Enum.TryParse<T>(value as string, out var source) && mapping.TryGetValue(source, out var icon))
			{
				return icon;
			}
			else if ((value is T source2) && mapping.TryGetValue(source2, out var icon2))
			{
				return icon2;
			}

			return null;
		}
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotImplementedException();

	static readonly Dictionary<DatObjectType, string> ObjectMapping = new()
	{
		{ DatObjectType.InterfaceSkin, "Monitor" },
		{ DatObjectType.Sound, "Speaker" },
		{ DatObjectType.Currency, "Cash" },
		{ DatObjectType.Steam, "Smoke" },
		{ DatObjectType.CliffEdge, "Landslide" },
		{ DatObjectType.Water, "Waves" },
		{ DatObjectType.Land, "LandFields" },
		{ DatObjectType.TownNames, "NoteText" },
		{ DatObjectType.Cargo, "ShippingPallet" },
		{ DatObjectType.Wall, "Wall" },
		{ DatObjectType.TrackSignal, "TrafficLight" },
		{ DatObjectType.LevelCrossing, "RailroadLight" },
		{ DatObjectType.StreetLight, "OutdoorLamp" },
		{ DatObjectType.Tunnel, "Tunnel" },
		{ DatObjectType.Bridge, "Bridge" },
		{ DatObjectType.TrackStation, "Domain" },
		{ DatObjectType.TrackExtra, "FenceElectric" },
		{ DatObjectType.Track, "Fence" },
		{ DatObjectType.RoadStation, "GasStationInUse" },
		{ DatObjectType.RoadExtra, "RoadVariant" },
		{ DatObjectType.Road, "Road" },
		{ DatObjectType.Airport, "Airport" },
		{ DatObjectType.Dock, "Ferry" },
		{ DatObjectType.Vehicle, "PlaneTrain" },
		{ DatObjectType.Tree, "PineTreeVariant" },
		{ DatObjectType.Snow, "Snowflake" },
		{ DatObjectType.Climate, "WeatherPartlyCloudy" },
		{ DatObjectType.HillShapes, "Terrain" },
		{ DatObjectType.Building, "OfficeBuilding" },
		{ DatObjectType.Scaffolding, "Crane" },
		{ DatObjectType.Industry, "Factory" },
		{ DatObjectType.Region, "Earth" },
		{ DatObjectType.Competitor, "AccountTie" },
		{ DatObjectType.ScenarioText, "ScriptText" },
	};

	static readonly Dictionary<DatVehicleType, string> VehicleMapping = new()
	{
		{ DatVehicleType.Train, "Train" },
		{ DatVehicleType.Bus, "Bus" },
		{ DatVehicleType.Truck, "Truck" },
		{ DatVehicleType.Tram, "Tram" },
		{ DatVehicleType.Aircraft, "Airplane" },
		{ DatVehicleType.Ship, "Sailboat" },
	};

	static readonly Dictionary<ObjectSource, string> SourceGameMapping = new()
	{
		{ ObjectSource.Custom, "HumanEdit" },
		{ ObjectSource.LocomotionSteam, "AccountTieHat" },
		{ ObjectSource.LocomotionGoG, "AccountTieHatOutline" },
		{ ObjectSource.OpenLoco, "AccountHardHat" },
	};

	static readonly Dictionary<DatFileType, string> DatTypeMapping = new()
	{
		{ DatFileType.Object, "Apps" },
		{ DatFileType.Scenario, "MapClock" },
		{ DatFileType.SaveGame, "ContentSave" },
		{ DatFileType.Tutorial, "School" },
		{ DatFileType.G1, "ImageAlbum" },
		{ DatFileType.Music, "Music" },
		{ DatFileType.SoundEffect, "Bugle" },
		{ DatFileType.Language, "TranslateVariant" },
		{ DatFileType.Scores, "Scoreboard" },
	};
}
