using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: true),
                    PasswordHashed = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblRoleTblUser",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRoleTblUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_TblRoleTblUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblRoleTblUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblRoleTblUser_UsersId",
                table: "TblRoleTblUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthorId",
                table: "Users",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DisplayName",
                table: "Users",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordSalt",
                table: "Users",
                column: "PasswordSalt",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblRoleTblUser");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
