using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.SourceData;

namespace OpenLoco.Definitions.DTO
{
	public static class DtoExtensions
	{
		public static DtoObjectEntry ToDtoEntry(this TblObject x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.ObjectSource,
				x.ObjectType,
				x.VehicleType,
				x.CreatedDate,
				x.ModifiedDate,
				x.UploadedDate);

		public static DtoObjectEntry ToDtoEntry(this TblDatObject x)
			=> x.Object.ToDtoEntry();

		public static DtoDatObjectEntry ToDtoDescriptor(this TblDatObject x)
			=> new(
				x.Id,
				x.DatName,
				x.DatChecksum,
				x.xxHash3,
				x.ObjectId,
				null);

		public static DtoScenarioEntry ToDtoEntry(this TblSC5File x)
			=> new(
				x.Id,
				x.Name);

		public static DtoItemPackEntry ToDtoEntry(this TblSC5FilePack x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.CreatedDate,
				x.ModifiedDate,
				x.UploadedDate,
				x.Licence);

		public static DtoItemPackEntry ToDtoEntry(this TblObjectPack x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.CreatedDate,
				x.ModifiedDate,
				x.UploadedDate,
				x.Licence);

		public static DtoItemPackDescriptor<DtoObjectEntry> ToDtoDescriptor(this ExpandedTblPack<TblObjectPack, TblObject> x)
			=> new(
				x.Pack.Id,
				x.Pack.Name,
				x.Pack.Description,
				x.Pack.CreatedDate,
				x.Pack.ModifiedDate,
				x.Pack.UploadedDate,
				[.. x.Items.Select(x => x.ToDtoEntry())],
				x.Authors,
				x.Tags,
				x.Pack.Licence);

		public static DtoItemPackDescriptor<DtoScenarioEntry> ToDtoDescriptor(this ExpandedTblPack<TblSC5FilePack, TblSC5File> x)
			=> new(
				x.Pack.Id,
				x.Pack.Name,
				x.Pack.Description,
				x.Pack.CreatedDate,
				x.Pack.ModifiedDate,
				x.Pack.UploadedDate,
				[.. x.Items.Select(x => x.ToDtoEntry())],
				x.Authors,
				x.Tags,
				x.Pack.Licence);
	}
}
