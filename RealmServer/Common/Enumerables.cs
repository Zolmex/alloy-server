#region

using System;
using System.ComponentModel;

#endregion

namespace Common;

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
    InvalidCredentials,
    
    [Description("Internal server error.")]
    InternalError
}

public enum FlushStatus
{
    Success,
    VersionMismatch,
    InternalError
}

public enum CreateCharacterStatus
{
    [Description("Success")] Success,

    [Description("You don't own this skin.")]
    SkinNotOwned,

    [Description("Characters limit reached.")]
    MaxCharactersReached,

    [Description("Internal server error.")]
    InternalError
}

public enum StatType
{
    MaxHP = 0,
    HP = 1,
    Size = 2,
    MaxMP = 3,
    MP = 4,
    NextLevelXp = 5,
    Experience = 6,
    Level = 7,
    Inventory0 = 8, // TODO: move all inventory/backpack item types/data into a single array-like stat
    Inventory1 = 9,
    Inventory2 = 10,
    Inventory3 = 11,
    Inventory4 = 12,
    Inventory5 = 13,
    Inventory6 = 14,
    Inventory7 = 15,
    Inventory8 = 16,
    Inventory9 = 17,
    Inventory10 = 18,
    Inventory11 = 19,
    Attack = 20,
    Defense = 21,
    Speed = 22,
    Vitality = 23,
    Wisdom = 24,
    Dexterity = 25,
    Condition1 = 26,
    NumStars = 27,
    Name = 28,
    Tex1 = 29,
    Tex2 = 30,
    MerchandiseType = 31,
    MerchandisePrice = 32,
    Credits = 33,
    Active = 34,
    AccountId = 35,
    Fame = 36,
    MerchandiseCurrency = 37,
    Connect = 38,
    MerchandiseCount = 39,
    MerchandiseMinsLeft = 40,
    MerchandiseDiscount = 41,
    MerchandiseRankReq = 42,
    CharFame = 43,
    NextClassQuestFame = 44,
    LegendaryRank = 45,
    SinkLevel = 46,
    AltTexture = 47,
    GuildName = 48,
    GuildRank = 49,
    Oxygen = 50,
    HealthPotionStack = 51,
    MagicPotionStack = 52,
    Backpack0 = 53,
    Backpack1 = 54,
    Backpack2 = 55,
    Backpack3 = 56,
    Backpack4 = 57,
    Backpack5 = 58,
    Backpack6 = 59,
    Backpack7 = 60,
    HasBackpack = 61,
    Texture = 62,
    InventoryData0 = 63,
    InventoryData1 = 64,
    InventoryData2 = 65,
    InventoryData3 = 66,
    InventoryData4 = 67,
    InventoryData5 = 68,
    InventoryData6 = 69,
    InventoryData7 = 70,
    InventoryData8 = 71,
    InventoryData9 = 72,
    InventoryData10 = 73,
    InventoryData11 = 74,
    InventoryData12 = 75,
    InventoryData13 = 76,
    InventoryData14 = 77,
    InventoryData15 = 78,
    InventoryData16 = 79,
    InventoryData17 = 80,
    InventoryData18 = 81,
    InventoryData19 = 82,
    Glow = 83,
    AltTextureIndex = 84,
    PortalUsable = 85,
    Unused3 = 86,
    Unused = 87,
    Unused2 = 88,
    Unused6 = 89,
    Unused7 = 90,
    Unused8 = 91,
    Unused9 = 92,
    Unused10 = 93,
    Unused11 = 94,
    Unused12 = 95,
    Unused4 = 96,
    Unused13 = 97,
    Unused14 = 98,
    MaxHPBonus = 99,
    MaxMPBonus = 100,
    AttackBonus = 101,
    DefenseBonus = 102,
    SpeedBonus = 103,
    DexterityBonus = 104,
    VitalityBonus = 105,
    WisdomBonus = 106,
    Unused15 = 107,
    Unused16 = 108,
    Unused17 = 109,
    Unused18 = 110,
    Unused19 = 111,
    Unused20 = 112,
    Unused21 = 113,
    Unused22 = 114,
    Unused5 = 115,
    Unused23 = 116,
    Unused24 = 117,
    Condition2 = 118,
    AccRank = 119,
    QuestId = 120,
    Unused25 = 121,
    Unused26 = 122,
    Unused27 = 123,
    Unused28 = 124,
    Unused29 = 125,

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