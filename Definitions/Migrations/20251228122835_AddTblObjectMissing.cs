using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTblObjectMissing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.RenameColumn(
				name: "ShipWakeOffset",
				table: "ObjVehicle",
				newName: "ShipWakeSpacing");

			_ = migrationBuilder.RenameColumn(
				name: "Clearance",
				table: "ObjTree",
				newName: "var_05");

			_ = migrationBuilder.RenameColumn(
				name: "DisplayOffset",
				table: "ObjTrack",
				newName: "VehicleDisplayListVerticalOffset");

			_ = migrationBuilder.RenameColumn(
				name: "DisplayOffset",
				table: "ObjRoad",
				newName: "VehicleDisplayListVerticalOffset");

			_ = migrationBuilder.RenameColumn(
				name: "WindowPlayerColor",
				table: "ObjInterface",
				newName: "WindowPlayerColour");

			_ = migrationBuilder.RenameColumn(
				name: "Flags",
				table: "ObjHillShapes",
				newName: "IsHeightMap");

			_ = migrationBuilder.RenameColumn(
				name: "MaxPremiumRate",
				table: "ObjCargo",
				newName: "NonPremiumRate");

			_ = migrationBuilder.RenameColumn(
				name: "AllowedPlaneTypes",
				table: "ObjAirport",
				newName: "Flags");

			_ = migrationBuilder.AddColumn<byte>(
				name: "InitialHeight",
				table: "ObjTree",
				type: "INTEGER",
				nullable: false,
				defaultValue: (byte)0);

			_ = migrationBuilder.AddColumn<byte>(
				name: "SeasonalVariants",
				table: "ObjTree",
				type: "INTEGER",
				nullable: false,
				defaultValue: (byte)0);

			_ = migrationBuilder.AddColumn<byte>(
				name: "var_04",
				table: "ObjTree",
				type: "INTEGER",
				nullable: false,
				defaultValue: (byte)0);

			_ = migrationBuilder.CreateTable(
				name: "ObjectsMissing",
				columns: table => new
				{
					Id = table.Column<ulong>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					DatName = table.Column<string>(type: "TEXT", nullable: false),
					DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
					ObjectType = table.Column<byte>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjectsMissing", x => x.Id);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObjectsMissing_DatName_DatChecksum",
				table: "ObjectsMissing",
				columns: new[] { "DatName", "DatChecksum" },
				unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "ObjectsMissing");

			_ = migrationBuilder.DropColumn(
				name: "InitialHeight",
				table: "ObjTree");

			_ = migrationBuilder.DropColumn(
				name: "SeasonalVariants",
				table: "ObjTree");

			_ = migrationBuilder.DropColumn(
				name: "var_04",
				table: "ObjTree");

			_ = migrationBuilder.RenameColumn(
				name: "ShipWakeSpacing",
				table: "ObjVehicle",
				newName: "ShipWakeOffset");

			_ = migrationBuilder.RenameColumn(
				name: "var_05",
				table: "ObjTree",
				newName: "Clearance");

			_ = migrationBuilder.RenameColumn(
				name: "VehicleDisplayListVerticalOffset",
				table: "ObjTrack",
				newName: "DisplayOffset");

			_ = migrationBuilder.RenameColumn(
				name: "VehicleDisplayListVerticalOffset",
				table: "ObjRoad",
				newName: "DisplayOffset");

			_ = migrationBuilder.RenameColumn(
				name: "WindowPlayerColour",
				table: "ObjInterface",
				newName: "WindowPlayerColor");

			_ = migrationBuilder.RenameColumn(
				name: "IsHeightMap",
				table: "ObjHillShapes",
				newName: "Flags");

			_ = migrationBuilder.RenameColumn(
				name: "NonPremiumRate",
				table: "ObjCargo",
				newName: "MaxPremiumRate");

			_ = migrationBuilder.RenameColumn(
				name: "Flags",
				table: "ObjAirport",
				newName: "AllowedPlaneTypes");
        }
    }
}
