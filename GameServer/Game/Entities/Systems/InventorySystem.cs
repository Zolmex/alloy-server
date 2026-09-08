using System.Collections.Concurrent;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Common;
using Common.Game;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Utilities;
using ArchWorld = Arch.Core.World;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Systems;

public readonly record struct SwapCommand(User User, SlotObjectData SlotA, SlotObjectData SlotB);

public partial class InventorySystem : BaseSystem<World, RealmTime> {
    
    private readonly ConcurrentQueue<SwapCommand> _swapCommands = new();
    private readonly ArchWorld _archWorld;

    public InventorySystem(World world, ArchWorld archWorld) : base(world) {
        _archWorld = archWorld;
    }
    
    public void Tick(ref RealmTime time) {
        while (_swapCommands.TryDequeue(out var cmd)) {
            ExecuteSwap(ref cmd);
        }
    }
    
    [Query(Parallel = true)]
    public void Process(ref Stats stats, ref Inventory inv) {
        if (inv.ItemUpdates.IsEmpty)
            return;
        
        stats.Set(StatType.HealthPotionStack, inv.PotionStacks[0]);
        stats.Set(StatType.MagicPotionStack, inv.PotionStacks[1]);
        for (var i = 0; i < inv.Size; i++) {
            if (inv.ItemUpdates.IsSet(i)) {
                stats.Set(StatType.Inventory0 + i, inv.Items[i]?.ObjectType ?? -1);
                stats.Set(StatType.InventoryData0 + i, inv.Items[i]?.ExportString());
            }
        }

        inv.ItemUpdates.Clear();
    }
    
    public void EnqueueSwap(User user, SlotObjectData slotA, SlotObjectData slotB) {
        _swapCommands.Enqueue(new SwapCommand(user, slotA, slotB));
    }
    
    private void ExecuteSwap(ref SwapCommand cmd) {
        var success = false;
        var entA = World.GetEntity(cmd.SlotA.ObjectId);
        var entB = World.GetEntity(cmd.SlotB.ObjectId);
        
        if (!_archWorld.IsAlive(entA) || !_archWorld.IsAlive(entB)) {
            cmd.User.SendPacket(new InvResult(1));
            return;
        }

        if (cmd.SlotA.ObjectId == cmd.SlotB.ObjectId) {
            if (cmd.SlotA.ObjectId == cmd.User.Session.PlayerId) 
                success = DoPlayerInvSwap(ref cmd, entA);
            else 
                success = DoContainerInvSwap(ref cmd, entA);
        }
        else {
            success = DoPlayerContainerInvSwap(ref cmd, entA, entB);
        }

        cmd.User.SendPacket(new InvResult(success ? 0 : 1));
    }
    
    private bool DoPlayerInvSwap(ref SwapCommand cmd, Entity playerEnt) {
        if (cmd.SlotA.SlotId == cmd.SlotB.SlotId)
            return false;
        
        if (!_archWorld.Has<Inventory>(playerEnt))
            return false;

        ref var playerInv = ref _archWorld.Get<Inventory>(playerEnt);
        if (cmd.SlotA.SlotId is 255 or 254) { // Unstack potion
            if (playerInv.Items[cmd.SlotB.SlotId] != null)
                return false;
            
            var itemType = cmd.SlotA.SlotId == 255 ? 2594 : 2595;
            var item = new Item(XmlLibrary.ItemDescs[(ushort)itemType].Root);
            
            var stackIdx = item.ObjectType == 2594 ? 0 : 1;
            if (playerInv.PotionStacks[stackIdx] <= 0)
                return false;
            
            playerInv.PotionStacks[stackIdx]--;
            playerInv.SetItem(cmd.SlotB.SlotId, item);
            return true;
        }
        
        if (cmd.SlotB.SlotId is 255 or 254) { // Stack potion
            var item = playerInv.Items[cmd.SlotA.SlotId];
            if (playerInv.StackPotion(item)) {
                playerInv.SetItem(cmd.SlotA.SlotId, null);
                return true;
            }
            return false;
        }
        
        var itemA = playerInv.Items[cmd.SlotA.SlotId];
        var itemB = playerInv.Items[cmd.SlotB.SlotId];
        
        if (!playerInv.IsEquippable(itemA, cmd.SlotB.SlotId) ||
            !playerInv.IsEquippable(itemB, cmd.SlotA.SlotId)) return false;

        playerInv.SwapSlots(cmd.SlotA.SlotId, cmd.SlotB.SlotId);
        return true;
    }

    private bool DoPlayerContainerInvSwap(ref SwapCommand cmd, Entity entA, Entity entB) {
        if (!_archWorld.Has<Inventory>(entA) || !_archWorld.Has<Inventory>(entB) ||
            !_archWorld.Has<Position>(entA) || !_archWorld.Has<Position>(entB)) 
            return false;

        var entAIsPlayer = entA.Has<PlayerType>();
        var entBIsPlayer = entB.Has<PlayerType>();
        
        if (entAIsPlayer == entBIsPlayer) return false; // One must be player, one container

        var playerEnt = entAIsPlayer ? entA : entB;
        var containerEnt = entAIsPlayer ? entB : entA;
        
        ref var playerInv = ref _archWorld.Get<Inventory>(playerEnt);
        ref var containerInv = ref _archWorld.Get<Inventory>(containerEnt);
        ref var playerPos = ref _archWorld.Get<Position>(playerEnt);
        ref var containerPos = ref _archWorld.Get<Position>(containerEnt);

        if (!containerInv.OwnedBy(cmd.User.Session.Account.Id))
            return false;
        
        if (playerPos.DistSqr(ref containerPos) > 9f) // 3 tiles squared
            return false;

        // Distance and one-way checks
        var playerIsEnt1 = playerEnt == entA;
        var containerDesc = XmlLibrary.ObjectDescs[containerEnt.Get<ObjectType>()];
        if ((playerIsEnt1 && containerDesc.Class == "OneWayContainer") || (!playerIsEnt1 && containerDesc.Class == "OneWayContainer"))
            return false;

        var playerSlot = playerIsEnt1 ? cmd.SlotA.SlotId : cmd.SlotB.SlotId;
        var containerSlot = playerIsEnt1 ? cmd.SlotB.SlotId : cmd.SlotA.SlotId;
        var playerItem = playerInv.Items[playerSlot];
        var containerItem = containerInv.Items[containerSlot];

        // Potion Stacking
        if (cmd.SlotA.SlotId is 255 or 254) { 
            if (!playerIsEnt1 || containerInv.Items[cmd.SlotB.SlotId] != null)
                return false;
            
            var itemType = cmd.SlotA.SlotId == 255 ? 2594 : 2595;
            var stackIdx = itemType == 2594 ? 0 : 1;
            
            if (playerInv.PotionStacks[stackIdx] <= 0)
                return false;
            
            playerInv.PotionStacks[stackIdx]--;
            
            containerInv.SetItem(cmd.SlotB.SlotId, new Item(XmlLibrary.ItemDescs[(ushort)itemType].Root));
            return true;
        }
        
        // Potion Unstacking
        if (cmd.SlotB.SlotId is 255 or 254) { 
            if (playerIsEnt1) return false;
            if (playerInv.StackPotion(containerItem)) {
                containerInv.SetItem(cmd.SlotA.SlotId, null);
                return true;
            }
            return false;
        }

        if (!playerInv.IsEquippable(containerItem, playerSlot) || !containerInv.IsEquippable(playerItem, containerSlot))
            return false;

        Swap(ref playerInv, ref containerInv, playerSlot, containerSlot, playerItem, containerItem);
        return true;
    }

    private bool DoContainerInvSwap(ref SwapCommand cmd, Entity containerEnt) {
        if (!_archWorld.Has<Inventory>(containerEnt))
            return false;

        ref var containerInv = ref _archWorld.Get<Inventory>(containerEnt);
        if (!containerInv.OwnedBy(cmd.User.Session.Account.Id))
            return false;

        var playerEnt = World.GetEntity(cmd.User.Session.PlayerId);
        if (playerEnt != Entity.Null && _archWorld.IsAlive(playerEnt)) {
            ref var pPos = ref _archWorld.Get<Position>(playerEnt);
            ref var cPos = ref _archWorld.Get<Position>(containerEnt);
            if (pPos.DistSqr(ref cPos) > 9f)
                return false;
        }

        var itemA = containerInv.Items[cmd.SlotA.SlotId];
        var itemB = containerInv.Items[cmd.SlotB.SlotId];
        Swap(ref containerInv, ref containerInv, cmd.SlotA.SlotId, cmd.SlotB.SlotId, itemA, itemB);
        return true;
    }

    private void Swap(ref Inventory invA, ref Inventory invB, int slotA, int slotB, Item itemA, Item itemB) {
        invA.SetItem(slotA, itemB);
        invB.SetItem(slotB, itemA);
    }
}