namespace Common.Resources.Xml.Descriptors;

public enum PathType : byte
{
    LinePath,
    WavyPath,
    CirclePath,
    AmplitudePath,
    BoomerangPath,
    AcceleratePath,
    DeceleratePath,
    ChangeSpeedPath,
    CombinedPath
}

public enum ItemField : byte // Values here must equal the class name they use (if it's an ItemData field)
{
    ObjectId,
    ObjectType,
    SlotType,
    Usable,
    BagType,
    Consumable,
    Potion,
    Soulbound,
    Tex1,
    Tex2,
    Tier,
    Description,
    RateOfFire,
    MpCost,
    FameBonus,
    NumProjectiles,
    ArcGap,
    DisplayId,
    Cooldown,
    Resurrects,
    Doses,
    MaxDoses,
    EquipmentBoosts,
    ActivateEffects,
    Projectile,
    Rarity,
    GemstoneLimit,
    Gemstones,
    Gemstone,
    ItemLevel,
    LevelIncreases
}

public enum ActivateEffectField : byte
{
    Index,
    EffectDesc,
    DurationMS,
    Range,
    Amount,
    TotalDamage,
    Radius,
    Color,
    MaxTargets,
    ObjectId,
    LevelIncreases
}

public enum ProjectileField : byte
{
    BulletId,
    ObjectId,
    LifetimeMS,
    Speed,
    Damage,
    MinDamage,
    MaxDamage,
    Effects,
    MultiHit,
    PassesCover,
    ArmorPiercing,
    Wavy,
    Parametric,
    Boomerang,
    Amplitude,
    Frequency,
    Magnitude,
    Size,
    LevelIncreases
}

public enum GemstoneField : byte
{
    SlotTypes,
    Origin,
    Boosts
}

public enum ConditionEffectField : byte
{
    Effect,
    DurationMS
}

public enum GemstoneBoostField : byte
{
    BoostType,
    Stat,
    Amount,
    BoostTarget
}

public enum LevelIncreaseField : byte
{
    Field,
    Rate,
    Amount
}

public enum EquipmentBoostField : byte
{
    Stat,
    Amount,
    LevelIncrease
}

public enum SheathDescField : byte
{
    Capacity,
    SlashDamage,
    Efficiency,
    ManaPerSlash,
    SlashCooldownMS,
    Radius,
    Effects,
    StanceDuration
}

public enum SpellDescField : byte
{
    MpCost,
    NumProjectiles,
    ProjectileId,
    MpRefundPerHit,
}

public enum QuiverDescField : byte
{
    MpCost,
    ProjectileId,
    ArrowChance,
    MaxArrows,
    UseMpArrows,
    MpProjectileId,
}

public enum PoisonDescField : byte
{
    MpCost,
    Effects,
    Damage,
    TickSpeed,
    Duration,
    ThrowRange,
    ThrowTravelTime,
    PoisonRange,
    SpreadRange,
    SpreadTargetsNum,
    SpreadEfficiency,
    ImpactDamage,
}

public enum HelmDescField : byte
{
    MpCost,
    Duration,
    StacksGain,
    StacksLost,
    StatsModifier,
    HoldEffects,
    MpDrain,
}

public enum CloakDescField : byte
{
    MpCost,
    Duration,
    StatsModifier,
    MinStatEfficiency,
    BoostDuration,
}

public enum SealDescField : byte
{
    MpCost,
    Duration,
    MaxStack,
    BannerSpeed,
    Radius,
    ShieldAmount,
    MaxAllies,
    EfficiencyReductionPerPlayer,
    StatsModifier,
    BoostDuration,
}

public enum FieldBoostType
{
    All,
    ItemLevel,
    Gemstone,
    Modifiers
}