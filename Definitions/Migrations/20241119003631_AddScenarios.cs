using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
	/// <inheritdoc />
	public partial class AddScenarios : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropTable(
				name: "TblLocoObjectTblModpack");

			_ = migrationBuilder.DropTable(
				name: "Modpacks");

			_ = migrationBuilder.AddColumn<int>(
				name: "TblSCV5FileId",
				table: "Tags",
				type: "INTEGER",
				nullable: true);

			_ = migrationBuilder.AddColumn<int>(
				name: "TblSCV5FileId",
				table: "Authors",
				type: "INTEGER",
				nullable: true);

			_ = migrationBuilder.CreateTable(
				name: "ObjectPacks",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Description = table.Column<string>(type: "TEXT", nullable: true),
					AuthorId = table.Column<int>(type: "INTEGER", nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjectPacks", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjectPacks_Authors_AuthorId",
						column: x => x.AuthorId,
						principalTable: "Authors",
						principalColumn: "Id");
				});

			_ = migrationBuilder.CreateTable(
				name: "SCV5FilePacks",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Description = table.Column<string>(type: "TEXT", nullable: true),
					AuthorId = table.Column<int>(type: "INTEGER", nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_SCV5FilePacks", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_SCV5FilePacks_Authors_AuthorId",
						column: x => x.AuthorId,
						principalTable: "Authors",
						principalColumn: "Id");
				});

			_ = migrationBuilder.CreateTable(
				name: "SCV5Files",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					UniqueName = table.Column<string>(type: "TEXT", nullable: false),
					DatName = table.Column<string>(type: "TEXT", nullable: false),
					DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
					ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
					Description = table.Column<string>(type: "TEXT", nullable: true),
					CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
					LastEditDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
					UploadDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')"),
					LicenceId = table.Column<int>(type: "INTEGER", nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_SCV5Files", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_SCV5Files_Licences_LicenceId",
						column: x => x.LicenceId,
						principalTable: "Licences",
						principalColumn: "Id");
				});

			_ = migrationBuilder.CreateTable(
				name: "TblLocoObjectTblLocoObjectPack",
				columns: table => new
				{
					ObjectPacksId = table.Column<int>(type: "INTEGER", nullable: false),
					ObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TblLocoObjectTblLocoObjectPack", x => new { x.ObjectPacksId, x.ObjectsId });
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblLocoObjectPack_ObjectPacks_ObjectPacksId",
						column: x => x.ObjectPacksId,
						principalTable: "ObjectPacks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblLocoObjectPack_Objects_ObjectsId",
						column: x => x.ObjectsId,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "TblSCV5FileTblSCV5FilePack",
				columns: table => new
				{
					FilesId = table.Column<int>(type: "INTEGER", nullable: false),
					SCV5FilePacksId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TblSCV5FileTblSCV5FilePack", x => new { x.FilesId, x.SCV5FilePacksId });
					_ = table.ForeignKey(
						name: "FK_TblSCV5FileTblSCV5FilePack_SCV5FilePacks_SCV5FilePacksId",
						column: x => x.SCV5FilePacksId,
						principalTable: "SCV5FilePacks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_TblSCV5FileTblSCV5FilePack_SCV5Files_FilesId",
						column: x => x.FilesId,
						principalTable: "SCV5Files",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_Tags_TblSCV5FileId",
				table: "Tags",
				column: "TblSCV5FileId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Authors_TblSCV5FileId",
				table: "Authors",
				column: "TblSCV5FileId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObjectPacks_AuthorId",
				table: "ObjectPacks",
				column: "AuthorId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObjectPacks_Name",
				table: "ObjectPacks",
				column: "Name",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_SCV5FilePacks_AuthorId",
				table: "SCV5FilePacks",
				column: "AuthorId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_SCV5FilePacks_Name",
				table: "SCV5FilePacks",
				column: "Name",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_SCV5Files_DatName_DatChecksum",
				table: "SCV5Files",
				columns: new[] { "DatName", "DatChecksum" },
				unique: true,
				descending: new[] { true, false });

			_ = migrationBuilder.CreateIndex(
				name: "IX_SCV5Files_LicenceId",
				table: "SCV5Files",
				column: "LicenceId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_SCV5Files_UniqueName",
				table: "SCV5Files",
				column: "UniqueName",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_TblLocoObjectTblLocoObjectPack_ObjectsId",
				table: "TblLocoObjectTblLocoObjectPack",
				column: "ObjectsId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_TblSCV5FileTblSCV5FilePack_SCV5FilePacksId",
				table: "TblSCV5FileTblSCV5FilePack",
				column: "SCV5FilePacksId");

			_ = migrationBuilder.AddForeignKey(
				name: "FK_Authors_SCV5Files_TblSCV5FileId",
				table: "Authors",
				column: "TblSCV5FileId",
				principalTable: "SCV5Files",
				principalColumn: "Id");

			_ = migrationBuilder.AddForeignKey(
				name: "FK_Tags_SCV5Files_TblSCV5FileId",
				table: "Tags",
				column: "TblSCV5FileId",
				principalTable: "SCV5Files",
				principalColumn: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropForeignKey(
				name: "FK_Authors_SCV5Files_TblSCV5FileId",
				table: "Authors");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_Tags_SCV5Files_TblSCV5FileId",
				table: "Tags");

			_ = migrationBuilder.DropTable(
				name: "TblLocoObjectTblLocoObjectPack");

			_ = migrationBuilder.DropTable(
				name: "TblSCV5FileTblSCV5FilePack");

			_ = migrationBuilder.DropTable(
				name: "ObjectPacks");

			_ = migrationBuilder.DropTable(
				name: "SCV5FilePacks");

			_ = migrationBuilder.DropTable(
				name: "SCV5Files");

			_ = migrationBuilder.DropIndex(
				name: "IX_Tags_TblSCV5FileId",
				table: "Tags");

			_ = migrationBuilder.DropIndex(
				name: "IX_Authors_TblSCV5FileId",
				table: "Authors");

			_ = migrationBuilder.DropColumn(
				name: "TblSCV5FileId",
				table: "Tags");

			_ = migrationBuilder.DropColumn(
				name: "TblSCV5FileId",
				table: "Authors");

			_ = migrationBuilder.CreateTable(
				name: "Modpacks",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					AuthorId = table.Column<int>(type: "INTEGER", nullable: true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Modpacks", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_Modpacks_Authors_AuthorId",
						column: x => x.AuthorId,
						principalTable: "Authors",
						principalColumn: "Id");
				});

			_ = migrationBuilder.CreateTable(
				name: "TblLocoObjectTblModpack",
				columns: table => new
				{
					ModpacksId = table.Column<int>(type: "INTEGER", nullable: false),
					ObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TblLocoObjectTblModpack", x => new { x.ModpacksId, x.ObjectsId });
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblModpack_Modpacks_ModpacksId",
						column: x => x.ModpacksId,
						principalTable: "Modpacks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblModpack_Objects_ObjectsId",
						column: x => x.ObjectsId,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_Modpacks_AuthorId",
				table: "Modpacks",
				column: "AuthorId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Modpacks_Name",
				table: "Modpacks",
				column: "Name",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_TblLocoObjectTblModpack_ObjectsId",
				table: "TblLocoObjectTblModpack",
				column: "ObjectsId");
		}
	}
}
