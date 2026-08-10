# AGENTS.md — Alloy Server

This file is the source of truth for any AI coding assistant (or human contributor)
working in this repository. It documents the **strict architectural constraints**,
the **tech stack**, and the **design patterns** that must be respected by every
change. When in doubt, the rules below override general C#/.NET conventions.

> **Scope:** These rules apply to the active services — `Common/`, `WebServer/`,
> and `GameServer/`. The `GameServerOld/` project is a legacy reference kept only
> for porting behavior scripts and packet handlers; **do not extend it**. New work
> targets the modern `GameServer/` (ECS-style Managers under
> `GameServer/Game/Entities/Systems/`).

---

## 1. Language & Stack

- **Language:** C# on **.NET 10** (`TargetFramework=net10.0`, `LangVersion=Latest`).
- **SDK:** Pinned via `global.json` (`version: 10.0.0`, `rollForward: latestMajor`,
  `allowPrerelease: true`).
- **Solution:** `RealmServer.sln` contains four projects:
  - `Common/` — shared library (DB client, IPC, models, XML/world resources, utils).
  - `WebServer/` — HTTP front-end + IPC hub.
  - `GameServer/` — real-time TCP simulation, the new ECS-based server.
  - `GameServerOld/` — legacy reference server (read-only intent).
- **Nullable:** disabled (`<Nullable>disable</Nullable>`). Reference types are **not**
  annotated; do not introduce `?` annotations or `ArgumentNullException` guards in
  a style inconsistent with the rest of the file.
- **Implicit usings:** enabled only on `GameServer`. `Common` and `WebServer` use
  explicit `using` directives. Match the file you are editing.
- **Culture:** Both entrypoints force `Thread.CurrentThread.CurrentCulture =
  CultureInfo.InvariantCulture`. Never use culture-dependent parsing/formatting.
- **Key NuGet packages (see `Common.csproj`):**
  - `LiteDB` 5.0.21 — persistence.
  - `StreamJsonRpc` 2.25.29 — IPC RPC.
  - `Newtonsoft.Json` 13.0.3 — HTTP serialization.
  - `Microsoft.Extensions.DependencyInjection.Abstractions` 10.0.1.
  - `Ionic.Zlib.Core` — map/stream compression.
- **Build / run:**
  ```pwsh
  dotnet build RealmServer.sln
  dotnet run --project WebServer
  dotnet run --project GameServer
  ```
  `WebServer` must be started first — it owns the IPC pipe server.

---

## 2. Solution Layout (where things live)

```
Common/
  Database/
    DbClient.cs          # static gateway to LiteDB collections + account/character APIs
    DbWriter.cs          # per-type Channels-based write-behind worker
    DbClientOld.cs       # legacy MySQL/DbServer client (do not extend)
    Models/              # POCO database models (Account, Character, Guild, ...)
  Messaging/
    IpcServer.cs         # WebServer-side pipe hub
    IpcClient.cs         # GameServer-side pipe client
    Proxies.cs           # [JsonRpcContract] RPC interfaces (IGameServerRpc, IWebServerRpc)
  Resources/
    Xml/                 # game XML descriptors + XmlLibrary
    World/               # .jm/.wmap maps + WorldLibrary
    Config/              # server config XML (engineConfig, databaseConfig, ...)
  Structs/               # wire-level structs (ObjectData, WorldPosData, ServerInfo, ...)
  Utilities/             # Logger, TimedLock, EnumUtils, MathUtils, etc.
WebServer/
  Handlers/              # one file per HTTP route (Account/, Char/, Guild/, Legends/, ...)
  Messaging/
    WebServerRpcHandler.cs   # IWebServerHandler impl + disconnect cleanup
GameServer/
  Game/
    Entities/
      Systems/           # ECS-style Managers (EntityManager, EntityStatsManager, ...)
      Components/        # EntityStats, EntityInventory, PlayerChat, ...
      Behaviors/         # State/Action/Transition behavior engine + per-boss libraries
    Network/             # TCP socket server + packet messaging (Incoming/Outgoing/)
    Worlds/              # World, ChunkMap, Realm/Nexus/Vault logic
  Messaging/
    GameServerRpcHandler.cs  # IGameServerRpc impl
GameServerOld/           # legacy — reference only
```

---

## 3. Database Architecture (LiteDB)

The persistence layer is **LiteDB** accessed directly by both `WebServer` and every
`GameServer` instance. There is **no separate database middleware server**.

### 3.1 Connection mode

- Both services open the **same LiteDB file** in **`ConnectionType.Shared`** mode:

  ```csharp
  // Common/Database/DbClient.cs
  var connectionString = new ConnectionString {
      Filename = dbFilePath,
      Connection  = ConnectionType.Shared
  };
  DbCon = new LiteDatabase(connectionString);
  ```

- `WebServer/Program.cs` and `GameServer/Program.cs` both call
  `DbClient.Load(DatabaseConfig.Config.DbFile)` at startup and `DbClient.Dispose()`
  on shutdown (GameServer wires this into `ProcessExit`).
- Because Shared mode serializes writes inside LiteDB itself, the cross-process
  write contention is handled **at the application layer** via the Supervisor
  Pattern (§6), not by a DB middleware.

### 3.2 Models are plain POCOs

- Database models live in `Common/Database/Models/` and **must be plain C# classes**.
- LiteDB maps the `Id` property (or any field/name ending in `id`/`_id`
  case-insensitively) as the document primary key automatically. **Do not** inherit
  from `BsonDocument`, do not add `[BsonId]` attributes, and do not store
  `BsonValue`-typed fields on models. POCOs are reflected directly:

  ```csharp
  // Correct — Common/Database/Models/Account.cs
  public class Account {
      public int Id { get; set; }     // → LiteDB _id (auto-mapped by name)
      public Guid LockOwner { get; set; }
      public string Name { get; set; }
      public List<Character> Characters { get; set; } = [];
  }
  ```

- **Why:** inheriting `BsonDocument` (or returning Bson types from the entity layer)
  produces large transient allocations each tick → heavy Gen-2 GC pressure on the
  60-tick loop. Keeping models as plain reference types lets the write-behind
  worker Upsert the same instance object the simulation holds.

### 3.3 Time properties — always UTC

- Any `DateTime` property that represents a wall-clock moment must be assigned via
  `DateTime.UtcNow`:

  ```csharp
  acc.CreatedAt   = DateTime.UtcNow;
  mute.ExpiresAt  = DateTime.UtcNow.AddHours(1);
  ```

- **Do not use `DateTime.Now`.** (A historical violation exists in
  `Account.Guest` using `DateTime.Now` — treat that as a bug, not a precedent.)
  Storing local time makes cross-server comparisons and mute/ban expiry
  comparisons unreliable.

### 3.4 Embedded vs. referenced data

Decide collection ownership per *access pattern*, never per "clean ER diagram":

| Pattern                                                                   | Where it lives                              | Examples                                   |
|---------------------------------------------------------------------------|---------------------------------------------|--------------------------------------------|
| Read **only** with the parent, no need for global unique ID or cross-query | **Embed** as a nested POCO/List on the parent | `Account.Characters`, `Account.VaultChests`, `Character.Stats`, `Character.ItemTypes[]` |
| Needs a **global unique ID** or must be queried independently             | **Separate collection** in `DbClient`       | `Guilds` (own collection, referenced by `Account.GuildId`); `Mutes`, `Bans`, `Logins` (each their own collection so moderators can query/maintain them without loading accounts) |

- When embedding, prefer `List<T>` over arrays for sub-document collections that can
  grow (`Characters`, `VaultChests`). Arrays are acceptable for fixed-size slots
  (e.g. `Character.ItemTypes` / `ItemDatas`, both length 20).
- Index every field you query by, in `DbClient.Load`:

  ```csharp
  Accounts.EnsureIndex(x => x.Name, unique: true);
  Accounts.EnsureIndex(x => x.GuildId);
  Mutes.EnsureIndex(x => x.TargetAccId);
  ```

### 3.5 CRUD entrypoint — `DbClient`

- All persistence goes through the static `Common.Database.DbClient`. Reads use the
  `ILiteCollection<T>` LINQ API directly (`FindById`, `FindOne`, `Exists`, `Count`).
- Writes **never** call `Upsert` inline. They call `DbClient.FlushAsync(model)`
  which enqueues into the write-behind channel (§4).
- Soft-delete semantics: do not physically remove documents. Mark a flag
  (`Character.IsDeleted`, etc.) and filter it out on read.

---

## 4. GameServer Performance — Write-Behind Channel

The game loop runs at **60 ticks per second** (`GameLogic.Run(config.MsPT)`,
`MsPT ≈ 16.66`). It **must never block on synchronous disk I/O**.

### 4.1 The `DbWriter<T>` worker

```csharp
// Common/Database/DbWriter.cs
public static class DbWriter<T> where T : class {
    private static readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

    public static void Init() =>
        _processingTask = Task.Factory.StartNew(
            ProcessAsync, CancellationToken.None,
            TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

    private static async Task ProcessAsync() {
        await foreach (var item in _channel.Reader.ReadAllAsync()) {
            try { DbClient.DbCon.GetCollection<T>().Upsert(item); }
            catch (Exception ex) { Console.WriteLine($"DB write failed: {ex.Message}"); }
        }
    }

    public static async Task WriteAsync(T model) => await _channel.Writer.WriteAsync(model);
    public static async Task StopAsync()        { _channel.Writer.Complete(); await _processingTask; }
}
```

- One worker **per model type** (`DbWriter<Account>.Init()`, `DbWriter<Guild>.Init()`, …),
  initialized from `DbClient.Load`.
- Producers call `DbClient.FlushAsync(model)` → `DbWriter<T>.WriteAsync(model)`.
  Use the **non-blocking** path: the channel is unbounded, so `WriteAsync` completes
  synchronously without yielding, but it **never touches disk** on the caller's
  thread. The synchronous `LiteDatabase.Upsert` happens off-thread inside
  `ProcessAsync`.
- **Do not** introduce `BoundedChannelOptions`, synchronous `WriteAsync(...).Wait()`,
  or direct `Upsert` calls from the tick loop. If you need backpressure, log a
  metric — don't block the tick.
- On shutdown, call `DbClient.Dispose()` (GameServer does this in `ProcessExit`).
  It calls `DbWriter<T>.StopAsync()` for every type, which `Complete()`s the writer
  and `await`s the draining worker so no enqueued writes are lost.

### 4.2 Rules for the tick loop

- The 60-tick path may: read from memory, mutate POCO fields, enqueue via
  `FlushAsync`, send packets, schedule behaviors.
- The 60-tick path **may not**: call `Upsert`, `Find*` against a hot collection
  inside a per-entity loop, `Thread.Sleep`, `.Result`/`.Wait()` on a Task, allocate
  large arrays per tick.
- Prefer the existing pooled/sparse collections in `Common/Utilities/Collections/`
  (`PooledList`, `SparseSet`, `BitArray2D`) for per-tick scratch.

---

## 5. Inter-Process Communication (IPC)

`WebServer` <-> `GameServer` communication is **StreamJsonRpc** over **Named Pipes**
(Windows default; same API works over Unix Domain Sockets on Linux).

### 5.1 Hub-and-Spoke

- `WebServer` is the **hub**. It hosts a single long-running accept loop in
  `Common/Messaging/IpcServer.cs`:

  ```csharp
  public const string PIPE_NAME = "alloy_rpc";

  while (!ct.IsCancellationRequested) {
      var pipeServer = new NamedPipeServerStream(
          PIPE_NAME,
          PipeDirection.InOut,
          NamedPipeServerStream.MaxAllowedServerInstances,  // many GameServers
          PipeTransmissionMode.Byte,
          PipeOptions.Asynchronous);
      await pipeServer.WaitForConnectionAsync(ct);
      _ = HandleClientConnectionAsync<THandler>(pipeServer, ct);   // per-client task
  }
  ```

  > Note: the central pipe name in-tree is **`alloy_rpc`** (still a single central
  > pipe; treat any reference to `alloy_central_pipe` as a synonym for the
  > hub endpoint and keep the constant centralized in `IpcServer.PIPE_NAME`).
  > Do **not** hard-code the pipe name elsewhere.

- Each `GameServer` is a **spoke**. On startup it connects as a client
  (`Common/Messaging/IpcClient.cs`) and registers itself:

  ```csharp
  // GameServer/Program.cs
  (_, WebServerRpc) = await IpcClient.ConnectAsync(new GameServerRpcHandler());
  await WebServerRpc.GameServerConnected(Program.Guid);  // Program.Guid = Guid.NewGuid()
  ```

### 5.2 RPC contracts

- Interfaces live in `Common/Messaging/Proxies.cs` and are annotated with
  `[JsonRpcContract]` + `[GenerateShape(...)]` (PolyShape source generator) so
  StreamJsonRpc can serialize them without reflection.

  ```csharp
  [JsonRpcContract]
  public partial interface IGameServerRpc {           // WebServer → GameServer
      Task<bool> GlobalAnnouncement(string from, string message);
      Task<ServerInfo> GetGameServer();
  }

  [JsonRpcContract]
  public partial interface IWebServerRpc {              // GameServer → WebServer
      Task GameServerConnected(Guid gameServerId);
  }

  public interface IWebServerHandler : IWebServerRpc {
      Guid ServerId { get; set; }
      void Attach(IGameServerRpc proxy);
      void Close();   // called by IpcServer on disconnect
  }
  ```

- Adding an RPC method:
  1. Declare it on `IGameServerRpc` **or** `IWebServerRpc` (keep the direction
     consistent with the caller).
  2. Implement it on `GameServerRpcHandler` (GameServer side) or
     `WebServerRpcHandler` (WebServer side).
  3. Keep DTOs in `Common/Structs/`, with `[GenerateShape]`-friendly primitives.
  4. **Never** pass LiteDB POCO instances over RPC. Project to a DTO first
     (`ServerInfo`, `AccountList`, etc.).

### 5.3 Connection lifecycle

- `WebServer` keeps `ConcurrentDictionary<Guid, IGameServerRpc> Clients` mapping
  each connected `GameServer.Guid` → its RPC proxy.
- `JsonRpc.Completion` completes when the pipe breaks (client crash, OS pipe
  severance, clean disconnect). After `Completion`, the per-client task calls
  `handler.Close()` which:
  1. Removes the entry from `IpcServer.Clients`.
  2. Runs the **Supervisor cleanup** (§6): bulk-clears `LockOwner` for every
     account locked to that `ServerId`.

---

## 6. State & Concurrency — The Supervisor Pattern

`WebServer` is the **supervisor**: it is the single authority for which GameServer
owns which account. `GameServer` instances do **not** coordinate with each other.

### 6.1 Account locking

- `Account.LockOwner : Guid` — `Guid.Empty` means free; otherwise it holds the
  `Program.Guid` of the GameServer that owns the session.
- Lock is acquired in `DbClient.VerifyAccount`:

  ```csharp
  if (acc.LockOwner != gameServerGuid) {
      if (acc.LockOwner != Guid.Empty)
          return (null, VerifyStatus.AccountInUse);   // already locked elsewhere
      acc.LockOwner = gameServerGuid;                // acquire
  }
  ```

- Rule: **a player account is locked to exactly one GameServer for the duration
  of a session.** This is what prevents two GameServers from concurrently mutating
  the same `Account`/`Character` document in the shared LiteDB file.

### 6.2 Lock release on clean disconnect

- Normal GameServer shutdown should `FlushAsync` state and let the pipe close.
- When the pipe breaks (crash, kill, network drop), the OS severs the stream;
  `JsonRpc.Completion` fires on the WebServer side; `WebServerRpcHandler.Close()`
  runs the cleanup query:

  ```csharp
  // WebServer/Messaging/WebServerRpcHandler.cs
  public void Close() {
      IpcServer.Clients.TryRemove(ServerId, out _);
      DbClient.Accounts.UpdateMany(
          acc => new Account { LockOwner = Guid.Empty },
          acc => acc.LockOwner == ServerId);
  }
  ```

  Every account owned by the dead GameServer is released atomically — no
  GameServer cooperation required, no heartbeat protocol, no grace period. The
  crash *is* the signal.

### 6.3 Concurrency rules

- `IpcServer.Clients` is a `ConcurrentDictionary`. Handlers may be invoked
  concurrently; keep them stateless or use thread-safe state.
- `DbClient` collections are **not** thread-safe for concurrent writes from
  multiple processes beyond what Shared mode serializes. The application-layer
  lock above is what makes writes safe — don't circumvent it by adding a second
  write path (e.g., a background "sweeper" on GameServer that Upserts accounts it
  doesn't own).
- On the GameServer, only the `DbWriter<T>` background worker performs Upserts.
  All other code enqueues. Inside the worker, writes for a given collection are
  naturally serialized (single consumer per `Channel<T>`).
- The WebServer is allowed to read/write LiteDB synchronously on its request
  threads; HTTP handler concurrency is bounded by the `SemaphoreSlim(200)` gate
  in `WebServer/Program.cs`.

---

## 7. XML / World Resource Loading

- Game content (items, objects, projectiles, players, ground) loads from XML in
  `Common/Resources/Xml/Data/` at startup via `XmlLibrary.Load(config.XmlsDir)`.
  Many of these files are linked (`<Link>...</Link>`) from the sibling
  `AlloyClient` repo — see `Common.csproj` `<Content Include="..\..\AlloyClient\...">`.
- Maps live in `Common/Resources/World/Data/` (`.jm` = j-map, `.wmap` = world map)
  and are loaded by `WorldLibrary.Load(config.WorldsDir)`. Each map has a sibling
  JSON config in `World/Data/Config/` describing realm/dungeon metadata.
- Merchants are declared in `Common/Resources/Xml/Merchants/NexusMerchants.xml`
  and loaded by `MerchantsLibrary.Load(config.MerchantsDir)`.
- **Rule:** Resources are loaded **once** at startup and treated as read-only for
  the lifetime of the process. Do not reload them at runtime; if a config must be
  refreshed, restart the service.

---

## 8. Logging & Diagnostics

- Use `Common.Utilities.Logger` (`new Logger(typeof(T))`). Levels:
  `Info`, `Debug`, `Warn`, `Error`, `Fatal`.
- Logs are written to `./logs/{ProcessName}/{level}/...` and to the console under
  a console lock. Output is padded to `PADDING = 18` chars for the logger name.
- Do **not** use `Console.WriteLine` from production code (the `DbWriter` catch
  block is an existing violation — prefer `_log.Error` when you touch it).
- `EasyTimer` and `TimedLock` (NR-Core-derived) are the standard tools for
  startup-phase timing and async lock acquisition; prefer them over ad-hoc
  `Stopwatch`/`lock` usage.

---

## 9. Coding Conventions (non-negotiable)

- **No comments** unless the surrounding file is already commented and the comment
  adds decision context (not a restatement of code).
- **No `BsonDocument`/`[BsonId]`/`BsonValue`** on models.
- **No `DateTime.Now`** — `DateTime.UtcNow` only.
- **No inline `Upsert`** on the GameServer.
- **No new `BsonMapper` global customizations** unless discussed; default POCO
  mapping is the contract.
- **No directly-constructed `LiteDatabase`** outside `DbClient.Load`. Everything
  goes through `DbClient`.
- **No passing of POCO persistence models over the RPC boundary** — project to a
  struct in `Common/Structs/`.
- Match existing file style: `ImplicitUsings` on in `GameServer`, off elsewhere;
  `Nullable` disabled everywhere; brace style is Allman; `var` for locals, explicit
  types for fields/properties/parameters.
- MinVer (`<MinVerTagPrefix>v`) generates versions from git tags on `GameServer`.
  `WebServer` keeps a manual `<AssemblyVersion>` — bump it on user-visible
  WebServer changes and keep them in sync conceptually.

---

## 10. Definition of Done for a Change

A change is ready to merge when **all** of the following are true:

- [ ] Builds with `dotnet build RealmServer.sln` on .NET 10.
- [ ] No `BsonDocument`/`DateTime.Now` introduced; UTC everywhere.
- [ ] No new synchronous LiteDB writes on the GameServer tick path.
- [ ] Any new RPC method has both the contract (Proxies.cs) and the matching
      handler implementation; no POCO leaked across the wire.
- [ ] New DB fields are indexed if they are queried by; collections follow §3.4
      (embed sub-documents vs. separate collection).
- [ ] New models are POCOs in `Common/Database/Models/`, registered in
      `DbClient.Load` (collection + `DbWriter<T>.Init()` + index).
- [ ] No project compiles with warnings introduced by the change.
- [ ] If touch `WebServerRpcHandler.Close()` or `Account.LockOwner` semantics,
      re-traced §6 (Supervisor Pattern) by hand.
