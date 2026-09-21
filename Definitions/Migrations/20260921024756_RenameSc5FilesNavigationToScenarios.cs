using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class RenameSc5FilesNavigationToScenarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblAuthorTblScenario_Scenarios_SC5FilesId",
                table: "TblAuthorTblScenario");

            migrationBuilder.DropForeignKey(
                name: "FK_TblScenarioTblTag_Scenarios_SC5FilesId",
                table: "TblScenarioTblTag");

            migrationBuilder.RenameColumn(
                name: "SC5FilesId",
                table: "TblScenarioTblTag",
                newName: "ScenariosId");

            migrationBuilder.RenameColumn(
                name: "SC5FilesId",
                table: "TblAuthorTblScenario",
                newName: "ScenariosId");

            migrationBuilder.RenameIndex(
                name: "IX_TblAuthorTblScenario_SC5FilesId",
                table: "TblAuthorTblScenario",
                newName: "IX_TblAuthorTblScenario_ScenariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblAuthorTblScenario_Scenarios_ScenariosId",
                table: "TblAuthorTblScenario",
                column: "ScenariosId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblScenarioTblTag_Scenarios_ScenariosId",
                table: "TblScenarioTblTag",
                column: "ScenariosId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblAuthorTblScenario_Scenarios_ScenariosId",
                table: "TblAuthorTblScenario");

            migrationBuilder.DropForeignKey(
                name: "FK_TblScenarioTblTag_Scenarios_ScenariosId",
                table: "TblScenarioTblTag");

            migrationBuilder.RenameColumn(
                name: "ScenariosId",
                table: "TblScenarioTblTag",
                newName: "SC5FilesId");

            migrationBuilder.RenameColumn(
                name: "ScenariosId",
                table: "TblAuthorTblScenario",
                newName: "SC5FilesId");

            migrationBuilder.RenameIndex(
                name: "IX_TblAuthorTblScenario_ScenariosId",
                table: "TblAuthorTblScenario",
                newName: "IX_TblAuthorTblScenario_SC5FilesId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblAuthorTblScenario_Scenarios_SC5FilesId",
                table: "TblAuthorTblScenario",
                column: "SC5FilesId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblScenarioTblTag_Scenarios_SC5FilesId",
                table: "TblScenarioTblTag",
                column: "SC5FilesId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
