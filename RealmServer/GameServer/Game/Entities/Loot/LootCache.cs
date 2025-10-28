#region

using System;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Loot
{
    public class LootCache
    {
        private List<double> _baseLootRolls = new();
        public List<double> LootRolls = new();

        public LootCache(int rollCount, double lootBoost)
        {
            for (var i = 0; i < rollCount; i++)
            {
                var roll = Random.Shared.NextDouble();
                _baseLootRolls.Add(roll);
                LootRolls.Add(roll * lootBoost);
            }
        }

        public void UpdateLootBoost(double boost)
        {
            LootRolls.Clear();
            for (var i = 0; i < _baseLootRolls.Count; i++)
            {
                LootRolls.Add(_baseLootRolls[i] * boost);
            }
        }
    }
}