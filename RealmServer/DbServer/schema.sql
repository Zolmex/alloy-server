CREATE TABLE `Accounts`
(
    `id`              integer PRIMARY KEY AUTO_INCREMENT,
    `name`            varchar(30) UNIQUE NOT NULL,
    `rank`            smallint           NOT NULL DEFAULT 0,
    `guild_name`      varchar(255),
    `is_admin`        boolean            NOT NULL,
    `is_banned`       boolean            NOT NULL,
    `is_muted`        boolean            NOT NULL,
    `max_chars`       smallint           NOT NULL DEFAULT 0,
    `vault_count`     smallint           NOT NULL DEFAULT 0,
    `next_char_id`    smallint           NOT NULL DEFAULT 0,
    `created_at`      datetime           NOT NULL DEFAULT (NOW()),
    `acc_stats_id`    integer            NOT NULL DEFAULT 0,
    `login_id`        integer            NOT NULL DEFAULT 0,
    `guild_member_id` integer,
    `guild_id`        integer
);

CREATE TABLE `Account_Stats`
(
    `id`              integer PRIMARY KEY AUTO_INCREMENT,
    `best_char_fame`  int unsigned NOT NULL DEFAULT 0,
    `current_fame`    int unsigned NOT NULL DEFAULT 0,
    `total_fame`      int unsigned NOT NULL DEFAULT 0,
    `current_credits` int unsigned NOT NULL DEFAULT 0,
    `total_credits`   int unsigned NOT NULL DEFAULT 0
);

CREATE TABLE `Account_Locks`
(
    `account_id` integer NOT NULL DEFAULT 0,
    `locked_id`  integer NOT NULL DEFAULT 0,
    PRIMARY KEY (`account_id`, `locked_id`)
);

CREATE TABLE `Account_Ignores`
(
    `account_id` integer NOT NULL DEFAULT 0,
    `ignored_id` integer NOT NULL DEFAULT 0,
    PRIMARY KEY (`account_id`, `ignored_id`)
);

CREATE TABLE `Account_Mutes`
(
    `id`           integer PRIMARY KEY AUTO_INCREMENT,
    `reason`       varchar(255),
    `muted_at`     datetime NOT NULL DEFAULT (NOW()),
    `expires_at`   datetime,
    `moderator_id` integer  NOT NULL DEFAULT 0,
    `muted_id`     integer  NOT NULL DEFAULT 0
);

CREATE TABLE `Account_Bans`
(
    `id`           integer PRIMARY KEY AUTO_INCREMENT,
    `reason`       varchar(255),
    `banned_at`    datetime NOT NULL DEFAULT (NOW()),
    `expires_at`   datetime,
    `moderator_id` integer  NOT NULL DEFAULT 0,
    `banned_id`    integer  NOT NULL DEFAULT 0,
    `enabled`      boolean  NOT NULL DEFAULT TRUE
);

CREATE TABLE `Account_Skins`
(
    `account_id` integer,
    `skin_type`  integer NOT NULL DEFAULT 0,
    PRIMARY KEY (`account_id`, `skin_type`)
);

CREATE TABLE `Account_Vault`
(
    `account_id` integer,
    `slot_id`    integer NOT NULL,
    `item_type`  smallint unsigned NOT NULL DEFAULT 0,
    `item_data`  blob,
    PRIMARY KEY (`account_id`, `slot_id`)
);

CREATE TABLE `Account_Gifts`
(
    `account_id` integer,
    `slot_id`    integer NOT NULL,
    `item_type`  smallint unsigned NOT NULL DEFAULT 0,
    `item_data`  blob,
    PRIMARY KEY (`account_id`, `slot_id`)
);

CREATE TABLE `Class_Stats`
(
    `id`           integer PRIMARY KEY AUTO_INCREMENT,
    `object_type`  smallint unsigned NOT NULL DEFAULT 0,
    `best_level`   smallint unsigned NOT NULL DEFAULT 0,
    `best_fame`    int unsigned NOT NULL DEFAULT 0,
    `acc_stats_id` integer NOT NULL DEFAULT 0
);

CREATE TABLE `Logins`
(
    `id`            integer PRIMARY KEY AUTO_INCREMENT,
    `name`          varchar(30) UNIQUE NOT NULL,
    `password_hash` text,
    `password_salt` text,
    `last_login_at` datetime,
    `ip_address`    varchar(30)
);

CREATE TABLE `Characters`
(
    `id`               integer PRIMARY KEY AUTO_INCREMENT,
    `acc_char_id`      integer  NOT NULL DEFAULT 0,
    `object_type`      smallint unsigned NOT NULL DEFAULT 0,
    `level`            smallint unsigned NOT NULL DEFAULT 0,
    `current_fame`     int unsigned NOT NULL DEFAULT 0,
    `xp_points`        int unsigned NOT NULL DEFAULT 0,
    `skin_type`        smallint unsigned NOT NULL DEFAULT 0,
    `texture_one`      smallint unsigned NOT NULL DEFAULT 0,
    `texture_two`      smallint unsigned NOT NULL DEFAULT 0,
    `pet_type`         smallint unsigned NOT NULL DEFAULT 0,
    `health_potions`   smallint unsigned NOT NULL DEFAULT 0,
    `magic_potions`    smallint unsigned NOT NULL DEFAULT 0,
    `is_dead`          boolean  NOT NULL,
    `is_deleted`       boolean  NOT NULL,
    `has_backpack`     boolean  NOT NULL,
    `created_at`       datetime NOT NULL DEFAULT (NOW()),
    `deleted_at`       datetime,
    `acc_id`           integer  NOT NULL DEFAULT 0,
    `char_stats_id`    integer  NOT NULL DEFAULT 0,
    `explo_stats_id`   integer  NOT NULL DEFAULT 0,
    `combat_stats_id`  integer  NOT NULL DEFAULT 0,
    `kill_stats_id`    integer  NOT NULL DEFAULT 0,
    `dungeon_stats_id` integer  NOT NULL DEFAULT 0
);

CREATE TABLE `Character_Stats`
(
    `id`        integer PRIMARY KEY AUTO_INCREMENT,
    `hp`        int unsigned NOT NULL DEFAULT 0,
    `mp`        int unsigned NOT NULL DEFAULT 0,
    `max_hp`    int unsigned NOT NULL DEFAULT 0,
    `max_mp`    int unsigned NOT NULL DEFAULT 0,
    `attack`    int unsigned NOT NULL DEFAULT 0,
    `defense`   int unsigned NOT NULL DEFAULT 0,
    `speed`     int unsigned NOT NULL DEFAULT 0,
    `dexterity` int unsigned NOT NULL DEFAULT 0,
    `vitality`  int unsigned NOT NULL DEFAULT 0,
    `wisdom`    int unsigned NOT NULL DEFAULT 0
);

CREATE TABLE `Character_Inventory`
(
    `character_id` integer,
    `slot_id`      integer NOT NULL,
    `item_type`    smallint unsigned NOT NULL DEFAULT 0,
    `item_data`    blob,
    PRIMARY KEY (`character_id`, `slot_id`)
);

CREATE TABLE `Character_Death`
(
    `id`         integer PRIMARY KEY AUTO_INCREMENT,
    `dead_at`    datetime NOT NULL DEFAULT (NOW()),
    `death_fame` int unsigned NOT NULL DEFAULT 0,
    `char_id`    integer  NOT NULL DEFAULT 0
);

CREATE TABLE `Exploration_Stats`
(
    `id`                 integer PRIMARY KEY AUTO_INCREMENT,
    `tiles_uncovered`    int unsigned NOT NULL DEFAULT 0,
    `quests_completed`   int unsigned NOT NULL DEFAULT 0,
    `escapes`            int unsigned NOT NULL DEFAULT 0,
    `near_death_escapes` int unsigned NOT NULL DEFAULT 0,
    `minutes_active`     int unsigned NOT NULL DEFAULT 0,
    `teleports`          int unsigned NOT NULL DEFAULT 0
);

CREATE TABLE `Combat_Stats`
(
    `id`               integer PRIMARY KEY AUTO_INCREMENT,
    `shots`            bigint unsigned NOT NULL DEFAULT 0,
    `shots_hit`        int unsigned NOT NULL DEFAULT 0,
    `level_up_assists` int unsigned NOT NULL DEFAULT 0,
    `potions_drank`    smallint unsigned NOT NULL DEFAULT 0,
    `abilities_used`   smallint unsigned NOT NULL DEFAULT 0,
    `damage_taken`     int unsigned NOT NULL DEFAULT 0,
    `damage_dealt`     int unsigned NOT NULL DEFAULT 0
);

CREATE TABLE `Kill_Stats`
(
    `id`              integer PRIMARY KEY AUTO_INCREMENT,
    `monster_kills`   int unsigned NOT NULL DEFAULT 0,
    `monster_assists` int unsigned NOT NULL DEFAULT 0,
    `god_kills`       int unsigned NOT NULL DEFAULT 0,
    `god_assists`     int unsigned NOT NULL DEFAULT 0,
    `oryx_kills`      smallint unsigned NOT NULL DEFAULT 0,
    `oryx_assists`    smallint unsigned NOT NULL DEFAULT 0,
    `cube_kills`      smallint unsigned NOT NULL DEFAULT 0,
    `cube_assists`    smallint unsigned NOT NULL DEFAULT 0,
    `blue_bags`       smallint unsigned NOT NULL DEFAULT 0,
    `cyan_bags`       smallint unsigned NOT NULL DEFAULT 0,
    `white_bags`      smallint unsigned NOT NULL DEFAULT 0
);

CREATE TABLE `Dungeon_Stats`
(
    `id`              integer PRIMARY KEY AUTO_INCREMENT,
    `dungeon_name`    varchar(255),
    `completed_count` smallint unsigned NOT NULL DEFAULT 0
);

CREATE TABLE `Guilds`
(
    `id`           integer PRIMARY KEY AUTO_INCREMENT,
    `name`         varchar(255),
    `level`        smallint NOT NULL DEFAULT 0,
    `current_fame` int unsigned NOT NULL DEFAULT 0,
    `total_fame`   int unsigned NOT NULL DEFAULT 0,
    `guild_board`  text,
    `created_at`   datetime NOT NULL DEFAULT (NOW())
);

CREATE TABLE `Guild_Members`
(
    `id`           integer PRIMARY KEY AUTO_INCREMENT,
    `guild_rank`   smallint NOT NULL DEFAULT 0,
    `last_seen_at` datetime NOT NULL DEFAULT (NOW()),
    `guild_id`     integer  NOT NULL DEFAULT 0
);

ALTER TABLE `Accounts`
    ADD FOREIGN KEY (`acc_stats_id`) REFERENCES `Account_Stats` (`id`);

ALTER TABLE `Accounts`
    ADD FOREIGN KEY (`login_id`) REFERENCES `Logins` (`id`);

ALTER TABLE `Accounts`
    ADD FOREIGN KEY (`guild_member_id`) REFERENCES `Guild_Members` (`id`);

ALTER TABLE `Accounts`
    ADD FOREIGN KEY (`guild_id`) REFERENCES `Guilds` (`id`);

ALTER TABLE `Account_Locks`
    ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Locks`
    ADD FOREIGN KEY (`locked_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Ignores`
    ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Ignores`
    ADD FOREIGN KEY (`ignored_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Mutes`
    ADD FOREIGN KEY (`moderator_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Mutes`
    ADD FOREIGN KEY (`muted_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Bans`
    ADD FOREIGN KEY (`moderator_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Bans`
    ADD FOREIGN KEY (`banned_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Skins`
    ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Vault`
    ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Gifts`
    ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Class_Stats`
    ADD FOREIGN KEY (`acc_stats_id`) REFERENCES `Account_Stats` (`id`);

ALTER TABLE `Characters`
    ADD FOREIGN KEY (`acc_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Characters`
    ADD FOREIGN KEY (`char_stats_id`) REFERENCES `Character_Stats` (`id`);

ALTER TABLE `Characters`
    ADD FOREIGN KEY (`explo_stats_id`) REFERENCES `Exploration_Stats` (`id`);

ALTER TABLE `Characters`
    ADD FOREIGN KEY (`combat_stats_id`) REFERENCES `Combat_Stats` (`id`);

ALTER TABLE `Characters`
    ADD FOREIGN KEY (`kill_stats_id`) REFERENCES `Kill_Stats` (`id`);

ALTER TABLE `Characters`
    ADD FOREIGN KEY (`dungeon_stats_id`) REFERENCES `Dungeon_Stats` (`id`);

ALTER TABLE `Character_Inventory`
    ADD FOREIGN KEY (`character_id`) REFERENCES `Characters` (`id`);

ALTER TABLE `Character_Death`
    ADD FOREIGN KEY (`char_id`) REFERENCES `Characters` (`id`);

ALTER TABLE `Guild_Members`
    ADD FOREIGN KEY (`guild_id`) REFERENCES `Guilds` (`id`);
