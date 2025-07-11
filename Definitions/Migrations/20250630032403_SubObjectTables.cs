using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class SubObjectTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatObjects_Objects_ObjectId",
                table: "DatObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Objects_Licences_LicenceId",
                table: "Objects");

            migrationBuilder.DropForeignKey(
                name: "FK_StringTable_Objects_ObjectId",
                table: "StringTable");

            migrationBuilder.DropForeignKey(
                name: "FK_TblAuthorTblObject_Objects_ObjectsId",
                table: "TblAuthorTblObject");

            migrationBuilder.DropForeignKey(
                name: "FK_TblObjectTblObjectPack_Objects_ObjectsId",
                table: "TblObjectTblObjectPack");

            migrationBuilder.DropForeignKey(
                name: "FK_TblObjectTblTag_Objects_ObjectsId",
                table: "TblObjectTblTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Objects",
                table: "Objects");

            migrationBuilder.RenameTable(
                name: "Objects",
                newName: "ObjHeader");

            migrationBuilder.RenameColumn(
                name: "RowText",
                table: "StringTable",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "RowName",
                table: "StringTable",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "RowLanguage",
                table: "StringTable",
                newName: "Language");

            migrationBuilder.RenameIndex(
                name: "IX_StringTable_RowText",
                table: "StringTable",
                newName: "IX_StringTable_Text");

            migrationBuilder.RenameIndex(
                name: "IX_Objects_Name",
                table: "ObjHeader",
                newName: "IX_ObjHeader_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Objects_LicenceId",
                table: "ObjHeader",
                newName: "IX_ObjHeader_LicenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObjHeader",
                table: "ObjHeader",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ObjAirport",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjAirport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjAirport_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjBridge",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjBridge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjBridge_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjBuilding",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjBuilding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjBuilding_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCargo",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCargo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCargo_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCliffEdge",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCliffEdge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCliffEdge_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjClimate",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjClimate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjClimate_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCompetitor",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCompetitor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCompetitor_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCurrency",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCurrency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCurrency_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjDock",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjDock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjDock_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjHillShapes",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjHillShapes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjHillShapes_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjIndustry",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjIndustry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjIndustry_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjInterface",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjInterface", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjInterface_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjLand",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjLand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjLand_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjLevelCrossing",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjLevelCrossing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjLevelCrossing_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRegion",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRegion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRegion_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRoad",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRoad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRoad_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRoadExtra",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRoadExtra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRoadExtra_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRoadStation",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRoadStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRoadStation_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjScaffolding",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjScaffolding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjScaffolding_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjScenarioText",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjScenarioText", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjScenarioText_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjSnow",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjSnow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjSnow_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjSound",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjSound", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjSound_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjSteam",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjSteam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjSteam_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjStreetLight",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjStreetLight", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjStreetLight_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTownNames",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTownNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTownNames_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrack",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrack_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrackExtra",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrackExtra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrackExtra_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrackSignal",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrackSignal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrackSignal_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrackStation",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrackStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrackStation_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTree",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTree", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTree_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTunnel",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTunnel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTunnel_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjWall",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjWall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjWall_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjWater",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjWater", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjWater_ObjHeader_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjAirport_ParentId",
                table: "ObjAirport",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjBridge_ParentId",
                table: "ObjBridge",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjBuilding_ParentId",
                table: "ObjBuilding",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCargo_ParentId",
                table: "ObjCargo",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCliffEdge_ParentId",
                table: "ObjCliffEdge",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjClimate_ParentId",
                table: "ObjClimate",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCompetitor_ParentId",
                table: "ObjCompetitor",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCurrency_ParentId",
                table: "ObjCurrency",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjDock_ParentId",
                table: "ObjDock",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjHillShapes_ParentId",
                table: "ObjHillShapes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjIndustry_ParentId",
                table: "ObjIndustry",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjInterface_ParentId",
                table: "ObjInterface",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjLand_ParentId",
                table: "ObjLand",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjLevelCrossing_ParentId",
                table: "ObjLevelCrossing",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRegion_ParentId",
                table: "ObjRegion",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoad_ParentId",
                table: "ObjRoad",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoadExtra_ParentId",
                table: "ObjRoadExtra",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoadStation_ParentId",
                table: "ObjRoadStation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjScaffolding_ParentId",
                table: "ObjScaffolding",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjScenarioText_ParentId",
                table: "ObjScenarioText",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjSnow_ParentId",
                table: "ObjSnow",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjSound_ParentId",
                table: "ObjSound",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjSteam_ParentId",
                table: "ObjSteam",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjStreetLight_ParentId",
                table: "ObjStreetLight",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTownNames_ParentId",
                table: "ObjTownNames",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrack_ParentId",
                table: "ObjTrack",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackExtra_ParentId",
                table: "ObjTrackExtra",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackSignal_ParentId",
                table: "ObjTrackSignal",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackStation_ParentId",
                table: "ObjTrackStation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTree_ParentId",
                table: "ObjTree",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTunnel_ParentId",
                table: "ObjTunnel",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjWall_ParentId",
                table: "ObjWall",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjWater_ParentId",
                table: "ObjWater",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatObjects_ObjHeader_ObjectId",
                table: "DatObjects",
                column: "ObjectId",
                principalTable: "ObjHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObjHeader_Licences_LicenceId",
                table: "ObjHeader",
                column: "LicenceId",
                principalTable: "Licences",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StringTable_ObjHeader_ObjectId",
                table: "StringTable",
                column: "ObjectId",
                principalTable: "ObjHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblAuthorTblObject_ObjHeader_ObjectsId",
                table: "TblAuthorTblObject",
                column: "ObjectsId",
                principalTable: "ObjHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblObjectTblObjectPack_ObjHeader_ObjectsId",
                table: "TblObjectTblObjectPack",
                column: "ObjectsId",
                principalTable: "ObjHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblObjectTblTag_ObjHeader_ObjectsId",
                table: "TblObjectTblTag",
                column: "ObjectsId",
                principalTable: "ObjHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatObjects_ObjHeader_ObjectId",
                table: "DatObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjHeader_Licences_LicenceId",
                table: "ObjHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_StringTable_ObjHeader_ObjectId",
                table: "StringTable");

            migrationBuilder.DropForeignKey(
                name: "FK_TblAuthorTblObject_ObjHeader_ObjectsId",
                table: "TblAuthorTblObject");

            migrationBuilder.DropForeignKey(
                name: "FK_TblObjectTblObjectPack_ObjHeader_ObjectsId",
                table: "TblObjectTblObjectPack");

            migrationBuilder.DropForeignKey(
                name: "FK_TblObjectTblTag_ObjHeader_ObjectsId",
                table: "TblObjectTblTag");

            migrationBuilder.DropTable(
                name: "ObjAirport");

            migrationBuilder.DropTable(
                name: "ObjBridge");

            migrationBuilder.DropTable(
                name: "ObjBuilding");

            migrationBuilder.DropTable(
                name: "ObjCargo");

            migrationBuilder.DropTable(
                name: "ObjCliffEdge");

            migrationBuilder.DropTable(
                name: "ObjClimate");

            migrationBuilder.DropTable(
                name: "ObjCompetitor");

            migrationBuilder.DropTable(
                name: "ObjCurrency");

            migrationBuilder.DropTable(
                name: "ObjDock");

            migrationBuilder.DropTable(
                name: "ObjHillShapes");

            migrationBuilder.DropTable(
                name: "ObjIndustry");

            migrationBuilder.DropTable(
                name: "ObjInterface");

            migrationBuilder.DropTable(
                name: "ObjLand");

            migrationBuilder.DropTable(
                name: "ObjLevelCrossing");

            migrationBuilder.DropTable(
                name: "ObjRegion");

            migrationBuilder.DropTable(
                name: "ObjRoad");

            migrationBuilder.DropTable(
                name: "ObjRoadExtra");

            migrationBuilder.DropTable(
                name: "ObjRoadStation");

            migrationBuilder.DropTable(
                name: "ObjScaffolding");

            migrationBuilder.DropTable(
                name: "ObjScenarioText");

            migrationBuilder.DropTable(
                name: "ObjSnow");

            migrationBuilder.DropTable(
                name: "ObjSound");

            migrationBuilder.DropTable(
                name: "ObjSteam");

            migrationBuilder.DropTable(
                name: "ObjStreetLight");

            migrationBuilder.DropTable(
                name: "ObjTownNames");

            migrationBuilder.DropTable(
                name: "ObjTrack");

            migrationBuilder.DropTable(
                name: "ObjTrackExtra");

            migrationBuilder.DropTable(
                name: "ObjTrackSignal");

            migrationBuilder.DropTable(
                name: "ObjTrackStation");

            migrationBuilder.DropTable(
                name: "ObjTree");

            migrationBuilder.DropTable(
                name: "ObjTunnel");

            migrationBuilder.DropTable(
                name: "ObjWall");

            migrationBuilder.DropTable(
                name: "ObjWater");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObjHeader",
                table: "ObjHeader");

            migrationBuilder.RenameTable(
                name: "ObjHeader",
                newName: "Objects");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "StringTable",
                newName: "RowText");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "StringTable",
                newName: "RowName");

            migrationBuilder.RenameColumn(
                name: "Language",
                table: "StringTable",
                newName: "RowLanguage");

            migrationBuilder.RenameIndex(
                name: "IX_StringTable_Text",
                table: "StringTable",
                newName: "IX_StringTable_RowText");

            migrationBuilder.RenameIndex(
                name: "IX_ObjHeader_Name",
                table: "Objects",
                newName: "IX_Objects_Name");

            migrationBuilder.RenameIndex(
                name: "IX_ObjHeader_LicenceId",
                table: "Objects",
                newName: "IX_Objects_LicenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Objects",
                table: "Objects",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DatObjects_Objects_ObjectId",
                table: "DatObjects",
                column: "ObjectId",
                principalTable: "Objects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Objects_Licences_LicenceId",
                table: "Objects",
                column: "LicenceId",
                principalTable: "Licences",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StringTable_Objects_ObjectId",
                table: "StringTable",
                column: "ObjectId",
                principalTable: "Objects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblAuthorTblObject_Objects_ObjectsId",
                table: "TblAuthorTblObject",
                column: "ObjectsId",
                principalTable: "Objects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblObjectTblObjectPack_Objects_ObjectsId",
                table: "TblObjectTblObjectPack",
                column: "ObjectsId",
                principalTable: "Objects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblObjectTblTag_Objects_ObjectsId",
                table: "TblObjectTblTag",
                column: "ObjectsId",
                principalTable: "Objects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
