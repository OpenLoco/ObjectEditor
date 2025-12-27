using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class SubObjectClimate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.AddColumn<byte>(
			name: "FirstSeason",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SeasonLength1",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SeasonLength2",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SeasonLength3",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SeasonLength4",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SummerSnowLine",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WinterSnowLine",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropColumn(
			name: "FirstSeason",
			table: "ObjClimate");

		_ = migrationBuilder.DropColumn(
			name: "SeasonLength1",
			table: "ObjClimate");

		_ = migrationBuilder.DropColumn(
			name: "SeasonLength2",
			table: "ObjClimate");

		_ = migrationBuilder.DropColumn(
			name: "SeasonLength3",
			table: "ObjClimate");

		_ = migrationBuilder.DropColumn(
			name: "SeasonLength4",
			table: "ObjClimate");

		_ = migrationBuilder.DropColumn(
			name: "SummerSnowLine",
			table: "ObjClimate");

		_ = migrationBuilder.DropColumn(
			name: "WinterSnowLine",
			table: "ObjClimate");
        }
    }
