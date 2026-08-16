using System;
using System.Reflection;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct PortalData : IEntityIdentifiable, IDisposable {
    private static readonly Dictionary<string, Type> _worldTypes = [];
    private static readonly Logger _log = new(typeof(PortalData));

    static PortalData() {
        var asm = Assembly.GetExecutingAssembly();
        foreach (var type in asm.GetTypes()) {
            if (type == typeof(World) || !type.IsSubclassOf(typeof(World)))
                continue;

            _worldTypes[type.Name] = type;
        }
    }
    
    public EntityId Id { get; set; }

    public bool DisplayPlayerCount;
    public bool Disabled;
    
    private readonly World _world;
    private World _worldLink;
    
    public PortalData(World world, ref Entity en) {
        Id = en.Id;
        _world = world;

        LoadWorld(ref en);
    }

    private void LoadWorld(ref Entity en) {
        if (en.Desc.RealmPortal)
            return;

        var worldName = en.Desc.DungeonName;
        var worldConfig = WorldLibrary.WorldConfigs.Values.FirstOrDefault(i => i.DisplayName == worldName);
        if (worldConfig.Name == null) {
            _log.Error($"World '{worldName}' not found ({en.Desc.ObjectId})");
            return;
        }

        if (!_worldTypes.TryGetValue(worldConfig.Name, out var worldType)) {
            _log.Error($"World logic doesn't exist for '{worldName}' ({en.Desc.ObjectId})");
            return;
        }
        
        var worldInstance = worldConfig.Id == 0
            ? (World)Activator.CreateInstance(worldType, worldConfig.Id, -1, worldConfig)
            : RealmManager.Worlds[worldConfig.Id];
        
        Init(worldInstance);
    }

    public void Init(World worldLink) {
        RealmManager.AddWorld(worldLink);
        LinkTo(worldLink);
    }

    public void LinkTo(World worldLink) {
        _worldLink = worldLink;
    }

    public World GetWorldInstance(User user) {
        if (_worldLink == null)
            return null;
        return _worldLink.GetInstance(user);
    }

    public void Tick(ref RealmTime time) {
        if (_worldLink == null)
            return;

        if (_worldLink.Deleted) {
            _world.LeaveWorld(Id);
        }
        
        if (DisplayPlayerCount) {
            ref var stats = ref _world.EntityStats.Get(Id);
            stats.Set(StatType.Name, $"{_worldLink.DisplayName} ({_worldLink.Users.Count}/{_worldLink.Config.MaxPlayers})");
        }
    }
    
    public void Dispose() {
        
    }
}