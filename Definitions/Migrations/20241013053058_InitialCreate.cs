﻿using System;
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
                name: "Modpacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modpacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modpacks_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UniqueName = table.Column<string>(type: "TEXT", nullable: false),
                    DatName = table.Column<string>(type: "TEXT", nullable: false),
                    DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectType = table.Column<byte>(type: "INTEGER", nullable: false),
                    VehicleType = table.Column<byte>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastEditDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UploadDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')"),
                    Availability = table.Column<int>(type: "INTEGER", nullable: false),
                    LicenceId = table.Column<int>(type: "INTEGER", nullable: true)
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
                name: "TblLocoObjectTblModpack",
                columns: table => new
                {
                    ModpacksId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblLocoObjectTblModpack", x => new { x.ModpacksId, x.ObjectsId });
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblModpack_Modpacks_ModpacksId",
                        column: x => x.ModpacksId,
                        principalTable: "Modpacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblLocoObjectTblModpack_Objects_ObjectsId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Licences_Name",
                table: "Licences",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modpacks_AuthorId",
                table: "Modpacks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Modpacks_Name",
                table: "Modpacks",
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
                name: "IX_Objects_UniqueName",
                table: "Objects",
                column: "UniqueName",
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
                name: "IX_TblLocoObjectTblModpack_ObjectsId",
                table: "TblLocoObjectTblModpack",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLocoObjectTblTag_TagsId",
                table: "TblLocoObjectTblTag",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblAuthorTblLocoObject");

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
