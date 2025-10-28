#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Inventory;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.INVSWAP)]
    public class InvSwap : IIncomingPacket
    {
        public SlotObjectData SlotObject1;
        public SlotObjectData SlotObject2;

        public void Read(NetworkReader rdr)
        {
            SlotObject1 = SlotObjectData.Read(rdr);
            SlotObject2 = SlotObjectData.Read(rdr);
        }

        public void Handle(User user)
        {
            // we need to be the owner of at least one of the inventories we are swapping. 
            //if (SlotObject1.ObjectId != user.GameInfo.Player.Id && SlotObject2.ObjectId != user.GameInfo.Player.Id)
            //    return;
            // Zolmex: or not

            bool success;
            if (SlotObject1.ObjectId == SlotObject2.ObjectId) // Same inventory swap
            {
                if (SlotObject1.ObjectId == user.GameInfo.Player.Id) // Player inv swap
                    success = DoPlayerInvSwap(user);
                else // Container inv swap
                    success = DoContainerInvSwap(user);
            }
            else
                success = DoPlayerContainerInvSwap(user);

            InvResult.Write(user.Network, success ? 0 : 1);
        }

        public bool DoPlayerInvSwap(User user)
        {
            // inv swap validation rules for player inv swap:
            // two inventories can be a player inventory, if they are the same object id
            // slot ids must be different
            // if we are swapping to hotbar, item must be equippable in that slot

            if (!user.GameInfo.World.Entities.TryGetValue(SlotObject1.ObjectId, out var en) || en is not Player player)
                return false;

            if (SlotObject1.SlotId == SlotObject2.SlotId)
                return false;

            var item1 = player.Inventory.GetItem(SlotObject1.SlotId); // If the slot is 255 or 254 (hp or mp pots) it will return the potion item
            var item2 = player.Inventory.GetItem(SlotObject2.SlotId);
            if (!PlayerInventory.IsEquippable(player, item1, SlotObject2.SlotId) ||
                !PlayerInventory.IsEquippable(player, item2, SlotObject1.SlotId))
                return false;

            if (item1 is { ObjectType: 2594 or 2595 }) // Handle potion stacking
                return player.Inventory.HandlePotionSwap(item1, item2, SlotObject1.SlotId, SlotObject2.SlotId);

            player.Inventory.SwapSlots(SlotObject1.SlotId, SlotObject2.SlotId);
            return true;
        }

        public bool DoPlayerContainerInvSwap(User user)
        {
            // inv swap validation rules for player-container inv swap
            // one entity must be a player
            // one entity must be a container
            // the player must be the owner of this container, or the container must be public
            // player must be within 3 tiles of the container
            // if we are swapping to hotbar, item must be equippable in that slot

            var world = user.GameInfo.World;
            if (!world.Entities.TryGetValue(SlotObject1.ObjectId, out var ent1))
                return false;

            if (!world.Entities.TryGetValue(SlotObject2.ObjectId, out var ent2))
                return false;

            var player = GetValidPlayer(ent1, ent2);
            if (player == null) return false;

            var container = GetValidContainer(ent1, ent2);
            if (container == null) return false;
            if (!container.IsPublic() && container.OwnerId != player.AccountId) return false;
            if (player.DistSqr(container) > 3f * 3f) return false;

            var playerIsEnt1 = player.Id == SlotObject1.ObjectId;
            if (container is OneWayContainer && playerIsEnt1) return false;

            var playerSlot = playerIsEnt1 ? SlotObject1.SlotId : SlotObject2.SlotId;
            var containerSlot = playerIsEnt1 ? SlotObject2.SlotId : SlotObject1.SlotId;
            var playerItem = player.Inventory.GetItem(playerSlot);
            var containerItem = container.Inventory.GetItem(containerSlot);

            if (!PlayerInventory.IsEquippable(player, playerItem, playerSlot))
                return false;

            if (playerSlot is 254 or 255 || containerItem is { ObjectType: 2594 or 2595 })
            { // Handle potion stacking
                if (!playerIsEnt1)
                {
                    if (!player.Inventory.AddToPotionStack(containerItem.ObjectType)) // If we can't add to the stack, cancel swap operation
                        return false;
                    container.Inventory.SetItemNoLock(null, containerSlot); // Clear potion from the container
                }
                else if (!player.Inventory.RemovePotionStack(playerItem.ObjectType))
                    return false;
            }

            EntityInventory.InventorySwap(player.Inventory, container.Inventory, playerSlot, containerSlot);
            if (container.Inventory.IsEmpty() && container is not VaultChest)
                container.TryLeaveWorld();

            return true;
        }

        public bool
            DoContainerInvSwap(
                User user) // Player can only perform a swap in a container if the container is public or owned by the player
        {
            var world = user.GameInfo.World;
            if (!world.Entities.TryGetValue(SlotObject1.ObjectId, out var en) || en is not Container container)
                return false;

            var player = user.GameInfo.Player;
            if (!container.IsPublic() && container.OwnerId != player.AccountId)
                return false;

            if (player.DistSqr(container) > 3f * 3f)
                return false;

            EntityInventory.InventorySwap(container.Inventory, SlotObject1.SlotId, SlotObject2.SlotId);
            if (container.Inventory.IsEmpty() && container is not VaultChest)
                container.TryLeaveWorld();

            return true;
        }

        public Player GetValidPlayer(Entity ent1, Entity ent2)
        {
            var p = ent1 as Player;
            var p2 = ent2 as Player;
            if (p != null && p2 != null)
                return null;
            else if (p == null && p2 == null)
                return null;

            return p ?? p2;
        }

        public Container GetValidContainer(Entity ent1, Entity ent2)
        {
            var c = ent1 as Container;
            var c2 = ent2 as Container;
            if (c != null && c2 != null)
                return null;
            else if (c == null && c2 == null)
                return null;

            return c ?? c2;
        }

        //public EntityInventory GetInventoryById(World world, int objId)
        //{
        //    Entity targetEntity = world.GetEntityById(objId);
        //    if (targetEntity == null)
        //        return null;

        //    if (targetEntity is Player p)
        //        return p.Inventory;
        //    else if (targetEntity is Container c)
        //        return c.Inventory;

        //    return null;
        //}

        public override string ToString()
        {
            var type = typeof(InvSwap);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }

            return ret;
        }
    }
}