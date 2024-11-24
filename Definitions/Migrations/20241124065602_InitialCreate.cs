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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

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
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
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
                name: "TblAuthorTblLocoObjectPack",
                columns: table => new
                {
                    AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectPacksId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblLocoObjectPack", x => new { x.AuthorsId, x.ObjectPacksId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblLocoObjectPack_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblLocoObjectPack_ObjectPacks_ObjectPacksId",
                        column: x => x.ObjectPacksId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblLocoObjectPackTblTag",
                columns: table => new
                {
                    ObjectPacksId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblLocoObjectPackTblTag", x => new { x.ObjectPacksId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblLocoObjectPackTblTag_ObjectPacks_ObjectPacksId",
                        column: x => x.ObjectPacksId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblLocoObjectPackTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAuthorTblLocoObject",
                columns: table => new
                {
                    AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblLocoObject", x => new { x.AuthorsId, x.ObjectsId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblLocoObject_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblLocoObject_Objects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "TblLocoObjectTblTag",
                columns: table => new
                {
                    ObjectsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblLocoObjectTblTag", x => new { x.ObjectsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblTag_Objects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAuthorTblSC5FilePack",
                columns: table => new
                {
                    AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SC5FilePacksId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblSC5FilePack", x => new { x.AuthorsId, x.SC5FilePacksId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblSC5FilePack_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblSC5FilePack_SC5FilePacks_SC5FilePacksId",
                        column: x => x.SC5FilePacksId,
                        principalTable: "SC5FilePacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblSC5FilePackTblTag",
                columns: table => new
                {
                    SC5FilePacksId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSC5FilePackTblTag", x => new { x.SC5FilePacksId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblSC5FilePackTblTag_SC5FilePacks_SC5FilePacksId",
                        column: x => x.SC5FilePacksId,
                        principalTable: "SC5FilePacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblSC5FilePackTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAuthorTblSC5File",
                columns: table => new
                {
                    AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SC5FilesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblSC5File", x => new { x.AuthorsId, x.SC5FilesId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblSC5File_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblSC5File_SC5Files_SC5FilesId",
                        column: x => x.SC5FilesId,
                        principalTable: "SC5Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "TblSC5FileTblTag",
                columns: table => new
                {
                    SC5FilesId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSC5FileTblTag", x => new { x.SC5FilesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblSC5FileTblTag_SC5Files_SC5FilesId",
                        column: x => x.SC5FilesId,
                        principalTable: "SC5Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblSC5FileTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_TblAuthorTblLocoObject_ObjectsId",
                table: "TblAuthorTblLocoObject",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAuthorTblLocoObjectPack_ObjectPacksId",
                table: "TblAuthorTblLocoObjectPack",
                column: "ObjectPacksId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAuthorTblSC5File_SC5FilesId",
                table: "TblAuthorTblSC5File",
                column: "SC5FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAuthorTblSC5FilePack_SC5FilePacksId",
                table: "TblAuthorTblSC5FilePack",
                column: "SC5FilePacksId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLocoObjectPackTblTag_TagsId",
                table: "TblLocoObjectPackTblTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLocoObjectTblLocoObjectPack_ObjectsId",
                table: "TblLocoObjectTblLocoObjectPack",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLocoObjectTblTag_TagsId",
                table: "TblLocoObjectTblTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSC5FilePackTblTag_TagsId",
                table: "TblSC5FilePackTblTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSC5FileTblSC5FilePack_SC5FilesId",
                table: "TblSC5FileTblSC5FilePack",
                column: "SC5FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSC5FileTblTag_TagsId",
                table: "TblSC5FileTblTag",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblAuthorTblLocoObject");

            migrationBuilder.DropTable(
                name: "TblAuthorTblLocoObjectPack");

            migrationBuilder.DropTable(
                name: "TblAuthorTblSC5File");

            migrationBuilder.DropTable(
                name: "TblAuthorTblSC5FilePack");

            migrationBuilder.DropTable(
                name: "TblLocoObjectPackTblTag");

            migrationBuilder.DropTable(
                name: "TblLocoObjectTblLocoObjectPack");

            migrationBuilder.DropTable(
                name: "TblLocoObjectTblTag");

            migrationBuilder.DropTable(
                name: "TblSC5FilePackTblTag");

            migrationBuilder.DropTable(
                name: "TblSC5FileTblSC5FilePack");

            migrationBuilder.DropTable(
                name: "TblSC5FileTblTag");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "ObjectPacks");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "SC5FilePacks");

            migrationBuilder.DropTable(
                name: "SC5Files");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Licences");
        }
    }
}
