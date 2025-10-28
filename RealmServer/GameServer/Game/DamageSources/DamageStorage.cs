#region

using GameServer.Game.Entities;
using System;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.DamageSources
{
    public class DamageStorage
    {
        private readonly Dictionary<Player, int> _playerToDamage = new();
        private readonly SortedSet<KeyValuePair<Player, int>> _sortedDamage = new(DamageComparer.Instance);

        // Cache for the most recently requested unfiltered top damage dealers
        private readonly List<KeyValuePair<Player, int>> _topDamageDealersCache = new();
        private int _cachedMaxCount = 0;
        private bool _isCacheDirty = true;

        /// <summary>
        /// Registers damage dealt by a player to this entity.
        /// </summary>
        /// <param name="player">Player dealing the damage.</param>
        /// <param name="damage">Amount of damage dealt.</param>
        /// <returns>True if the player is a new attacker.</returns>
        public bool RegisterDamage(Player player, int damage)
        {
            if (damage <= 0)
                return false;

            bool newAttacker = false;
            
            if (_playerToDamage.TryGetValue(player, out var currentDamage))
            {
                _sortedDamage.Remove(new KeyValuePair<Player, int>(player, currentDamage));
                _playerToDamage[player] = currentDamage + damage;
            }
            else
            {
                _playerToDamage[player] = damage;
                newAttacker = true;
            }
            
            _sortedDamage.Add(new KeyValuePair<Player, int>(player, currentDamage + damage));
            _isCacheDirty = true;
            
            return newAttacker;
        }

        /// <summary>
        /// Gets damage for a specific player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>Damage dealt by the player or 0 if the player was not found.</returns>
        public int GetDamageForPlayer(Player player)
        {
            return _playerToDamage.GetValueOrDefault(player, 0);
        }

        /// <summary>
        /// Gets the top N damage dealers.
        /// </summary>
        /// <param name="maxCount">Maximum amount of players to return.</param>
        /// <returns>List of top damage dealers.</returns>
        public List<KeyValuePair<Player, int>> GetTopDamageDealers(int maxCount)
        {
            if (maxCount <= 0)
            {
                _topDamageDealersCache.Clear();
                return _topDamageDealersCache;
            }
            
            if (!_isCacheDirty && _cachedMaxCount == maxCount)
                return _topDamageDealersCache;
            
            UpdateCache(maxCount);
            return _topDamageDealersCache;
        }
        
        /// <summary>
        /// Gets the top N damage dealers with filtering.
        /// </summary>
        /// <param name="maxCount">Maximum amount of players to return.</param>
        /// <param name="filter">Filter predicate to apply to players.</param>
        /// <param name="resultList">Optional list to populate with results (cleared first).</param>
        /// <returns>List of filter top damage dealers.</returns>
        public List<KeyValuePair<Player, int>> GetTopDamageDealers(int maxCount, Func<Player, bool> filter, List<KeyValuePair<Player, int>> resultList = null)
        {
            var result = resultList ?? new List<KeyValuePair<Player, int>>();
            result.Clear();
            
            if (maxCount <= 0)
                return result;

            int added = 0;
            foreach (var kvp in _sortedDamage)
            {
                if (!filter(kvp.Key)) 
                    continue;
                
                result.Add(kvp);
                added++;
                if (added >= maxCount)
                    break;
            }
            
            return result;
        }

        /// <summary>
        /// Updates the internal cache of top damage dealers
        /// </summary>
        private void UpdateCache(int maxCount)
        {
            _topDamageDealersCache.Clear();
            _cachedMaxCount = maxCount;
            
            int added = 0;
            foreach (var kvp in _sortedDamage)
            {
                _topDamageDealersCache.Add(kvp);
                added++;
                if (added >= maxCount)
                    break;
            }
            
            _isCacheDirty = false;
        }

        public IEnumerable<Player> GetAttackers()
        {
            return _playerToDamage.Keys;
        }

        public void Clear()
        {
            _sortedDamage.Clear();
            _playerToDamage.Clear();
        }

        private class DamageComparer : IComparer<KeyValuePair<Player, int>>
        {
            public static readonly DamageComparer Instance = new();

            public int Compare(KeyValuePair<Player, int> x, KeyValuePair<Player, int> y)
                => y.Value.CompareTo(x.Value);
        }
    }
}