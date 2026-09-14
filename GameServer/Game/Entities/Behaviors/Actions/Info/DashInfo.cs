using System.Collections.Generic;
using System.Numerics;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Behaviors.Actions.Info;

public class DashInfo {
    public int DashCount { get; set; }
    public float DashAngle { get; set; }
    public EntityId CurrentTargetID { get; set; }
    public int DashCooldown { get; set; }
    public int CycleCooldown { get; set; }
    public long DashStarted { get; set; }
    public Vector2 DashStartPos { get; set; }
    public bool Dashing { get; set; }
    public bool InCycle { get; set; }
    public bool DashStartSent { get; set; }
    public HashSet<EntityId> HitThisDash { get; set; } = new();
}