#region

using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        // This file is used to synchronize player's state with the server and client. Also used for anticheat 

        public int TimeSinceLastEnemyHit;
        public int TimeSinceLastHit;
        public long LastMoveAck;
        public long LastShootAck;
        public int LastClientShootTime;

        private readonly ConcurrentDictionary<int, List<ServerShootInfo>> _serverShootAcks = new();

        public bool ValidateAttackSpeed(Item weapon, int clientTime)
        {
            if (LastShootAck == 0) // First time shooting
                return true;

            var attackPeriod = 1 / (GetAttackFrequency() * weapon.RateOfFire) * 1000;
            var serverDelta = RealmManager.RealTime.ElapsedMilliseconds - LastShootAck;
            var clientDelta = clientTime - LastClientShootTime;

            // Validate client time
            var diff = Math.Abs(serverDelta - clientDelta);
            
            // Note: Temporarily disabled this, is there a point for it?
            // if (diff > 300) // Over 300 ms is a lot of lag, doesn't hurt to be safe here
            //     return false;

            var delta = Math.Max(serverDelta, clientDelta);
            return delta >= attackPeriod + diff - 5;
        }

        public bool ValidateMove(float posX, float posY)
        {
            if (Teleporting)
            { // Player will be moved on GotoAck
                return false;
            }

            var moveThresh =
                4 + (this.GetSpeed(MovementSpeed) *
                     (TimeSinceLastMove() / 1000f)); // 4 tiles to account for any lag or speed modifiers
            moveThresh *= moveThresh; // Squared
            var dist = this.DistSqr(posX, posY);
            // Console.WriteLine($"Move: dist:{dist} thresh:{moveThresh}");
            return dist <= moveThresh;
        }

        private void AwaitServerShoot(int itemType, Vector2 pos, float angle, float angleInc, int[] damageList,
            float[] critList)
        {
            if (!_serverShootAcks.TryGetValue(itemType, out var list))
            {
                list = new List<ServerShootInfo>();
                _serverShootAcks.TryAdd(itemType, list);
            }

            using (TimedLock.Lock(list))
                list.Add(new ServerShootInfo(itemType, pos, angle, angleInc, damageList, critList));
        }

        public bool ValidateServerShoot(int itemType, WorldPosData pos, float angle, float angleInc, int[] damageList,
            float[] critList)
        {
            var vecPos = new Vector2(pos.X, pos.Y);
            if (!_serverShootAcks.TryGetValue(itemType, out var list))
                return false;

            using (TimedLock.Lock(list))
                return list.Remove(new ServerShootInfo(itemType, vecPos, angle, angleInc, damageList, critList));
        }

        public long TimeSinceLastMove()
        {
            return RealmManager.RealTime.ElapsedMilliseconds - LastMoveAck;
        }

        public void FailedShoot(int projCount)
        {
            using (TimedLock.Lock(_projIdLock))
                _nextProjectileId += (ushort)projCount;
        }
    }

    public record struct ServerShootInfo(
        int ItemType,
        Vector2 Pos,
        float Angle,
        float AngleInc,
        int[] DamageList,
        float[] CritList)
    {
        public bool Equals(ServerShootInfo other)
        {
            return ItemType.Equals(other.ItemType) &&
                   Pos.Equals(other.Pos) &&
                   Angle.Equals(other.Angle) &&
                   AngleInc.Equals(other.AngleInc) &&
                   DamageList.SequenceEqual(other.DamageList) &&
                   CritList.SequenceEqual(other.CritList);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ItemType, Pos, Angle, AngleInc, DamageList, CritList);
        }
    }
}