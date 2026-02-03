using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Common.Database.Models;

namespace DbServer.Database;

public partial class AlloyContext : DbContext
{
    public AlloyContext()
    {
    }

    public AlloyContext(DbContextOptions<AlloyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountSkin> AccountSkins { get; set; }

    public virtual DbSet<AccountStat> AccountStats { get; set; }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<CharacterDeath> CharacterDeaths { get; set; }

    public virtual DbSet<CharacterInventory> CharacterInventories { get; set; }

    public virtual DbSet<CharacterStat> CharacterStats { get; set; }

    public virtual DbSet<ClassStat> ClassStats { get; set; }

    public virtual DbSet<CombatStat> CombatStats { get; set; }

    public virtual DbSet<DungeonStat> DungeonStats { get; set; }

    public virtual DbSet<ExplorationStat> ExplorationStats { get; set; }

    public virtual DbSet<Guild> Guilds { get; set; }

    public virtual DbSet<GuildMember> GuildMembers { get; set; }

    public virtual DbSet<KillStat> KillStats { get; set; }
    public virtual DbSet<Login> Logins { get; set; }
    
    public virtual DbSet<AccountLock> AccountLocks { get; set; }
    
    public virtual DbSet<AccountIgnore> AccountIgnores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.AccStatsId, "acc_stats_id");

            entity.HasIndex(e => e.GuildMemberId, "guild_member_id");

            entity.HasIndex(e => e.LoginId, "login_id");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccStatsId).HasColumnName("acc_stats_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GuildMemberId).HasColumnName("guild_member_id");
            entity.Property(e => e.GuildName)
                .HasMaxLength(255)
                .HasColumnName("guild_name");
            entity.Property(e => e.IsAdmin).HasColumnName("is_admin");
            entity.Property(e => e.IsBanned).HasColumnName("is_banned");
            entity.Property(e => e.LoginId).HasColumnName("login_id");
            entity.Property(e => e.MaxChars).HasColumnName("max_chars");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.NextCharId).HasColumnName("next_char_id");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.VaultCount).HasColumnName("vault_count");

            entity.HasOne(d => d.AccStats).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.AccStatsId)
                .HasConstraintName("Accounts_ibfk_1");

            entity.HasOne(d => d.GuildMember).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.GuildMemberId)
                .HasConstraintName("Accounts_ibfk_3");

            entity.HasOne(d => d.Login).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.LoginId)
                .HasConstraintName("Accounts_ibfk_2");
        });

        modelBuilder.Entity<AccountSkin>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.SkinType }).HasName("PRIMARY");

            entity.ToTable("Account_Skins");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.SkinType).HasColumnName("skin_type");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountSkins)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Account_Skins_ibfk_1");
        });
        
        modelBuilder.Entity<AccountIgnore>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.IgnoredId }).HasName("PRIMARY");

            entity.ToTable("Account_Ignores");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.IgnoredId).HasColumnName("ignored_id");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountIgnores)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Account_Ignores_ibfk_1");
        });
        
        modelBuilder.Entity<AccountLock>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.LockedId }).HasName("PRIMARY");

            entity.ToTable("Account_Locks");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.LockedId).HasColumnName("locked_id");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountLocks)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Account_Locks_ibfk_1");
        });

        modelBuilder.Entity<AccountStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Account_Stats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BestCharFame).HasColumnName("best_char_fame");
            entity.Property(e => e.CurrentCredits).HasColumnName("current_credits");
            entity.Property(e => e.CurrentFame).HasColumnName("current_fame");
            entity.Property(e => e.TotalCredits).HasColumnName("total_credits");
            entity.Property(e => e.TotalFame).HasColumnName("total_fame");
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.AccId, "acc_id");

            entity.HasIndex(e => e.CharStatsId, "char_stats_id");

            entity.HasIndex(e => e.CombatStatsId, "combat_stats_id");

            entity.HasIndex(e => e.DungeonStatsId, "dungeon_stats_id");

            entity.HasIndex(e => e.ExploStatsId, "explo_stats_id");

            entity.HasIndex(e => e.KillStatsId, "kill_stats_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccCharId).HasColumnName("acc_char_id");
            entity.Property(e => e.AccId).HasColumnName("acc_id");
            entity.Property(e => e.CharStatsId).HasColumnName("char_stats_id");
            entity.Property(e => e.CombatStatsId).HasColumnName("combat_stats_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentFame).HasColumnName("current_fame");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DungeonStatsId).HasColumnName("dungeon_stats_id");
            entity.Property(e => e.ExploStatsId).HasColumnName("explo_stats_id");
            entity.Property(e => e.HasBackpack).HasColumnName("has_backpack");
            entity.Property(e => e.HealthPotions).HasColumnName("health_potions");
            entity.Property(e => e.IsDead).HasColumnName("is_dead");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.KillStatsId).HasColumnName("kill_stats_id");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.MagicPotions).HasColumnName("magic_potions");
            entity.Property(e => e.ObjectType).HasColumnName("object_type");
            entity.Property(e => e.PetType).HasColumnName("pet_type");
            entity.Property(e => e.SkinType).HasColumnName("skin_type");
            entity.Property(e => e.TextureOne).HasColumnName("texture_one");
            entity.Property(e => e.TextureTwo).HasColumnName("texture_two");
            entity.Property(e => e.XpPoints).HasColumnName("xp_points");

            entity.HasOne(d => d.Acc).WithMany(p => p.Characters)
                .HasForeignKey(d => d.AccId)
                .HasConstraintName("Characters_ibfk_1");

            entity.HasOne(d => d.CharStats).WithMany(p => p.Characters)
                .HasForeignKey(d => d.CharStatsId)
                .HasConstraintName("Characters_ibfk_2");

            entity.HasOne(d => d.CombatStats).WithMany(p => p.Characters)
                .HasForeignKey(d => d.CombatStatsId)
                .HasConstraintName("Characters_ibfk_4");

            entity.HasOne(d => d.DungeonStats).WithMany(p => p.Characters)
                .HasForeignKey(d => d.DungeonStatsId)
                .HasConstraintName("Characters_ibfk_6");

            entity.HasOne(d => d.ExploStats).WithMany(p => p.Characters)
                .HasForeignKey(d => d.ExploStatsId)
                .HasConstraintName("Characters_ibfk_3");

            entity.HasOne(d => d.KillStats).WithMany(p => p.Characters)
                .HasForeignKey(d => d.KillStatsId)
                .HasConstraintName("Characters_ibfk_5");
        });

        modelBuilder.Entity<CharacterDeath>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Character_Death");

            entity.HasIndex(e => e.CharId, "char_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CharId).HasColumnName("char_id");
            entity.Property(e => e.DeadAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("datetime")
                .HasColumnName("dead_at");
            entity.Property(e => e.DeathFame).HasColumnName("death_fame");

            entity.HasOne(d => d.Char).WithMany(p => p.CharacterDeaths)
                .HasForeignKey(d => d.CharId)
                .HasConstraintName("Character_Death_ibfk_1");
        });

        modelBuilder.Entity<CharacterInventory>(entity =>
        {
            entity.HasKey(e => new { e.CharacterId, e.SlotId }).HasName("PRIMARY");

            entity.ToTable("Character_Inventory");

            entity.Property(e => e.CharacterId).HasColumnName("character_id");
            entity.Property(e => e.SlotId).HasColumnName("slot_id");
            entity.Property(e => e.ItemData)
                .HasColumnType("blob")
                .HasColumnName("item_data");
            entity.Property(e => e.ItemType).HasColumnName("item_type");

            entity.HasOne(d => d.Character).WithMany(p => p.CharacterInventories)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Character_Inventory_ibfk_1");
        });

        modelBuilder.Entity<CharacterStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Character_Stats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attack).HasColumnName("attack");
            entity.Property(e => e.Defense).HasColumnName("defense");
            entity.Property(e => e.Dexterity).HasColumnName("dexterity");
            entity.Property(e => e.Hp).HasColumnName("hp");
            entity.Property(e => e.MaxHp).HasColumnName("max_hp");
            entity.Property(e => e.MaxMp).HasColumnName("max_mp");
            entity.Property(e => e.Mp).HasColumnName("mp");
            entity.Property(e => e.Speed).HasColumnName("speed");
            entity.Property(e => e.Vitality).HasColumnName("vitality");
            entity.Property(e => e.Wisdom).HasColumnName("wisdom");
        });

        modelBuilder.Entity<ClassStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Class_Stats");

            entity.HasIndex(e => e.AccStatsId, "acc_stats_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccStatsId).HasColumnName("acc_stats_id");
            entity.Property(e => e.BestFame).HasColumnName("best_fame");
            entity.Property(e => e.BestLevel).HasColumnName("best_level");
            entity.Property(e => e.ObjectType).HasColumnName("object_type");

            entity.HasOne(d => d.AccStats).WithMany(p => p.ClassStats)
                .HasForeignKey(d => d.AccStatsId)
                .HasConstraintName("Class_Stats_ibfk_1");
        });

        modelBuilder.Entity<CombatStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Combat_Stats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AbilitiesUsed).HasColumnName("abilities_used");
            entity.Property(e => e.DamageDealt).HasColumnName("damage_dealt");
            entity.Property(e => e.DamageTaken).HasColumnName("damage_taken");
            entity.Property(e => e.LevelUpAssists).HasColumnName("level_up_assists");
            entity.Property(e => e.PotionsDrank).HasColumnName("potions_drank");
            entity.Property(e => e.Shots).HasColumnName("shots");
            entity.Property(e => e.ShotsHit).HasColumnName("shots_hit");
        });

        modelBuilder.Entity<DungeonStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Dungeon_Stats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompletedCount).HasColumnName("completed_count");
            entity.Property(e => e.DungeonName)
                .HasMaxLength(255)
                .HasColumnName("dungeon_name");
        });

        modelBuilder.Entity<ExplorationStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Exploration_Stats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Escapes).HasColumnName("escapes");
            entity.Property(e => e.MinutesActive).HasColumnName("minutes_active");
            entity.Property(e => e.NearDeathEscapes).HasColumnName("near_death_escapes");
            entity.Property(e => e.QuestsCompleted).HasColumnName("quests_completed");
            entity.Property(e => e.Teleports).HasColumnName("teleports");
            entity.Property(e => e.TilesUncovered).HasColumnName("tiles_uncovered");
        });

        modelBuilder.Entity<Guild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentFame).HasColumnName("current_fame");
            entity.Property(e => e.GuildBoard)
                .HasColumnType("text")
                .HasColumnName("guild_board");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TotalFame).HasColumnName("total_fame");
        });

        modelBuilder.Entity<GuildMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Guild_Members");

            entity.HasIndex(e => e.GuildId, "guild_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GuildId).HasColumnName("guild_id");
            entity.Property(e => e.GuildRank).HasColumnName("guild_rank");
            entity.Property(e => e.LastSeenAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("datetime")
                .HasColumnName("last_seen_at");

            entity.HasOne(d => d.Guild).WithMany(p => p.GuildMembers)
                .HasForeignKey(d => d.GuildId)
                .HasConstraintName("Guild_Members_ibfk_1");
        });

        modelBuilder.Entity<KillStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Kill_Stats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BlueBags).HasColumnName("blue_bags");
            entity.Property(e => e.CubeAssists).HasColumnName("cube_assists");
            entity.Property(e => e.CubeKills).HasColumnName("cube_kills");
            entity.Property(e => e.CyanBags).HasColumnName("cyan_bags");
            entity.Property(e => e.GodAssists).HasColumnName("god_assists");
            entity.Property(e => e.GodKills).HasColumnName("god_kills");
            entity.Property(e => e.MonsterAssists).HasColumnName("monster_assists");
            entity.Property(e => e.MonsterKills).HasColumnName("monster_kills");
            entity.Property(e => e.OryxAssists).HasColumnName("oryx_assists");
            entity.Property(e => e.OryxKills).HasColumnName("oryx_kills");
            entity.Property(e => e.WhiteBags).HasColumnName("white_bags");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IPAddress)
                .HasMaxLength(30)
                .HasColumnName("ip_address");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("datetime")
                .HasColumnName("last_login_at");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("text")
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasColumnType("text")
                .HasColumnName("password_salt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
