using Avalonia.Data.Converters;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenLoco.Gui.Models.Converters
{
	public class EnumToMaterialIconConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return TryGetIcon(value, DatTypeMapping)
				?? TryGetIcon(value, ObjectMapping)
				?? TryGetIcon(value, VehicleMapping)
				?? TryGetIcon(value, SourceGameMapping);

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

		static readonly Dictionary<ObjectType, string> ObjectMapping = new()
		{
			{ ObjectType.InterfaceSkin, "Monitor" },
			{ ObjectType.Sound, "Speaker" },
			{ ObjectType.Currency, "Cash" },
			{ ObjectType.Steam, "Smoke" },
			{ ObjectType.CliffEdge, "Landslide" },
			{ ObjectType.Water, "Waves" },
			{ ObjectType.Land, "LandFields" },
			{ ObjectType.TownNames, "NoteText" },
			{ ObjectType.Cargo, "ShippingPallet" },
			{ ObjectType.Wall, "Wall" },
			{ ObjectType.TrainSignal, "TrafficLight" },
			{ ObjectType.LevelCrossing, "RailroadLight" },
			{ ObjectType.StreetLight, "OutdoorLamp" },
			{ ObjectType.Tunnel, "Tunnel" },
			{ ObjectType.Bridge, "Bridge" },
			{ ObjectType.TrainStation, "Domain" },
			{ ObjectType.TrackExtra, "FenceElectric" },
			{ ObjectType.Track, "Fence" },
			{ ObjectType.RoadStation, "GasStationInUse" },
			{ ObjectType.RoadExtra, "RoadVariant" },
			{ ObjectType.Road, "Road" },
			{ ObjectType.Airport, "Airport" },
			{ ObjectType.Dock, "Ferry" },
			{ ObjectType.Vehicle, "PlaneTrain" },
			{ ObjectType.Tree, "PineTreeVariant" },
			{ ObjectType.Snow, "Snowflake" },
			{ ObjectType.Climate, "WeatherPartlyCloudy" },
			{ ObjectType.HillShapes, "Terrain" },
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
