using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TheWeebDenShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BannerPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Short = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Volumes = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    StoreId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "CreatedAt", "Description", "Genre", "Image", "IsApproved", "IsBanned", "Name", "Price", "Rating", "Short", "Stock", "StoreId", "Volumes" },
                values: new object[,]
                {
                    { "p1", "Koyoharu Gotouge", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2481), "Complete 23-volume Demon Slayer manga box set with premium storage case. Follow Tanjiro's entire journey from beginning to end.", "Action", "images/demon-slayer.jpg", true, false, "Demon Slayer Box Set (Vol.1–23)", 10640m, 4.7999999999999998, "Kimetsu no Yaiba complete manga box set", 12, null, 23 },
                    { "p10", "Hiromu Arakawa", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2505), "Edward and Alphonse Elric attempt to use alchemy to bring their mother back to life with devastating consequences.", "Fantasy", "images/fullmetal-alchemist.jpg", true, false, "Fullmetal Alchemist Vol. 1", 559m, 4.7999999999999998, "The Elric brothers' alchemical journey", 22, null, 1 },
                    { "p11", "Makoto Shinkai", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2506), "The original light novel that inspired the beloved anime film about two teenagers who mysteriously swap bodies.", "Romance", "images/your-name.jpg", true, false, "Your Name Light Novel", 727m, 4.5999999999999996, "The story behind the hit anime film", 35, null, 1 },
                    { "p12", "Hayao Miyazaki", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2507), "A beautifully illustrated picture book adaptation of Hayao Miyazaki's Academy Award-winning film.", "Fantasy", "images/spirited-away.jpg", true, false, "Spirited Away Picture Book", 1119m, 4.7000000000000002, "Studio Ghibli's masterpiece in book form", 5, null, 1 },
                    { "p13", "Fujino Omori & Toshi Aoi", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2509), "Will Serfort enrolls in a prestigious magic academy despite being unable to use magic, relying solely on his sword skills to prove himself among talented mages.", "Action", "images/wistoria.jpg", true, false, "Wistoria: Wand and Sword Season 1 Blu-ray", 2799m, 4.5, "Magic academy action anime complete season", 25, null, 1 },
                    { "p14", "Kei Urana", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2510), "Follow Rudo as he's exiled to the trash-filled abyss and discovers mysterious powers. Complete volumes 1-6 from Kei Urana's acclaimed dark fantasy series.", "Action", "images/gachiakuta.jpg", true, false, "Gachiakuta Vol. 1-6 Collection", 3919m, 4.7000000000000002, "Post-apocalyptic action manga bundle", 18, null, 6 },
                    { "p15", "Yukinobu Tatsu", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2512), "Momo and Okarun's wild adventures battling aliens and spirits. Deluxe box set featuring volumes 1-8 of Yukinobu Tatsu's hit supernatural action-comedy manga.", "Action", "images/dandandan.jpg", true, false, "DanDaDan Vol. 1-8 Ultimate Box Set", 5039m, 4.7999999999999998, "Aliens, ghosts, and supernatural chaos", 14, null, 8 },
                    { "p16", "Ken Wakui", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2513), "Takemichi Hanagaki travels back in time to save his ex-girlfriend from a tragic fate. First two volumes of Ken Wakui's thrilling time-leap gang story.", "Action", "images/tokyorevengers.jpg", true, false, "Tokyo Revengers Vol. 1-2", 1119m, 4.5999999999999996, "Time-traveling delinquent action", 32, null, 2 },
                    { "p2", "Gege Akutami", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2492), "Yuji Itadori swallows Sukuna's finger and begins his chaotic days at Tokyo Jujutsu High School.", "Action", "images/jujutsu-kaisen.jpg", true, false, "Jujutsu Kaisen Vol. 1", 503m, 4.5999999999999996, "Yuji Itadori begins his journey", 34, null, 1 },
                    { "p3", "Tatsuki Fujimoto", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2494), "Denji and Pochita's strange partnership. A bloody yet heartbreaking story collected in Part 1 box set.", "Action", "images/chainsaw-man.jpg", true, false, "Chainsaw Man Box Set (Vol.1–11)", 4479m, 4.7000000000000002, "Chainsaw Man Part 1 box set", 0, null, 11 },
                    { "p4", "Hajime Isayama", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2495), "When titans breach Wall Maria, Eren Yeager witnesses unimaginable horror and vows revenge against the titans.", "Action", "images/attack-on-titan.jpg", true, false, "Attack on Titan Vol. 1", 559m, 4.9000000000000004, "Humanity's fight for survival begins", 7, null, 1 },
                    { "p5", "Kohei Horikoshi", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2497), "Izuku Midoriya dreams of becoming a hero despite being born without a Quirk in a world where superpowers are common.", "Action", "images/my-hero-academia.jpg", true, false, "My Hero Academia Vol. 1", 503m, 4.5, "Izuku's journey to become a hero", 30, null, 1 },
                    { "p6", "Eiichiro Oda", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2499), "Monkey D. Luffy sets out to sea to find the legendary treasure One Piece and become the next Pirate King.", "Adventure", "images/one-piece.jpg", true, false, "One Piece Vol. 1", 559m, 4.7999999999999998, "Luffy begins his pirate adventure", 40, null, 1 },
                    { "p7", "Masashi Kishimoto", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2500), "Follow Naruto Uzumaki from his academy days through the Chunin Exams in this comprehensive box set.", "Action", "images/naruto.jpg", true, false, "Naruto Box Set 1 (Vol.1–27)", 8959m, 4.7000000000000002, "First Naruto box set collection", 15, null, 27 },
                    { "p8", "Sui Ishida", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2502), "College student Ken Kaneki's life changes forever when he encounters a ghoul and becomes a half-ghoul himself.", "Horror", "images/tokyo-ghoul.jpg", true, false, "Tokyo Ghoul Vol. 1", 503m, 4.4000000000000004, "Ken Kaneki's transformation begins", 0, null, 1 },
                    { "p9", "Tsugumi Ohba", new DateTime(2026, 3, 17, 10, 39, 47, 728, DateTimeKind.Utc).AddTicks(2503), "The complete Death Note series featuring Light Yagami's psychological battle with the detective L.", "Thriller", "images/death-note.jpg", true, false, "Death Note Box Set", 3079m, 4.9000000000000004, "Complete Death Note collection", 18, null, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

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
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId",
                table: "Products",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_OwnerId",
                table: "Stores",
                column: "OwnerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
