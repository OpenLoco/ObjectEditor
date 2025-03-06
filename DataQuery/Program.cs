// See https://aka.ms/new-console-template for more information
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using System.Reflection;

var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
var logger = new Logger();
var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

//QueryCostIndices(dir, logger, index);
//QueryCargoCategories(dir, logger, index);
QueryVehicleBodyUnkSprites(dir, logger, index);

Console.WriteLine("done");

Console.ReadLine();

static void QueryVehicleBodyUnkSprites(string dir, Logger logger, ObjectIndex index)
{
	var results = new List<(ObjectIndexEntry Obj, ObjectSource ObjectSource)>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Vehicle && x.ObjectSource == ObjectSource.LocomotionSteam))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(dir, obj.Filename), logger);
			if (o?.LocoObject != null)
			{
				var struc = (VehicleObject)o.Value.LocoObject.Object;
				var header = o.Value.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum);

				if (struc.BodySprites.Any(x => x.Flags.HasFlag(BodySpriteFlags.HasUnkSprites)))
				{
					results.Add((obj, source));
				}

				//results.Add((obj, struc.CargoCategory, o.Value.LocoObject.StringTable.Table["Name"][LanguageId.English_UK], source));
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.Filename} - {ex.Message}");
		}
	}

	const string csvHeader = "DatName, ObjectSource";
	var lines = results
		.OrderBy(x => x.Obj.DatName)
		.Select(x => string.Join(',', x.Obj.DatName, x.ObjectSource));

	File.WriteAllLines("vehicleBodiesWithUnkSpritesFlag.csv", [csvHeader, .. lines]);
}

static void QueryCargoCategories(string dir, Logger logger, ObjectIndex index)
{
	var results = new List<(ObjectIndexEntry Obj, CargoCategory CargoCategory, string LocalisedName, ObjectSource ObjectSource)>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Cargo))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(dir, obj.Filename), logger);
			if (o?.LocoObject != null)
			{
				var struc = (CargoObject)o.Value.LocoObject.Object;

				var header = o.Value.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum);

				results.Add((obj, struc.CargoCategory, o.Value.LocoObject.StringTable.Table["Name"][LanguageId.English_UK], source));
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.Filename} - {ex.Message}");
		}
	}

	Console.WriteLine("writing to file");

	const string csvHeader = "DatName, CargoCategory, LocalisedName, ObjectSource";
	var lines = results
		.OrderBy(x => x.Obj.DatName)
		.Select(x => string.Join(',', x.Obj.DatName, (int)x.CargoCategory, x.LocalisedName, x.ObjectSource));
	File.WriteAllLines("cargoCategories.csv", [csvHeader, .. lines]);
}

static void QueryCostIndices(string dir, Logger logger, ObjectIndex index)
{
	var results = new List<(ObjectIndexEntry Obj, byte CostIndex, short? RunCostIndex)>();

	foreach (var obj in index.Objects)
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(dir, obj.Filename), logger);
			if (o?.LocoObject != null)
			{
				var struc = o.Value.LocoObject.Object;
				var type = struc.GetType();

				var costIndexProperty = type.GetProperty("CostIndex", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				var paymentIndexProperty = type.GetProperty("PaymentIndex", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				var runCostIndexProperty = type.GetProperty("RunCostIndex", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				byte? costIndex = null;
				byte? runCostIndex = null;

				if (costIndexProperty?.PropertyType == typeof(byte) && costIndexProperty.GetValue(struc) is byte costIndexValue)
				{
					costIndex = costIndexValue;
				}
				else if (paymentIndexProperty?.PropertyType == typeof(byte) && paymentIndexProperty.GetValue(struc) is byte paymentIndexValue)
				{
					costIndex = paymentIndexValue;
				}

				if (runCostIndexProperty?.PropertyType == typeof(byte) && runCostIndexProperty.GetValue(struc) is byte runCostIndexValue)
				{
					runCostIndex = runCostIndexValue;
				}

				if (costIndex != null)
				{
					results.Add((obj, costIndex.Value, runCostIndex));
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.Filename} - {ex.Message}");
		}
	}

	Console.WriteLine("writing to file");

	const string header = "DatName, ObjectType, CostIndex, RunCostIndex";
	var lines = results
		.OrderBy(x => x.Obj.DatName)
		.Select(x => string.Join(',', x.Obj.DatName, x.Obj.ObjectType, x.CostIndex, x.RunCostIndex));
	File.WriteAllLines("costIndex.csv", [header, .. lines]);
}
