#region

using Common;
using System;
using System.Collections.Generic;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities
{
    public static class EntityUtils
    {
        private const float MinAttackMult = 0.5f;
        private const float MaxAttackMult = 2f;
        private const float MinAttackFreq = 0.0015f;
        private const float MaxAttackFreq = 0.008f;

        public static float DistSqr(this Entity a, float x, float y)
        {
            return DistSqr(a.Position.X, a.Position.Y, x, y);
        }

        public static float DistSqr(WorldPosData pos1, WorldPosData pos2)
        {
            return DistSqr(pos1.X, pos1.Y, pos2.X, pos2.Y);
        }

        public static float DistSqr(Vector2 a, Vector2 b)
        {
            return DistSqr(a.X, a.Y, b.X, b.Y);
        }

        public static float DistSqr(float x1, float y1, float x2, float y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            return (dx * dx) + (dy * dy);
        }

        public static float DistSqr(this Entity a, Entity b)
        {
            var dx = a.Position.X - b.Position.X;
            var dy = a.Position.Y - b.Position.Y;
            return (dx * dx) + (dy * dy);
        }

        public static double TileDistSqr(this Entity a, int tileX, int tileY)
        {
            var dx = (int)a.Position.X - tileX;
            var dy = (int)a.Position.Y - tileY;
            return (dx * dx) + (dy * dy);
        }

        public static double TileDistSqr(this Entity a, Entity b)
        {
            var dx = (int)a.Position.X - (int)b.Position.X;
            var dy = (int)a.Position.Y - (int)b.Position.Y;
            return (dx * dx) + (dy * dy);
        }

        public static float GetAngleBetween(this Entity a, Vector2 pos)
        {
            return a.GetAngleBetween(pos.X, pos.Y);
        }

        public static float GetAngleBetween(this Entity a, Entity b)
        {
            return a.GetAngleBetween(b.Position.X, b.Position.Y);
        }

        public static float GetAngleBetween(this Entity a, float x, float y)
        {
            return MathF.Atan2(y - a.Position.Y, x - a.Position.X);
        }

        public static float GetAngleBetween(this Vector2 pos, Vector2 pos2)
        {
            return MathF.Atan2(pos2.Y - pos.Y, pos2.X - pos.X);
        }

        public static Player GetNearestPlayer(this Entity entity, float radiusSqr, Predicate<Player> cond = null, float minRadiusSqr = 0)
        {
            var query = new SearchQuery("Player", new IntPoint((int)entity.Position.X, (int)entity.Position.Y), (int)MathF.Sqrt(radiusSqr), (int)MathF.Sqrt(minRadiusSqr));
            if (entity.World.SearchCache.TryGetValue(query, out var result))
                return result.NearestEntity as Player;

            Player ret = null;
            var center = entity.Tile?.Chunk;
            if (center == null)
                return ret;

            var players = new List<Player>();
            var minDist = radiusSqr;
            for (var cY = center.CY - Player.ACTIVE_RADIUS; cY <= center.CY + Player.ACTIVE_RADIUS; cY++)
                for (var cX = center.CX - Player.ACTIVE_RADIUS; cX <= center.CX + Player.ACTIVE_RADIUS; cX++)
                {
                    var chunk = entity.World.Map.Chunks[cX, cY];
                    if (chunk != null)
                        foreach (var kvp in chunk.Players)
                        {
                            var plr = kvp.Value;
                            var distSqr = plr.DistSqr(entity);
                            if (distSqr < radiusSqr && distSqr > minRadiusSqr)
                            {
                                players.Add(plr);

                                if (distSqr < minDist && (cond == null || cond(plr)))
                                {
                                    ret = plr;
                                    minDist = distSqr;
                                }
                            }
                        }
                }

            entity.World.SearchCache.Add(query, new SearchQueryResult(players, ret));
            return ret;
        }

        public static Player GetFarthestPlayer(this Entity entity, float radiusSqr, Predicate<Player> cond = null, float minRadiusSqr = 0)
        {
            Player ret = null;
            var center = entity.Tile?.Chunk;
            if (center == null) return ret;
            var maxDist = 0f;
            for (var cY = center.CY - Player.ACTIVE_RADIUS; cY <= center.CY + Player.ACTIVE_RADIUS; cY++)
                for (var cX = center.CX - Player.ACTIVE_RADIUS; cX <= center.CX + Player.ACTIVE_RADIUS; cX++)
                {
                    var chunk = entity.World.Map.Chunks[cX, cY];
                    if (chunk != null)
                        foreach (var kvp in chunk.Players)
                        {
                            var plr = kvp.Value;
                            var distSqr = plr.DistSqr(entity);
                            if (distSqr > maxDist && (cond == null || cond(plr)))
                            {
                                ret = plr;
                                maxDist = distSqr;
                            }
                        }
                }

            return ret;
        }

        public static bool IsPlayerWithin(this Entity entity, float radius)
        {
            return entity.World.IsPlayerWithin(entity.Position.X, entity.Position.Y, radius);
        }

        public static float GetSpeed(this Entity entity, float speed)
        {
            if (entity.IsPlayer)
            {
                var p = (Player)entity;
                if (p.HasConditionEffect(ConditionEffectIndex.Slowed))
                    return 1;

                if (p.HasConditionEffect(ConditionEffectIndex.Speedy))
                    speed *= 1.5f;

                var tileSpeedMult = entity.Tile.TileDesc.Speed; // Sink level is not supported so just use the tile speed
                return speed * tileSpeedMult;
            }

            if (entity is Character chr)
            {
                if (chr.HasConditionEffect(ConditionEffectIndex.Slowed))
                    return 1;

                if (chr.HasConditionEffect(ConditionEffectIndex.Speedy))
                    speed *= 1.5f;
                return speed;
            }

            return speed;
        }

        public static double GetDistanceBetween(this Entity entity, Entity entity2) //the diagonal distance
        {
            return GetDistanceBetween(entity.Position.X, entity2.Position.X, entity.Position.Y, entity.Position.Y);
        }

        public static double GetDistanceBetween(float x1, float x2, float y1, float y2) //the diagonal distance
        {
            double dx = Math.Abs(x1 - x2);
            double dy = Math.Abs(y1 - y2);
            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        public static float GetDistanceBetweenF(this Entity entity, Entity entity2) //the diagonal distance
        {
            return GetDistanceBetweenF(entity.Position.X, entity2.Position.X, entity.Position.Y, entity2.Position.Y);
        }

        public static float GetDistanceBetweenF(float x1, float x2, float y1, float y2) //the diagonal distance
        {
            var dx = Math.Abs(x1 - x2);
            var dy = Math.Abs(y1 - y2);
            return MathF.Sqrt((dx * dx) + (dy * dy));
        }
        //public static int GetDefenseDamage(this Character entity, int damage, int def, bool ignoreDef)
        //{
        //    if (entity.HasAnyConditionEffects(ConditionEffectIndex.Invulnerable, ConditionEffectIndex.Invincible))
        //        return 0;

        //    if (entity.HasConditionEffect(ConditionEffectIndex.ArmorBroken) || ignoreDef)
        //        return damage;

        //    // curse check

        //    if (entity.HasConditionEffect(ConditionEffectIndex.Armored))
        //        def *= 2;

        //    var min = (int)(damage * 0.15);
        //    return Math.Max(damage - def, min);
        //}

        //public static int GetAttackDamage(this Character entity, int damage)
        //{
        //    if (entity.HasConditionEffect(ConditionEffectIndex.Weak))
        //        return (int)(damage * MinAttackMult);

        //    if (entity.HasConditionEffect(ConditionEffectIndex.Damaging))
        //        damage = (int)(damage * 1.5);

        //    return damage;
        //}

        //public static int GetAttackDamage(this Player player, int min, int max, bool isAbility)
        //{
        //    var damage = (int)player.User.Random.NextIntRange((uint)min, (uint)max);
        //    damage = GetAttackDamage(player, damage);
        //    var mult = isAbility ? 1 : MinAttackMult + player.Attack / 75.0 * (MaxAttackMult - MinAttackMult);
        //    return (int)(damage * mult);
        //}

        //public static float GetAttackFrequency(this Player player)
        //{
        //    if (player.HasConditionEffect(ConditionEffectIndex.Dazed))
        //        return MinAttackFreq;

        //    var ret = MinAttackFreq + player.Dexterity / 75 * (MaxAttackFreq - MinAttackFreq);
        //    if (player.HasConditionEffect(ConditionEffectIndex.Berserk))
        //        ret *= 1.5f;

        //    return ret;
        //}

        //public static int GetAttackPeriod(this Player player, float rof)
        //    => (int)(1 / player.GetAttackFrequency() * (1 / rof));
    }
}