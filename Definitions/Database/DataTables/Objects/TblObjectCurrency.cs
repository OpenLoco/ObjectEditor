using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectCurrency : DbSubObject, IConvertibleToTable<TblObjectCurrency, CurrencyObject>
	{
		public uint8_t Separator { get; set; }
		public uint8_t Factor { get; set; }

		public static TblObjectCurrency FromObject(TblObject tbl, CurrencyObject obj)
			=> new()
			{
				Parent = tbl,
				Separator = obj.Separator,
				Factor = obj.Factor,
			};
	}
}
