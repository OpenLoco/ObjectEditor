//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace Definitions.Migrations
//{
//	/// <inheritdoc />
//	public partial class AddIdentity : Migration
//	{
//		/// <inheritdoc />
//		protected override void Up(MigrationBuilder migrationBuilder)
//		{
//			_ = migrationBuilder.CreateTable(
//				name: "AspNetRoles",
//				columns: table => new
//				{
//					Id = table.Column<string>(type: "TEXT", nullable: false),
//					Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
//					NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
//					ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
//				},
//				constraints: table =>
//				{
//					_ = table.PrimaryKey("PK_AspNetRoles", x => x.Id);
//				});

//			_ = migrationBuilder.CreateTable(
//				name: "AspNetUsers",
//				columns: table => new
//				{
//					Id = table.Column<string>(type: "TEXT", nullable: false),
//					UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
//					NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
//					Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
//					NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
//					EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
//					PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
//					SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
//					ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
//					PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
//					PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
//					TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
//					LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
//					LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
//					AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
//				},
//				constraints: table =>
//				{
//					_ = table.PrimaryKey("PK_AspNetUsers", x => x.Id);
//				});

//			_ = migrationBuilder.CreateTable(
//				name: "AspNetRoleClaims",
//				columns: table => new
//				{
//					Id = table.Column<int>(type: "INTEGER", nullable: false)
//						.Annotation("Sqlite:Autoincrement", true),
//					RoleId = table.Column<string>(type: "TEXT", nullable: false),
//					ClaimType = table.Column<string>(type: "TEXT", nullable: true),
//					ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
//				},
//				constraints: table =>
//				{
//					_ = table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
//					_ = table.ForeignKey(
//						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
//						column: x => x.RoleId,
//						principalTable: "AspNetRoles",
//						principalColumn: "Id",
//						onDelete: ReferentialAction.Cascade);
//				});

//			_ = migrationBuilder.CreateTable(
//				name: "AspNetUserClaims",
//				columns: table => new
//				{
//					Id = table.Column<int>(type: "INTEGER", nullable: false)
//						.Annotation("Sqlite:Autoincrement", true),
//					UserId = table.Column<string>(type: "TEXT", nullable: false),
//					ClaimType = table.Column<string>(type: "TEXT", nullable: true),
//					ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
//				},
//				constraints: table =>
//				{
//					_ = table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
//					_ = table.ForeignKey(
//						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
//						column: x => x.UserId,
//						principalTable: "AspNetUsers",
//						principalColumn: "Id",
//						onDelete: ReferentialAction.Cascade);
//				});

//			_ = migrationBuilder.CreateTable(
//				name: "AspNetUserLogins",
//				columns: table => new
//				{
//					LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
//					ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
//					ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
//					UserId = table.Column<string>(type: "TEXT", nullable: false)
//				},
//				constraints: table =>
//				{
//					_ = table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
//					_ = table.ForeignKey(
//						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
//						column: x => x.UserId,
//						principalTable: "AspNetUsers",
//						principalColumn: "Id",
//						onDelete: ReferentialAction.Cascade);
//				});

//			_ = migrationBuilder.CreateTable(
//				name: "AspNetUserRoles",
//				columns: table => new
//				{
//					UserId = table.Column<string>(type: "TEXT", nullable: false),
//					RoleId = table.Column<string>(type: "TEXT", nullable: false)
//				},
//				constraints: table =>
//				{
//					_ = table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
//					_ = table.ForeignKey(
//						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
//						column: x => x.RoleId,
//						principalTable: "AspNetRoles",
//						principalColumn: "Id",
//						onDelete: ReferentialAction.Cascade);
//					_ = table.ForeignKey(
//						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
//						column: x => x.UserId,
//						principalTable: "AspNetUsers",
//						principalColumn: "Id",
//						onDelete: ReferentialAction.Cascade);
//				});

//			_ = migrationBuilder.CreateTable(
//				name: "AspNetUserTokens",
//				columns: table => new
//				{
//					UserId = table.Column<string>(type: "TEXT", nullable: false),
//					LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
//					Name = table.Column<string>(type: "TEXT", nullable: false),
//					Value = table.Column<string>(type: "TEXT", nullable: true)
//				},
//				constraints: table =>
//				{
//					_ = table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
//					_ = table.ForeignKey(
//						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
//						column: x => x.UserId,
//						principalTable: "AspNetUsers",
//						principalColumn: "Id",
//						onDelete: ReferentialAction.Cascade);
//				});

//			_ = migrationBuilder.CreateIndex(
//				name: "IX_AspNetRoleClaims_RoleId",
//				table: "AspNetRoleClaims",
//				column: "RoleId");

//			_ = migrationBuilder.CreateIndex(
//				name: "RoleNameIndex",
//				table: "AspNetRoles",
//				column: "NormalizedName",
//				unique: true);

//			_ = migrationBuilder.CreateIndex(
//				name: "IX_AspNetUserClaims_UserId",
//				table: "AspNetUserClaims",
//				column: "UserId");

//			_ = migrationBuilder.CreateIndex(
//				name: "IX_AspNetUserLogins_UserId",
//				table: "AspNetUserLogins",
//				column: "UserId");

//			_ = migrationBuilder.CreateIndex(
//				name: "IX_AspNetUserRoles_RoleId",
//				table: "AspNetUserRoles",
//				column: "RoleId");

//			_ = migrationBuilder.CreateIndex(
//				name: "EmailIndex",
//				table: "AspNetUsers",
//				column: "NormalizedEmail");

//			_ = migrationBuilder.CreateIndex(
//				name: "UserNameIndex",
//				table: "AspNetUsers",
//				column: "NormalizedUserName",
//				unique: true);
//		}

//		/// <inheritdoc />
//		protected override void Down(MigrationBuilder migrationBuilder)
//		{
//			_ = migrationBuilder.DropTable(
//				name: "AspNetRoleClaims");

//			_ = migrationBuilder.DropTable(
//				name: "AspNetUserClaims");

//			_ = migrationBuilder.DropTable(
//				name: "AspNetUserLogins");

//			_ = migrationBuilder.DropTable(
//				name: "AspNetUserRoles");

//			_ = migrationBuilder.DropTable(
//				name: "AspNetUserTokens");

//			_ = migrationBuilder.DropTable(
//				name: "AspNetRoles");

//			_ = migrationBuilder.DropTable(
//				name: "AspNetUsers");
//		}
//	}
//}
