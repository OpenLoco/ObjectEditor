using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Types;
using System.Text.Json.Serialization;

namespace Definitions.ObjectModels
{
	[JsonConverter(typeof(Serialization.LocoObjectJsonConverter))]
	public class LocoObject(ObjectType objectType, ILocoStruct obj, StringTable stringTable, ImageTable? imageTable = null)
	{
		public ObjectType ObjectType { get; init; } = objectType;
		public ILocoStruct Object { get; set; } = obj;
		public StringTable StringTable { get; set; } = stringTable;

		public ImageTable? ImageTable { get; set; } = imageTable;
	}
}
