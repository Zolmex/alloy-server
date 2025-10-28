#region

using GameServer.Game.Entities.Inventory;

#endregion

namespace GameServer.Game.Entities
{
    public class Container : Entity
    {
        public int OwnerId = -1; // -1 = Public
        public readonly EntityInventory Inventory;
        public long EndTime;

        public bool IsPublic()
        {
            return OwnerId == -1;
        }

        public static int INVENTORY_SIZE = 8;

        public Container(ushort objType, int numSlots, int ownerId = -1, int TTL = 60000) : base(objType)
        {
            Inventory = new EntityInventory(this, numSlots);
            OwnerId = ownerId;
            EndTime = TTL == -1 ? -1 : RealmManager.WorldTime.TotalElapsedMs + TTL;
        }

        public bool IsVisibleTo(Player player)
        {
            if (IsPublic()) return true;

            return player.AccountId == OwnerId;
        }

        public void Tick(RealmTime time)
        {
            if (EndTime != -1 && time.TotalElapsedMs > EndTime)
                TryLeaveWorld();
        }
    }
}