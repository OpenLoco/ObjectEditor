using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.SourceData;

namespace OpenLoco.Definitions.DTO
{
	public static class DtoExtensions
	{
		public static DtoObjectLookupFromDatDescriptor ToDtoDescriptor(this TblObjectLookupFromDat x, ICollection<DtoDatFileDetails> DatObjects)
			=> new(
				x.Id,
				DatObjects,
				x.Object.ObjectSource,
				x.Object.ObjectType,
				x.Object.VehicleType,
				x.Object.Name,
				x.Object.Description,
				x.Object.CreationDate,
				x.Object.LastEditDate,
				x.Object.UploadDate);

		public static DtoObjectDescriptor ToDtoDescriptor(this TblLocoObject x)
			=> new(
				x.Id,
				x.ObjectSource,
				x.ObjectType,
				x.VehicleType,
				x.Name,
				x.Description,
				x.CreationDate,
				x.LastEditDate,
				x.UploadDate);

		public static DtoObjectEntry ToDtoEntry(this TblLocoObject x)
			=> new(
				x.Id);

		public static DtoObjectLookupFromDatEntry ToDtoEntry(this TblObjectLookupFromDat x)
			=> new(
				x.Id,
				x.DatName,
				x.DatChecksum,
				x.xxHash3,
				x.ObjectId);

		public static DtoScenarioEntry ToDtoEntry(this TblSC5File x)
			=> new(
				x.Id,
				x.Name);

		public static DtoItemPackEntry ToDtoEntry(this TblSC5FilePack x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.CreationDate,
				x.LastEditDate,
				x.UploadDate,
				x.Licence);

		public static DtoItemPackEntry ToDtoEntry(this TblLocoObjectPack x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.CreationDate,
				x.LastEditDate,
				x.UploadDate,
				x.Licence);

		public static DtoItemPackDescriptor<DtoObjectEntry> ToDtoDescriptor(this ExpandedTblPack<TblLocoObjectPack, TblLocoObject> x)
			=> new(
				x.Pack.Id,
				x.Pack.Name,
				x.Pack.Description,
				[.. x.Items.Select(x => x.ToDtoEntry())],
				x.Authors,
				x.Pack.CreationDate,
				x.Pack.LastEditDate,
				x.Pack.UploadDate,
				x.Tags,
				x.Pack.Licence);

		public static DtoItemPackDescriptor<DtoScenarioEntry> ToDtoDescriptor(this ExpandedTblPack<TblSC5FilePack, TblSC5File> x)
			=> new(
				x.Pack.Id,
				x.Pack.Name,
				x.Pack.Description,
				[.. x.Items.Select(x => x.ToDtoEntry())],
				x.Authors,
				x.Pack.CreationDate,
				x.Pack.LastEditDate,
				x.Pack.UploadDate,
				x.Tags,
				x.Pack.Licence);
	}
}
