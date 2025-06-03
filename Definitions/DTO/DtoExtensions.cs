using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.SourceData;

namespace OpenLoco.Definitions.DTO
{
	public static class DtoExtensions
	{
		public static DtoObjectDescriptor ToDtoDescriptor(this ExpandedTbl<TblObject, TblObjectPack> x)
			=> new(
				x!.Object.Id,
				x!.Object.Name,
				x!.Object.DatObjects.FirstOrDefault()?.DatName ?? "<--->",
				x!.Object.Description,
				x!.Object.ObjectSource,
				x!.Object.ObjectType,
				x!.Object.VehicleType,
				x!.Object.CreatedDate,
				x!.Object.ModifiedDate,
				x!.Object.UploadedDate,
				x!.Object.Licence?.ToDtoEntry(),
				[.. x.Authors.Select(x => x.ToDtoEntry())],
				[.. x.Tags.Select(x => x.ToDtoEntry())],
				[.. x.Packs.Select(x => x.ToDtoEntry())],
				[.. x.Object.DatObjects.Select(x => x.ToDtoEntry())],
				x.Object.StringTable.ToDtoDescriptor(x.Object.Id));

		public static DtoStringTableDescriptor ToDtoDescriptor(this ICollection<TblStringTable> x, int ObjectId)
		{
			var table = x
				.Select(x => x.ToDtoEntry())
				.GroupBy(x => x.RowName)
				.ToDictionary(
					x => x.Key,
					x => x.ToDictionary(x => x.RowLanguage, x => x.RowText));

			return new DtoStringTableDescriptor(table, ObjectId);
		}

		public static DtoItemPackDescriptor<DtoObjectEntry> ToDtoDescriptor(this TblObjectPack x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.CreatedDate,
				x.ModifiedDate,
				x.UploadedDate,
				[.. x.Objects.Select(x => x.ToDtoEntry())],
				[.. x.Authors.Select(x => x.ToDtoEntry())],
				[.. x.Tags.Select(x => x.ToDtoEntry())],
				x.Licence?.ToDtoEntry());

		public static DtoItemPackDescriptor<DtoObjectEntry> ToDtoDescriptor(this ExpandedTblPack<TblObjectPack, TblObject> x)
			=> new(
				x.Pack.Id,
				x.Pack.Name,
				x.Pack.Description,
				x.Pack.CreatedDate,
				x.Pack.ModifiedDate,
				x.Pack.UploadedDate,
				[.. x.Items.Select(x => x.ToDtoEntry())],
				[.. x.Authors.Select(x => x.ToDtoEntry())],
				[.. x.Tags.Select(x => x.ToDtoEntry())],
				x.Pack.Licence?.ToDtoEntry());

		public static DtoItemPackDescriptor<DtoScenarioEntry> ToDtoDescriptor(this ExpandedTblPack<TblSC5FilePack, TblSC5File> x)
			=> new(
				x.Pack.Id,
				x.Pack.Name,
				x.Pack.Description,
				x.Pack.CreatedDate,
				x.Pack.ModifiedDate,
				x.Pack.UploadedDate,
				[.. x.Items.Select(x => x.ToDtoEntry())],
				[.. x.Authors.Select(x => x.ToDtoEntry())],
				[.. x.Tags.Select(x => x.ToDtoEntry())],
				x.Pack.Licence?.ToDtoEntry());

		#region New

		public static DtoStringTableEntry ToDtoEntry(this TblStringTable table)
			=> new(table.Id, table.RowName, table.RowLanguage, table.RowText, table.ObjectId);

		public static TblStringTable ToTable(this DtoStringTableEntry dto)
			=> new() { Id = dto.Id, RowName = dto.RowName, RowLanguage = dto.RowLanguage, RowText = dto.RowText, ObjectId = dto.ObjectId };

		public static DtoDatObjectEntry ToDtoEntry(this TblDatObject table)
			=> new(table.Id, table.DatName, table.DatChecksum, table.xxHash3, table.ObjectId);

		public static TblDatObject ToTable(this DtoDatObjectEntry dto)
			=> new() { Id = dto.Id, DatName = dto.DatName, DatChecksum = dto.DatChecksum, xxHash3 = dto.xxHash3, ObjectId = dto.ObjectId };

		public static DtoObjectEntry ToDtoEntry(this TblObject table)
			=> new(table.Id, table.Name, table.DatObjects.FirstOrDefault()?.DatName ?? "<--->", table.Description, table.ObjectSource, table.ObjectType, table.VehicleType, table.CreatedDate, table.ModifiedDate, table.UploadedDate);

		public static TblObject ToTable(this DtoObjectEntry dto)
			=> new() { Id = dto.Id, Name = dto.InternalName, Description = dto.Description, ObjectSource = dto.ObjectSource, ObjectType = dto.ObjectType, VehicleType = dto.VehicleType, CreatedDate = dto.CreatedDate, ModifiedDate = dto.ModifiedDate, UploadedDate = dto.UploadedDate };

		public static DtoScenarioEntry ToDtoEntry(this TblSC5File table)
			=> new(table.Id, table.Name);

		public static TblSC5File ToTable(this DtoScenarioEntry dto)
			=> new() { Name = dto.Name, Id = dto.Id };

		public static DtoAuthorEntry ToDtoEntry(this TblAuthor table)
			=> new(table.Id, table.Name);

		public static TblAuthor ToTable(this DtoAuthorEntry dto)
			=> new() { Name = dto.Name, Id = dto.Id };

		public static DtoTagEntry ToDtoEntry(this TblTag table)
			=> new(table.Id, table.Name);

		public static TblTag ToTable(this DtoTagEntry dto)
			=> new() { Name = dto.Name, Id = dto.Id };

		public static DtoLicenceEntry ToDtoEntry(this TblLicence table)
			=> new(table.Id, table.Name, table.Text);

		public static TblLicence ToTable(this DtoLicenceEntry dto)
			=> new() { Name = dto.Name, Id = dto.Id, Text = dto.LicenceText };

		public static DtoItemPackEntry ToDtoEntry(this TblObjectPack x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.CreatedDate,
				x.ModifiedDate,
				x.UploadedDate,
				x.Licence?.ToDtoEntry());
		public static TblObjectPack ToTable(this DtoItemPackEntry dto)
			=> new() { Id = dto.Id, Name = dto.Name, Description = dto.Description, CreatedDate = dto.CreatedDate, ModifiedDate = dto.ModifiedDate, UploadedDate = dto.UploadedDate, Licence = dto.Licence?.ToTable() };

		public static TblObjectPack ToTable(this DtoItemPackDescriptor<DtoObjectEntry> x)
			=> new()
			{
				Id = x.Id,
				Name = x.Name,
				Description = x.Description,
				Objects = [.. x.Items.Select(x => x.ToTable())],
				Authors = [.. x.Authors.Select(x => x.ToTable())],
				CreatedDate = x.CreatedDate,
				ModifiedDate = x.ModifiedDate,
				UploadedDate = x.UploadedDate,
				Tags = [.. x.Tags.Select(x => x.ToTable())],
				Licence = x.Licence?.ToTable()
			};

		public static DtoItemPackDescriptor<DtoScenarioEntry> ToDtoEntry(this TblSC5FilePack x)
			=> new(
				x.Id,
				x.Name,
				x.Description,
				x.CreatedDate,
				x.ModifiedDate,
				x.UploadedDate,
				[.. x.SC5Files.Select(x => x.ToDtoEntry())],
				[.. x.Authors.Select(x => x.ToDtoEntry())],
				[.. x.Tags.Select(x => x.ToDtoEntry())],
				x.Licence?.ToDtoEntry());

		public static TblSC5FilePack ToTable(this DtoItemPackDescriptor<DtoScenarioEntry> x)
			=> new()
			{
				Id = x.Id,
				Name = x.Name,
				Description = x.Description,
				SC5Files = [.. x.Items.Select(x => x.ToTable())],
				Authors = [.. x.Authors.Select(x => x.ToTable())],
				CreatedDate = x.CreatedDate,
				ModifiedDate = x.ModifiedDate,
				UploadedDate = x.UploadedDate,
				Tags = [.. x.Tags.Select(x => x.ToTable())],
				Licence = x.Licence?.ToTable()
			};

		#endregion
	}
}
