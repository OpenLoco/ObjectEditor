using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoco.Schema.Migrations
{
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
					TblAuthorId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_Authors", x => x.TblAuthorId));

			_ = migrationBuilder.CreateTable(
				name: "Licences",
				columns: table => new
				{
					TblLicenceId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Text = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_Licences", x => x.TblLicenceId));

			_ = migrationBuilder.CreateTable(
				name: "Modpacks",
				columns: table => new
				{
					TblModpackId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_Modpacks", x => x.TblModpackId));

			_ = migrationBuilder.CreateTable(
				name: "Tags",
				columns: table => new
				{
					TblTagId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_Tags", x => x.TblTagId));

			_ = migrationBuilder.CreateTable(
				name: "Objects",
				columns: table => new
				{
					TblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					OriginalName = table.Column<string>(type: "TEXT", nullable: false),
					OriginalChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
					OriginalBytes = table.Column<byte[]>(type: "BLOB", nullable: false),
					SourceGame = table.Column<byte>(type: "INTEGER", nullable: false),
					ObjectType = table.Column<byte>(type: "INTEGER", nullable: false),
					VehicleType = table.Column<byte>(type: "INTEGER", nullable: true),
					Description = table.Column<string>(type: "TEXT", nullable: true),
					AuthorTblAuthorId = table.Column<int>(type: "INTEGER", nullable: true),
					CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
					LastEditDate = table.Column<DateTime>(type: "TEXT", nullable: true),
					Availability = table.Column<int>(type: "INTEGER", nullable: false),
					LicenceTblLicenceId = table.Column<int>(type: "INTEGER", nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Objects", x => x.TblLocoObjectId);
					_ = table.ForeignKey(
						name: "FK_Objects_Authors_AuthorTblAuthorId",
						column: x => x.AuthorTblAuthorId,
						principalTable: "Authors",
						principalColumn: "TblAuthorId");
					_ = table.ForeignKey(
						name: "FK_Objects_Licences_LicenceTblLicenceId",
						column: x => x.LicenceTblLicenceId,
						principalTable: "Licences",
						principalColumn: "TblLicenceId");
				});

			_ = migrationBuilder.CreateTable(
				name: "TblLocoObjectTblModpack",
				columns: table => new
				{
					ModpacksTblModpackId = table.Column<int>(type: "INTEGER", nullable: false),
					ObjectsTblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TblLocoObjectTblModpack", x => new { x.ModpacksTblModpackId, x.ObjectsTblLocoObjectId });
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblModpack_Modpacks_ModpacksTblModpackId",
						column: x => x.ModpacksTblModpackId,
						principalTable: "Modpacks",
						principalColumn: "TblModpackId",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblModpack_Objects_ObjectsTblLocoObjectId",
						column: x => x.ObjectsTblLocoObjectId,
						principalTable: "Objects",
						principalColumn: "TblLocoObjectId",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "TblLocoObjectTblTag",
				columns: table => new
				{
					ObjectsTblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false),
					TagsTblTagId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TblLocoObjectTblTag", x => new { x.ObjectsTblLocoObjectId, x.TagsTblTagId });
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblTag_Objects_ObjectsTblLocoObjectId",
						column: x => x.ObjectsTblLocoObjectId,
						principalTable: "Objects",
						principalColumn: "TblLocoObjectId",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_TblLocoObjectTblTag_Tags_TagsTblTagId",
						column: x => x.TagsTblTagId,
						principalTable: "Tags",
						principalColumn: "TblTagId",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_Objects_AuthorTblAuthorId",
				table: "Objects",
				column: "AuthorTblAuthorId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Objects_LicenceTblLicenceId",
				table: "Objects",
				column: "LicenceTblLicenceId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_TblLocoObjectTblModpack_ObjectsTblLocoObjectId",
				table: "TblLocoObjectTblModpack",
				column: "ObjectsTblLocoObjectId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_TblLocoObjectTblTag_TagsTblTagId",
				table: "TblLocoObjectTblTag",
				column: "TagsTblTagId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropTable(
				name: "TblLocoObjectTblModpack");

			_ = migrationBuilder.DropTable(
				name: "TblLocoObjectTblTag");

			_ = migrationBuilder.DropTable(
				name: "Modpacks");

			_ = migrationBuilder.DropTable(
				name: "Objects");

			_ = migrationBuilder.DropTable(
				name: "Tags");

			_ = migrationBuilder.DropTable(
				name: "Authors");

			_ = migrationBuilder.DropTable(
				name: "Licences");
		}
	}
}