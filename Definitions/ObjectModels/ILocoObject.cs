namespace Definitions.ObjectModels;

public interface ILocoObject : IHasG1Elements
{
	ILocoStruct Object { get; set; }

	StringTable StringTable { get; set; }
}
