using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
	/// <inheritdoc />
	public partial class LinkUserToAuthor2 : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.DropForeignKey(
			//    name: "FK_AspNetUsers_Authors_AssociatedAuthorId",
			//    table: "AspNetUsers");

			//migrationBuilder.DropIndex(
			//    name: "IX_AspNetUsers_AssociatedAuthorId",
			//    table: "AspNetUsers");

			//_ = migrationBuilder.DropColumn(
			//	name: "AssociatedAuthorId",
			//	table: "Authors");

			//migrationBuilder.AlterColumn<ulong>(
			//    name: "AssociatedAuthorId",
			//    table: "AspNetUsers",
			//    type: "INTEGER",
			//    nullable: true,
			//    oldClrType: typeof(ulong),
			//    oldType: "INTEGER");

			//migrationBuilder.CreateIndex(
			//    name: "IX_AspNetUsers_AssociatedAuthorId",
			//    table: "AspNetUsers",
			//    column: "AssociatedAuthorId");

			//migrationBuilder.AddForeignKey(
			//    name: "FK_AspNetUsers_Authors_AssociatedAuthorId",
			//    table: "AspNetUsers",
			//    column: "AssociatedAuthorId",
			//    principalTable: "Authors",
			//    principalColumn: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropForeignKey(
				name: "FK_AspNetUsers_Authors_AssociatedAuthorId",
				table: "AspNetUsers");

			_ = migrationBuilder.DropIndex(
				name: "IX_AspNetUsers_AssociatedAuthorId",
				table: "AspNetUsers");

			_ = migrationBuilder.AddColumn<ulong>(
				name: "AssociatedAuthorId",
				table: "Authors",
				type: "INTEGER",
				nullable: false,
				defaultValue: 0ul);

			_ = migrationBuilder.AlterColumn<ulong>(
				name: "AssociatedAuthorId",
				table: "AspNetUsers",
				type: "INTEGER",
				nullable: false,
				defaultValue: 0ul,
				oldClrType: typeof(ulong),
				oldType: "INTEGER",
				oldNullable: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_AssociatedAuthorId",
				table: "AspNetUsers",
				column: "AssociatedAuthorId",
				unique: true);

			_ = migrationBuilder.AddForeignKey(
				name: "FK_AspNetUsers_Authors_AssociatedAuthorId",
				table: "AspNetUsers",
				column: "AssociatedAuthorId",
				principalTable: "Authors",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
