using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.CreateTable(
			name: "Authors",
			columns: table => new
			{
				Id = table.Column<int>(type: "INTEGER", nullable: false)
					.Annotation("Sqlite:Autoincrement", true),
				Name = table.Column<string>(type: "TEXT", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_Authors", x => x.Id);
			});

		_ = migrationBuilder.CreateTable(
			name: "Licences",
			columns: table => new
			{
				Id = table.Column<int>(type: "INTEGER", nullable: false)
					.Annotation("Sqlite:Autoincrement", true),
				Name = table.Column<string>(type: "TEXT", nullable: false),
				Text = table.Column<string>(type: "TEXT", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_Licences", x => x.Id);
			});

		_ = migrationBuilder.CreateTable(
			name: "Tags",
			columns: table => new
			{
				Id = table.Column<int>(type: "INTEGER", nullable: false)
					.Annotation("Sqlite:Autoincrement", true),
				Name = table.Column<string>(type: "TEXT", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_Tags", x => x.Id);
			});

		_ = migrationBuilder.CreateTable(
			name: "ObjectPacks",
			columns: table => new
			{
				Id = table.Column<int>(type: "INTEGER", nullable: false)
					.Annotation("Sqlite:Autoincrement", true),
				Name = table.Column<string>(type: "TEXT", nullable: false),
				Description = table.Column<string>(type: "TEXT", nullable: true),
				LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
				CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_ObjectPacks", x => x.Id);
				_ = table.ForeignKey(
					name: "FK_ObjectPacks_Licences_LicenceId",
					column: x => x.LicenceId,
					principalTable: "Licences",
					principalColumn: "Id");
			});

		_ = migrationBuilder.CreateTable(
			name: "Objects",
			columns: table => new
			{
				Id = table.Column<int>(type: "INTEGER", nullable: false)
					.Annotation("Sqlite:Autoincrement", true),
				DatName = table.Column<string>(type: "TEXT", nullable: false),
				DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
				ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
				ObjectType = table.Column<byte>(type: "INTEGER", nullable: false),
				VehicleType = table.Column<byte>(type: "INTEGER", nullable: true),
				Availability = table.Column<int>(type: "INTEGER", nullable: false),
				Name = table.Column<string>(type: "TEXT", nullable: false),
				Description = table.Column<string>(type: "TEXT", nullable: true),
				LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
				CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_Objects", x => x.Id);
				_ = table.ForeignKey(
					name: "FK_Objects_Licences_LicenceId",
					column: x => x.LicenceId,
					principalTable: "Licences",
					principalColumn: "Id");
			});

		_ = migrationBuilder.CreateTable(
			name: "SC5FilePacks",
			columns: table => new
			{
				Id = table.Column<int>(type: "INTEGER", nullable: false)
					.Annotation("Sqlite:Autoincrement", true),
				Name = table.Column<string>(type: "TEXT", nullable: false),
				Description = table.Column<string>(type: "TEXT", nullable: true),
				LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
				CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_SC5FilePacks", x => x.Id);
				_ = table.ForeignKey(
					name: "FK_SC5FilePacks_Licences_LicenceId",
					column: x => x.LicenceId,
					principalTable: "Licences",
					principalColumn: "Id");
			});

		_ = migrationBuilder.CreateTable(
			name: "SC5Files",
			columns: table => new
			{
				Id = table.Column<int>(type: "INTEGER", nullable: false)
					.Annotation("Sqlite:Autoincrement", true),
				ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
				Name = table.Column<string>(type: "TEXT", nullable: false),
				Description = table.Column<string>(type: "TEXT", nullable: true),
				LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
				CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
				UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_SC5Files", x => x.Id);
				_ = table.ForeignKey(
					name: "FK_SC5Files_Licences_LicenceId",
					column: x => x.LicenceId,
					principalTable: "Licences",
					principalColumn: "Id");
			});

		_ = migrationBuilder.CreateTable(
			name: "TblAuthorTblObjectPack",
			columns: table => new
			{
				AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
				ObjectPacksId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblAuthorTblObjectPack", x => new { x.AuthorsId, x.ObjectPacksId });
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblObjectPack_Authors_AuthorsId",
					column: x => x.AuthorsId,
					principalTable: "Authors",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblObjectPack_ObjectPacks_ObjectPacksId",
					column: x => x.ObjectPacksId,
					principalTable: "ObjectPacks",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblObjectPackTblTag",
			columns: table => new
			{
				ObjectPacksId = table.Column<int>(type: "INTEGER", nullable: false),
				TagsId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblObjectPackTblTag", x => new { x.ObjectPacksId, x.TagsId });
				_ = table.ForeignKey(
					name: "FK_TblObjectPackTblTag_ObjectPacks_ObjectPacksId",
					column: x => x.ObjectPacksId,
					principalTable: "ObjectPacks",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblObjectPackTblTag_Tags_TagsId",
					column: x => x.TagsId,
					principalTable: "Tags",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblAuthorTblObject",
			columns: table => new
			{
				AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
				ObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblAuthorTblObject", x => new { x.AuthorsId, x.ObjectsId });
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblObject_Authors_AuthorsId",
					column: x => x.AuthorsId,
					principalTable: "Authors",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblObject_Objects_ObjectsId",
					column: x => x.ObjectsId,
					principalTable: "Objects",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblObjectTblObjectPack",
			columns: table => new
			{
				ObjectPacksId = table.Column<int>(type: "INTEGER", nullable: false),
				ObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblObjectTblObjectPack", x => new { x.ObjectPacksId, x.ObjectsId });
				_ = table.ForeignKey(
					name: "FK_TblObjectTblObjectPack_ObjectPacks_ObjectPacksId",
					column: x => x.ObjectPacksId,
					principalTable: "ObjectPacks",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblObjectTblObjectPack_Objects_ObjectsId",
					column: x => x.ObjectsId,
					principalTable: "Objects",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblObjectTblTag",
			columns: table => new
			{
				ObjectsId = table.Column<int>(type: "INTEGER", nullable: false),
				TagsId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblObjectTblTag", x => new { x.ObjectsId, x.TagsId });
				_ = table.ForeignKey(
					name: "FK_TblObjectTblTag_Objects_ObjectsId",
					column: x => x.ObjectsId,
					principalTable: "Objects",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblObjectTblTag_Tags_TagsId",
					column: x => x.TagsId,
					principalTable: "Tags",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblAuthorTblSC5FilePack",
			columns: table => new
			{
				AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
				SC5FilePacksId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblAuthorTblSC5FilePack", x => new { x.AuthorsId, x.SC5FilePacksId });
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblSC5FilePack_Authors_AuthorsId",
					column: x => x.AuthorsId,
					principalTable: "Authors",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblSC5FilePack_SC5FilePacks_SC5FilePacksId",
					column: x => x.SC5FilePacksId,
					principalTable: "SC5FilePacks",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblSC5FilePackTblTag",
			columns: table => new
			{
				SC5FilePacksId = table.Column<int>(type: "INTEGER", nullable: false),
				TagsId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblSC5FilePackTblTag", x => new { x.SC5FilePacksId, x.TagsId });
				_ = table.ForeignKey(
					name: "FK_TblSC5FilePackTblTag_SC5FilePacks_SC5FilePacksId",
					column: x => x.SC5FilePacksId,
					principalTable: "SC5FilePacks",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblSC5FilePackTblTag_Tags_TagsId",
					column: x => x.TagsId,
					principalTable: "Tags",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblAuthorTblSC5File",
			columns: table => new
			{
				AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
				SC5FilesId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblAuthorTblSC5File", x => new { x.AuthorsId, x.SC5FilesId });
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblSC5File_Authors_AuthorsId",
					column: x => x.AuthorsId,
					principalTable: "Authors",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblAuthorTblSC5File_SC5Files_SC5FilesId",
					column: x => x.SC5FilesId,
					principalTable: "SC5Files",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblSC5FileTblSC5FilePack",
			columns: table => new
			{
				SC5FilePacksId = table.Column<int>(type: "INTEGER", nullable: false),
				SC5FilesId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblSC5FileTblSC5FilePack", x => new { x.SC5FilePacksId, x.SC5FilesId });
				_ = table.ForeignKey(
					name: "FK_TblSC5FileTblSC5FilePack_SC5FilePacks_SC5FilePacksId",
					column: x => x.SC5FilePacksId,
					principalTable: "SC5FilePacks",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblSC5FileTblSC5FilePack_SC5Files_SC5FilesId",
					column: x => x.SC5FilesId,
					principalTable: "SC5Files",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateTable(
			name: "TblSC5FileTblTag",
			columns: table => new
			{
				SC5FilesId = table.Column<int>(type: "INTEGER", nullable: false),
				TagsId = table.Column<int>(type: "INTEGER", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_TblSC5FileTblTag", x => new { x.SC5FilesId, x.TagsId });
				_ = table.ForeignKey(
					name: "FK_TblSC5FileTblTag_SC5Files_SC5FilesId",
					column: x => x.SC5FilesId,
					principalTable: "SC5Files",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				_ = table.ForeignKey(
					name: "FK_TblSC5FileTblTag_Tags_TagsId",
					column: x => x.TagsId,
					principalTable: "Tags",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateIndex(
			name: "IX_Licences_Name",
			table: "Licences",
			column: "Name",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjectPacks_LicenceId",
			table: "ObjectPacks",
			column: "LicenceId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjectPacks_Name",
			table: "ObjectPacks",
			column: "Name",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_Objects_DatName_DatChecksum",
			table: "Objects",
			columns: new[] { "DatName", "DatChecksum" },
			unique: true,
			descending: new[] { true, false });

		_ = migrationBuilder.CreateIndex(
			name: "IX_Objects_LicenceId",
			table: "Objects",
			column: "LicenceId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_Objects_Name",
			table: "Objects",
			column: "Name",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_SC5FilePacks_LicenceId",
			table: "SC5FilePacks",
			column: "LicenceId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_SC5FilePacks_Name",
			table: "SC5FilePacks",
			column: "Name",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_SC5Files_LicenceId",
			table: "SC5Files",
			column: "LicenceId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_SC5Files_Name",
			table: "SC5Files",
			column: "Name",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_Tags_Name",
			table: "Tags",
			column: "Name",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblAuthorTblObject_ObjectsId",
			table: "TblAuthorTblObject",
			column: "ObjectsId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblAuthorTblObjectPack_ObjectPacksId",
			table: "TblAuthorTblObjectPack",
			column: "ObjectPacksId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblAuthorTblSC5File_SC5FilesId",
			table: "TblAuthorTblSC5File",
			column: "SC5FilesId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblAuthorTblSC5FilePack_SC5FilePacksId",
			table: "TblAuthorTblSC5FilePack",
			column: "SC5FilePacksId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblObjectPackTblTag_TagsId",
			table: "TblObjectPackTblTag",
			column: "TagsId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblObjectTblObjectPack_ObjectsId",
			table: "TblObjectTblObjectPack",
			column: "ObjectsId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblObjectTblTag_TagsId",
			table: "TblObjectTblTag",
			column: "TagsId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblSC5FilePackTblTag_TagsId",
			table: "TblSC5FilePackTblTag",
			column: "TagsId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblSC5FileTblSC5FilePack_SC5FilesId",
			table: "TblSC5FileTblSC5FilePack",
			column: "SC5FilesId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_TblSC5FileTblTag_TagsId",
			table: "TblSC5FileTblTag",
			column: "TagsId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.DropTable(
			name: "TblAuthorTblObject");

		_ = migrationBuilder.DropTable(
			name: "TblAuthorTblObjectPack");

		_ = migrationBuilder.DropTable(
			name: "TblAuthorTblSC5File");

		_ = migrationBuilder.DropTable(
			name: "TblAuthorTblSC5FilePack");

		_ = migrationBuilder.DropTable(
			name: "TblObjectPackTblTag");

		_ = migrationBuilder.DropTable(
			name: "TblObjectTblObjectPack");

		_ = migrationBuilder.DropTable(
			name: "TblObjectTblTag");

		_ = migrationBuilder.DropTable(
			name: "TblSC5FilePackTblTag");

		_ = migrationBuilder.DropTable(
			name: "TblSC5FileTblSC5FilePack");

		_ = migrationBuilder.DropTable(
			name: "TblSC5FileTblTag");

		_ = migrationBuilder.DropTable(
			name: "Authors");

		_ = migrationBuilder.DropTable(
			name: "ObjectPacks");

		_ = migrationBuilder.DropTable(
			name: "Objects");

		_ = migrationBuilder.DropTable(
			name: "SC5FilePacks");

		_ = migrationBuilder.DropTable(
			name: "SC5Files");

		_ = migrationBuilder.DropTable(
			name: "Tags");

		_ = migrationBuilder.DropTable(
			name: "Licences");
	}
}
