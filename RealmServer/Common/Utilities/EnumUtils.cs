#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace Common.Utilities
{
    public static class EnumUtils
    {
        private static Dictionary<Enum, string> _descriptors = new();

        public static void Load()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (type.IsEnum) // Load enum descriptors
                {
                    var enums = type.GetEnumValues();
                    if (enums != null)
                    {
                        foreach (Enum value in enums)
                        {
                            var name = Enum.GetName(type, value);
                            var field = type.GetField(name);
                            if (field != null)
                            {
                                var attr =
                                    Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                                if (attr != null)
                                {
                                    _descriptors.Add(value, attr.Description);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string GetDescription(this Enum value)
        {
            if (!_descriptors.TryGetValue(value, out var ret))
            {
                throw new KeyNotFoundException($"Enumerable '{value}' does not have a descriptor.");
            }

            return ret;
        }

        public static T[] CommaToArray<T>(this string x, string delim = ",")
        {
            if (string.IsNullOrWhiteSpace(x))
                return new T[0];

            if (typeof(T) == typeof(string))
                return x.Split(delim).Select(_ => (T)(object)_.Trim()).ToArray();
            else
                return x.Split(delim).Select(_ => (T)(object)(T)Utils.FromPrefix<T>(_.Trim())).ToArray();
        }

        public static string ToCommaSepString<T>(this IEnumerable<T> src, string delim = ", ", bool brackets = false)
        {
            if (src == null)
                return "";

            var ret = new StringBuilder();
            if (brackets)
                ret.Append("[");
            for (var i = 0; i < src.Count(); i++)
            {
                if (i != 0) ret.Append(delim);
                ret.Append(src.ElementAt(i)?.ToString() ?? null);
            }

            if (brackets)
                ret.Append("]");
            return ret.ToString();
        }

        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            return source.Count() <= 0
                ? default
                : source switch
                {
                    List<T> list => list[Random.Shared.Next(0, list.Count)],
                    T[] arr => arr[Random.Shared.Next(0, arr.Length)],
                    _ => source.ElementAt(Random.Shared.Next(0, source.Count()))
                };
        }

        public static int IndexOf<T>(this T[] array, T element)
        {
            return Array.IndexOf(array, element);
        }

        // Thread-safe .ToArray() method
        //public static T[] ToArray<T>(this IEnumerable<T> src, object lockObj)
        //{
        //    using (TimedLock.Lock(lockObj)
        //        return src.ToArray();
        //}

        public static void Clear(this Array arr)
        {
            Array.Clear(arr, 0, arr.Length);
        }

        //technically an enum util :grin
        public static StatType TextToStatType(string txt)
        {
            return TextToStatTypeMap.TryGetValue(txt, out var statType) ? statType : StatType.None;
        }

        private static readonly Dictionary<string, StatType> TextToStatTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["hp"] = StatType.MaxHP,
            ["maxhp"] = StatType.MaxHP,
            ["health"] = StatType.MaxHP,
            ["mp"] = StatType.MaxMP,
            ["maxmp"] = StatType.MaxMP,
            ["mana"] = StatType.MaxMP,
            ["ms"] = StatType.MaxMS,
            ["maxms"] = StatType.MaxMS,
            ["magicshield"] = StatType.MaxMS,
            ["shield"] = StatType.MaxMS,
            ["att"] = StatType.Attack,
            ["atk"] = StatType.Attack,
            ["attack"] = StatType.Attack,
            ["def"] = StatType.Defense,
            ["defense"] = StatType.Defense,
            ["dex"] = StatType.Dexterity,
            ["dexterity"] = StatType.Dexterity,
            ["wis"] = StatType.Wisdom,
            ["wisdom"] = StatType.Wisdom,
            ["spd"] = StatType.MovementSpeed,
            ["speed"] = StatType.MovementSpeed,
            ["movementspeed"] = StatType.MovementSpeed,
            ["liferegen"] = StatType.LifeRegeneration,
            ["liferegeneration"] = StatType.LifeRegeneration,
            ["hpregen"] = StatType.LifeRegeneration,
            ["hpregeneration"] = StatType.LifeRegeneration,
            ["dodge"] = StatType.DodgeChance,
            ["dodgechance"] = StatType.DodgeChance,
            ["crit"] = StatType.CriticalChance,
            ["critchance"] = StatType.CriticalChance,
            ["criticalchance"] = StatType.CriticalChance,
            ["critdmg"] = StatType.CriticalDamage,
            ["critdamage"] = StatType.CriticalDamage,
            ["crititcaldamage"] = StatType.CriticalDamage,
            ["crititcaldmg"] = StatType.CriticalDamage,
            ["manaregen"] = StatType.ManaRegeneration,
            ["mpregen"] = StatType.ManaRegeneration,
            ["manaregeneration"] = StatType.ManaRegeneration,
            ["mpregeneration"] = StatType.ManaRegeneration,
            ["msregen"] = StatType.MSRegenRate,
            ["msregenrate"] = StatType.MSRegenRate,
            ["magicshieldregen"] = StatType.MSRegenRate,
            ["magicshieldregenrate"] = StatType.MSRegenRate,
            ["dmgmult"] = StatType.DamageMultiplier,
            ["dmgmultiplier"] = StatType.DamageMultiplier,
            ["damagemultiplier"] = StatType.DamageMultiplier,
            ["armor"] = StatType.Armor,
            ["attspd"] = StatType.AttackSpeed,
            ["attackspd"] = StatType.AttackSpeed,
            ["attackspeed"] = StatType.AttackSpeed,
            ["attspeed"] = StatType.AttackSpeed,
            ["atkspd"] = StatType.AttackSpeed
        };
    }
}