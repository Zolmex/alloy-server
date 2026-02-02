CREATE TABLE `Accounts` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(30) UNIQUE NOT NULL,
  `rank` smallint,
  `guild_name` varchar(255),
  `is_admin` boolean,
  `is_banned` boolean,
  `max_chars` smallint,
  `vault_count` smallint,
  `next_char_id` smallint,
  `created_at` datetime DEFAULT (NOW()),
  `acc_stats_id` integer,
  `login_id` integer,
  `guild_member_id` integer
);

CREATE TABLE `Account_Stats` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `best_char_fame` int unsigned,
  `current_fame` int unsigned,
  `total_fame` int unsigned,
  `current_credits` int unsigned,
  `total_credits` int unsigned
);

CREATE TABLE `Account_Locks` (
  `account_id` integer,
  `locked_id` integer,
  PRIMARY KEY (`account_id`, `locked_id`)
);

CREATE TABLE `Account_Ignores` (
  `account_id` integer,
  `ignored_id` integer,
  PRIMARY KEY (`account_id`, `ignored_id`)
);

CREATE TABLE `Class_Stats` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `object_type` smallint unsigned,
  `best_level` smallint unsigned,
  `best_fame` int unsigned,
  `acc_stats_id` integer
);

CREATE TABLE `Logins` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(30) UNIQUE NOT NULL,
  `password_hash` text,
  `password_salt` text,
  `last_login_at` datetime,
  `ip_address` varchar(30)
);

CREATE TABLE `Account_Skins` (
  `account_id` integer,
  `skin_type` integer,
  PRIMARY KEY (`account_id`, `skin_type`)
);

CREATE TABLE `Characters` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `acc_char_id` integer,
  `object_type` smallint unsigned,
  `level` smallint unsigned,
  `current_fame` int unsigned,
  `xp_points` int unsigned,
  `skin_type` smallint unsigned,
  `texture_one` smallint unsigned,
  `texture_two` smallint unsigned,
  `pet_type` smallint unsigned,
  `health_potions` smallint unsigned,
  `magic_potions` smallint unsigned,
  `is_dead` boolean,
  `is_deleted` boolean,
  `has_backpack` boolean,
  `created_at` datetime DEFAULT (NOW()),
  `deleted_at` datetime,
  `acc_id` integer,
  `char_stats_id` integer,
  `explo_stats_id` integer,
  `combat_stats_id` integer,
  `kill_stats_id` integer,
  `dungeon_stats_id` integer
);

CREATE TABLE `Character_Stats` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `hp` int unsigned,
  `mp` int unsigned,
  `max_hp` int unsigned,
  `max_mp` int unsigned,
  `attack` int unsigned,
  `defense` int unsigned,
  `speed` int unsigned,
  `dexterity` int unsigned,
  `vitality` int unsigned,
  `wisdom` int unsigned
);

CREATE TABLE `Character_Inventory` (
  `character_id` integer,
  `slot_id` integer,
  `item_type` smallint unsigned,
  `item_data` blob,
  PRIMARY KEY (`character_id`, `slot_id`)
);

CREATE TABLE `Character_Death` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `dead_at` datetime DEFAULT (NOW()),
  `death_fame` int unsigned,
  `char_id` integer
);

CREATE TABLE `Exploration_Stats` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `tiles_uncovered` int unsigned,
  `quests_completed` int unsigned,
  `escapes` int unsigned,
  `near_death_escapes` int unsigned,
  `minutes_active` int unsigned,
  `teleports` int unsigned
);

CREATE TABLE `Combat_Stats` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `shots` bigint unsigned,
  `shots_hit` int unsigned,
  `level_up_assists` int unsigned,
  `potions_drank` smallint unsigned,
  `abilities_used` smallint unsigned,
  `damage_taken` int unsigned,
  `damage_dealt` int unsigned
);

CREATE TABLE `Kill_Stats` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `monster_kills` int unsigned,
  `monster_assists` int unsigned,
  `god_kills` int unsigned,
  `god_assists` int unsigned,
  `oryx_kills` smallint unsigned,
  `oryx_assists` smallint unsigned,
  `cube_kills` smallint unsigned,
  `cube_assists` smallint unsigned,
  `blue_bags` smallint unsigned,
  `cyan_bags` smallint unsigned,
  `white_bags` smallint unsigned
);

CREATE TABLE `Dungeon_Stats` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `dungeon_name` varchar(255),
  `completed_count` smallint unsigned
);

CREATE TABLE `Guilds` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(255),
  `level` smallint,
  `current_fame` int unsigned,
  `total_fame` int unsigned,
  `guild_board` text,
  `created_at` datetime DEFAULT (NOW())
);

CREATE TABLE `Guild_Members` (
  `id` integer PRIMARY KEY AUTO_INCREMENT,
  `guild_rank` smallint,
  `last_seen_at` datetime DEFAULT (NOW()),
  `guild_id` integer
);

ALTER TABLE `Accounts` ADD FOREIGN KEY (`acc_stats_id`) REFERENCES `Account_Stats` (`id`);

ALTER TABLE `Accounts` ADD FOREIGN KEY (`login_id`) REFERENCES `Logins` (`id`);

ALTER TABLE `Accounts` ADD FOREIGN KEY (`guild_member_id`) REFERENCES `Guild_Members` (`id`);

ALTER TABLE `Account_Locks` ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Locks` ADD FOREIGN KEY (`locked_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Ignores` ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Account_Ignores` ADD FOREIGN KEY (`ignored_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Class_Stats` ADD FOREIGN KEY (`acc_stats_id`) REFERENCES `Account_Stats` (`id`);

ALTER TABLE `Account_Skins` ADD FOREIGN KEY (`account_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Characters` ADD FOREIGN KEY (`acc_id`) REFERENCES `Accounts` (`id`);

ALTER TABLE `Characters` ADD FOREIGN KEY (`char_stats_id`) REFERENCES `Character_Stats` (`id`);

ALTER TABLE `Characters` ADD FOREIGN KEY (`explo_stats_id`) REFERENCES `Exploration_Stats` (`id`);

ALTER TABLE `Characters` ADD FOREIGN KEY (`combat_stats_id`) REFERENCES `Combat_Stats` (`id`);

ALTER TABLE `Characters` ADD FOREIGN KEY (`kill_stats_id`) REFERENCES `Kill_Stats` (`id`);

ALTER TABLE `Characters` ADD FOREIGN KEY (`dungeon_stats_id`) REFERENCES `Dungeon_Stats` (`id`);

ALTER TABLE `Character_Inventory` ADD FOREIGN KEY (`character_id`) REFERENCES `Characters` (`id`);

ALTER TABLE `Character_Death` ADD FOREIGN KEY (`char_id`) REFERENCES `Characters` (`id`);

ALTER TABLE `Guild_Members` ADD FOREIGN KEY (`guild_id`) REFERENCES `Guilds` (`id`);
