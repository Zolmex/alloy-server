#region

using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Worlds;
using System;
using System.Numerics;
using System.Threading;

#endregion

namespace GameServer.Game.Entities
{
    public class Entity : IIdentifiable
    {
        private static readonly Random _random = new();

        protected readonly Logger _log;
        private static int _nextEntityId;

        #region STATS

        public string Name
        {
            get => Stats.Get<string>(StatType.Name);
            set => Stats.Set(StatType.Name, value);
        }

        public int MaxHP
        {
            get => Stats.Get<int>(StatType.MaxHP);
            set => Stats.Set(StatType.MaxHP, value);
        }

        public int HP
        {
            get => Stats.Get<int>(StatType.HP);
            set => Stats.Set(StatType.HP, value);
        }

        public int Size
        {
            get => Stats.Get<int>(StatType.Size);
            set => Stats.Set(StatType.Size, value);
        }

        public int MaxMS
        {
            get => Stats.Get<int>(StatType.MaxMS);
            set => Stats.Set(StatType.MaxMS, value, true);
        }

        public int MS
        {
            get => Stats.Get<int>(StatType.MS);
            set => Stats.Set(StatType.MS, value, true);
        }

        #endregion

        public int Lifetime { get; set; }

        public float HpPerc => (float)HP / MaxHP;
        public int Id { get; set; }
        public ObjectDesc Desc { get; }
        public EntityStats Stats { get; }
        public WorldTile Tile { get; private set; }
        public World World { get; private set; }
        public bool Dead { get; protected set; }
        public bool IsPlayer { get; protected set; }
        public bool IsConnected { get; protected set; }
        public bool Spawned { get; set; }

        public bool Initialized;

        public WorldPosData Position;
        public WorldPosData PrevPosition;
        public WorldPosData SpawnPosition;
        public const int MAX_POS_HISTORY_SIZE = 20;
        public event Action<Entity> DeathEvent;
        public Character Parent; // Entity that spawned this entity

        protected readonly object _deathLock = new();

        public event Action<World> OnEnteredWorld;
        public event Action<World> OnLeftWorld;

        public Entity(ushort type, int lifetime = -1)
        {
            _log = new Logger(GetType());

            Id = Interlocked.Increment(ref _nextEntityId);
            Desc = XmlLibrary.ObjectDescs[type];
            Stats = new EntityStats(this);
            Lifetime = lifetime;
        }

        public static Entity Resolve(ushort objType)
        {
            var desc = XmlLibrary.ObjectDescs[objType];

            if (desc.Class != null)
                switch (desc.Class)
                {
                    case "ConnectedWall":
                    case "CaveWall":
                        return new ConnectedObject(objType);
                    case "Portal": // Dungeon portals
                    case "GuildHallPortal":
                        var dungeonName = desc.DungeonName;
                        if (dungeonName != null)
                            return new Portal(objType);
                        break;
                    case "Character":
                        if (desc.Enemy)
                            return new Enemy(objType);
                        return new Character(objType);
                    case "ClosedVaultChest":
                        return new ClosedVaultChest(objType);
                    case "Container":
                        return new Container(objType, 8, -1, -1);
                    case "GuildMerchant":
                        return new GuildMerchant(objType);
                }

            if (desc.Enemy)
                return new Enemy(objType);

            return new Entity(objType);
        }

        public void EnterWorld(World world)
        {
            World = world;
            World.AddEntity(this);
            OnEnteredWorld?.Invoke(world);
        }

        public virtual void Initialize()
        {
            LoadStats();

            SpawnPosition = new WorldPosData(Position.X, Position.Y);

            Tile = World.Map[(int)Position.X, (int)Position.Y];
            Tile.Chunk.Insert(this);

            Initialized = true;
        }

        public virtual bool Tick(RealmTime time)
        {
            if (!Initialized)
                return false;

            if (Lifetime > 0)
            {
                Lifetime -= 1000 / GameServerConfig.Config.TPS;

                if (Lifetime <= 0)
                    TryLeaveWorld();
            }

            UpdateTileChunk();
            Stats.Update();
            return true;
        }

        protected virtual void LoadStats()
        {
            Stats.Initializing = true;

            Name = Desc.ObjectId;
            MaxHP = Desc.MaxHP;
            HP = MaxHP;
            Size = Desc.MaxSize != 0 ? _random.Next(Desc.MinSize, Desc.MaxSize) : Desc.Size;

            Stats.Initializing = false;
        }

        public virtual bool MoveRelative(Vector2 relVec)
        {
            return MoveRelative(relVec.X, relVec.Y);
        }

        public virtual bool MoveRelative(float relX, float relY)
        {
            return Move(Position.X + relX, Position.Y + relY);
        }

        public virtual bool Move(Vector2 pos)
        {
            return Move(pos.X, pos.Y);
        }

        public virtual bool Move(float posX, float posY)
        {
            if (Dead)
                return false;

            if (World != null && !World.IsPassable(posX, posY))
                return false;

            PrevPosition.X = Position.X;
            PrevPosition.Y = Position.Y;
            Position.X = posX;
            Position.Y = posY;
            return true;
        }

        protected void UpdateTileChunk()
        {
            if (World != null && Position.X >= 0 && Position.X < World.Map.Width && Position.Y >= 0 && Position.Y < World.Map.Height)
            {
                var oldChunk = Tile?.Chunk;
                Tile = World.Map[(int)Position.X, (int)Position.Y];

                var newChunk = Tile?.Chunk;
                if (newChunk != null && newChunk != oldChunk)
                {
                    oldChunk?.Remove(this);
                    newChunk.Insert(this);
                }
            }
        }

        public virtual bool TryLeaveWorld()
        {
            using (TimedLock.Lock(_deathLock))
            {
                if (Dead)
                    return false;

                Dead = true;
            }

            LeaveWorld();
            return true;
        }

        protected virtual void LeaveWorld()
        {
            DeathEvent?.Invoke(this);
            DeathEvent = null;
            Tile?.Chunk?.Remove(this);
            World.RemoveEntity(this);

            OnLeftWorld?.Invoke(World);
            OnLeftWorld = null;
            OnEnteredWorld = null;

            World.OnUpdate(Dispose); // Avoid disposing in the middle of a tick
        }

        // moveTo: absolute world position
        public void MoveTowards(RealmTime time, Vector2 moveTo, float tilesPerSecond)
        {
            var angle = this.GetAngleBetween(moveTo);
            var dist = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            var speed = this.GetSpeed(tilesPerSecond) * (time.ElapsedMsDelta / 1000f);
            dist *= speed;

            if (moveTo.DistSqr(Position.ToVec2()) < dist.LengthSquared())
            { // If the distance we're about to move is greater than the distance to the desired position, set position to the desired position
                Move(moveTo.X, moveTo.Y);
                return;
            }

            MoveRelative(dist);
        }

        public void MoveTowards(RealmTime time, float angleRad, float tilesPerSecond)
        {
            var dist = new Vector2(MathF.Cos(angleRad), MathF.Sin(angleRad));
            var speed = this.GetSpeed(tilesPerSecond) * (time.ElapsedMsDelta / 1000f);
            dist *= speed;
            MoveRelative(dist);
        }

        public virtual void Dispose()
        {
            Initialized = false;
            Stats.Clear();
        }
    }
}