using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class AddGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "TblStringTable",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "Tags",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "SC5Files",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "SC5FilePacks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "Objects",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "ObjectPacks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "Licences",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "DatObjects",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "TblStringTable");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "SC5Files");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "SC5FilePacks");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "ObjectPacks");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "Licences");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "DatObjects");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "Authors");
        }
    }
}
