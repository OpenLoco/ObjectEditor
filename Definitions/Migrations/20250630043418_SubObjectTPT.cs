using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
	/// <inheritdoc />
	public partial class SubObjectTPT : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropForeignKey(
				name: "FK_DatObjects_ObjHeader_ObjectId",
				table: "DatObjects");

			//migrationBuilder.DropForeignKey(
			//    name: "FK_ObjHeader_DbSubObject_SubObjectId",
			//    table: "ObjHeader");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_ObjHeader_Licences_LicenceId",
				table: "ObjHeader");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_StringTable_ObjHeader_ObjectId",
				table: "StringTable");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_TblAuthorTblObject_ObjHeader_ObjectsId",
				table: "TblAuthorTblObject");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_TblObjectTblObjectPack_ObjHeader_ObjectsId",
				table: "TblObjectTblObjectPack");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_TblObjectTblTag_ObjHeader_ObjectsId",
				table: "TblObjectTblTag");

			_ = migrationBuilder.DropPrimaryKey(
				name: "PK_ObjHeader",
				table: "ObjHeader");

			//_ = migrationBuilder.DropIndex(
			//	name: "IX_ObjHeader_SubObjectId",
			//	table: "ObjHeader");

			//_ = migrationBuilder.DropPrimaryKey(
			//	name: "PK_DbSubObject",
			//	table: "DbSubObject");

			_ = migrationBuilder.DropColumn(
				name: "SubObjectId",
				table: "ObjHeader");

			//_ = migrationBuilder.DropColumn(
			//	name: "Discriminator",
			//	table: "DbSubObject");

			_ = migrationBuilder.RenameTable(
				name: "ObjHeader",
				newName: "Objects");

			//_ = migrationBuilder.RenameTable(
			//	name: "DbSubObject",
			//	newName: "ObjWater");

			_ = migrationBuilder.RenameIndex(
				name: "IX_ObjHeader_Name",
				table: "Objects",
				newName: "IX_Objects_Name");

			_ = migrationBuilder.RenameIndex(
				name: "IX_ObjHeader_LicenceId",
				table: "Objects",
				newName: "IX_Objects_LicenceId");

			_ = migrationBuilder.AddPrimaryKey(
				name: "PK_Objects",
				table: "Objects",
				column: "Id");

			//_ = migrationBuilder.AddPrimaryKey(
			//	name: "PK_ObjWater",
			//	table: "ObjWater",
			//	column: "Id");

			_ = migrationBuilder.CreateTable(
				name: "ObjAirport",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjAirport", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjAirport_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjBridge",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjBridge", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjBridge_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjBuilding",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjBuilding", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjBuilding_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjCargo",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjCargo", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjCargo_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjCliffEdge",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjCliffEdge", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjCliffEdge_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjClimate",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjClimate", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjClimate_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjCompetitor",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjCompetitor", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjCompetitor_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjCurrency",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjCurrency", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjCurrency_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjDock",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjDock", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjDock_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjHillShapes",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjHillShapes", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjHillShapes_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjIndustry",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjIndustry", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjIndustry_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjInterface",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjInterface", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjInterface_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjLand",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjLand", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjLand_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjLevelCrossing",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjLevelCrossing", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjLevelCrossing_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjRegion",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjRegion", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjRegion_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjRoad",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjRoad", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjRoad_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjRoadExtra",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjRoadExtra", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjRoadExtra_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjRoadStation",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjRoadStation", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjRoadStation_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjScaffolding",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjScaffolding", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjScaffolding_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjScenarioText",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjScenarioText", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjScenarioText_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjSnow",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjSnow", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjSnow_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjSound",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjSound", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjSound_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjSteam",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjSteam", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjSteam_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjStreetLight",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjStreetLight", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjStreetLight_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjTownNames",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjTownNames", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjTownNames_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjTrack",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjTrack", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjTrack_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjTrackExtra",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjTrackExtra", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjTrackExtra_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjTrackSignal",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjTrackSignal", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjTrackSignal_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjTrackStation",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjTrackStation", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjTrackStation_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjTree",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjTree", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjTree_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjTunnel",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjTunnel", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjTunnel_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjVehicle",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjVehicle", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjVehicle_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjWall",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjWall", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjWall_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjWater",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjWater", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ObjWater_Objects_Id",
						column: x => x.Id,
						principalTable: "Objects",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.AddForeignKey(
				name: "FK_DatObjects_Objects_ObjectId",
				table: "DatObjects",
				column: "ObjectId",
				principalTable: "Objects",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_Objects_Licences_LicenceId",
				table: "Objects",
				column: "LicenceId",
				principalTable: "Licences",
				principalColumn: "Id");

			_ = migrationBuilder.AddForeignKey(
				name: "FK_ObjWater_Objects_Id",
				table: "ObjWater",
				column: "Id",
				principalTable: "Objects",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_StringTable_Objects_ObjectId",
				table: "StringTable",
				column: "ObjectId",
				principalTable: "Objects",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_TblAuthorTblObject_Objects_ObjectsId",
				table: "TblAuthorTblObject",
				column: "ObjectsId",
				principalTable: "Objects",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_TblObjectTblObjectPack_Objects_ObjectsId",
				table: "TblObjectTblObjectPack",
				column: "ObjectsId",
				principalTable: "Objects",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_TblObjectTblTag_Objects_ObjectsId",
				table: "TblObjectTblTag",
				column: "ObjectsId",
				principalTable: "Objects",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropForeignKey(
				name: "FK_DatObjects_Objects_ObjectId",
				table: "DatObjects");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_Objects_Licences_LicenceId",
				table: "Objects");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_ObjWater_Objects_Id",
				table: "ObjWater");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_StringTable_Objects_ObjectId",
				table: "StringTable");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_TblAuthorTblObject_Objects_ObjectsId",
				table: "TblAuthorTblObject");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_TblObjectTblObjectPack_Objects_ObjectsId",
				table: "TblObjectTblObjectPack");

			_ = migrationBuilder.DropForeignKey(
				name: "FK_TblObjectTblTag_Objects_ObjectsId",
				table: "TblObjectTblTag");

			_ = migrationBuilder.DropTable(
				name: "ObjAirport");

			_ = migrationBuilder.DropTable(
				name: "ObjBridge");

			_ = migrationBuilder.DropTable(
				name: "ObjBuilding");

			_ = migrationBuilder.DropTable(
				name: "ObjCargo");

			_ = migrationBuilder.DropTable(
				name: "ObjCliffEdge");

			_ = migrationBuilder.DropTable(
				name: "ObjClimate");

			_ = migrationBuilder.DropTable(
				name: "ObjCompetitor");

			_ = migrationBuilder.DropTable(
				name: "ObjCurrency");

			_ = migrationBuilder.DropTable(
				name: "ObjDock");

			_ = migrationBuilder.DropTable(
				name: "ObjHillShapes");

			_ = migrationBuilder.DropTable(
				name: "ObjIndustry");

			_ = migrationBuilder.DropTable(
				name: "ObjInterface");

			_ = migrationBuilder.DropTable(
				name: "ObjLand");

			_ = migrationBuilder.DropTable(
				name: "ObjLevelCrossing");

			_ = migrationBuilder.DropTable(
				name: "ObjRegion");

			_ = migrationBuilder.DropTable(
				name: "ObjRoad");

			_ = migrationBuilder.DropTable(
				name: "ObjRoadExtra");

			_ = migrationBuilder.DropTable(
				name: "ObjRoadStation");

			_ = migrationBuilder.DropTable(
				name: "ObjScaffolding");

			_ = migrationBuilder.DropTable(
				name: "ObjScenarioText");

			_ = migrationBuilder.DropTable(
				name: "ObjSnow");

			_ = migrationBuilder.DropTable(
				name: "ObjSound");

			_ = migrationBuilder.DropTable(
				name: "ObjSteam");

			_ = migrationBuilder.DropTable(
				name: "ObjStreetLight");

			_ = migrationBuilder.DropTable(
				name: "ObjTownNames");

			_ = migrationBuilder.DropTable(
				name: "ObjTrack");

			_ = migrationBuilder.DropTable(
				name: "ObjTrackExtra");

			_ = migrationBuilder.DropTable(
				name: "ObjTrackSignal");

			_ = migrationBuilder.DropTable(
				name: "ObjTrackStation");

			_ = migrationBuilder.DropTable(
				name: "ObjTree");

			_ = migrationBuilder.DropTable(
				name: "ObjTunnel");

			_ = migrationBuilder.DropTable(
				name: "ObjVehicle");

			_ = migrationBuilder.DropTable(
				name: "ObjWall");

			_ = migrationBuilder.DropPrimaryKey(
				name: "PK_Objects",
				table: "Objects");

			_ = migrationBuilder.DropPrimaryKey(
				name: "PK_ObjWater",
				table: "ObjWater");

			_ = migrationBuilder.RenameTable(
				name: "Objects",
				newName: "ObjHeader");

			_ = migrationBuilder.RenameTable(
				name: "ObjWater",
				newName: "DbSubObject");

			_ = migrationBuilder.RenameIndex(
				name: "IX_Objects_Name",
				table: "ObjHeader",
				newName: "IX_ObjHeader_Name");

			_ = migrationBuilder.RenameIndex(
				name: "IX_Objects_LicenceId",
				table: "ObjHeader",
				newName: "IX_ObjHeader_LicenceId");

			_ = migrationBuilder.AddColumn<ulong>(
				name: "SubObjectId",
				table: "ObjHeader",
				type: "INTEGER",
				nullable: false,
				defaultValue: 0ul);

			_ = migrationBuilder.AddColumn<string>(
				name: "Discriminator",
				table: "DbSubObject",
				type: "TEXT",
				maxLength: 34,
				nullable: false,
				defaultValue: "");

			_ = migrationBuilder.AddPrimaryKey(
				name: "PK_ObjHeader",
				table: "ObjHeader",
				column: "Id");

			_ = migrationBuilder.AddPrimaryKey(
				name: "PK_DbSubObject",
				table: "DbSubObject",
				column: "Id");

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObjHeader_SubObjectId",
				table: "ObjHeader",
				column: "SubObjectId",
				unique: true);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_DatObjects_ObjHeader_ObjectId",
				table: "DatObjects",
				column: "ObjectId",
				principalTable: "ObjHeader",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_ObjHeader_DbSubObject_SubObjectId",
				table: "ObjHeader",
				column: "SubObjectId",
				principalTable: "DbSubObject",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_ObjHeader_Licences_LicenceId",
				table: "ObjHeader",
				column: "LicenceId",
				principalTable: "Licences",
				principalColumn: "Id");

			_ = migrationBuilder.AddForeignKey(
				name: "FK_StringTable_ObjHeader_ObjectId",
				table: "StringTable",
				column: "ObjectId",
				principalTable: "ObjHeader",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_TblAuthorTblObject_ObjHeader_ObjectsId",
				table: "TblAuthorTblObject",
				column: "ObjectsId",
				principalTable: "ObjHeader",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_TblObjectTblObjectPack_ObjHeader_ObjectsId",
				table: "TblObjectTblObjectPack",
				column: "ObjectsId",
				principalTable: "ObjHeader",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_TblObjectTblTag_ObjHeader_ObjectsId",
				table: "TblObjectTblTag",
				column: "ObjectsId",
				principalTable: "ObjHeader",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
