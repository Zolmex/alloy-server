using System.Collections.Generic;
using System.Numerics;
using Arch.Core;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Behaviors.Actions.Info;

public class DashInfo {
    public int DashCount { get; set; }
    public float DashAngle { get; set; }
    public Entity CurrentTarget { get; set; }
    public int DashCooldown { get; set; }
    public int CycleCooldown { get; set; }
    public long DashStarted { get; set; }
    public Vector2 DashStartPos { get; set; }
    public bool Dashing { get; set; }
    public bool InCycle { get; set; }
    public bool DashStartSent { get; set; }
    public HashSet<Entity> HitThisDash { get; set; } = new();
}