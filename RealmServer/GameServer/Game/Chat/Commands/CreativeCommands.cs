#region

using Common;
using Common.API.Helpers;
using Common.API.Requests;
using Common.Enums;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Linq;

#endregion

namespace GameServer.Game.Chat.Commands;

[Command("eff", CommandPermissionLevel.Creative)]
public class EffCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (!Enum.TryParse<ConditionEffectIndex>(args, out var eff))
        {
            player.SendError($"Invalid effect: {args}");
            return;
        }

        if (player.HasConditionEffect(eff))
            player.RemoveConditionEffect(eff);
        else
            player.ApplyConditionEffect(eff, -1);
    }
}

[Command("clearspawn", CommandPermissionLevel.Creative)]
public class ClearspawnCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var entities = player.World.GetEnemiesByName(args, player.Position.X, player.Position.Y, 32f);
        var entityCount = 0;
        foreach (var entity in entities)
        {
            if (!entity.Spawned)
                continue;

            entity.Death(player.Name);
            entityCount++;
        }

        if (entityCount > 0)
            player.SendInfo($"Cleared {entityCount} {args}");
    }
}

[Command("heal", CommandPermissionLevel.Creative)]
public class HealCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (string.IsNullOrEmpty(args))
        {
            player.SendError("Usage: /heal {amount}");
            return;
        }

        if (int.TryParse(args, out var result))
            player.Heal(result);
    }
}

[Command("die", CommandPermissionLevel.Creative)]
public class DieCommand : Command
{
    public override void Execute(Player player, string args)
    {
        player.User.SendPacket(new Notification(player.Id, "LOLOLOLOL DYING IN 1 SECOND LOLOLOLOL", 0xFF0000));
        RealmManager.AddTimedAction(1000, () =>
        {
            if (player != null && player.World != null)
                player.Damage(99999, player);
        });
    }
}

[Command("level", CommandPermissionLevel.Creative)]
public class LevelCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (int.TryParse(args, out var lvl))
            for (var i = 0; i < lvl; i++)
            {
                var givenXp = player.NextLevelXpGoal;

                player.SendInfo($"Given {givenXp} xp.");
                player.GainXP(null, givenXp);
            }
        else
            player.SendInfo("Usage: /level (increases level by amount by giving xp)");
    }
}

[Command("xp", CommandPermissionLevel.Creative)]
public class XpCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (int.TryParse(args, out var xp))
        {
            player.GainXP(null, xp);
            player.SendInfo($"Given {xp} xp.");
        }
        else
            player.SendInfo("Usage: /xp (amount)");
    }
}

[Command("setstat", CommandPermissionLevel.Creative)]
public class SetStatCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var txt = args.Split(' ');

        if (txt.Length != 2)
        {
            player.SendInfo("Usage: /setstat {statname} {value}");
            return;
        }

        float amount = 0;

        if (int.TryParse(txt[1], out var amt))
            amount = amt;
        else if (float.TryParse(txt[1], out var famt))
            amount = famt;

        if (amount == 0)
        {
            player.SendInfo("Invalid Amount!");
            return;
        }

        var type = EnumUtils.TextToStatType(txt[0]);

        if (type == StatType.None)
        {
            player.SendInfo("Invalid Stat Type!");
            return;
        }

        if (type == StatType.Attack || type == StatType.Defense || type == StatType.Dexterity ||
            type == StatType.Wisdom)
        {
            player.Char.MainStats[type] = (int)amount;
            player.RecalculateStats();
        }
        else
        {
            player.Char.BaseStats[type] = amount;
            player.RecalculateStats();
        }

        player.SaveCharacter(true);

        player.HandleEntityStatChanged(player, type, amount);
        player.SendInfo($"Set {type} to {amount}");
    }
}

[Command("god", CommandPermissionLevel.Creative)]
public class GodCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (player.HasConditionEffect(ConditionEffectIndex.Invincible))
            player.RemoveConditionEffect(ConditionEffectIndex.Invincible);
        else
            player.ApplyConditionEffect(ConditionEffectIndex.Invincible, -1);
    }
}

[Command("test", CommandPermissionLevel.Creative)]
public class TestCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var item = XmlLibrary.Id2Item("Demon Blade");
        player.World.DropLoot(player.Position.X, player.Position.Y, new Item[1] { new(item.Root) });
    }
}

[Command("drop", CommandPermissionLevel.Creative)]
public class DropCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var item = XmlLibrary.Id2Item(args);
        if (item == null)
        {
            player.SendInfo($"Item {args} does not exist");
            return;
        }

        player.World.DropLoot(player.Position.X, player.Position.Y, new Item[1] { new(item.Root) },
            player);
    }
}

[Command("droploot", CommandPermissionLevel.Creative)]
public class DropLootCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (args == "0")
        {
            var eventSpawn = new EventSpawnRequest(player.Name, SubscribableEvent.Avatar);
            APIHelper.SendRequestAuth(eventSpawn);
        }
        else if (args == "1")
        {
            var eventDeath = new EventDeathRequest(player.Name, SubscribableEvent.LordoftheLostLands);
            APIHelper.SendRequestAuth(eventDeath);
        }
        else
        {
            var showMessage = new DropLootRequest(player.Name, args, LootDropRarity.All);
            APIHelper.SendRequestAuth(showMessage);
        }
    }
}

[Command("constellation", CommandPermissionLevel.Creative)]
public class ConstellationTestCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            player.SendHelp("Constellations Reset!");
            player.PrimaryConstellation = -1;
            player.SecondaryConstellation = -1;
            player.PrimaryNodeData = -1;
            player.SecondaryNodeData = -1;
            player.PrimaryNodes = new int[] { -1, -1, -1, -1 };
            player.SecondaryNodes = new int[] { -1, -1, -1, -1 };
            player.ReloadConstellationMods();
            return;
        }

        if (args != "0" && args != "1" && args != "2" && args != "3" && args != "4")
        {
            player.SendError("Invalid Constellation! 0 = Indus 1 = Apus 2 = Cygnus 3 = Grus 4 = Pyxis");
            return;
        }

        if (player.PrimaryConstellation == -1)
        {
            player.PrimaryConstellation = int.Parse(args);
            //player.PrimaryNodeData = 1111;
            player.PrimaryNodes = player.ConvertNodeData(player.PrimaryNodeData);
            player.SendInfo("Set Primary Constellation To " + args);
        }
        else if (player.SecondaryConstellation == -1)
        {
            player.SecondaryConstellation = int.Parse(args);
            //player.SecondaryNodeData = 1111;
            player.SecondaryNodes = player.ConvertNodeData(player.SecondaryNodeData);
            player.SendInfo("Set Secondary Constellation To " + args);
        }
    }
}

[Command("give", CommandPermissionLevel.Creative)]
public class GiveCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var item = XmlLibrary.Id2Item(args);
        if (item == null)
        {
            player.SendError($"Item {args} does not exist.");
            return;
        }

        var nextSlot = player.Inventory.GetNextAvailableSlot();
        if (nextSlot == -1)
        {
            player.SendError("Not enough space in inventory.");
            return;
        }

        player.Inventory.SetItem(nextSlot, new Item(item.Root));
    }
}

[Command("gotoEvent", CommandPermissionLevel.Creative)]
public class GotoEventCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (string.IsNullOrEmpty(args))
            args = "self";

        if (player.World is not Realm realm)
        {
            player.SendError("You can only use this command in the realm");
            return;
        }

        var realmEvent = realm.GetActiveEvent();
        if (realmEvent == null || realmEvent.Dead)
        {
            player.SendError("There is no event alive at the moment.");
            return;
        }

        var reconList = CommandManager.GetPlayers(args, player);
        foreach (var plr in reconList)
            plr.TeleportTo(realmEvent.Position, true);
    }
}

[Command("recon", CommandPermissionLevel.Creative)]
public class ReconnectCommand : Command
{
    public override async void Execute(Player player, string args)
    {
        var options = args.Split(' ');
        if (args.Length < 1 || options.Length < 2)
        {
            player.SendHelp("Usage: /recon (self/nearby/all/server) (world name)");
            return;
        }

        var worldName = options[1];
        if (!int.TryParse(worldName, out var worldId))
            worldId = -1;

        if (worldId == -1 && (worldName == "Realm" || worldName == "Test"))
        {
            player.SendError("Invalid.");
            return;
        }

        World world = null;
        if (worldId == -1)
        {
            foreach (var kvp in RealmManager.Worlds)
                if (kvp.Value.DisplayName == worldName)
                {
                    world = kvp.Value;
                    break;
                }

            if (world == null)
            {
                try
                {
                    world = new World(worldName, -1);
                    if (world.Config.DisplayName == null)
                    {
                        player.SendError("Invalid world name.");
                        return;
                    }

                    await RealmManager.AddWorld(world);
                }
                catch (Exception e)
                {
                    CommandManager.LogError(e);
                    player.SendError(e.Message);
                    return;
                }
            }
        }
        else RealmManager.Worlds.TryGetValue(worldId, out world);

        if (world == null)
        {
            player.SendError($"World {(worldId == -1 ? worldName : worldId)} not found.");
            return;
        }

        var reconList = CommandManager.GetPlayers(options[0], player);
        foreach (var plr in reconList)
            plr.User.ReconnectTo(world);
    }
}

[Command("setpiece", CommandPermissionLevel.Creative)]
public class SetpieceCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (player.AccRank < (int)CommandPermissionLevel.Moderator && player.World is not TestWorld)
        {
            player.SendError("Can only use this command in a test world.");
            return;
        }

        var words = args.Split(' ');
        if (words.Length < 1)
        {
            player.SendHelp("Usage: /setpiece (setpiece name) (map index, default 0)");
            return;
        }

        var mapIndex = 0;
        if (words.Length > 1)
        {
            mapIndex = int.Parse(words[1]);
        }

        if (!WorldLibrary.MapDatas.TryGetValue(words[0], out var setpiece))
        {
            player.SendError($"Invalid setpiece: {words[0]}");
            return;
        }

        var map = setpiece[mapIndex];
        player.World.SpawnSetPiece(words[0], (int)player.Position.X, (int)player.Position.Y, mapIndex, null, true);
    }
}

[Command("spawn", CommandPermissionLevel.Creative)]
public class SpawnCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (player.AccRank < (int)CommandPermissionLevel.Moderator && player.World is not TestWorld)
        {
            player.SendError("Can only use this command in a test world.");
            return;
        }

        if (string.IsNullOrWhiteSpace(args))
        {
            player.SendHelp("/spawn <count> <entity>");
            return;
        }

        var rgs = args.Split(' ');

        int spawnCount;
        if (!int.TryParse(rgs[0], out spawnCount))
            spawnCount = -1;

        var desc = XmlLibrary.Id2Object(string.Join(' ', spawnCount == -1 ? rgs : rgs.Skip(1)), false);
        if (spawnCount == -1)
            spawnCount = 1;

        if (desc == null)
        {
            player.SendError("null object desc");
            return;
        }

        if (desc.Player || desc.Static)
        {
            player.SendError("Can't spawn this entity");
            return;
        }

        player.SendInfo($"Spawning <{spawnCount}> <{desc.DisplayId}> in 2 seconds");

        var pos = player.Position;

        RealmManager.AddTimedAction(2000, () =>
        {
            for (var i = 0; i < spawnCount; i++)
            {
                var entity = Entity.Resolve(desc.ObjectType);
                entity.Spawned = true;
                entity.Move(pos.X, pos.Y);
                entity.EnterWorld(player.World);
            }
        });
    }
}

[Command("mod", CommandPermissionLevel.Creative)]
public class ModifyCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var words = args.Split(' ');
        var slot = int.Parse(words[0]);
        var fieldStr = words[1];
        var value = words[2];

        var item = player.Inventory[slot];
        if (item == null)
            return;

        var field = Enum.Parse<ItemField>(fieldStr);
        item.UpdateField((byte)field, value);
        player.Inventory.UpdateSlots(slot);
    }
}


[Command("reloadbehaviors", CommandPermissionLevel.Developer)]
public class ReloadBehaviorsCommand : Command
{
    public override void Execute(Player player, string args)
    {
        player.SendInfo("Reloading behavior files...");
        if (RealmManager.ReloadAllBehaviors())
            player.SendInfo("Successfully reloaded behaviors.");
        else player.SendError("Failed to reload behaviors. Check GameServer console for more information.");
    }
}