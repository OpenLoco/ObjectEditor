namespace OpenLoco.Dat.Types
{
	public interface IImageTableNameProvider
	{
		public bool TryGetImageName(int id, out string? value);
	}

	public class DefaultImageTableNameProvider : IImageTableNameProvider
	{
		public bool TryGetImageName(int id, out string? value)
		{
			value = id.ToString();
			return true;
		}
	}
}
