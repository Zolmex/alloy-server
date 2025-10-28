#region

using Common;
using Common.FeatureData;
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

        public int PrimaryConstellation { get => Stats.Get<int>(StatType.PrimaryConstellation); set => Stats.Set(StatType.PrimaryConstellation, value, true); }
        public int SecondaryConstellation { get => Stats.Get<int>(StatType.SecondaryConstellation); set => Stats.Set(StatType.SecondaryConstellation, value, true); }
        public int PrimaryNodeData { get => Stats.Get<int>(StatType.PrimaryNodeData); set => Stats.Set(StatType.PrimaryNodeData, value, true); }
        public int SecondaryNodeData { get => Stats.Get<int>(StatType.SecondaryNodeData); set => Stats.Set(StatType.SecondaryNodeData, value, true); }

        public int[] PrimaryNodes;
        public int[] SecondaryNodes;

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

        public void ReloadConstellationMods()
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

            _addModsFromConstellation(ref mods, PrimaryConstellation, PrimaryNodes);
            _addModsFromConstellation(ref mods, SecondaryConstellation, SecondaryNodes);
            _addModsFromInventory(ref mods);

            //skill tree

            return mods;
        }

        private void _addModsFromConstellation(ref List<ModTypes> mods, int constellation, int[] nodes)
        {
            if (constellation == -1)
                return;

            for (var i = 0; i < 4; i++)
            {
                var node = ConstellationsData.Instance.Data.NodeList.FirstOrDefault(n =>
                    n.Constellation == constellation &&
                    n.Large == (i == 0) &&
                    n.Row == (i == 0 ? 0 : i - 1) &&
                    n.Id == nodes[i]);

                if (node != null)
                    mods.Add(ConstellationNodeNameToModType(node.Name));
            }
        }

        private void _addModsFromInventory(ref List<ModTypes> mods)
        {
            foreach (var item in Inventory.GetItems().Where(i => i != null))
                mods.Add(ItemNameToModType(item.DisplayName));
        }

        public static ModTypes ConstellationNodeNameToModType(string name)
        {
            return name switch
            {
                "Nature's Vengeance" => ModTypes.NaturesVengeance,
                "Steady Assault" => ModTypes.SteadyAssault,
                "Masterpiece" => ModTypes.Masterpiece,
                "Diverging Destiny" => ModTypes.DivergingDestiny,
                "Rising Fury" => ModTypes.RisingFury,
                "Beat-em-up" => ModTypes.BeatEmUp,
                "Holy Excalibur" => ModTypes.HolyExcalibur,
                "Lifeblood" => ModTypes.Lifeblood,
                "Delightful Harvest" => ModTypes.DelightfulHarvest,
                "Inner Machinations" => ModTypes.InnerMachinations,
                "Blood Reaper" => ModTypes.BloodReaper,
                "Soul Reaper" => ModTypes.SoulReaper,
                "Mind Reaper" => ModTypes.MindReaper,
                "Primal Strike" => ModTypes.PrimalStrike,
                _ => ModTypes.None
            };
        }

        public static ModTypes ItemNameToModType(string name)
        {
            return name switch
            {
                _ => ModTypes.None
            };
        }

        /*public static ModTypes GetConstellationModFromIndex(int index)
        {
            return index switch
            {
                0 => ModTypes.NaturesVengeance,
                1 => ModTypes.SteadyAssault,
                2 => ModTypes.Masterpiece,
                _ => 0
            };
        }

        public int GetNodeIndex(ConstellationNode node)
        {
            int index = node.Constellation * 13;

            if (!node.Large)
            {
                index += 4;
                index += node.Row * 3;
            }
            index += node.Id - 1;

            return index;
        }*/
    }
}