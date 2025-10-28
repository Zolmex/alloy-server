#region

using Common;
using Common.Enums;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.StorageClasses;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Entities.Inventory;
using GameServer.Utilities.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace GameServer.Game.Worlds
{
    public class World : IIdentifiable
    {
        public const string NEXUS = "Nexus";
        public const int NEXUS_ID = -1;
        public const int TEST_ID = -2;
        public const string REALM = "Realm";
        public const string VAULT = "Vault";
        public const int UNBLOCKED_SIGHT = 0;

        public static Action<LootDrop> OnLootDrop;
        public static Action<Entity> OnAddEntity;
        public static Action<Entity> OnRemoveEntity;

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public bool Active { get; set; }
        public bool Disposed { get; private set; }
        public bool Deleted { get; private set; }
        public WorldConfig Config { get; private set; }

        public WorldMap Map { get; private set; }
        public int MapId { get; private set; }
        public string Music { get; private set; }
        public bool Initialized { get; protected set; }

        private static int _nextWorldId;

        public readonly LazyCollection<Entity> Entities = new(); // Complete list of entities in this world
        public readonly LazyCollection<Character> Characters = new();
        public readonly LazyCollection<Enemy> Enemies = new();
        public readonly LazyCollection<Enemy> Quests = new();
        public readonly LazyCollection<Player> Players = new();
        public readonly Dictionary<string, Player> PlayerNames = new();
        public readonly LazyCollection<Entity> ActiveEntities = new();
        public readonly LazyCollection<SearchQuery, SearchQueryResult> SearchCache = new();

        private readonly List<Tuple<long, Action>> _timers = new();

        private event Action _onUpdate;
        private readonly object _updateLock = new();
        private event Action<Player> _playerTick;
        private readonly object _playerTickLock = new();
        private event Action _onInitialize;
        private readonly object _initializeLock = new();
        public event Action<Entity> OnEntityTick;

        protected readonly Logger _log;

        protected long _startTime;
        protected long _lastActiveTime;

        public World(string name, int mapId, int worldId = 0)
        {
            _log = new Logger(GetType());

            Id = worldId == 0 ? Interlocked.Increment(ref _nextWorldId) : worldId;
            MapId = mapId;

            if (WorldLibrary.WorldConfigs.TryGetValue(name, out var config))
                Config = config;
            else
                _log.Error($"Couldn't find config for world {name}");

            Music = Config.Music;
            DisplayName = Config.DisplayName ?? Config.Name;
            Name = Config.Name;

            Entities.OnAdd += en => en.Initialize();
        }

        public virtual void Reset()
        {
            Deleted = false;
        }

        public virtual void Initialize()
        {
            LoadMap(Config.Name, MapId);
            InitializeEntities();

            _startTime = RealmManager.WorldTime.TotalElapsedMs;
            _lastActiveTime = RealmManager.WorldTime.TotalElapsedMs;
            Initialized = true;

            using (TimedLock.Lock(_initializeLock))
                _onInitialize?.Invoke();
        }

        protected virtual void InitializeEntities()
        {
            LoadMerchants();
        }

        private void LoadMerchants()
        {
            foreach (var kvp in Map.Regions)
            {
                var region = kvp.Key;
                if (!MerchantsLibrary.Merchants.TryGetValue(region.ToString(), out var merchantDesc))
                    continue;

                foreach (var pos in kvp.Value)
                {
                    var en = new Merchant(458, merchantDesc);
                    en.Move(pos.X + 0.5f, pos.Y + 0.5f);
                    en.EnterWorld(this);
                }
            }
        }

        public void LoadMap(string mapName, int mapId)
        {
            if (!WorldLibrary.MapDatas.TryGetValue(mapName, out var maps))
            { // Get map names
                _log.Error($"Invalid map {mapName}");
                return;
            }

            if (maps.Length > 0)
            {
                if (mapId == -1)
                    mapId = Random.Shared.Next(maps.Length - 1);

                var jsonMap = maps[mapId];

                Map = new WorldMap(this, jsonMap);
                MapId = mapId;

                foreach (var en in Map.Entities) // Load entities from the map
                    en.EnterWorld(this);
            }
        }

        public void LoadJsonMap(string json, string mapName)
        {
            Map = new WorldMap(this, new MapData(json, mapName));

            foreach (var en in Map.Entities) // Load entities from the map
                en.EnterWorld(this);
        }

        public virtual void AddEntity(Entity en) // Should always call Entity.EnterWorld, don't call this directly
        {
            Entities.Add(en);

            OnAddEntity?.Invoke(en);

            if (en.IsPlayer)
            {
                _lastActiveTime = RealmManager.WorldTime.TotalElapsedMs;
                Players.Add(en as Player);
            }
            else if (en is Character chr)
            {
                Characters.Add(chr);

                if (en is Enemy enemy)
                {
                    Enemies.Add(enemy);

                    if (enemy.Desc.Quest && enemy.Desc.Level != -1)
                        Quests.Add(enemy);
                }
            }
        }

        public virtual void RemoveEntity(Entity en)
        {
            Entities.Remove(en);

            OnRemoveEntity?.Invoke(en);

            if (en.IsPlayer)
            {
                Players.Remove(en as Player);
            }
            else if (en is Character chr)
            {
                Characters.Remove(chr);

                if (en is Enemy enemy)
                {
                    Enemies.Remove(enemy);

                    if (enemy.Desc.Quest && enemy.Desc.Level != -1)
                        Quests.Remove(enemy);
                }
            }
        }

        public void Update() // Different from Tick, synchronizes collections and other stuff maybe
        {
            using (TimedLock.Lock(_updateLock))
            {
                _onUpdate?.Invoke();
                _onUpdate = null;
            }

            Entities.Update();
            Players.Update();
            Characters.Update();
            Enemies.Update();
            Quests.Update();
        }

        public void OnUpdate(Action act)
        {
            using (TimedLock.Lock(_updateLock))
                _onUpdate += act;
        }

        public void OnInitialize(Action act)
        {
            using (TimedLock.Lock(_initializeLock))
                _onInitialize += act;
        }

        private void HandleTimers()
        {
            using (TimedLock.Lock(_timers))
            {
                for (var i = 0; i < _timers.Count; i++)
                {
                    var timer = _timers[i];
                    if (timer.Item1 <= RealmManager.WorldTime.TickCount)
                    {
                        timer.Item2();
                        _timers.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void AddTimedAction(int time, Action act)
        {
            using (TimedLock.Lock(_timers))
                _timers.Add(Tuple.Create(RealmManager.WorldTime.TickCount + TicksFromTime(time), act));
        }

        public static long TicksFromTime(long time)
        {
            return time / (1000 / RealmManager.TPS);
        }

        public virtual void Tick(RealmTime time)
        {
            if (!Initialized)
                return;

            if (Deleted)
            {
                OnDelete();

                Update(); // Perform one last time

                RealmManager.RemoveWorld(this);
                return;
            }

            if (!Config.LongLasting && Players.Count == 0 && time.TotalElapsedMs - _lastActiveTime > 60000)
            {
                Delete();
                return;
            }

            HandleTimers();

            for (var cY = 0; cY < Map.Chunks.Height; cY++)
                for (var cX = 0; cX < Map.Chunks.Width; cX++)
                {
                    var chunk = Map.Chunks[cX, cY];
                    chunk.Update();
                    if (chunk.TickCount == time.TickCount || chunk.Players.Count == 0)
                        continue;

                    for (var nearbyY = chunk.CY - Player.ACTIVE_RADIUS;
                         nearbyY <= chunk.CY + Player.ACTIVE_RADIUS;
                         nearbyY++) // Tick nearby chunks
                        for (var nearbyX = chunk.CX - Player.ACTIVE_RADIUS;
                             nearbyX <= chunk.CX + Player.ACTIVE_RADIUS;
                             nearbyX++)
                        {
                            var ch = Map.Chunks[nearbyX, nearbyY];
                            if (ch == null ||
                                ch.TickCount == time.TickCount) // Make sure we don't tick the same chunk twice lol
                                continue;

                            ch.Tick(time);
                        }
                }

            ActiveEntities.Update();

            using (TimedLock.Lock(_playerTickLock))
            {
                Parallel.ForEach(Players, kvp =>
                {
                    var plr = kvp.Value;
                    if (plr.Dead)
                        return;

                    plr.Tick(time);
                    _playerTick?.Invoke(plr);
                });
                _playerTick = null;
            }

            ActiveEntities.Clear();
            SearchCache.Clear();
        }

        public void InvokeEntityTick(Entity en)
        {
            OnEntityTick?.Invoke(en);
        }

        public bool IsPassable(double x, double y, bool spawning = false, bool bypassNoWalk = false)
        {
            var x_ = (int)x;
            var y_ = (int)y;

            if (!Map.Contains(x_, y_))
                return false;

            var tile = Map[x_, y_];
            if (tile.TileDesc.NoWalk && !bypassNoWalk)
                return false;

            if (tile.ObjectType == 0 || tile.Object == null || tile.Object.Dead)
                return true;

            return !tile.Object.Desc.FullOccupy && !tile.Object.Desc.EnemyOccupySquare &&
                   (!spawning || !tile.Object.Desc.OccupySquare);
        }

        public bool IsOccupied(double x, double y)
        {
            var x_ = (int)x;
            var y_ = (int)y;

            if (!Map.Contains(x_, y_))
                return true;

            var tile = Map[x_, y_];
            if (tile.Object == null || tile.Object.Dead)
                return false;

            return tile.Object.Desc.FullOccupy || tile.Object.Desc.OccupySquare;
        }

        public Entity SpawnSetPiece(string spName, int spawnX, int spawnY, int mapIndex = -1, string eventId = null, bool center = false)
        {
            if (spawnX < 0 || spawnY < 0 || spawnX > Map.Width || spawnY > Map.Height)
                return null;

            if (!WorldLibrary.MapDatas.TryGetValue(spName, out var setpiece))
            {
                _log.Error($"Invalid setpiece: {spName}");
                return null;
            }

            Entity ret = null;
            var map = mapIndex == -1 ? setpiece.RandomElement() : setpiece[mapIndex];

            if (center)
            {
                spawnX -= map.Width / 2;
                spawnY -= map.Height / 2;
            }

            for (var spY = 0; spY < map.Height; spY++)
                for (var spX = 0; spX < map.Width; spX++)
                {
                    var x = spawnX + spX;
                    var y = spawnY + spY;
                    if (x < 0 || y < 0 || x > Map.Width || y > Map.Height)
                        continue;

                    var tile = Map[x, y];
                    var spTile = map.Tiles[spX, spY];
                    if (spTile.GroundType != 255)
                    {
                        tile.Object?.TryLeaveWorld();
                        tile.Update(spTile);
                    }

                    if (spTile.ObjectType != 0xff && spTile.ObjectType != 0)
                    {
                        tile.Object?.TryLeaveWorld();

                        var entity = Entity.Resolve(spTile.ObjectType);
                        if (entity.Desc.Static)
                        {
                            if (entity.Desc.BlocksSight)
                                tile.BlocksSight = true;
                            tile.Object = entity;
                        }

                        entity.Move(x + 0.5f, y + 0.5f);
                        entity.EnterWorld(this);

                        if (!string.IsNullOrEmpty(eventId) && eventId == entity.Desc.ObjectId)
                            ret = entity;
                    }

                    if (tile.Region != TileRegion.None)
                    {
                        var pos = new WorldPosData() { X = x, Y = y };
                        Map.Regions[tile.Region].Add(pos);
                    }

                    BroadcastAll(plr => plr.TileUpdate(tile));
                }

            return ret;
        }

        public void BroadcastAll(Action<Player> act)
        {
            using (TimedLock.Lock(_playerTickLock))
                _playerTick += act;
        }

        public void Delete()
        {
            Active = false;
            Deleted = true; // The actual deletion of the world is performed in Tick
        }

        protected virtual void OnDelete()
        {
            foreach (var plr in Players.Values)
                plr.User.ReconnectTo(NEXUS_ID);

            foreach (var en in Entities.Values)
                en.TryLeaveWorld();
        }

        public virtual void Dispose()
        {
            Disposed = true;
            ActiveEntities.Clear();
            Entities.Clear();
            Characters.Clear();
            Enemies.Clear();
            Players.Clear();
            Quests.Clear();
        }

        public Player GetPlayerById(int id)
        {
            if (Players.TryGetValue(id, out var plr))
                return plr;
            return null;
        }

        public IEnumerable<Player> GetAllPlayersWithin(float x, float y, float maxRadius, Predicate<Player> cond = null, float minRadius = 0)
        {
            var cx = (int)(x / ChunkMap.CHUNK_SIZE);
            var cy = (int)(y / ChunkMap.CHUNK_SIZE);
            if (Map.Chunks[cx, cy] == null)
                yield break;

            var maxRadSqr = maxRadius * maxRadius;
            var minRadSqr = minRadius * minRadius;
            var cRadius = (int)Math.Ceiling(maxRadius / ChunkMap.CHUNK_SIZE);
            for (var i = -cRadius; i <= cRadius; i++)
            {
                var cY = cy + i;
                for (var j = -cRadius; j <= cRadius; j++)
                {
                    var cX = cx + j;
                    var chunk = Map.Chunks[cX, cY];
                    if (chunk == null)
                        continue;

                    foreach (var kvp in chunk.Players)
                    {
                        var plr = kvp.Value;
                        var distSqr = plr.DistSqr(x, y);
                        if (distSqr <= maxRadSqr && distSqr > minRadSqr && (cond == null || cond(plr)))
                            yield return plr;
                    }
                }
            }
        }

        public bool IsPlayerWithin(float x, float y, float radius)
        {
            return GetAllPlayersWithin(x, y, radius).Any();
        }

        public IEnumerable<Entity> GetEntitiesWithin(float x, float y, float radius, Predicate<Entity> pred = null)
        {
            var cx = (int)(x / ChunkMap.CHUNK_SIZE);
            var cy = (int)(y / ChunkMap.CHUNK_SIZE);
            if (Map.Chunks[cx, cy] == null)
                yield break;

            var radSqr = radius * radius;
            var cRadius = (int)Math.Ceiling(radius / ChunkMap.CHUNK_SIZE);
            for (var i = -cRadius; i <= cRadius; i++)
            {
                var cY = cy + i;
                for (var j = -cRadius; j <= cRadius; j++)
                {
                    var cX = cx + j;
                    var chunk = Map.Chunks[cX, cY];
                    if (chunk == null)
                        continue;

                    foreach (var kvp in chunk.Entities)
                    {
                        var en = kvp.Value;
                        if ((pred == null || pred.Invoke(en)) && en.DistSqr(x, y) <= radSqr)
                            yield return en;
                    }
                }
            }
        }

        public IEnumerable<Character> GetEnemiesWithin(float x, float y, float radius)
        {
            var cx = (int)(x / ChunkMap.CHUNK_SIZE);
            var cy = (int)(y / ChunkMap.CHUNK_SIZE);
            if (Map.Chunks[cx, cy] == null)
                yield break;

            var radSqr = radius * radius;
            var cRadius = (int)Math.Ceiling(radius / ChunkMap.CHUNK_SIZE);
            for (var i = -cRadius; i <= cRadius; i++)
            {
                var cY = cy + i;
                for (var j = -cRadius; j <= cRadius; j++)
                {
                    var cX = cx + j;
                    var chunk = Map.Chunks[cX, cY];
                    if (chunk == null)
                        continue;

                    foreach (var kvp in chunk.Entities)
                    {
                        var en = kvp.Value;
                        if (en is Character chr && chr.DistSqr(x, y) <= radSqr)
                            yield return chr;
                    }
                }
            }
        }

        public IEnumerable<Character> GetEnemiesWithBehavior<T>(Character owner, float radius)
            where T : EntityBehavior
        {
            return GetEnemiesWithBehavior<T>(owner.Position.X, owner.Position.Y, radius);
        }

        public IEnumerable<Character> GetEnemiesWithBehavior<T>(float x, float y, float radius)
            where T : EntityBehavior
        {
            var enemiesWithin = GetEnemiesWithin(x, y, radius);
            if (!enemiesWithin.Any())
                return enemiesWithin;

            return enemiesWithin.Where(ent => ent.GetBehavior() is T);
        }

        public IEnumerable<Character> GetEnemiesByName(string name, float x, float y, float radius)
        {
            var query = new SearchQuery(name, new IntPoint((int)x, (int)y), radius, 0);
            if (SearchCache.TryGetValue(query, out var result))
                return result.Entities;

            var enemiesWithin = GetEnemiesWithin(x, y, radius);
            if (name == null || !enemiesWithin.Any())
            {
                SearchCache.Add(query, new SearchQueryResult(enemiesWithin, null));
                return enemiesWithin;
            }

            return enemiesWithin.Where(ent => ent.Desc.ObjectId != null && ent.Desc.ObjectId == name);
        }

        public IEnumerable<Character> GetEnemiesByName(IEnumerable<string> names, float x, float y, float radius)
        {
            foreach (var name in names)
            {
                var enemies = GetEnemiesByName(name, x, y, radius);
                foreach (var enemy in enemies)
                    yield return enemy;
            }
        }

        public Character GetNearestEnemyByName(string name, float x, float y, float radius)
        {
            var enemies = GetEnemiesByName(name, x, y, radius);
            if (!enemies.Any())
                return null;

            Character nearest = null;
            var minDist = float.MaxValue;
            foreach (var en in enemies)
            {
                if (en.DistSqr(x, y) >= minDist)
                    continue;

                minDist = en.DistSqr(x, y);
                nearest = en;
            }

            return nearest;
        }

        public Entity SpawnEntity(string entityName, Vector2 pos)
        {
            return SpawnEntity(entityName, pos.X, pos.Y);
        }

        public Entity SpawnEntity(string entityName, WorldPosData worldPosData)
        {
            return SpawnEntity(entityName, worldPosData.X, worldPosData.Y);
        }

        public Entity SpawnEntity(string entityName, float x, float y)
        {
            var desc = XmlLibrary.Id2Object(entityName);
            if (desc == null)
                throw new NullReferenceException($"Entity {entityName} not found. Double-check your spelling!");

            var entity = Entity.Resolve(desc.ObjectType);
            entity.Move(x, y);
            entity.EnterWorld(this);
            return entity;
        }

        public void DropLootWithOverflow(float posX, float posY, List<Item> items, Player owner = null)
        {
            if (items.Count > Container.INVENTORY_SIZE)
            {
                var itemsChunks = items.Chunk(Container.INVENTORY_SIZE);
                foreach (var itemChunk in itemsChunks)
                    DropLoot(posX, posY, itemChunk.ToArray(), owner);
            }
            else
            {
                DropLoot(posX, posY, items.ToArray(), owner);
            }
        }

        public void DropLoot(float posX, float posY, Item[] items, Player owner = null)
        {
            var highestBagType = 0;
            foreach (var item in items)
            {
                if (item.BagType > highestBagType)
                    highestBagType = item.BagType;

#warning TODO: Read in rarities on items
                OnLootDrop?.Invoke(new LootDrop(owner?.Name, item.DisplayName, LootDropRarity.All));
            }

            var bagType = EntityInventory.GetBagIdFromType((EntityInventory.BagType)highestBagType);
            CreateContainerAt(posX, posY, items, owner.AccountId, bagType);
        }

        public void CreateContainerAt(float posX, float posY, Item[] items, EntityInventory.BagType bagType,
            int owner = -1)
        {
            CreateContainerAt(posX, posY, items, owner, EntityInventory.GetBagIdFromType(bagType));
        }

        public void CreateContainerAt(float posX, float posY, Item[] items, int owner = -1, ushort bagType = 1280)
        {
            var bag = new Container(bagType, 8, owner);
            bag.Move(posX, posY);
            bag.Inventory.SetItems(items);
            bag.EnterWorld(this);
        }

        public void Taunt(Character host, string taunt)
        {
            BroadcastAll(plr =>
            {
                if (host.DistSqr(plr) < Player.SIGHT_RADIUS_SQR)
                    plr.SendEnemy(host, taunt);
            });
        }

        public Player GetPlayerByName(string name)
        {
            return Players.FirstOrDefault(p => p.Value.Name == name).Value;
        }
    }
}