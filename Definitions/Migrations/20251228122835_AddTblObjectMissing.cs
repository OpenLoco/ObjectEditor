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
            migrationBuilder.RenameColumn(
                name: "ShipWakeOffset",
                table: "ObjVehicle",
                newName: "ShipWakeSpacing");

            migrationBuilder.RenameColumn(
                name: "Clearance",
                table: "ObjTree",
                newName: "var_05");

            migrationBuilder.RenameColumn(
                name: "DisplayOffset",
                table: "ObjTrack",
                newName: "VehicleDisplayListVerticalOffset");

            migrationBuilder.RenameColumn(
                name: "DisplayOffset",
                table: "ObjRoad",
                newName: "VehicleDisplayListVerticalOffset");

            migrationBuilder.RenameColumn(
                name: "WindowPlayerColor",
                table: "ObjInterface",
                newName: "WindowPlayerColour");

            migrationBuilder.RenameColumn(
                name: "Flags",
                table: "ObjHillShapes",
                newName: "IsHeightMap");

            migrationBuilder.RenameColumn(
                name: "MaxPremiumRate",
                table: "ObjCargo",
                newName: "NonPremiumRate");

            migrationBuilder.RenameColumn(
                name: "AllowedPlaneTypes",
                table: "ObjAirport",
                newName: "Flags");

            migrationBuilder.AddColumn<byte>(
                name: "InitialHeight",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SeasonalVariants",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "var_04",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "MissingObjects",
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
                    table.PrimaryKey("PK_MissingObjects", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MissingObjects_DatName_DatChecksum",
                table: "MissingObjects",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissingObjects");

            migrationBuilder.DropColumn(
                name: "InitialHeight",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "SeasonalVariants",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "var_04",
                table: "ObjTree");

            migrationBuilder.RenameColumn(
                name: "ShipWakeSpacing",
                table: "ObjVehicle",
                newName: "ShipWakeOffset");

            migrationBuilder.RenameColumn(
                name: "var_05",
                table: "ObjTree",
                newName: "Clearance");

            migrationBuilder.RenameColumn(
                name: "VehicleDisplayListVerticalOffset",
                table: "ObjTrack",
                newName: "DisplayOffset");

            migrationBuilder.RenameColumn(
                name: "VehicleDisplayListVerticalOffset",
                table: "ObjRoad",
                newName: "DisplayOffset");

            migrationBuilder.RenameColumn(
                name: "WindowPlayerColour",
                table: "ObjInterface",
                newName: "WindowPlayerColor");

            migrationBuilder.RenameColumn(
                name: "IsHeightMap",
                table: "ObjHillShapes",
                newName: "Flags");

            migrationBuilder.RenameColumn(
                name: "NonPremiumRate",
                table: "ObjCargo",
                newName: "MaxPremiumRate");

            migrationBuilder.RenameColumn(
                name: "Flags",
                table: "ObjAirport",
                newName: "AllowedPlaneTypes");
        }
    }
}
