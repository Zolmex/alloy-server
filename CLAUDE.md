# CLAUDE.md

This file provides guidance to Claude Code when working with code in this repository.

## Alloy Server

An open-source private server backend for Realm of the Mad God. C# / .NET 10.

## Build and Run

- **Build:** `dotnet build RealmServer.sln`
- **Run a project:** `dotnet run --project <GameServer|DbServer|WebServer>`
- **Build release:** `dotnet build -c Release`

Binaries output to `bin/debug/` or `bin/release/` at the solution root.

## Service Architecture

Three main services communicate over TCP:

- **DbServer** (`DbServer/`) — Write-back cache layer between Web/Game servers and MySQL. Uses EF Core with `IDbContextFactory<AlloyContext>`. Runs as an ASP.NET generic host (`IHostedService`).
- **WebServer** (`WebServer/`) — HTTP server (`HttpListener`) for account management, char operations, and web APIs. No ASP.NET stack; handles requests via `RequestHandler` dispatch.
- **GameServer** (`GameServer/`) — Custom TCP socket server with a real-time game loop for packet processing, world simulation, and entity logic.

All three depend on the **Common** project (`Common/`).

## Inter-Service Communication

- GameServer and WebServer connect to DbServer via `AppConnection` in `Common/Network/` using a custom TCP message protocol.
- Messages implement `IAppMessage` and use request/response matching with `SendAndReceiveAsync<TAck>()`.
- `AppListener` runs in DbServer to accept and track connections from WebServer and GameServer instances.
- `DbClient` (`Common/Database/DbClient.cs`) is the client-side facade for sending DB-bound messages to DbServer.

## GameServer Architecture

### Game Loop

Entry: `GameServer/Program.cs` → `GameLogic.Run(mspt)`.

```
GameLogic.Run(mspt)
  → Update()           // dequeues pending actions, updates all worlds, handles incoming packets + outgoing socket data for all users
  → TickWorlds()       // calls world.Tick() for every active world
```

`RealmTime` tracks elapsed and total time across the loop. `GameLogic.Enqueue(Action)` schedules work to run on the next `Update()` call.

### ECS-like Component System

- `Entity` is a `struct` with an `EntityId` and `ObjectType`. It does not hold component data directly.
- Components are stored in system managers (`ManagerBase<T>` arrays) owned by `World`:
  - `EntityStats`, `EntityBehavior`, `EntityCombat`, `EntityEvents`, `EntityInventory`, `EntityProjectiles`
- Each system manager stores `T[]` component arrays indexed by `EntityId`. This is a SoA-style (struct of arrays) layout.
- Managers are ticked per-world in `World.Tick(ref RealmTime)`.
- Thread-safety assumption: everything happens on the single game loop thread. `RealmManager` uses immutable dictionaries (`ImmutableDictionary`) for safe cross-thread reads of worlds/users.

### World System

`World` is the simulation container. A `World`:
- owns `EntityManager`, `ProjectileManager`, and all component system managers
- has a `WorldMap` loaded from `.jm`/`.wmap` files
- has subtypes in `GameServer/Game/Worlds/Logic/` (e.g., `Nexus`, `Vault`)

Worlds are created via `RealmManager.AddWorld()` and tracked in `RealmManager.Worlds`.

### Entity Lifecycle

1. `World.EnterWorld(ref Entity)` assigns an `EntityId` and allocates a slot in the `Entities` manager.
2. `Entity.Init(world, pos)` sets up component state.
3. `World.RemoveEntity(EntityId)` enqueues removal; cleanup defers so it doesn't happen mid-tick.

### Networking

- `SocketServer` (`GameServer/Game/Network/SocketServer.cs`) is a custom async TCP acceptor using `SocketAsyncEventArgs`.
- Accepted sockets become `User`s, which have a `User.Network` (`NetworkHandler`) for reading/writing game packets.
- Packets defined in `GameServer/Game/Network/Messaging/Incoming/` and `Outgoing/`.

### Resource Loading

At startup, `Program.cs` calls:
- `XmlLibrary.Load(dir)` — loads all `.xml` files into `ObjectDescs`, `PlayerDescs`, `ItemDescs`, `TileDescs`, etc.
- `MerchantsLibrary.Load(dir)` — loads merchant data
- `WorldLibrary.Load(dir)` — loads `.jm`/`.wmap` and `.json` world configs into `MapDatas` and `WorldConfigs`
- `BehaviorLibrary.Load()` — loads entity behaviors
- `CommandManager.Load()` — loads chat commands

## DbServer Architecture

- Runs via `IHostedService` (`NetworkService`). `AppListener` accepts TCP connections from GameServer and WebServer.
- Handlers live in `DbServer/Messaging/`. Each handles an `IAppMessage` type, performs DB operations via `IDbContextFactory<AlloyContext>`, and returns an `IAppMessageAck`.
- EF Core migrations are in `DbServer/Migrations/`.

## WebServer Architecture

- Simple `HttpListener`-based request router.
- Handlers in `WebServer/Handlers/` (e.g., `Account/`, `Char/`, `Guild/`).
- Dispatches incoming request paths via `RequestHandler`.
- Also connects to DbServer using `DbClient`.

## Common / Shared

### Networking (`Common/Network/`)
- `AppConnection` — TCP connection with message framing, send/receive channel, and ack matching.
- `AppListener` — accepts TCP connections and tracks them by name.
- `SpanReader`/`SpanWriter` — low-level binary serialization used by network code.

### Database (`Common/Database/`)
- `DbClient` — facade for sending RPC-like messages to DbServer.
- `DbModel` — base class for models that can be flushed back to the database via `FlushAsync`.
- Models under `Common/Database/Models/` are shared between all servers.

### Resources (`Common/Resources/`)
- `XmlLibrary` — parses XML game data (items, enemies, objects, tiles, etc.) into descriptor structs.
- `WorldLibrary` — parses world map data.
- Config classes (e.g. `GameServerConfig`, `AppEngineConfig`, `DatabaseConfig`) load XML config files at startup.

### Utilities
- `Logger` — custom logging with `LogLevel`.
- `EasyTimer` — measures and logs elapsed time for startup phases.
- `BitMask256` — 256-bit flag set used for entity stat tracking.

## Configuration

- **.NET version:** 10.0 (see `global.json`)
- **Nullable settings vary:**
  - `DbServer`: `enable`
  - `Common`: `enable` (via `LangVersion` = Latest, but no explicit nullable — default is `enable` in .NET 6+)
  - `WebServer`, `GameServer`: `disable`
- `Common` uses `AllowUnsafeBlocks` for `Span`-based network code.
- `GameServer` includes Roslyn (`Microsoft.CodeAnalysis`) for runtime compilation and `MinVer` for versioning.
- `Common.csproj` has many linked files from an external `AlloyClient` repo (via `Content Include="../../AlloyClient"`). This is not a git submodule; the files must exist at that relative path.

## Important Notes

- `GameServerOld/` is a legacy project kept in the solution but is not the active GameServer.
- `Docker` support is only for MySQL (via `docker-compose.yml`). The server binaries are not containerized yet.
- When modifying network messages:
  1. Add the `IAppMessage` / `IAppMessageAck` in `Common/Network/Messaging/Impl/`.
  2. Add the handler in `DbServer/Messaging/`.
  3. Expose it via `DbClient` if called from GameServer or WebServer (for web -> DB flow WebServer uses `DbClient` differently, but the message still needs a handler in DbServer).
