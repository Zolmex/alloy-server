#region

using System;
using System.ComponentModel;

#endregion

namespace Common
{
    public enum RegisterStatus
    {
        [Description("Success.")] Success,
        [Description("Name taken.")] NameInUse,
        [Description("Invalid name.")] InvalidName,
        [Description("Invalid password.")] InvalidPassword,

        [Description("Accounts limit reached.")]
        MaxAccountsReached,
        [Description("Internal server error")] InternalError
    }

    public enum BuyStatus
    {
        [Description("Success.")] Success,
        [Description("Not enough gold.")] NotEnoughCredits,
        [Description("Not enough fame.")] NotEnoughFame
    }

    public enum CharResult
    {
        [Description("Success")] Success,

        [Description("You don't own this skin.")]
        SkinNotOwned,

        [Description("Characters limit reached.")]
        MaxCharactersReached,

        [Description("Internal server error.")]
        InternalError
    }

    public enum VerifyStatus
    {
        [Description("Success")] Success,

        [Description("Invalid account credentials.")]
        InvalidCredentials
    }

    public enum StatType
    {
        MaxHP,
        HP,
        Size,
        MaxMP,
        MP,
        NextLevelXp,
        Experience,
        Level,
        Inventory0,
        Inventory1,
        Inventory2,
        Inventory3,
        Inventory4,
        Inventory5,
        Inventory6,
        Inventory7,
        Inventory8,
        Inventory9,
        Inventory10,
        Inventory11,
        Attack,
        Defense,
        MovementSpeed,
        LifeRegeneration,
        Wisdom,
        Dexterity,
        Condition1,
        NumStars,
        Name,
        Tex1,
        Tex2,
        MerchandiseType,
        MerchandisePrice,
        Credits,
        Active,
        AccountId,
        Fame,
        MerchandiseCurrency,
        Connect,
        MerchandiseCount,
        MerchandiseMinsLeft,
        MerchandiseDiscount,
        MerchandiseRankReq,
        CharFame,
        NextClassQuestFame,
        LegendaryRank,
        SinkLevel,
        AltTexture,
        GuildName,
        GuildRank,
        Oxygen,
        HealthPotionStack,
        MagicPotionStack,
        Backpack0,
        Backpack1,
        Backpack2,
        Backpack3,
        Backpack4,
        Backpack5,
        Backpack6,
        Backpack7,
        HasBackpack,
        Texture,
        InventoryData0,
        InventoryData1,
        InventoryData2,
        InventoryData3,
        InventoryData4,
        InventoryData5,
        InventoryData6,
        InventoryData7,
        InventoryData8,
        InventoryData9,
        InventoryData10,
        InventoryData11,
        InventoryData12,
        InventoryData13,
        InventoryData14,
        InventoryData15,
        InventoryData16,
        InventoryData17,
        InventoryData18,
        InventoryData19,
        PrimaryConstellation,
        SecondaryConstellation,
        PrimaryNodeData,
        SecondaryNodeData,
        DodgeChance,
        CriticalChance,
        CriticalDamage,
        MaxMS,
        MS,
        ManaRegeneration,
        MSRegenRate,
        Armor,
        DamageMultiplier,
        UNUSED,
        StatPoints,
        TimeInCombat,
        MaxHPBonus,
        MaxMPBonus,
        AttackBonus,
        DefenseBonus,
        MovementSpeedBonus,
        DexterityBonus,
        LifeRegenerationBonus,
        WisdomBonus,
        DodgeChanceBonus,
        CriticalChanceBonus,
        CriticalDamageBonus,
        MaxMSBonus,
        ManaRegenerationBonus,
        MSRegenRateBonus,
        ArmorBonus,
        DamageBonus,
        UNUSED2,
        AttackSpeed,
        AttackSpeedBonus,
        Condition2,
        AccRank,
        QuestId,
        PartyId,
        AbilityDataA,
        AbilityDataB,
        AbilityDataC,
        AbilityDataD,

        StatTypeCount, // Don't remove this
        None = int.MaxValue
    }

    public enum ItemType
    {
        All,
        Sword,
        Dagger,
        Bow,
        Tome,
        Shield,
        Leather,
        Plate,
        Wand,
        Ring,
        Potion,
        Spell,
        Seal,
        Cloak,
        Robe,
        Quiver,
        Helm,
        Staff,
        Poison,
        Skull,
        Trap,
        Orb,
        Prism,
        Scepter,
        Katana,
        Shuriken,

        Weapon = 50,
        Ability = 51,
        Armor = 52
    }

    [Flags]
    public enum ConditionEffectIndex : byte
    {
        Nothing = 0,
        Dead = 1,
        Quiet = 2,
        Weak = 3,
        Slowed = 4,
        Sick = 5,
        Dazed = 6,
        Stunned = 7,
        Blind = 8,
        Hallucinating = 9,
        Drunk = 10,
        Confused = 11,
        StunImmune = 12,
        Invisible = 13,
        Paralyzed = 14,
        Speedy = 15,
        Bleeding = 16,
        ArmorBrokenImmune = 17,
        Healing = 18,
        Damaging = 19,
        Berserk = 20,
        Paused = 21,
        Stasis = 22,
        StasisImmune = 23,
        Invincible = 24,
        Invulnerable = 25,
        Armored = 26,
        ArmorBroken = 27,
        Hexed = 28,
        NinjaSpeedy = 29,
        Unstable = 30,
        Darkness = 31,
        SlowedImmune = 32,
        DazedImmune = 33,
        ParalyzedImmune = 34,
        Petrify = 35,
        PetrifiedImmune = 36,
        PetEffectIcon = 37,
        Curse = 38,
        CurseImmune = 39,
        HpBoost = 40,
        MpBoost = 41,
        AttBoost = 42,
        DefBoost = 43,
        SpdBoost = 44,
        VitBoost = 45,
        WisBoost = 46,
        DexBoost = 47,
        Silenced = 48,
        Exposed = 49,
        Energized = 50,
        HpDebuff = 51,
        MpDebuff = 52,
        AttDebuff = 53,
        DefDebuff = 54,
        SpdDebuff = 55,
        VitDebuff = 56,
        WisDebuff = 57,
        DexDebuff = 58,
        Inspired = 59,
        ManaDeplete = 60,
        SheatheStance = 61,
        
        ConditionCount
    }

    public enum ActivateEffectIndex
    {
        None,
        Create,
        Dye,
        Shoot,
        IncrementStat,
        Heal,
        Magic,
        HealNova,
        StatBoostSelf,
        StatBoostAura,
        BulletNova,
        ConditionEffectSelf,
        ConditionEffectAura,
        Teleport,
        PoisonGrenade,
        VampireBlast,
        Trap,
        StasisBlast,
        Pet,
        Decoy,
        Lightning,
        UnlockPortal,
        MagicNova,
        ClearConditionEffectAura,
        RemoveNegativeConditions,
        ClearConditionEffectSelf,
        RemoveNegativeConditionsSelf,
        SheatheAttack,
        DazeBlast,
        PermaPet,
        Backpack,
        XPBoost,
        LDBoost,
        LTBoost,
        UnlockSkin,
        MysteryDyes,
        GenericActivate,
        Unlock,
        ObjectToss,
        Fame,

        // to be added
        LevelTwenty,
        MarkAndTeleport,
        SelfTransform,
        GroupTransform,
        CreatePortal,
        Exchange,
        ChangeObject,
        UnlockPetSkin,
        CreatePet,
        TeleportToObject,
        MysteryPortal,
        KillRealmHeroes,
        BulletCreate
    }

    public enum ShowEffectIndex : byte
    {
        Unknown = 0,
        Heal = 1,
        Teleport = 2,
        Stream = 3,
        Throw = 4,
        Nova = 5,
        Poison = 6,
        Line = 7,
        Burst = 8,
        Flow = 9,
        Ring = 10,
        Lightning = 11,
        Collapse = 12,
        Coneblast = 13,
        Jitter = 14,
        Flash = 15,
        ThrowProjectile = 16,
        Inspired = 17,
        SheatheSlash = 18
    }

    public enum TerrainType
    {
        None,
        Mountains,
        HighSand,
        HighPlains,
        HighForest,
        MidSand,
        MidPlains,
        MidForest,
        LowSand,
        LowPlains,
        LowForest,
        ShoreSand,
        ShorePlains,
        BeachTowels
    }

    public enum CurrencyType
    {
        Gold,
        Fame,
        GuildFame
    }

    public enum GuildRank
    {
        Founder = 40,
        Leader = 30,
        Officer = 20,
        Member = 10,
        Initiate = 0
    }
}