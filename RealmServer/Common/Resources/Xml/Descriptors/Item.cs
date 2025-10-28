using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;
public class Item : ItemData
{
    private static readonly Logger _log = new(typeof(Item));

    public string DisplayName => DisplayId ?? ObjectId;
    public readonly XElement Root;

    public override Type FieldsEnum => typeof(ItemField);

    public string ObjectId
    {
        get => GetValue<string>(0);
        set => SetValue(0, value);
    }

    public ushort ObjectType
    {
        get => GetValue<ushort>(1);
        set => SetValue(1, value);
    }

    public int SlotType
    {
        get => GetValue<int>(2);
        set => SetValue(2, value);
    }

    public bool Usable
    {
        get => GetValue<bool>(3);
        set => SetValue(3, value);
    }

    public int BagType
    {
        get => GetValue<int>(4);
        set => SetValue(4, value);
    }

    public bool Consumable
    {
        get => GetValue<bool>(5);
        set => SetValue(5, value);
    }

    public bool Potion
    {
        get => GetValue<bool>(6);
        set => SetValue(6, value);
    }

    public bool Soulbound
    {
        get => GetValue<bool>(7);
        set => SetValue(7, value);
    }

    public int Tex1
    {
        get => GetValue<int>(8);
        set => SetValue(8, value);
    }

    public int Tex2
    {
        get => GetValue<int>(9);
        set => SetValue(9, value);
    }

    public int Tier
    {
        get => GetValue<int>(10);
        set => SetValue(10, value);
    }

    public string Description
    {
        get => GetValue<string>(11);
        set => SetValue(11, value);
    }

    public float RateOfFire
    {
        get => GetValue<float>(12);
        set => SetValue(12, value);
    }

    public int MpCost
    {
        get => GetValue<int>(13);
        set => SetValue(13, value);
    }

    public int FameBonus
    {
        get => GetValue<int>(14);
        set => SetValue(14, value);
    }

    public byte NumProjectiles
    {
        get => GetValue<byte>(15);
        set => SetValue(15, value);
    }

    public float ArcGap
    {
        get => GetValue<float>(16);
        set => SetValue(16, value);
    }

    public string DisplayId
    {
        get => GetValue<string>(17);
        set => SetValue(17, value);
    }

    public int Cooldown
    {
        get => GetValue<int>(18);
        set => SetValue(18, value);
    }

    public bool Resurrects
    {
        get => GetValue<bool>(19);
        set => SetValue(19, value);
    }

    public int Doses
    {
        get => GetValue<int>(20);
        set => SetValue(20, value);
    }

    public int MaxDoses
    {
        get => GetValue<int>(21);
        set => SetValue(21, value);
    }

    public EquipmentBoost[] StatBoosts
    {
        get => GetValue<EquipmentBoost[]>(22);
        set => SetValue(22, value);
    }

    public ActivateEffectDesc[] ActivateEffects
    {
        get => GetValue<ActivateEffectDesc[]>(23);
        set => SetValue(23, value);
    }

    public ProjectileDesc[] Projectiles
    {
        get => GetValue<ProjectileDesc[]>(24);
        set => SetValue(24, value);
    }

    public string Rarity
    {
        get => GetValue<string>(25);
        set => SetValue(25, value);
    }

    public int GemstoneLimit
    {
        get => GetValue<int>(26);
        set => SetValue(26, value);
    }

    public int[] Gemstones
    {
        get => GetValue<int[]>(27);
        set => SetValue(27, value);
    }

    public GemstoneDesc Gemstone
    {
        get => GetValue<GemstoneDesc>(28);
        set => SetValue(28, value);
    }

    public int ItemLevel
    {
        get => GetValue<int>(29);
        set => SetValue(29, value);
    }

    public LevelIncreaseDesc[] LevelIncreases
    {
        get => GetValue<LevelIncreaseDesc[]>(30);
        set => SetValue(30, value);
    }

    public int UpgradeLevels
    {
        get => GetValue<int>(31);
        set => SetValue(31, value);
    }

    public int MpEndCost
    {
        get => GetValue<int>(32);
        set => SetValue(32, value);
    }

    public int MaxCharge
    {
        get => GetValue<int>(33);
        set => SetValue(33, value);
    }

    public int MpCostPerSecond
    {
        get => GetValue<int>(34);
        set => SetValue(34, value);
    }

    public SheathDesc Sheath
    {
        get => GetValue<SheathDesc>(35);
        set => SetValue(35, value);
    }

    public SpellDesc Spell
    {
        get => GetValue<SpellDesc>(36);
        set => SetValue(36, value);
    }
    
    public QuiverDesc Quiver
    {
        get => GetValue<QuiverDesc>(37);
        set => SetValue(37, value);
    }
    
    public PoisonDesc Poison
    {
        get => GetValue<PoisonDesc>(38);
        set => SetValue(38, value);
    }
    
    public HelmDesc Helm
    {
        get => GetValue<HelmDesc>(39);
        set => SetValue(39, value);
    }
    
    public CloakDesc Cloak
    {
        get => GetValue<CloakDesc>(40);
        set => SetValue(40, value);
    }
    
    public SealDesc Seal
    {
        get => GetValue<SealDesc>(41);
        set => SetValue(41, value);
    }

    public readonly TextureDesc Texture;
    public readonly bool MultiPhase;

    private readonly Dictionary<FieldBoostType, Dictionary<string, ItemFieldBoost>>
        _fieldBoosts = new(); // This contains all types of boosts. Field is a string (to allow for deepfields)

    private readonly List<FieldBoostType> _fieldBoostUpdates = new();

    public Item(XElement e)
    {
        if (e == null) // Null when instanced by itemdata import
        {
            _initialized = true;
            return;
        }

        Root = e;
        ObjectType = e.GetAttribute<ushort>("type");
        ObjectId = e.GetAttribute<string>("id");
        SlotType = e.GetValue<int>("SlotType");
        Usable = e.HasElement("Usable");
        BagType = e.GetValue<int>("BagType");
        Consumable = e.HasElement("Consumable") || e.HasElement("InvUse");
        Potion = e.HasElement("Potion");
        Soulbound = e.HasElement("Soulbound");
        Tex1 = (int)e.GetValue<uint>("Tex1", 0);
        Tex2 = (int)e.GetValue<uint>("Tex2", 0);
        Tier = e.GetValue<int>("Tier", -1);
        Description = e.GetValue<string>("Description");
        RateOfFire = e.GetValue<float>("RateOfFire", 1);
        MpCost = e.GetValue<int>("MpCost");
        MpEndCost = e.GetValue<int>("MpEndCost");
        FameBonus = e.GetValue<int>("FameBonus");
        NumProjectiles = e.GetValue<byte>("NumProjectiles", (byte)(SlotType == 25 ? 3 : 1)); // Shurikens have 3 shots as a base
        ArcGap = e.GetValue<float>("ArcGap", 11.25f);
        DisplayId = e.GetValue<string>("DisplayId");
        Doses = e.GetValue<int>("Doses");
        Cooldown = (int)(e.GetValue<float>("Cooldown", 0.5f) * 1000);
        Resurrects = e.HasElement("Resurrects");
        MaxDoses = e.GetValue<int>("Doses");
        Rarity = e.GetValue<string>("Rarity", "Common");
        GemstoneLimit = e.GetValue<int>("GemstoneLimit");
        ItemLevel = e.GetValue<int>("ItemLevel", -1);
        UpgradeLevels = e.GetValue<int>("UpgradeLevels", 0);

        StatBoosts = e.Elements("ActivateOnEquip")
            .Select(i => new EquipmentBoost(i, this, (byte)ItemField.EquipmentBoosts))
            .ToArray();
        ActivateEffects = e.Elements("Activate")
            .Select(i => new ActivateEffectDesc(i, this, (byte)ItemField.ActivateEffects))
            .ToArray();
        Projectiles = e.Elements("Projectile")
            .Select(i =>
            {
                var proj = new ProjectileDesc(i, this, (byte)ItemField.Projectile);
                proj.SetContainer(ObjectType);
                return proj;
            }).ToArray();
        Gemstone = e.HasElement("Gemstone")
            ? new GemstoneDesc(e.Element("Gemstone"), this, (byte)ItemField.Gemstone)
            : null;
        Gemstones = null; // Necessary for item data to work properly...
        LevelIncreases = e.Elements("LevelIncrease")
            .Select(i => new LevelIncreaseDesc(i, this, (byte)ItemField.LevelIncreases))
            .ToArray();

        Texture = e.HasElement("Texture") ? new TextureDesc(e.Element("Texture")) : null;

        MultiPhase = e.HasElement("MultiPhase");
        MaxCharge = e.GetValue<int>("MaxCharge", 1000);
        MpCostPerSecond = e.GetValue<int>("MpCostPerSecond", 50);

        Sheath = e.HasElement("Sheath") ? new SheathDesc(e.Element("Sheath")) : null;
        Spell = e.HasElement("Spell") ? new SpellDesc(e.Element("Spell")) : null;
        Quiver = e.HasElement("Quiver") ? new QuiverDesc(e.Element("Quiver")) : null;
        Poison = e.HasElement("Poison") ? new PoisonDesc(e.Element("Poison")) : null;
        Helm = e.HasElement("Helm") ? new HelmDesc(e.Element("Helm")) : null;
        Cloak = e.HasElement("Cloak") ? new CloakDesc(e.Element("Cloak")) : null;
        Seal = e.HasElement("Seal") ? new SealDesc(e.Element("Seal")) : null;

        _initialized = true;
    }

    public override List<byte>
        Export(List<byte> data = null) // Field boosts should not be counted as the real value, but the original itemdata values. FOR SAVING TO DB ONLY
    {
        ResetFieldBoosts();

        var export = base.Export(data);

        ReloadFieldBoosts();
        return export;
    }

    private void ReloadFieldBoosts()
    {
        ReloadItemLevelBoosts();
        ReloadGemstoneBoosts();

        ApplyFieldBoosts();
    }

    public void AddModifierBoost(ItemData data, string field, bool flat, float amount)
    {
        AddFieldBoost(FieldBoostType.Modifiers, GetFullFieldStr(field, data), flat, amount);

        _fieldBoostUpdates.Add(FieldBoostType.Modifiers);
        ApplyFieldBoosts();
        _fieldBoostUpdates.Clear();
    }

    public void RemoveModifierBoost(ItemData data, string field, bool flat, float amount)
    {
        RemoveFieldBoost(FieldBoostType.Modifiers, GetFullFieldStr(field, data), flat, amount);

        _fieldBoostUpdates.Add(FieldBoostType.Modifiers);
        ApplyFieldBoosts();
        _fieldBoostUpdates.Clear();
    }

    protected override void HandleFieldUpdate(byte field)
    {
        if (field == (byte)ItemField.ItemLevel)
            ReloadItemLevelBoosts();

        if (field == (byte)ItemField.Gemstones)
            ReloadGemstoneBoosts();

        if (_fieldBoostUpdates.Count > 0)
        {
            ApplyFieldBoosts();
            _fieldBoostUpdates.Clear();
        }
    }

    private void ReloadItemLevelBoosts()
    {
        ResetFieldBoosts(FieldBoostType.ItemLevel);
        if (!_initialized || ItemLevel < 0)
            return;

        foreach (var inc in LevelIncreases) // Item increases
            AddFieldBoost(FieldBoostType.ItemLevel, inc.Field, true, inc.Amount * (ItemLevel / inc.Rate));

        foreach (var statBoost in StatBoosts) // Equipment boost increases
            if (statBoost.LevelIncrease != null)
                AddFieldBoost(FieldBoostType.ItemLevel, GetFullFieldStr(statBoost.LevelIncrease.Field, statBoost),
                    true,
                    statBoost.LevelIncrease.Amount * (ItemLevel / statBoost.LevelIncrease.Rate));

        foreach (var ae in ActivateEffects) // Activate effects increases
            if (ae.LevelIncreases != null)
                foreach (var inc in ae.LevelIncreases)
                    AddFieldBoost(FieldBoostType.ItemLevel, GetFullFieldStr(inc.Field, ae), true,
                        inc.Amount * (ItemLevel / inc.Rate));

        foreach (var Projectile in Projectiles) // Projectile increases
            if (Projectile is { LevelIncreases: not null }) 
                foreach (var inc in Projectile.LevelIncreases)
                    AddFieldBoost(FieldBoostType.ItemLevel, GetFullFieldStr(inc.Field, Projectile), true,
                        inc.Amount * (ItemLevel / inc.Rate));

        _fieldBoostUpdates.Add(FieldBoostType.ItemLevel);
    }

    public void ReloadGemstoneBoosts()
    {
        ResetFieldBoosts(FieldBoostType.Gemstone);
        if (!_initialized || Gemstones == null || Gemstones.Length < 1)
            return;

        foreach (var gemType in Gemstones)
        {
            var gemItem = XmlLibrary.Gemstones[(ushort)gemType];
            if (gemItem.Gemstone.Boosts?.Length < 1)
                continue;

            foreach (var boost in gemItem.Gemstone.Boosts)
            {
                if (boost.BoostTarget != "Item")
                    continue;

                AddFieldBoost(FieldBoostType.Gemstone, boost.Stat, boost.BoostType == "Static", boost.Amount);
            }
        }

        _fieldBoostUpdates.Add(FieldBoostType.Gemstone);
    }

    public void UpdateDeepFields(string statStr, float baseAmount, bool flat = true, bool add = true)
    {
        ItemData baseData = XmlLibrary.ItemDescs[ObjectType]; // Cache for original values
        var
            fieldInfo = GetDeepField(statStr, this,
                baseData); // Find deepest field, data and original data (xml values)

        var field = fieldInfo.Item1;
        var obj = fieldInfo.Item2;
        baseData = fieldInfo.Item3;

        var amount = add ? baseAmount : -baseAmount;
        var prevValue = obj.GetValue(field);
        var baseValue = baseData.GetValue(field); // Only used for % boosts
        if (baseValue == null)
        {
            Logger.Error($"Null itemdata ({obj.GetType()}) field: {field}");
            return;
        }

        // Update deepest field
        if (!flat) // Percentage boost
            AddProportionalValue(obj, field, baseValue, prevValue, amount);
        else
            AddFlatValue(obj, field, prevValue, amount);
    }

    private void AddFieldBoost(FieldBoostType fieldBoostType, string fieldStr, bool flat, float amount)
    {
        if (!_fieldBoosts.TryGetValue(fieldBoostType, out var fieldBoosts)) // Create new boost type
            fieldBoosts = _fieldBoosts[fieldBoostType] = new Dictionary<string, ItemFieldBoost>();

        if (!fieldBoosts.TryGetValue(fieldStr, out var boost))
        {
            fieldBoosts[fieldStr] = new ItemFieldBoost(flat, amount);
            return;
        }

        boost.Add(flat, amount);
    }

    private void RemoveFieldBoost(FieldBoostType fieldBoostType, string fieldStr, bool flat, float amount)
    {
        if (!_fieldBoosts.TryGetValue(fieldBoostType, out var fieldBoosts)) // Boost doesn't exist
            return;

        if (!fieldBoosts.TryGetValue(fieldStr, out var boost))
            return;

        boost.Add(flat, -amount); // Substraction
    }

    private void ResetFieldBoosts(FieldBoostType fieldBoostType = FieldBoostType.All) // Wipe all boosts of the same type
    {
        if (fieldBoostType == FieldBoostType.All)
            foreach (var boost in _fieldBoosts.Values)
            {
                RemoveFieldBoosts(boost);
                boost.Clear();
            }

        if (!_fieldBoosts.TryGetValue(fieldBoostType, out var boosts))
            return;

        RemoveFieldBoosts(boosts);
        boosts.Clear();
    }

    private void RemoveFieldBoosts(Dictionary<string, ItemFieldBoost> boosts)
    {
        foreach (var kvp in boosts)
        {
            var fieldStr = kvp.Key;
            var boost = kvp.Value;
            if (boost.FlatAmount != 0)
                UpdateDeepFields(fieldStr, boost.FlatAmount, true, false);
            if (boost.ProportionalAmount != 0)
                UpdateDeepFields(fieldStr, boost.ProportionalAmount, false, false);
        }
    }

    private void ApplyFieldBoosts() // This should only be called after boosts have been resetted
    {
        foreach (var fieldBoostType in _fieldBoostUpdates)
            if (_fieldBoosts.TryGetValue(fieldBoostType, out var fieldBoosts))
                foreach (var kvp in fieldBoosts)
                {
                    var fieldStr = kvp.Key;
                    var boost = kvp.Value;
                    if (boost.FlatAmount != 0)
                        UpdateDeepFields(fieldStr, boost.FlatAmount);
                    if (boost.ProportionalAmount != 0)
                        UpdateDeepFields(fieldStr, boost.ProportionalAmount, false);
                }
    }

    // This method is for adding to numeric values where you don't know if it's a byte, int, or float
    private static void AddFlatValue(ItemData obj, byte field, object value, float amount)
    {
        if (value is int intValue)
            obj.SetValue(field, (int)(intValue + amount), true);
        else if (value is byte byteValue)
            obj.SetValue(field, (byte)(byteValue + amount), true);
        else if (value is float floatValue)
            obj.SetValue(field, floatValue + amount, true);
        else
            throw new Exception($"INVALID NON-NUMERIC TYPE: Field:{field} Type:{obj.GetType()}");
    }

    private static void AddProportionalValue(ItemData obj, byte field, object origValue, object prevValue,
        float amount)
    {
        var floatAmount = amount / 100f;
        if (origValue is int intValue) // Gotta do each calculation for int and float separately... and byte...
            obj.SetValue(field, (int)prevValue + (int)(intValue * floatAmount), true);
        else if (origValue is float floatValue)
            obj.SetValue(field, (float)prevValue + (floatValue * floatAmount), true);
        else if (origValue is byte byteValue)
            obj.SetValue(field, (byte)prevValue + (byte)(byteValue * floatAmount), true);
        else
            throw new Exception($"INVALID NON-NUMERIC TYPE: Field:{field} Type:{obj.GetType()}");
    }

    private static (byte, ItemData, ItemData) GetDeepField(string statStr, ItemData data, ItemData origData) // Tuple: (field, data, original data)
    {
        byte field;
        var fields = statStr.Split("::");
        if (fields.Length > 1) // If we want to edit properties of an item data property, we do so with :: (e.g. Projectile::MaxDamage)
        {
            for (var i = 0; i < fields.Length - 1; i++) // Minus one to not assume the right part of the :: as an ItemData
            {
                var rawField = fields[i];
                var indexIndicator = rawField.IndexOf("|"); // Arrays will always end in s
                var elementIndex = indexIndicator == -1 ? -1 : int.Parse(rawField.Substring(indexIndicator + 1));
                var cleanFieldName = indexIndicator == -1 ? rawField : rawField.Substring(0, indexIndicator); // Remove everything after the element index indicator

                field = (byte)Enum.Parse(data.FieldsEnum, cleanFieldName); // This is the field of the itemdata property
                if (elementIndex != -1) // The data object is inside an array
                {
                    data = data.GetValue<ItemData[]>(field)[elementIndex];
                    origData = origData.GetValue<ItemData[]>(field)[elementIndex];
                }
                else
                {
                    data = data.GetValue<ItemData>(field);
                    origData = origData.GetValue<ItemData>(field);
                }
            }

            field = (byte)Enum.Parse(data.FieldsEnum,
                fields[fields.Length - 1]); // This is the field we're modifying
        }
        else
            field = (byte)Enum.Parse<ItemField>(statStr);

        return (field, data, origData);
    }

    // Here we're accessing the field (e.g. Stat) from an itemdata child (e.g. EquipmentBoost) and we're trying to get the full field str: (e.g. EquipmentBoost::Stat)
    private static string GetFullFieldStr(string statStr, ItemData data)
    {
        var parents = GetParents(data).Reverse(); // Reverse to get the deepest parent

        var ret = "";
        foreach (var parent in parents)
            ret += parent + "::";
        ret += statStr;

        return ret;
    }

    private static IEnumerable<string> GetParents(ItemData data) // THIS WORKS DON'T TOUCH IT
    {
        if (data.Parent == null)
        {
            yield return data.GetType().Name;
            yield break;
        }

        var val = data.Parent.GetValue(data.ParentField);
        var name = data.GetType().Name;
        if (val is ItemData[] arr)
            name += "s|" + arr.IndexOf(data); // Append the index of the element we're modifying

        yield return name;

        var curData = data;
        var prevData = data;
        while (curData.Parent != null)
        {
            curData = curData.Parent;
            val = curData.GetValue(prevData.ParentField);
            name = curData.GetType().Name;
            if (val is ItemData[] arrr)
                name += "s|" + arrr.IndexOf(prevData);

            if (curData.Parent != null)
                yield return name;
            prevData = curData;
        }
    }
}
