#region

using Common;
using Common.Utilities;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Utilities.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities;

public partial class Player
{
    public const int TP_COOLDOWN = 10000; // 10 seconds

    public const int SIGHT_RADIUS = 20;
    public const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
    public const int ACTIVE_RADIUS = 2; // Activate surrounding chunks

    private readonly HashSet<WorldTile> _visibleTiles = new();
    private readonly HashSet<WorldTile> _tilesDiscovered = new();
    private readonly List<WorldTile> _newTiles = new();

    private readonly ConcurrentQueue<Entity> _deadEntities = new();
    private readonly LazyCollection<Entity> _visibleEntities = new();
    private readonly LazyCollection<Entity> _visibleStaticEntities = new();
    private readonly List<ObjectData> _newEntities = new();
    private readonly List<ObjectDropData> _oldEntities = new();
    private readonly object _updateLock = new();

    private readonly Action<Entity> _onDeathHandler;
    private readonly Action<Entity, StatType, object> _entityStatChangedHandler;

    public bool Teleporting;
    public int TPCooldownLeft;
    private WorldPosData _tpPos;

    private void InitPlayerSight()
    {
        _visibleEntities.OnAdd += OnEntityAdded;
        _visibleEntities.OnRemove += OnEntityRemoved;
        _visibleStaticEntities.OnAdd += OnEntityAdded;
        _visibleStaticEntities.OnRemove += OnEntityRemoved;
    }

    public bool TeleportTo(WorldPosData pos, bool force = false)
    {
        if (!force)
        {
            if (Teleporting)
                return false;

            if (TPCooldownLeft > 0)
                return false;

            TPCooldownLeft = TP_COOLDOWN;
        }

        Teleporting = true;

        _tpPos = pos;
        User.SendPacket(new Goto(pos));
        return true;
    }

    public void FinishTeleport()
    {
        Teleporting = false;
        Move(_tpPos.X, _tpPos.Y);
    }

    public override bool Move(float posX, float posY)
    {
        if (User.State != ConnectionState.Ready || User.GameInfo.State != GameState.Playing || !Initialized || Dead)
            return false;

        ProcessUnconfirmedHits(posX, posY);
        if (!base.Move(posX, posY))
            return false;

        return true;
    }

    private void ProcessUnconfirmedHits(float posX, float posY)
    {
        using (TimedLock.Lock(_unconfirmedHits))
        {
            var toRemove = new HashSet<Projectile>();
            foreach (var unconfirmedHit in _unconfirmedHits)
            {
                var timeOfUnconfirmation = unconfirmedHit.GetUnconfirmedHitTime(this);
                if (timeOfUnconfirmation == 0)
                {
                    toRemove.Add(unconfirmedHit);
                    continue;
                }

                if (RealmManager.WorldTime.TotalElapsedMs >
                    timeOfUnconfirmation) // GREAT. we have a move ack after the time the projectile should have collided. evaluate
                {
                    if (unconfirmedHit.CheckUnconfirmedHit(this, posX,
                            posY)) // PERFECT CASE. hit has been confirmed. no problems
                    {
                        unconfirmedHit.ConfirmHit(this);
                        toRemove.Add(unconfirmedHit);
                        continue;
                    }

                    // unfortunately we could not confirm the hit, how big was this time gap?
                    var timeDifference = RealmManager.WorldTime.TotalElapsedMs - timeOfUnconfirmation;
                    if (timeDifference < 100)
                    {
                        // okay, this movement has only covered 0.1 of a second, if it didnt go through. then fine
                        unconfirmedHit.RemoveHit(this);
                        toRemove.Add(unconfirmedHit);
                        continue;
                    }

                    // okay its been over 0.1 seconds, lets try work out where the player has been
                    var newPosVec = new Vector2(posX, posY);
                    var originalPosVec = new Vector2(Position.X, Position.Y);
                    var posDiffVec = newPosVec - originalPosVec;

                    var hit = false;
                    for (var i = timeOfUnconfirmation; i < RealmManager.WorldTime.TotalElapsedMs; i += 20)
                    {
                        float vectorPerc = timeDifference / i;
                        var assumedMovement = originalPosVec + (posDiffVec * vectorPerc);
                        if (unconfirmedHit.CheckUnconfirmedHit(this, assumedMovement.X, assumedMovement.Y))
                        {
                            hit = true;
                            unconfirmedHit.ConfirmHit(this);
                            toRemove.Add(unconfirmedHit);
                            break;
                        }
                    }

                    if (!hit)
                    { // okay. we didn't hit
                        unconfirmedHit.RemoveHit(this);
                        toRemove.Add(unconfirmedHit);
                    }
                }
            }

            foreach (var toRem in toRemove)
                _unconfirmedHits.Remove(toRem);
        }
    }

    private void SendUpdate()
    {
        GetNewTiles();
        ProcessVisibleEntities();

        if (_newTiles.Count > 0 || _newEntities.Count > 0 || _oldEntities.Count > 0)
            User.SendPacket(new Update(_newTiles,
                _newEntities,
                _oldEntities,
                _entityStatUpdates));

        _newTiles.Clear();
        _newEntities.Clear();
        _oldEntities.Clear();
    }

    private void GetNewTiles()
    {
        _visibleTiles.Clear();
        switch (World.Config.Blocksight)
        {
            case World.UNBLOCKED_SIGHT:
                var pX = (int)Position.X;
                var pY = (int)Position.Y;
                var width = World.Map.Width;
                var height = World.Map.Height;
                for (var y = pY - SIGHT_RADIUS; y <= pY + SIGHT_RADIUS; y++)
                    for (var x = pX - SIGHT_RADIUS; x <= pX + SIGHT_RADIUS; x++)
                        if (x >= 0 && x < width && y >= 0 && y < height &&
                            this.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR)
                        {
                            var tile = World.Map[x, y];

                            _visibleTiles.Add(tile);
                            if (_tilesDiscovered.Add(tile)) // This is a newly discovered tile
                                _newTiles.Add(tile);
                        }

                break;
        }
    }

    public void TileUpdate(WorldTile tile)
    {
        _tilesDiscovered.Remove(tile);
    }

    private void ProcessVisibleEntities()
    {
        if (Quest != null && Quest.Id != QuestId)
        { // This means we have a new quest
            QuestId = Quest.Id;
            _visibleEntities.Add(Quest);
        }
        else if (Quest == null)
            QuestId = -1;

        foreach (var kvp in _visibleEntities)
        { // Some entities could have gone out of the active chunks so we have to manually check
            var en = kvp.Value;
            if (!IsEntityVisible(en))
                _visibleEntities.Remove(en);
        }

        _visibleEntities.Update();
        _visibleStaticEntities.Update();
    }

    public void HandleEntityTick(Entity en)
    {
        if (en.Dead)
            return;

        var chunk = en.Tile.Chunk;
        if (chunk.DistSqr(Tile.Chunk) > ACTIVE_RADIUS)
        {
            return;
        }

        if (IsEntityVisible(en))
        {
            if (en.Desc.Static)
            {
                _visibleStaticEntities.Add(en);
                return;
            }

            _visibleEntities.Add(en);
            return;
        }

        if (en.Desc.Static) // Ignore static entities out of sight
            return;

        _visibleEntities.Remove(en);
    }

    private void OnEntityAdded(Entity en)
    {
        en.DeathEvent += _onDeathHandler;
        using (TimedLock.Lock(en.Stats.StatChangedListeners))
            en.Stats.StatChangedListeners.Add(this);
        _newEntities.Add(en.Stats.GetObjectData(Id));
    }

    private void OnEntityRemoved(Entity en)
    {
        en.DeathEvent -= _onDeathHandler;
        using (TimedLock.Lock(en.Stats.StatChangedListeners))
            en.Stats.StatChangedListeners.Remove(this);
        _oldEntities.Add(en.Stats.GetObjectDropData());
    }

    public void AddAllPlayers() // Should only be called once, in Initialize method
    {
        foreach (var kvp in World.Players)
        {
            var plr = kvp.Value;
            if (!plr.Dead)
                _visibleEntities.Add(plr);
        }
    }

    public void AddPlayerEntity(Player plr)
    {
        if (!plr.Dead)
            _visibleEntities.Add(plr);
    }

    private bool IsEntityVisible(Entity en)
    {
        if (en.IsPlayer || en.Desc.KeepInSight)
            return !en.Dead;

        if (_visibleTiles.Contains(en.Tile))
        {
            if (en is Container c)
            {
                return c.IsVisibleTo(this);
            }

            return !en.Dead;
        }

        if (en == Quest || en == DamageCounterTarget)
            return !en.Dead;

        return false;
    }

    private void HandleEntityDeath(Entity en)
    {
        if (Dead)
            return;

        if (en.Desc.Static)
        {
            _visibleStaticEntities.Remove(en);
            return;
        }

        _visibleEntities.Remove(en);
    }
}