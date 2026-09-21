using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Licences",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectsMissing",
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
                    table.PrimaryKey("PK_ObjectsMissing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssociatedAuthorId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Graphics",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Graphics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Graphics_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Graphics_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Music",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Music", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Music_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Music_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObjectPacks",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectPacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectPacks_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObjectPacks_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubObjectId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ObjectType = table.Column<byte>(type: "INTEGER", nullable: false),
                    VehicleType = table.Column<byte>(type: "INTEGER", nullable: true),
                    Availability = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objects_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Objects_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenarioPacks",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioPacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioPacks_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenarioPacks_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scenarios_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Scenarios_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SoundEffects",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoundEffects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoundEffects_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SoundEffects_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tutorials",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LicenceId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    UploadedDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "date('now')"),
                    ObjectSource = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutorials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tutorials_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tutorials_Licences_LicenceId",
                        column: x => x.LicenceId,
                        principalTable: "Licences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DatObjects",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatName = table.Column<string>(type: "TEXT", nullable: false),
                    DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
                    xxHash3 = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ObjectId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatObjects_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjAirport",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildingComponents = table.Column<string>(type: "json", nullable: false),
                    LargeTiles = table.Column<uint>(type: "INTEGER", nullable: false),
                    MinX = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    MinY = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    MaxX = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    MaxY = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildingPositions = table.Column<string>(type: "json", nullable: false),
                    MovementNodes = table.Column<string>(type: "json", nullable: false),
                    MovementEdges = table.Column<string>(type: "json", nullable: false),
                    RequiredClearEdges = table.Column<uint>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjAirport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjAirport_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjBridge",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Flags = table.Column<byte>(type: "INTEGER", nullable: false),
                    ClearHeight = table.Column<ushort>(type: "INTEGER", nullable: false),
                    DeckDepth = table.Column<short>(type: "INTEGER", nullable: false),
                    SpanLength = table.Column<byte>(type: "INTEGER", nullable: false),
                    PillarSpacing = table.Column<byte>(type: "INTEGER", nullable: false),
                    MaxSpeed = table.Column<short>(type: "INTEGER", nullable: false),
                    MaxHeight = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    BaseCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    HeightCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    DisabledTrackFlags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CompatibleTrackObjects = table.Column<string>(type: "json", nullable: false),
                    CompatibleRoadObjects = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjBridge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjBridge_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjBuilding",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Flags = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<ushort>(type: "INTEGER", nullable: false),
                    DemolishRatingReduction = table.Column<short>(type: "INTEGER", nullable: false),
                    ScaffoldingSegmentType = table.Column<byte>(type: "INTEGER", nullable: false),
                    ScaffoldingColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    Colours = table.Column<uint>(type: "INTEGER", nullable: false),
                    GeneratorFunction = table.Column<byte>(type: "INTEGER", nullable: false),
                    AverageNumberOnMap = table.Column<byte>(type: "INTEGER", nullable: false),
                    BuildingComponents = table.Column<string>(type: "json", nullable: false),
                    ProducedQuantity = table.Column<string>(type: "json", nullable: false),
                    ProducedCargoType = table.Column<string>(type: "json", nullable: false),
                    ConsumedCargoType = table.Column<string>(type: "json", nullable: false),
                    ProducedCargoQuantity = table.Column<string>(type: "json", nullable: false),
                    ConsumedCargoQuantity = table.Column<string>(type: "json", nullable: false),
                    TownAmenityCategory = table.Column<byte>(type: "INTEGER", nullable: false),
                    ElevatorHeightSequences = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjBuilding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjBuilding_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCargo",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitWeight = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CargoTransferTime = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CargoCategory = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Flags = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumPlatformVariations = table.Column<byte>(type: "INTEGER", nullable: false),
                    StationCargoDensity = table.Column<byte>(type: "INTEGER", nullable: false),
                    PremiumDays = table.Column<byte>(type: "INTEGER", nullable: false),
                    MaxNonPremiumDays = table.Column<byte>(type: "INTEGER", nullable: false),
                    NonPremiumRate = table.Column<ushort>(type: "INTEGER", nullable: false),
                    PenaltyRate = table.Column<ushort>(type: "INTEGER", nullable: false),
                    PaymentFactor = table.Column<ushort>(type: "INTEGER", nullable: false),
                    PaymentIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    UnitSize = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCargo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCargo_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCliffEdge",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCliffEdge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCliffEdge_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjClimate",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstSeason = table.Column<int>(type: "INTEGER", nullable: false),
                    WinterSnowLine = table.Column<byte>(type: "INTEGER", nullable: false),
                    SummerSnowLine = table.Column<byte>(type: "INTEGER", nullable: false),
                    SeasonLength1 = table.Column<byte>(type: "INTEGER", nullable: false),
                    SeasonLength2 = table.Column<byte>(type: "INTEGER", nullable: false),
                    SeasonLength3 = table.Column<byte>(type: "INTEGER", nullable: false),
                    SeasonLength4 = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjClimate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjClimate_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCompetitor",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AvailableNamePrefixes = table.Column<uint>(type: "INTEGER", nullable: false),
                    AvailableNameSuffixes = table.Column<uint>(type: "INTEGER", nullable: false),
                    Emotions = table.Column<int>(type: "INTEGER", nullable: false),
                    Intelligence = table.Column<byte>(type: "INTEGER", nullable: false),
                    Aggressiveness = table.Column<byte>(type: "INTEGER", nullable: false),
                    Competitiveness = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCompetitor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCompetitor_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjCurrency",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Separator = table.Column<byte>(type: "INTEGER", nullable: false),
                    Factor = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjCurrency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjCurrency_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjDock",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildingComponents = table.Column<string>(type: "json", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BoatPosition = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjDock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjDock_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjHillShapes",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HillHeightMapCount = table.Column<byte>(type: "INTEGER", nullable: false),
                    MountainHeightMapCount = table.Column<byte>(type: "INTEGER", nullable: false),
                    IsHeightMap = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjHillShapes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjHillShapes_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjIndustry",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmImagesPerGrowthStage = table.Column<uint>(type: "INTEGER", nullable: false),
                    MinNumBuildings = table.Column<byte>(type: "INTEGER", nullable: false),
                    MaxNumBuildings = table.Column<byte>(type: "INTEGER", nullable: false),
                    Colours = table.Column<uint>(type: "INTEGER", nullable: false),
                    BuildingSizeFlags = table.Column<uint>(type: "INTEGER", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    TotalOfTypeInScenario = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    ScaffoldingSegmentType = table.Column<byte>(type: "INTEGER", nullable: false),
                    ScaffoldingColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    MapColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<uint>(type: "INTEGER", nullable: false),
                    FarmTileNumImageAngles = table.Column<byte>(type: "INTEGER", nullable: false),
                    FarmGrowthStageWithNoProduction = table.Column<byte>(type: "INTEGER", nullable: false),
                    FarmNumFields = table.Column<byte>(type: "INTEGER", nullable: false),
                    FarmNumStagesOfGrowth = table.Column<byte>(type: "INTEGER", nullable: false),
                    MonthlyClosureChance = table.Column<byte>(type: "INTEGER", nullable: false),
                    BuildingComponents = table.Column<string>(type: "json", nullable: false),
                    AnimationSequences = table.Column<string>(type: "json", nullable: false),
                    RandomAnimations = table.Column<string>(type: "json", nullable: false),
                    InitialProductionRate = table.Column<string>(type: "json", nullable: false),
                    ProducedCargo = table.Column<string>(type: "json", nullable: false),
                    RequiredCargo = table.Column<string>(type: "json", nullable: false),
                    NumFarmTileImages = table.Column<byte>(type: "INTEGER", nullable: false),
                    WallTypes = table.Column<string>(type: "json", nullable: false),
                    BuildingWall = table.Column<string>(type: "json", nullable: true),
                    BuildingWallEntrance = table.Column<string>(type: "json", nullable: true),
                    Buildings = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjIndustry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjIndustry_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjInterface",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MapTooltipObjectColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    MapTooltipCargoColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    TooltipColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    ErrorColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    WindowPlayerColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    WindowTitlebarColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    WindowColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    WindowConstructionColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    WindowTerraFormColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    WindowMapColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    WindowOptionsColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    Colour_11 = table.Column<byte>(type: "INTEGER", nullable: false),
                    TopToolbarPrimaryColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    TopToolbarSecondaryColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    TopToolbarTertiaryColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    TopToolbarQuaternaryColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    CompanyInfoToolbarColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    TimeToolbarColour = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjInterface", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjInterface_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjLand",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumGrowthStages = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumImageAngles = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    NumImagesPerGrowthStage = table.Column<uint>(type: "INTEGER", nullable: false),
                    DistributionPattern = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumVariations = table.Column<byte>(type: "INTEGER", nullable: false),
                    VariationLikelihood = table.Column<byte>(type: "INTEGER", nullable: false),
                    CliffEdgeHeader = table.Column<string>(type: "json", nullable: false),
                    ReplacementLandHeader = table.Column<string>(type: "json", nullable: true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjLand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjLand_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjLevelCrossing",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    ClosedAnimationFrameInterval = table.Column<byte>(type: "INTEGER", nullable: false),
                    ClosedAnimationFrameCount = table.Column<byte>(type: "INTEGER", nullable: false),
                    TransitionAnimationFrameCount = table.Column<byte>(type: "INTEGER", nullable: false),
                    TransitionAnimationDelayBitmask = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjLevelCrossing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjLevelCrossing_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRegion",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleDrivingSide = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CargoInfluenceObjects = table.Column<string>(type: "json", nullable: false),
                    DependentObjects = table.Column<string>(type: "json", nullable: false),
                    CargoInfluenceTownFilter = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRegion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRegion_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRoad",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoadPieces = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    TunnelCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    MaxCurveSpeed = table.Column<short>(type: "INTEGER", nullable: false),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    PaintStyle = table.Column<byte>(type: "INTEGER", nullable: false),
                    VehicleDisplayListVerticalOffset = table.Column<byte>(type: "INTEGER", nullable: false),
                    TargetTownSize = table.Column<byte>(type: "INTEGER", nullable: false),
                    Tunnel = table.Column<string>(type: "json", nullable: false),
                    Bridges = table.Column<string>(type: "json", nullable: false),
                    Stations = table.Column<string>(type: "json", nullable: false),
                    RoadMods = table.Column<string>(type: "json", nullable: false),
                    TracksAndRoads = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRoad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRoad_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRoadExtra",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoadPieces = table.Column<ushort>(type: "INTEGER", nullable: false),
                    PaintStyle = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRoadExtra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRoadExtra_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjRoadStation",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaintStyle = table.Column<byte>(type: "INTEGER", nullable: false),
                    Height = table.Column<byte>(type: "INTEGER", nullable: false),
                    RoadPieces = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<byte>(type: "INTEGER", nullable: false),
                    CompatibleRoadObjects = table.Column<string>(type: "json", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CargoType = table.Column<string>(type: "json", nullable: true),
                    CargoOffsets = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjRoadStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjRoadStation_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjScaffolding",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SegmentHeights = table.Column<string>(type: "json", nullable: false),
                    RoofHeights = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjScaffolding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjScaffolding_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjScenarioText",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjScenarioText", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjScenarioText_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjSnow",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjSnow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjSnow_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjSound",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShouldLoop = table.Column<byte>(type: "INTEGER", nullable: false),
                    Volume = table.Column<uint>(type: "INTEGER", nullable: false),
                    SoundObjectData = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjSound", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjSound_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjSteam",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumStationaryTicks = table.Column<byte>(type: "INTEGER", nullable: false),
                    SpriteWidth = table.Column<byte>(type: "INTEGER", nullable: false),
                    SpriteHeightNegative = table.Column<byte>(type: "INTEGER", nullable: false),
                    SpriteHeightPositive = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ImageOffset = table.Column<uint>(type: "INTEGER", nullable: false),
                    FrameInfoType0 = table.Column<string>(type: "json", nullable: false),
                    FrameInfoType1 = table.Column<string>(type: "json", nullable: false),
                    SoundEffects = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjSteam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjSteam_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjStreetLight",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DesignedYears = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjStreetLight", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjStreetLight_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTownNames",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MorphemeCategories = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTownNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTownNames_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrack",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrackPieces = table.Column<ushort>(type: "INTEGER", nullable: false),
                    StationTrackPieces = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    TunnelCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    MaxCurveSpeed = table.Column<short>(type: "INTEGER", nullable: false),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    VehicleDisplayListVerticalOffset = table.Column<byte>(type: "INTEGER", nullable: false),
                    var_06 = table.Column<byte>(type: "INTEGER", nullable: false),
                    Tunnel = table.Column<string>(type: "json", nullable: false),
                    TrackMods = table.Column<string>(type: "json", nullable: false),
                    Signals = table.Column<string>(type: "json", nullable: false),
                    TracksAndRoads = table.Column<string>(type: "json", nullable: false),
                    Bridges = table.Column<string>(type: "json", nullable: false),
                    Stations = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrack_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrackExtra",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrackPieces = table.Column<ushort>(type: "INTEGER", nullable: false),
                    PaintStyle = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrackExtra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrackExtra_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrackSignal",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    AnimationSpeed = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumFrames = table.Column<byte>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CompatibleTrackObjects = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrackSignal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrackSignal_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTrackStation",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaintStyle = table.Column<byte>(type: "INTEGER", nullable: false),
                    Height = table.Column<byte>(type: "INTEGER", nullable: false),
                    TrackPieces = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    SellCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    PlatformType = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<byte>(type: "INTEGER", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CompatibleTrackObjects = table.Column<string>(type: "json", nullable: false),
                    CargoOffsets = table.Column<string>(type: "json", nullable: false),
                    DiagonalCargoOffsetBytes = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTrackStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTrackStation_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTree",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InitialHeight = table.Column<byte>(type: "INTEGER", nullable: false),
                    Height = table.Column<byte>(type: "INTEGER", nullable: false),
                    MinHeight = table.Column<byte>(type: "INTEGER", nullable: false),
                    MaxHeight = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumRotations = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumGrowthStages = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ShadowImageOffset = table.Column<ushort>(type: "INTEGER", nullable: false),
                    SeasonState = table.Column<byte>(type: "INTEGER", nullable: false),
                    CurrentSeason = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    BuildCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    ClearCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    Colours = table.Column<uint>(type: "INTEGER", nullable: false),
                    Rating = table.Column<short>(type: "INTEGER", nullable: false),
                    DemolishRatingReduction = table.Column<short>(type: "INTEGER", nullable: false),
                    VariantFlags = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTree", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTree_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjTunnel",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjTunnel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjTunnel_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjVehicle",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mode = table.Column<byte>(type: "INTEGER", nullable: false),
                    Type = table.Column<byte>(type: "INTEGER", nullable: false),
                    NumCarComponents = table.Column<byte>(type: "INTEGER", nullable: false),
                    RoadOrTrackType = table.Column<string>(type: "json", nullable: true),
                    TrackTypeId = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    Reliability = table.Column<byte>(type: "INTEGER", nullable: false),
                    RunCostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    RunCostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    CompanyColourSchemeIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    CompatibleVehicles = table.Column<string>(type: "json", nullable: false),
                    RequiredTrackExtras = table.Column<string>(type: "json", nullable: false),
                    CarComponents = table.Column<string>(type: "json", nullable: false),
                    BodySprites = table.Column<string>(type: "json", nullable: false),
                    BogieSprites = table.Column<string>(type: "json", nullable: false),
                    Power = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Speed = table.Column<short>(type: "INTEGER", nullable: false),
                    RackSpeed = table.Column<short>(type: "INTEGER", nullable: false),
                    Weight = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Flags = table.Column<ushort>(type: "INTEGER", nullable: false),
                    MaxCargo = table.Column<string>(type: "json", nullable: false),
                    CompatibleCargoCategories = table.Column<string>(type: "json", nullable: false),
                    CargoTypeSpriteOffsets = table.Column<string>(type: "json", nullable: false),
                    NumSimultaneousCargoTypes = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParticleEmitters = table.Column<string>(type: "json", nullable: false),
                    ShipWakeSpacing = table.Column<byte>(type: "INTEGER", nullable: false),
                    DesignedYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ObsoleteYear = table.Column<ushort>(type: "INTEGER", nullable: false),
                    RackRail = table.Column<string>(type: "json", nullable: true),
                    DrivingSoundType = table.Column<byte>(type: "INTEGER", nullable: false),
                    FrictionSound = table.Column<string>(type: "json", nullable: true),
                    SimpleMotorSound = table.Column<string>(type: "json", nullable: true),
                    GearboxMotorSound = table.Column<string>(type: "json", nullable: true),
                    DrivingSound = table.Column<string>(type: "json", nullable: true),
                    StartSounds = table.Column<string>(type: "json", nullable: false),
                    CrossingSounds = table.Column<string>(type: "json", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjVehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjVehicle_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjWall",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ToolId = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags1 = table.Column<byte>(type: "INTEGER", nullable: false),
                    Height = table.Column<byte>(type: "INTEGER", nullable: false),
                    Flags2 = table.Column<byte>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjWall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjWall_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjWater",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CostIndex = table.Column<byte>(type: "INTEGER", nullable: false),
                    CostFactor = table.Column<short>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjWater", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjWater_Objects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StringTable",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<byte>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    ObjectId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StringTable_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblObjectTblObjectPack",
                columns: table => new
                {
                    ObjectPacksId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ObjectsId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblObjectTblObjectPack", x => new { x.ObjectPacksId, x.ObjectsId });
                    table.ForeignKey(
                        name: "FK_TblObjectTblObjectPack_ObjectPacks_ObjectPacksId",
                        column: x => x.ObjectPacksId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblObjectTblObjectPack_Objects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblScenarioTblScenarioPack",
                columns: table => new
                {
                    ScenarioPacksId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ScenariosId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblScenarioTblScenarioPack", x => new { x.ScenarioPacksId, x.ScenariosId });
                    table.ForeignKey(
                        name: "FK_TblScenarioTblScenarioPack_ScenarioPacks_ScenarioPacksId",
                        column: x => x.ScenarioPacksId,
                        principalTable: "ScenarioPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblScenarioTblScenarioPack_Scenarios_ScenariosId",
                        column: x => x.ScenariosId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TblGraphicsId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    TblMusicId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    TblSoundEffectId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    TblTutorialId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authors_Graphics_TblGraphicsId",
                        column: x => x.TblGraphicsId,
                        principalTable: "Graphics",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authors_Music_TblMusicId",
                        column: x => x.TblMusicId,
                        principalTable: "Music",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authors_SoundEffects_TblSoundEffectId",
                        column: x => x.TblSoundEffectId,
                        principalTable: "SoundEffects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authors_Tutorials_TblTutorialId",
                        column: x => x.TblTutorialId,
                        principalTable: "Tutorials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TblGraphicsId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    TblMusicId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    TblSoundEffectId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    TblTutorialId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Graphics_TblGraphicsId",
                        column: x => x.TblGraphicsId,
                        principalTable: "Graphics",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Music_TblMusicId",
                        column: x => x.TblMusicId,
                        principalTable: "Music",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_SoundEffects_TblSoundEffectId",
                        column: x => x.TblSoundEffectId,
                        principalTable: "SoundEffects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Tutorials_TblTutorialId",
                        column: x => x.TblTutorialId,
                        principalTable: "Tutorials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblAuthorTblObject",
                columns: table => new
                {
                    AuthorsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ObjectsId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblObject", x => new { x.AuthorsId, x.ObjectsId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblObject_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblObject_Objects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAuthorTblObjectPack",
                columns: table => new
                {
                    AuthorsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ObjectPacksId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblObjectPack", x => new { x.AuthorsId, x.ObjectPacksId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblObjectPack_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblObjectPack_ObjectPacks_ObjectPacksId",
                        column: x => x.ObjectPacksId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAuthorTblScenario",
                columns: table => new
                {
                    AuthorsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SC5FilesId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblScenario", x => new { x.AuthorsId, x.SC5FilesId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblScenario_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblScenario_Scenarios_SC5FilesId",
                        column: x => x.SC5FilesId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAuthorTblScenarioPack",
                columns: table => new
                {
                    AuthorsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ScenarioPacksId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthorTblScenarioPack", x => new { x.AuthorsId, x.ScenarioPacksId });
                    table.ForeignKey(
                        name: "FK_TblAuthorTblScenarioPack_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAuthorTblScenarioPack_ScenarioPacks_ScenarioPacksId",
                        column: x => x.ScenarioPacksId,
                        principalTable: "ScenarioPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblObjectPackTblTag",
                columns: table => new
                {
                    ObjectPacksId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblObjectPackTblTag", x => new { x.ObjectPacksId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblObjectPackTblTag_ObjectPacks_ObjectPacksId",
                        column: x => x.ObjectPacksId,
                        principalTable: "ObjectPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblObjectPackTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblObjectTblTag",
                columns: table => new
                {
                    ObjectsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblObjectTblTag", x => new { x.ObjectsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblObjectTblTag_Objects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblObjectTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblScenarioPackTblTag",
                columns: table => new
                {
                    ScenarioPacksId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblScenarioPackTblTag", x => new { x.ScenarioPacksId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblScenarioPackTblTag_ScenarioPacks_ScenarioPacksId",
                        column: x => x.ScenarioPacksId,
                        principalTable: "ScenarioPacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblScenarioPackTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblScenarioTblTag",
                columns: table => new
                {
                    SC5FilesId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblScenarioTblTag", x => new { x.SC5FilesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TblScenarioTblTag_Scenarios_SC5FilesId",
                        column: x => x.SC5FilesId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblScenarioTblTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AssociatedAuthorId",
                table: "AspNetUsers",
                column: "AssociatedAuthorId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Name",
                table: "Authors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblGraphicsId",
                table: "Authors",
                column: "TblGraphicsId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblMusicId",
                table: "Authors",
                column: "TblMusicId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblSoundEffectId",
                table: "Authors",
                column: "TblSoundEffectId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_TblTutorialId",
                table: "Authors",
                column: "TblTutorialId");

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_DatName_DatChecksum",
                table: "DatObjects",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true,
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_ObjectId",
                table: "DatObjects",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_xxHash3",
                table: "DatObjects",
                column: "xxHash3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Graphics_LicenceId",
                table: "Graphics",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Graphics_Name",
                table: "Graphics",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Graphics_OwnerUserId",
                table: "Graphics",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Licences_Name",
                table: "Licences",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Music_LicenceId",
                table: "Music",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Music_Name",
                table: "Music",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Music_OwnerUserId",
                table: "Music",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjAirport_ParentId",
                table: "ObjAirport",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjBridge_ParentId",
                table: "ObjBridge",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjBuilding_ParentId",
                table: "ObjBuilding",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCargo_ParentId",
                table: "ObjCargo",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCliffEdge_ParentId",
                table: "ObjCliffEdge",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjClimate_ParentId",
                table: "ObjClimate",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCompetitor_ParentId",
                table: "ObjCompetitor",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjCurrency_ParentId",
                table: "ObjCurrency",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjDock_ParentId",
                table: "ObjDock",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectPacks_LicenceId",
                table: "ObjectPacks",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectPacks_Name",
                table: "ObjectPacks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectPacks_OwnerUserId",
                table: "ObjectPacks",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_LicenceId",
                table: "Objects",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_Name",
                table: "Objects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objects_OwnerUserId",
                table: "Objects",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsMissing_DatName_DatChecksum",
                table: "ObjectsMissing",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true,
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_ObjHillShapes_ParentId",
                table: "ObjHillShapes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjIndustry_ParentId",
                table: "ObjIndustry",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjInterface_ParentId",
                table: "ObjInterface",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjLand_ParentId",
                table: "ObjLand",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjLevelCrossing_ParentId",
                table: "ObjLevelCrossing",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRegion_ParentId",
                table: "ObjRegion",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoad_ParentId",
                table: "ObjRoad",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoadExtra_ParentId",
                table: "ObjRoadExtra",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoadStation_ParentId",
                table: "ObjRoadStation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjScaffolding_ParentId",
                table: "ObjScaffolding",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjScenarioText_ParentId",
                table: "ObjScenarioText",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjSnow_ParentId",
                table: "ObjSnow",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjSound_ParentId",
                table: "ObjSound",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjSteam_ParentId",
                table: "ObjSteam",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjStreetLight_ParentId",
                table: "ObjStreetLight",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTownNames_ParentId",
                table: "ObjTownNames",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrack_ParentId",
                table: "ObjTrack",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackExtra_ParentId",
                table: "ObjTrackExtra",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackSignal_ParentId",
                table: "ObjTrackSignal",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackStation_ParentId",
                table: "ObjTrackStation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTree_ParentId",
                table: "ObjTree",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjTunnel_ParentId",
                table: "ObjTunnel",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjVehicle_ParentId",
                table: "ObjVehicle",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjWall_ParentId",
                table: "ObjWall",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjWater_ParentId",
                table: "ObjWater",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPacks_LicenceId",
                table: "ScenarioPacks",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPacks_Name",
                table: "ScenarioPacks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioPacks_OwnerUserId",
                table: "ScenarioPacks",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_LicenceId",
                table: "Scenarios",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_Name",
                table: "Scenarios",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_OwnerUserId",
                table: "Scenarios",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SoundEffects_LicenceId",
                table: "SoundEffects",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SoundEffects_Name",
                table: "SoundEffects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoundEffects_OwnerUserId",
                table: "SoundEffects",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StringTable_ObjectId",
                table: "StringTable",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StringTable_Text",
                table: "StringTable",
                column: "Text");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblGraphicsId",
                table: "Tags",
                column: "TblGraphicsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblMusicId",
                table: "Tags",
                column: "TblMusicId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblSoundEffectId",
                table: "Tags",
                column: "TblSoundEffectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TblTutorialId",
                table: "Tags",
                column: "TblTutorialId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAuthorTblObject_ObjectsId",
                table: "TblAuthorTblObject",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAuthorTblObjectPack_ObjectPacksId",
                table: "TblAuthorTblObjectPack",
                column: "ObjectPacksId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAuthorTblScenario_SC5FilesId",
                table: "TblAuthorTblScenario",
                column: "SC5FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAuthorTblScenarioPack_ScenarioPacksId",
                table: "TblAuthorTblScenarioPack",
                column: "ScenarioPacksId");

            migrationBuilder.CreateIndex(
                name: "IX_TblObjectPackTblTag_TagsId",
                table: "TblObjectPackTblTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblObjectTblObjectPack_ObjectsId",
                table: "TblObjectTblObjectPack",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblObjectTblTag_TagsId",
                table: "TblObjectTblTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblScenarioPackTblTag_TagsId",
                table: "TblScenarioPackTblTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TblScenarioTblScenarioPack_ScenariosId",
                table: "TblScenarioTblScenarioPack",
                column: "ScenariosId");

            migrationBuilder.CreateIndex(
                name: "IX_TblScenarioTblTag_TagsId",
                table: "TblScenarioTblTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tutorials_LicenceId",
                table: "Tutorials",
                column: "LicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tutorials_Name",
                table: "Tutorials",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tutorials_OwnerUserId",
                table: "Tutorials",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Authors_AssociatedAuthorId",
                table: "AspNetUsers",
                column: "AssociatedAuthorId",
                principalTable: "Authors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Graphics_AspNetUsers_OwnerUserId",
                table: "Graphics");

            migrationBuilder.DropForeignKey(
                name: "FK_Music_AspNetUsers_OwnerUserId",
                table: "Music");

            migrationBuilder.DropForeignKey(
                name: "FK_SoundEffects_AspNetUsers_OwnerUserId",
                table: "SoundEffects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tutorials_AspNetUsers_OwnerUserId",
                table: "Tutorials");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DatObjects");

            migrationBuilder.DropTable(
                name: "ObjAirport");

            migrationBuilder.DropTable(
                name: "ObjBridge");

            migrationBuilder.DropTable(
                name: "ObjBuilding");

            migrationBuilder.DropTable(
                name: "ObjCargo");

            migrationBuilder.DropTable(
                name: "ObjCliffEdge");

            migrationBuilder.DropTable(
                name: "ObjClimate");

            migrationBuilder.DropTable(
                name: "ObjCompetitor");

            migrationBuilder.DropTable(
                name: "ObjCurrency");

            migrationBuilder.DropTable(
                name: "ObjDock");

            migrationBuilder.DropTable(
                name: "ObjectsMissing");

            migrationBuilder.DropTable(
                name: "ObjHillShapes");

            migrationBuilder.DropTable(
                name: "ObjIndustry");

            migrationBuilder.DropTable(
                name: "ObjInterface");

            migrationBuilder.DropTable(
                name: "ObjLand");

            migrationBuilder.DropTable(
                name: "ObjLevelCrossing");

            migrationBuilder.DropTable(
                name: "ObjRegion");

            migrationBuilder.DropTable(
                name: "ObjRoad");

            migrationBuilder.DropTable(
                name: "ObjRoadExtra");

            migrationBuilder.DropTable(
                name: "ObjRoadStation");

            migrationBuilder.DropTable(
                name: "ObjScaffolding");

            migrationBuilder.DropTable(
                name: "ObjScenarioText");

            migrationBuilder.DropTable(
                name: "ObjSnow");

            migrationBuilder.DropTable(
                name: "ObjSound");

            migrationBuilder.DropTable(
                name: "ObjSteam");

            migrationBuilder.DropTable(
                name: "ObjStreetLight");

            migrationBuilder.DropTable(
                name: "ObjTownNames");

            migrationBuilder.DropTable(
                name: "ObjTrack");

            migrationBuilder.DropTable(
                name: "ObjTrackExtra");

            migrationBuilder.DropTable(
                name: "ObjTrackSignal");

            migrationBuilder.DropTable(
                name: "ObjTrackStation");

            migrationBuilder.DropTable(
                name: "ObjTree");

            migrationBuilder.DropTable(
                name: "ObjTunnel");

            migrationBuilder.DropTable(
                name: "ObjVehicle");

            migrationBuilder.DropTable(
                name: "ObjWall");

            migrationBuilder.DropTable(
                name: "ObjWater");

            migrationBuilder.DropTable(
                name: "StringTable");

            migrationBuilder.DropTable(
                name: "TblAuthorTblObject");

            migrationBuilder.DropTable(
                name: "TblAuthorTblObjectPack");

            migrationBuilder.DropTable(
                name: "TblAuthorTblScenario");

            migrationBuilder.DropTable(
                name: "TblAuthorTblScenarioPack");

            migrationBuilder.DropTable(
                name: "TblObjectPackTblTag");

            migrationBuilder.DropTable(
                name: "TblObjectTblObjectPack");

            migrationBuilder.DropTable(
                name: "TblObjectTblTag");

            migrationBuilder.DropTable(
                name: "TblScenarioPackTblTag");

            migrationBuilder.DropTable(
                name: "TblScenarioTblScenarioPack");

            migrationBuilder.DropTable(
                name: "TblScenarioTblTag");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ObjectPacks");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "ScenarioPacks");

            migrationBuilder.DropTable(
                name: "Scenarios");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Graphics");

            migrationBuilder.DropTable(
                name: "Music");

            migrationBuilder.DropTable(
                name: "SoundEffects");

            migrationBuilder.DropTable(
                name: "Tutorials");

            migrationBuilder.DropTable(
                name: "Licences");
        }
    }
}
