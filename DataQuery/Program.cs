// See https://aka.ms/new-console-template for more information
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.FileParsing;
using System.Reflection;

var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
var logger = new Logger();
var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

var results = new List<(ObjectIndexEntry Obj, byte CostIndex, short? RunCostIndex)>();
var count = 0;

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

	count++;

	if (count % 500 == 0)
	{
		Console.WriteLine($"{count}/{index.Objects.Count} ({count / (float)index.Objects.Count * 100:F2}%)");
	}

	//if (count > 100)
	//{
	//	break;
	//}
}

Console.WriteLine("writing to file");

var header = "DatName, ObjectType, CostIndex, RunCostIndex";
var lines = results
	.OrderBy(x => x.Obj.DatName)
	.Select(x => string.Join(',', x.Obj.DatName, x.Obj.ObjectType, x.CostIndex, x.RunCostIndex));
File.WriteAllLines("costIndex.csv", [header, .. lines]);

Console.WriteLine("done");

Console.ReadLine();
