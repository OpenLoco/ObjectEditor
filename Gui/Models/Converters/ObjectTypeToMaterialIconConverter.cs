using Avalonia.Data.Converters;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenLoco.Gui.Models.Converters
{
	public class ObjectTypeToMaterialIconConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (Enum.TryParse<DatFileType>(value as string, out var datType) && DatTypeMapping.TryGetValue(datType, out var datIcon))
			{
				return datIcon;
			}

			if (Enum.TryParse<ObjectType>(value as string, out var objType) && ObjectMapping.TryGetValue(objType, out var objectIcon))
			{
				return objectIcon;
			}

			if (Enum.TryParse<VehicleType>(value as string, out var vehType) && VehicleMapping.TryGetValue(vehType, out var vehicleIcon))
			{
				return vehicleIcon;
			}

			if (Enum.TryParse<ObjectSource>(value as string, out var objectSource) && SourceGameMapping.TryGetValue(objectSource, out var sourceIcon))
			{
				return sourceIcon;
			}
			else if ((value is ObjectSource objectSource2) && SourceGameMapping.TryGetValue(objectSource2, out var sourceIcon2))
			{
				return sourceIcon2;
			}

			return null;
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotImplementedException();

		static readonly Dictionary<ObjectType, string> ObjectMapping = new()
		{
			{ ObjectType.InterfaceSkin, "Monitor" },
			{ ObjectType.Sound, "Speaker" },
			{ ObjectType.Currency, "Cash" },
			{ ObjectType.Steam, "Smoke" },
			{ ObjectType.CliffEdge, "Landslide" },
			{ ObjectType.Water, "Waves" },
			{ ObjectType.Land, "Terrain" },
			{ ObjectType.TownNames, "NoteText" },
			{ ObjectType.Cargo, "ShippingPallet" },
			{ ObjectType.Wall, "Wall" },
			{ ObjectType.TrainSignal, "TrafficLight" },
			{ ObjectType.LevelCrossing, "RailroadLight" },
			{ ObjectType.StreetLight, "CeilingLight" },
			{ ObjectType.Tunnel, "Tunnel" },
			{ ObjectType.Bridge, "Bridge" },
			{ ObjectType.TrainStation, "Domain" },
			{ ObjectType.TrackExtra, "FenceElectric" },
			{ ObjectType.Track, "Fence" },
			{ ObjectType.RoadStation, "Monitor" },
			{ ObjectType.RoadExtra, "RoadVariant" },
			{ ObjectType.Road, "Road" },
			{ ObjectType.Airport, "Airport" },
			{ ObjectType.Dock, "Ferry" },
			{ ObjectType.Vehicle, "PlaneTrain" },
			{ ObjectType.Tree, "PineTreeVariant" },
			{ ObjectType.Snow, "Snowflake" },
			{ ObjectType.Climate, "WeatherPartlyCloudy" },
			{ ObjectType.HillShapes, "Map" },
			{ ObjectType.Building, "OfficeBuilding" },
			{ ObjectType.Scaffolding, "Crane" },
			{ ObjectType.Industry, "Factory" },
			{ ObjectType.Region, "Earth" },
			{ ObjectType.Competitor, "AccountTie" },
			{ ObjectType.ScenarioText, "ScriptText" },
		};

		static readonly Dictionary<VehicleType, string> VehicleMapping = new()
		{
			{ VehicleType.Train, "Train" },
			{ VehicleType.Bus, "Bus" },
			{ VehicleType.Truck, "Truck" },
			{ VehicleType.Tram, "Tram" },
			{ VehicleType.Aircraft, "Airplane" },
			{ VehicleType.Ship, "Sailboat" },
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
}
