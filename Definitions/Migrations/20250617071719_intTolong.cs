using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoco.Definitions.Database.Migrations
{
	/// <inheritdoc />
	public partial class intTolong : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "Tags");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "StringTable");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "SC5Files");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "SC5FilePacks");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "Objects");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "ObjectPacks");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "Licences");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "DatObjects");

			_ = migrationBuilder.DropColumn(
				name: "GuidId",
				table: "Authors");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "Tags",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "StringTable",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "SC5Files",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "SC5FilePacks",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "Objects",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "ObjectPacks",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "Licences",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "DatObjects",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			_ = migrationBuilder.AddColumn<Guid>(
				name: "GuidId",
				table: "Authors",
				type: "TEXT",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
		}
	}
}
