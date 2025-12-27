using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class LinkUserToAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.AddColumn<ulong>(
			name: "AssociatedAuthorId",
			table: "Authors",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "UserId",
			table: "AspNetUserTokens",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT");

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "Id",
			table: "AspNetUsers",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT")
			.Annotation("Sqlite:Autoincrement", true);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "AssociatedAuthorId",
			table: "AspNetUsers",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "RoleId",
			table: "AspNetUserRoles",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT");

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "UserId",
			table: "AspNetUserRoles",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT");

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "UserId",
			table: "AspNetUserLogins",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT");

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "UserId",
			table: "AspNetUserClaims",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT");

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "Id",
			table: "AspNetRoles",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT")
			.Annotation("Sqlite:Autoincrement", true);

		_ = migrationBuilder.AlterColumn<ulong>(
			name: "RoleId",
			table: "AspNetRoleClaims",
			type: "INTEGER",
			nullable: false,
			oldClrType: typeof(Guid),
			oldType: "TEXT");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropForeignKey(
			name: "FK_AspNetUsers_Authors_AssociatedAuthorId",
			table: "AspNetUsers");

		_ = migrationBuilder.DropIndex(
			name: "IX_AspNetUsers_AssociatedAuthorId",
			table: "AspNetUsers");

		_ = migrationBuilder.DropColumn(
			name: "AssociatedAuthorId",
			table: "Authors");

		_ = migrationBuilder.DropColumn(
			name: "AssociatedAuthorId",
			table: "AspNetUsers");

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "UserId",
			table: "AspNetUserTokens",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER");

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "Id",
			table: "AspNetUsers",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER")
			.OldAnnotation("Sqlite:Autoincrement", true);

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "RoleId",
			table: "AspNetUserRoles",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER");

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "UserId",
			table: "AspNetUserRoles",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER");

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "UserId",
			table: "AspNetUserLogins",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER");

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "UserId",
			table: "AspNetUserClaims",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER");

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "Id",
			table: "AspNetRoles",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER")
			.OldAnnotation("Sqlite:Autoincrement", true);

		_ = migrationBuilder.AlterColumn<Guid>(
			name: "RoleId",
			table: "AspNetRoleClaims",
			type: "TEXT",
			nullable: false,
			oldClrType: typeof(ulong),
			oldType: "INTEGER");
        }
    }
