using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class UnknownBulkUpgrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClosingFrames",
                table: "ObjLevelCrossing",
                newName: "TransitionAnimationFrameCount");

            migrationBuilder.RenameColumn(
                name: "ClosedFrames",
                table: "ObjLevelCrossing",
                newName: "TransitionAnimationDelayBitmask");

            migrationBuilder.RenameColumn(
                name: "AnimationSpeed",
                table: "ObjLevelCrossing",
                newName: "ClosedAnimationFrameInterval");

            migrationBuilder.RenameColumn(
                name: "PlayerInfoToolbarColour",
                table: "ObjInterface",
                newName: "CompanyInfoToolbarColour");

            migrationBuilder.RenameColumn(
                name: "AvailablePlaystyles",
                table: "ObjCompetitor",
                newName: "AvailableNameSuffixes");

            migrationBuilder.AddColumn<byte>(
                name: "ClosedAnimationFrameCount",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAnimationFrameCount",
                table: "ObjLevelCrossing");

            migrationBuilder.RenameColumn(
                name: "TransitionAnimationFrameCount",
                table: "ObjLevelCrossing",
                newName: "ClosingFrames");

            migrationBuilder.RenameColumn(
                name: "TransitionAnimationDelayBitmask",
                table: "ObjLevelCrossing",
                newName: "ClosedFrames");

            migrationBuilder.RenameColumn(
                name: "ClosedAnimationFrameInterval",
                table: "ObjLevelCrossing",
                newName: "AnimationSpeed");

            migrationBuilder.RenameColumn(
                name: "CompanyInfoToolbarColour",
                table: "ObjInterface",
                newName: "PlayerInfoToolbarColour");

            migrationBuilder.RenameColumn(
                name: "AvailableNameSuffixes",
                table: "ObjCompetitor",
                newName: "AvailablePlaystyles");
        }
    }
}
