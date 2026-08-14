<p align="center">
  <img src="./Imagotype.png" alt="Alloy" height="200">
</p>

<p align="center">
Alloy Server is a Realm of the Mad God server emulator for the private server community. Meant for anyone who wants to play around with a few friends, learn how it works, and/or maybe start their own game project in the private server space.
</p>

## Introduction
Alloy Server is the required backend for the [Alloy Client](https://github.com/NotTheLegend/AlloyClient), these projects are meant to run together out of the box. The pillars that guide the design decisions behind every component in this project are **performance, simplicity, and maintainability**.

### How it runs
The server is designed to run on a single machine. There are two services: **AccountServer and GameServer**. The AccountServer listens to HTTP requests regarding account information or out-of-game operations. Anything that happens in-game is handled by the GameServer, managing the worlds, entities, and all gameplay logic. There can be multiple instances of GameServer running in the same machine, each one listening to connections on a different port.

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Tools used for development:
   - Visual Studio / JetBrains Rider
   - GitHub Desktop (recommended)
   - Notepad++ (optional)

### Build & Run
Alloy is designed to **work out of the box** with its companion C# client,
[NotTheLegend/AlloyClient](https://github.com/NotTheLegend/AlloyClient).

```pwsh
# 1. Build the whole solution
dotnet build RealmServer.sln

# 2. Start the Account Server
dotnet run --project AccountServer

# 3. In another terminal, start the GameServer
dotnet run --project GameServer
```

The first time you run the server an `alloy.db` file is created, this is your database storage.

### Default ports
| Service     | Port  | Config file                         |
|-------------|-------|-------------------------------------|
| AccountServer   | 8080  | `Common/Resources/Config/Data/appEngineConfig.xml` |
| GameServer  | 2050  | `Common/Resources/Config/Data/gameServerConfig.xml` |

Tune capacity (`MaxPlayers`, `TPS`, `RealmCount`) in `gameServerConfig.xml`.

### Making an admin account
[TODO]

## Contributing

Small contributions are always welcome. I'm not gonna read +50 changed files PRs so don't even try.

- Issues: Report a bug, mistake, or concern with the project or how the project works.
- Pull requests: Bug fixes, optimizations, documentation updates. Always target `dev` branch.
- If you're a community developer curious about how things work, the
  [`AGENTS.md`](./AGENTS.md) file has a very comprehensive description of the project's modules and technical details of its architecture.

## Credits

### Lead Developer
- [Zolmex](https://github.com/Zolmex)

### With help from
- [nekoT](https://github.com/EtichBruh)
- [realm-server](https://github.com/dhojka7/realm-server) — inspiration and code
  snippets referenced inside the source.
- [NR-Core](https://github.com/cp-nilly/NR-CORE/) — `TimedLock` and other utilities.

### Thanks to
Programmers and content developers of **Astrum**. Without Astrum, this project
wouldn't exist.

- [patpot](https://github.com/patpot) — Programmer
- [minuie](https://github.com/minuie) — Programmer
- [Shmitty](https://github.com/Shmitttty) — Artist and programmer
- [Pixyde](https://github.com/Pixyde) — Programmer
- [Nevrine](https://github.com/Nevrinee) — Content
- [Panny](https://github.com/ExtraPanny) — Content
- [Evil](https://github.com/itsEvil) — Programmer
- **unwised** — Artist

## License

Alloy is released under the **MIT License**. See [`LICENSE`](./LICENSE) for the
full text.
