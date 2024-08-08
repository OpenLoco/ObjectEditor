using Avalonia.Data.Converters;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace AvaGui.Models
{
	public class ObjectTypeToMaterialIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (Enum.TryParse<ObjectType>(value as string, out var objType) && ObjectMapping.TryGetValue(objType, out var objectIcon))
			{
				return objectIcon;
			}

			if (Enum.TryParse<VehicleType>(value as string, out var vehType) && VehicleMapping.TryGetValue(vehType, out var vehicleIcon))
			{
				return vehicleIcon;
			}

			if (value is SourceGame sourceType && SourceGameMapping.TryGetValue(sourceType, out var sourceIcon))
			{
				return sourceIcon;
			}

			return null;
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotImplementedException();

		public static Dictionary<ObjectType, string> ObjectMapping = new Dictionary<ObjectType, string>()
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

		public static Dictionary<VehicleType, string> VehicleMapping = new Dictionary<VehicleType, string>()
		{
			{ VehicleType.Train, "Train" },
			{ VehicleType.Bus, "Bus" },
			{ VehicleType.Truck, "Truck" },
			{ VehicleType.Tram, "Tram" },
			{ VehicleType.Aircraft, "Airplane" },
			{ VehicleType.Ship, "Sailboat" },
		};

		public static Dictionary<SourceGame, string> SourceGameMapping = new Dictionary<SourceGame, string>()
		{
			{ SourceGame.Custom, "AccountEdit" },
			{ SourceGame.DataFile, "File" },
			{ SourceGame.Vanilla, "AccountTieHat" },
		};
	}

	public abstract record FileSystemItemBase(string Path, string Name, ObservableCollection<FileSystemItemBase>? SubNodes = null)
	{
		public string NameComputed
			=> $"{Name}{(SubNodes == null ? string.Empty : $" ({SubNodes.Count})")}"; // nested interpolated string...what have i become
	}

	public record FileSystemItem(string Path, string Name, SourceGame SourceGame)
		: FileSystemItemBase(Path, Name, null);

	public record FileSystemItemGroup(string Path, ObjectType ObjectType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Path, ObjectType.ToString(), SubNodes);

	public record FileSystemVehicleGroup(string Path, VehicleType VehicleType, ObservableCollection<FileSystemItemBase> SubNodes)
		: FileSystemItemBase(Path, VehicleType.ToString(), SubNodes);
}
