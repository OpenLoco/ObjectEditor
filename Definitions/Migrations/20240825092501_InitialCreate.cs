using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    TblAuthorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.TblAuthorId);
                });

            migrationBuilder.CreateTable(
                name: "Licences",
                columns: table => new
                {
                    TblLicenceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licences", x => x.TblLicenceId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TblTagId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TblTagId);
                });

            migrationBuilder.CreateTable(
                name: "Modpacks",
                columns: table => new
                {
                    TblModpackId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorTblAuthorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modpacks", x => x.TblModpackId);
                    table.ForeignKey(
                        name: "FK_Modpacks_Authors_AuthorTblAuthorId",
                        column: x => x.AuthorTblAuthorId,
                        principalTable: "Authors",
                        principalColumn: "TblAuthorId");
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    TblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PathOnDisk = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalName = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
                    IsVanilla = table.Column<bool>(type: "INTEGER", nullable: false),
                    ObjectType = table.Column<byte>(type: "INTEGER", nullable: false),
                    VehicleType = table.Column<byte>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorTblAuthorId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastEditDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UploadDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')"),
                    Availability = table.Column<int>(type: "INTEGER", nullable: false),
                    LicenceTblLicenceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.TblLocoObjectId);
                    table.ForeignKey(
                        name: "FK_Objects_Authors_AuthorTblAuthorId",
                        column: x => x.AuthorTblAuthorId,
                        principalTable: "Authors",
                        principalColumn: "TblAuthorId");
                    table.ForeignKey(
                        name: "FK_Objects_Licences_LicenceTblLicenceId",
                        column: x => x.LicenceTblLicenceId,
                        principalTable: "Licences",
                        principalColumn: "TblLicenceId");
                });

            migrationBuilder.CreateTable(
                name: "TblLocoObjectTblModpack",
                columns: table => new
                {
                    ModpacksTblModpackId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectsTblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblLocoObjectTblModpack", x => new { x.ModpacksTblModpackId, x.ObjectsTblLocoObjectId });
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblModpack_Modpacks_ModpacksTblModpackId",
                        column: x => x.ModpacksTblModpackId,
                        principalTable: "Modpacks",
                        principalColumn: "TblModpackId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblModpack_Objects_ObjectsTblLocoObjectId",
                        column: x => x.ObjectsTblLocoObjectId,
                        principalTable: "Objects",
                        principalColumn: "TblLocoObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblLocoObjectTblTag",
                columns: table => new
                {
                    ObjectsTblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsTblTagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblLocoObjectTblTag", x => new { x.ObjectsTblLocoObjectId, x.TagsTblTagId });
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblTag_Objects_ObjectsTblLocoObjectId",
                        column: x => x.ObjectsTblLocoObjectId,
                        principalTable: "Objects",
                        principalColumn: "TblLocoObjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblTag_Tags_TagsTblTagId",
                        column: x => x.TagsTblTagId,
                        principalTable: "Tags",
                        principalColumn: "TblTagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Licences_Name",
                table: "Licences",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modpacks_AuthorTblAuthorId",
                table: "Modpacks",
                column: "AuthorTblAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Modpacks_Name",
                table: "Modpacks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objects_AuthorTblAuthorId",
                table: "Objects",
                column: "AuthorTblAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_LicenceTblLicenceId",
                table: "Objects",
                column: "LicenceTblLicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_Name",
                table: "Objects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objects_OriginalName_OriginalChecksum",
                table: "Objects",
                columns: new[] { "OriginalName", "OriginalChecksum" },
                unique: true,
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_Objects_PathOnDisk",
                table: "Objects",
                column: "PathOnDisk",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblLocoObjectTblModpack_ObjectsTblLocoObjectId",
                table: "TblLocoObjectTblModpack",
                column: "ObjectsTblLocoObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLocoObjectTblTag_TagsTblTagId",
                table: "TblLocoObjectTblTag",
                column: "TagsTblTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblLocoObjectTblModpack");

            migrationBuilder.DropTable(
                name: "TblLocoObjectTblTag");

            migrationBuilder.DropTable(
                name: "Modpacks");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Licences");
        }
    }
}
