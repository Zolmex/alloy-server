using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities;

namespace GameServer.Game.Entities.Systems;

public readonly record struct SwapCommand(User User, SlotObjectData SlotA, SlotObjectData SlotB);

public class EntityInventoryManager(World world, int capacity) : ManagerBase<EntityInventory>(world, capacity) {

    private readonly ConcurrentQueue<SwapCommand> _swapCommands = [];
    
    public void EnqueueSwap(User user, SlotObjectData slotA, SlotObjectData slotB) {
        _swapCommands.Enqueue(new SwapCommand(user, slotA, slotB));
    }

    private void ExecuteSwap(ref SwapCommand cmd) {
        bool success;
        if (cmd.SlotA.ObjectId == cmd.SlotB.ObjectId) // Same inventory swap
        {
            if (cmd.SlotA.ObjectId == cmd.User.GameInfo.Player.Id) // Player inv swap
                success = DoPlayerInvSwap(ref cmd);
            else // Container inv swap
                success = DoContainerInvSwap(ref cmd);
        }
        else {
            success = DoPlayerContainerInvSwap(ref cmd);
        }

        cmd.User.SendPacket(new InvResult(success ? 0 : 1));
    }
    
    public bool DoPlayerInvSwap(ref SwapCommand cmd) {
        // inv swap validation rules for player inv swap:
        // two inventories can be a player inventory, if they are the same object id
        // slot ids must be different
        // if we are swapping to hotbar, item must be equippable in that slot

        if (cmd.SlotA.SlotId == cmd.SlotB.SlotId)
            return false;

        ref var playerInv = ref _world.EntityInventories.Get(cmd.SlotA.ObjectId);
        if (playerInv.Id == 0)
            return false;

        if (cmd.SlotA.SlotId is 255 or 254) { // Handle potion un-stacking
            if (playerInv[cmd.SlotB.SlotId] != null)
                return false;
            
            var item = playerInv.UnstackPotion(cmd.SlotA.SlotId);
            var ret = item != null;
            if (ret)
                playerInv.SetItem(cmd.SlotB.SlotId, item);
            return ret;
        }
        
        if (cmd.SlotB.SlotId is 255 or 254) { // Handle potion stacking
            var item = playerInv[cmd.SlotA.SlotId];
            var ret = playerInv.StackPotion(item);
            if (ret)
                playerInv.SetItem(cmd.SlotA.SlotId, null);
            return ret;
        }
        
        var item1 = playerInv[cmd.SlotA.SlotId];
        var item2 = playerInv[cmd.SlotB.SlotId];
        if (!playerInv.IsEquippable(item1, cmd.SlotB.SlotId) ||
            !playerInv.IsEquippable(item2, cmd.SlotA.SlotId))
            return false;

        playerInv.SwapSlots(cmd.SlotA.SlotId, cmd.SlotB.SlotId);
        return true;
    }

    public bool DoPlayerContainerInvSwap(ref SwapCommand cmd) {
        // inv swap validation rules for player-container inv swap
        // one entity must be a player
        // one entity must be a container
        // the player must be the owner of this container, or the container must be public
        // player must be within 3 tiles of the container
        // if we are swapping to hotbar, item must be equippable in that slot

        ref var ent1 = ref _world.EntityInventories.Get(cmd.SlotA.ObjectId);
        if (ent1.Id == 0)
            return false;

        ref var ent2 = ref _world.EntityInventories.Get(cmd.SlotB.ObjectId);
        if (ent2.Id == 0)
            return false;

        ref var en1 = ref _world.Entities.Get(ent1.Id);
        ref var en2 = ref _world.Entities.Get(ent2.Id);
        
        ref var playerStats = ref _world.EntityStats.Get(ent1.Id);
        ref var containerStats = ref _world.EntityStats.Get(ent2.Id);
        ref var playerInv = ref ent1;
        ref var containerInv = ref ent2;
        
        if (en1.Type != EntityType.Player) {
            if (en2.Type == EntityType.Player) {
                playerStats = ref _world.EntityStats.Get(ent2.Id);
                containerStats = ref _world.EntityStats.Get(ent1.Id);
                playerInv = ref ent2;
                containerInv = ref ent1;
            }
            else {
                return false; // Neither container is player
            }
        }
        
        if (playerStats.Id == 0 || containerInv.Id == 0)
            return false;

        if (containerInv.OwnerAccountId != -1 && containerInv.OwnerAccountId != cmd.User.GameInfo.Account.Id)
            return false;
        
        if (playerStats.DistSqr(ref containerStats) > 3f * 3f)
            return false;

        var playerIsEnt1 = playerStats.Id == cmd.SlotA.ObjectId;
        if ((playerIsEnt1 && en2.Desc.Class == "OneWayContainer") || en1.Desc.Class == "OneWayContainer")
            return false;

        var playerSlot = playerIsEnt1 ? cmd.SlotA.SlotId : cmd.SlotB.SlotId;
        var containerSlot = playerIsEnt1 ? cmd.SlotB.SlotId : cmd.SlotA.SlotId;
        var playerItem = playerInv[playerSlot];
        var containerItem = playerInv[playerSlot];

        if (cmd.SlotA.SlotId is 255 or 254) { // Handle potion stacking
            if (!playerIsEnt1) // Illegal action
                return false;
            
            if (containerInv[cmd.SlotB.SlotId] != null) // Can't swap potion with a regular item, needs to be empty slot
                return false;

            var item = playerInv.UnstackPotion(cmd.SlotA.SlotId);
            var ret = item != null;
            if (ret)
                playerInv.SetItem(cmd.SlotB.SlotId, item);
            return true;
        }
        
        if (cmd.SlotB.SlotId is 255 or 254) { // Handle potion un-stacking
            if (playerIsEnt1) // Illegal action
                return false;

            var item = containerInv[cmd.SlotA.SlotId];
            var ret = playerInv.StackPotion(item);
            if (ret)
                containerInv.SetItem(cmd.SlotA.SlotId, null);
            return true;
        }
        
        if (!playerInv.IsEquippable(playerItem, playerSlot))
            return false;

        Swap(ref playerInv, ref containerInv, playerSlot, containerSlot, playerItem, containerItem);
        return true;
    }

    // Player can only perform a swap in a container if the container is public or owned by the player
    public bool DoContainerInvSwap(ref SwapCommand cmd) {
        ref var containerInv = ref _world.EntityInventories.Get(cmd.SlotA.ObjectId);
        if (containerInv.Id == 0)
            return false;

        if (containerInv.OwnerAccountId != -1 && containerInv.OwnerAccountId != cmd.User.GameInfo.Account.Id)
            return false;

        ref var plrStats = ref _world.EntityStats.Get(cmd.User.GameInfo.PlayerId);
        ref var containerStats = ref _world.EntityStats.Get(containerInv.Id);
        if (plrStats.DistSqr(ref containerStats) > 3f * 3f)
            return false;

        var itemA = containerInv[cmd.SlotA.ObjectId];
        var itemB = containerInv[cmd.SlotB.ObjectId];
        Swap(ref containerInv, ref containerInv, cmd.SlotA.SlotId, cmd.SlotB.SlotId, itemA, itemB);
        return true;
    }

    private void Swap(ref EntityInventory invA, ref EntityInventory invB, int slotA, int slotB, Item itemA, Item itemB) {
        invA.SetItem(slotA, itemB);
        invB.SetItem(slotB, itemA);
        
        if (invB.IsEmpty()) // Careful, assumed to be a bag
            _world.LeaveWorld(invB.Id);
    }
    
    public override void Tick(ref RealmTime time) {
        while (_swapCommands.TryDequeue(out var command)) {
            ExecuteSwap(ref command);
        }
        
        foreach (ref var inv in this) {
            inv.Tick(ref time);
        }
    }
}