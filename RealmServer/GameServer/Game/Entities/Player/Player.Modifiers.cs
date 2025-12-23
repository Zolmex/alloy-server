#region

using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using GameServer.Game.DamageSources;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Entities.Stacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        public const int InCombatDuration = 6000;
        public bool IsInCombat = false;
        public int TimeInCombat { get => Stats.Get<int>(StatType.TimeInCombat); set => Stats.Set(StatType.TimeInCombat, value, true); }

        public void InCombatTick()
        {
            TimeInCombat += 1000 / GameServerConfig.Config.TPS;
            if (TimeInCombat % 1000 == 0)
            {
                if (TimeSinceLastEnemyHit > InCombatDuration && TimeSinceLastHit > InCombatDuration)
                {
                    IsInCombat = false;
                    TimeInCombat = 0;
                }

                InCombat?.Invoke();
            }
        }

        public void ReloadMods()
        {
            RemoveActiveModifiers();
            AddActiveModifiers();
        }

        public void AddActiveModifiers()
        {
            TickStatChanges = true;
            foreach (var type in GetActivatedMods())
            {
                var mod = ModifierRegistry.Get(type);
                if (mod == null)
                    continue;

                mod.Initialize(this);
                mod.SubscribeEvents();
                ActiveModifiers.TryAdd(type, mod);
            }

            ApplyModifiers();
        }

        public void RemoveActiveModifiers()
        {
            TickStatChanges = false;
            DisposeMods();
            ResetBonuses();
        }

        private void DisposeMods()
        {
            foreach (var mod in ActiveModifiers.Values)
                mod.Remove();
            ActiveModifiers.Clear();
        }

        public void ApplyModifiers()
        {
            foreach (var modifier in ActiveModifiers.Values)
                modifier.Apply();
        }

        public List<ModTypes> GetActivatedMods()
        {
            var mods = new List<ModTypes> { ModTypes.EquipmentBonuses };

            AddModsFromInventory(ref mods);

            //skill tree

            return mods;
        }

        private void AddModsFromInventory(ref List<ModTypes> mods)
        {
            foreach (var item in Inventory.GetItems().Where(i => i != null))
                mods.Add(ItemNameToModType(item.DisplayName));
        }

        public static ModTypes ItemNameToModType(string name)
        {
            return name switch
            {
                _ => ModTypes.None
            };
        }
    }
}