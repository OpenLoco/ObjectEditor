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
                    table.PrimaryKey("PK_Licences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectPacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastEditDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UploadDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectPacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectPacks_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatName = table.Column<string>(type: "TEXT", nullable: false),
                    DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
                    SourceGame = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectType = table.Column<byte>(type: "INTEGER", nullable: false),
                    VehicleType = table.Column<byte>(type: "INTEGER", nullable: true),
                    Availability = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastEditDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UploadDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objects_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SC5FilePacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastEditDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UploadDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SC5FilePacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SC5FilePacks_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SC5Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SourceGame = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastEditDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UploadDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SC5Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SC5Files_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblLocoObjectTblLocoObjectPack",
                columns: table => new
                {
                    ObjectPacksId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblLocoObjectTblLocoObjectPack", x => new { x.ObjectPacksId, x.ObjectsId });
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblLocoObjectPack_ObjectPacks_ObjectPacksId",
                        column: x => x.ObjectPacksId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblLocoObjectPack_Objects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    TblLocoObjectPackId = table.Column<int>(type: "INTEGER", nullable: true),
                    TblSC5FileId = table.Column<int>(type: "INTEGER", nullable: true),
                    TblSC5FilePackId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authors_ObjectPacks_TblLocoObjectPackId",
                        column: x => x.TblLocoObjectPackId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authors_Objects_TblLocoObjectId",
                        column: x => x.TblLocoObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authors_SC5FilePacks_TblSC5FilePackId",
                        column: x => x.TblSC5FilePackId,
                        principalTable: "SC5FilePacks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authors_SC5Files_TblSC5FileId",
                        column: x => x.TblSC5FileId,
                        principalTable: "SC5Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    TblLocoObjectPackId = table.Column<int>(type: "INTEGER", nullable: true),
                    TblSC5FileId = table.Column<int>(type: "INTEGER", nullable: true),
                    TblSC5FilePackId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_ObjectPacks_TblLocoObjectPackId",
                        column: x => x.TblLocoObjectPackId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Objects_TblLocoObjectId",
                        column: x => x.TblLocoObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_SC5FilePacks_TblSC5FilePackId",
                        column: x => x.TblSC5FilePackId,
                        principalTable: "SC5FilePacks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_SC5Files_TblSC5FileId",
                        column: x => x.TblSC5FileId,
                        principalTable: "SC5Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblSC5FileTblSC5FilePack",
                columns: table => new
                {
                    SC5FilePacksId = table.Column<int>(type: "INTEGER", nullable: false),
                    SC5FilesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSC5FileTblSC5FilePack", x => new { x.SC5FilePacksId, x.SC5FilesId });
                    table.ForeignKey(
                        name: "FK_TblSC5FileTblSC5FilePack_SC5FilePacks_SC5FilePacksId",
                        column: x => x.SC5FilePacksId,
                        principalTable: "SC5FilePacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblSC5FileTblSC5FilePack_SC5Files_SC5FilesId",
                        column: x => x.SC5FilesId,
                        principalTable: "SC5Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblLocoObjectId",
                table: "Authors",
                column: "TblLocoObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblLocoObjectPackId",
                table: "Authors",
                column: "TblLocoObjectPackId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblSC5FileId",
                table: "Authors",
                column: "TblSC5FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblSC5FilePackId",
                table: "Authors",
                column: "TblSC5FilePackId");

            migrationBuilder.CreateIndex(
                name: "IX_Licences_Name",
                table: "Licences",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectPacks_LicenceId",
                table: "ObjectPacks",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectPacks_Name",
                table: "ObjectPacks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objects_DatName_DatChecksum",
                table: "Objects",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true,
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_Objects_LicenceId",
                table: "Objects",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_Name",
                table: "Objects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SC5FilePacks_LicenceId",
                table: "SC5FilePacks",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SC5FilePacks_Name",
                table: "SC5FilePacks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SC5Files_LicenceId",
                table: "SC5Files",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SC5Files_Name",
                table: "SC5Files",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblLocoObjectId",
                table: "Tags",
                column: "TblLocoObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblLocoObjectPackId",
                table: "Tags",
                column: "TblLocoObjectPackId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblSC5FileId",
                table: "Tags",
                column: "TblSC5FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblSC5FilePackId",
                table: "Tags",
                column: "TblSC5FilePackId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLocoObjectTblLocoObjectPack_ObjectsId",
                table: "TblLocoObjectTblLocoObjectPack",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSC5FileTblSC5FilePack_SC5FilesId",
                table: "TblSC5FileTblSC5FilePack",
                column: "SC5FilesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "TblLocoObjectTblLocoObjectPack");

            migrationBuilder.DropTable(
                name: "TblSC5FileTblSC5FilePack");

            migrationBuilder.DropTable(
                name: "ObjectPacks");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "SC5FilePacks");

            migrationBuilder.DropTable(
                name: "SC5Files");

            migrationBuilder.DropTable(
                name: "Licences");
        }
    }
}
