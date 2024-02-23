namespace OpenLoco.ObjectEditor.Types
{
	public interface ILocoImageTableNames
	{
		public bool TryGetImageName(int id, out string? value);
	}
}
