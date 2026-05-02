using System;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public struct PlayerSightComponent : IIdentifiable, IDisposable {
    public int Id { get; set; }

    
    
    public void Dispose() {
        
    }
}