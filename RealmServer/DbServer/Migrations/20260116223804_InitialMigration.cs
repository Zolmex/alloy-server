using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace DbServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Account_Stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    best_char_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    current_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    total_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    current_credits = table.Column<uint>(type: "int unsigned", nullable: true),
                    total_credits = table.Column<uint>(type: "int unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Character_Stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    hp = table.Column<uint>(type: "int unsigned", nullable: true),
                    mp = table.Column<uint>(type: "int unsigned", nullable: true),
                    max_hp = table.Column<uint>(type: "int unsigned", nullable: true),
                    max_mp = table.Column<uint>(type: "int unsigned", nullable: true),
                    attack = table.Column<uint>(type: "int unsigned", nullable: true),
                    defense = table.Column<uint>(type: "int unsigned", nullable: true),
                    speed = table.Column<uint>(type: "int unsigned", nullable: true),
                    dexterity = table.Column<uint>(type: "int unsigned", nullable: true),
                    vitality = table.Column<uint>(type: "int unsigned", nullable: true),
                    wisdom = table.Column<uint>(type: "int unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Combat_Stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    shots = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    shots_hit = table.Column<uint>(type: "int unsigned", nullable: true),
                    level_up_assists = table.Column<uint>(type: "int unsigned", nullable: true),
                    potions_drank = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    abilities_used = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    damage_taken = table.Column<uint>(type: "int unsigned", nullable: true),
                    damage_dealt = table.Column<uint>(type: "int unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dungeon_Stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    dungeon_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    completed_count = table.Column<ushort>(type: "smallint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Exploration_Stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tiles_uncovered = table.Column<uint>(type: "int unsigned", nullable: true),
                    quests_completed = table.Column<uint>(type: "int unsigned", nullable: true),
                    escapes = table.Column<uint>(type: "int unsigned", nullable: true),
                    near_death_escapes = table.Column<uint>(type: "int unsigned", nullable: true),
                    minutes_active = table.Column<uint>(type: "int unsigned", nullable: true),
                    teleports = table.Column<uint>(type: "int unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    level = table.Column<short>(type: "smallint", nullable: true),
                    current_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    total_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    guild_board = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kill_Stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    monster_kills = table.Column<uint>(type: "int unsigned", nullable: true),
                    monster_assists = table.Column<uint>(type: "int unsigned", nullable: true),
                    god_kills = table.Column<uint>(type: "int unsigned", nullable: true),
                    god_assists = table.Column<uint>(type: "int unsigned", nullable: true),
                    oryx_kills = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    oryx_assists = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    cube_kills = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    cube_assists = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    blue_bags = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    cyan_bags = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    white_bags = table.Column<ushort>(type: "smallint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    password_salt = table.Column<string>(type: "text", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    ip_address = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Class_Stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    object_type = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    best_level = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    best_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    acc_stats_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "Class_Stats_ibfk_1",
                        column: x => x.acc_stats_id,
                        principalTable: "Account_Stats",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Guild_Members",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    guild_rank = table.Column<short>(type: "smallint", nullable: true),
                    last_seen_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "NOW()"),
                    guild_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "Guild_Members_ibfk_1",
                        column: x => x.guild_id,
                        principalTable: "Guilds",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    rank = table.Column<short>(type: "smallint", nullable: true),
                    guild_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    is_admin = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_banned = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    max_chars = table.Column<short>(type: "smallint", nullable: true),
                    vault_count = table.Column<short>(type: "smallint", nullable: true),
                    next_char_id = table.Column<short>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "NOW()"),
                    acc_stats_id = table.Column<int>(type: "int", nullable: true),
                    login_id = table.Column<int>(type: "int", nullable: true),
                    guild_member_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "Accounts_ibfk_1",
                        column: x => x.acc_stats_id,
                        principalTable: "Account_Stats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Accounts_ibfk_2",
                        column: x => x.login_id,
                        principalTable: "Logins",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Accounts_ibfk_3",
                        column: x => x.guild_member_id,
                        principalTable: "Guild_Members",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Account_Skins",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false),
                    skin_type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.account_id, x.skin_type });
                    table.ForeignKey(
                        name: "Account_Skins_ibfk_1",
                        column: x => x.account_id,
                        principalTable: "Accounts",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    acc_char_id = table.Column<int>(type: "int", nullable: true),
                    object_type = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    level = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    current_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    xp_points = table.Column<uint>(type: "int unsigned", nullable: true),
                    skin_type = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    texture_one = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    texture_two = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    pet_type = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    health_potions = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    magic_potions = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    is_dead = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    has_backpack = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    acc_id = table.Column<int>(type: "int", nullable: true),
                    char_stats_id = table.Column<int>(type: "int", nullable: true),
                    explo_stats_id = table.Column<int>(type: "int", nullable: true),
                    combat_stats_id = table.Column<int>(type: "int", nullable: true),
                    kill_stats_id = table.Column<int>(type: "int", nullable: true),
                    dungeon_stats_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "Characters_ibfk_1",
                        column: x => x.acc_id,
                        principalTable: "Accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Characters_ibfk_2",
                        column: x => x.char_stats_id,
                        principalTable: "Character_Stats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Characters_ibfk_3",
                        column: x => x.explo_stats_id,
                        principalTable: "Exploration_Stats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Characters_ibfk_4",
                        column: x => x.combat_stats_id,
                        principalTable: "Combat_Stats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Characters_ibfk_5",
                        column: x => x.kill_stats_id,
                        principalTable: "Kill_Stats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Characters_ibfk_6",
                        column: x => x.dungeon_stats_id,
                        principalTable: "Dungeon_Stats",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Character_Death",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    dead_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "NOW()"),
                    death_fame = table.Column<uint>(type: "int unsigned", nullable: true),
                    char_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "Character_Death_ibfk_1",
                        column: x => x.char_id,
                        principalTable: "Characters",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Character_Inventory",
                columns: table => new
                {
                    character_id = table.Column<int>(type: "int", nullable: false),
                    slot_id = table.Column<int>(type: "int", nullable: false),
                    item_type = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    item_data = table.Column<byte[]>(type: "blob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.character_id, x.slot_id });
                    table.ForeignKey(
                        name: "Character_Inventory_ibfk_1",
                        column: x => x.character_id,
                        principalTable: "Characters",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "acc_stats_id",
                table: "Accounts",
                column: "acc_stats_id");

            migrationBuilder.CreateIndex(
                name: "guild_member_id",
                table: "Accounts",
                column: "guild_member_id");

            migrationBuilder.CreateIndex(
                name: "login_id",
                table: "Accounts",
                column: "login_id");

            migrationBuilder.CreateIndex(
                name: "name",
                table: "Accounts",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "char_id",
                table: "Character_Death",
                column: "char_id");

            migrationBuilder.CreateIndex(
                name: "acc_id",
                table: "Characters",
                column: "acc_id");

            migrationBuilder.CreateIndex(
                name: "char_stats_id",
                table: "Characters",
                column: "char_stats_id");

            migrationBuilder.CreateIndex(
                name: "combat_stats_id",
                table: "Characters",
                column: "combat_stats_id");

            migrationBuilder.CreateIndex(
                name: "dungeon_stats_id",
                table: "Characters",
                column: "dungeon_stats_id");

            migrationBuilder.CreateIndex(
                name: "explo_stats_id",
                table: "Characters",
                column: "explo_stats_id");

            migrationBuilder.CreateIndex(
                name: "kill_stats_id",
                table: "Characters",
                column: "kill_stats_id");

            migrationBuilder.CreateIndex(
                name: "acc_stats_id1",
                table: "Class_Stats",
                column: "acc_stats_id");

            migrationBuilder.CreateIndex(
                name: "guild_id",
                table: "Guild_Members",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "name1",
                table: "Logins",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account_Skins");

            migrationBuilder.DropTable(
                name: "Character_Death");

            migrationBuilder.DropTable(
                name: "Character_Inventory");

            migrationBuilder.DropTable(
                name: "Class_Stats");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Character_Stats");

            migrationBuilder.DropTable(
                name: "Exploration_Stats");

            migrationBuilder.DropTable(
                name: "Combat_Stats");

            migrationBuilder.DropTable(
                name: "Kill_Stats");

            migrationBuilder.DropTable(
                name: "Dungeon_Stats");

            migrationBuilder.DropTable(
                name: "Account_Stats");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Guild_Members");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
