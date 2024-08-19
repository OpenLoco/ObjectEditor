using System.Text.Json.Serialization;

namespace OpenLoco.Common
{
	public class GlenDBSchema
	{
		public IList<GlenDbData> data { get; set; }
	}

	public class GlenDbData
	{
		[JsonPropertyName("C00")] public string ObjectName { get; set; }
		[JsonPropertyName("C01")] public string Image { get; set; }
		[JsonPropertyName("C02")] public string DescriptionAndFile { get; set; }
		[JsonPropertyName("C03")] public string ClassNumber { get; set; }
		[JsonPropertyName("C04")] public string Type { get; set; }
		[JsonPropertyName("C05")] public string TrackType { get; set; }
		[JsonPropertyName("C06")] public string Designed { get; set; }
		[JsonPropertyName("C07")] public string Obsolete { get; set; }
		[JsonPropertyName("C08")] public string Speed { get; set; }
		[JsonPropertyName("C09")] public string Power { get; set; }
		[JsonPropertyName("C10")] public string Weight { get; set; }
		[JsonPropertyName("C11")] public string Reliability { get; set; }
		[JsonPropertyName("C12")] public string Length { get; set; }
		[JsonPropertyName("C13")] public string NumCompat { get; set; }
		[JsonPropertyName("C14")] public string Sprites { get; set; }
		[JsonPropertyName("C15")] public string CargoCapacity1 { get; set; }
		[JsonPropertyName("C16")] public string CargoType1 { get; set; }
		[JsonPropertyName("C17")] public string CargoCapacity2 { get; set; }
		[JsonPropertyName("C18")] public string CargoType2 { get; set; }
		[JsonPropertyName("C19")] public string Creator { get; set; }
		[JsonPropertyName("C20")] public string _Tags { get; set; }

		public string[] Tags => _Tags.Split(" ");
	}
}
