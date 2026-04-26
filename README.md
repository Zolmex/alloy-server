# Alloy Server
An open-source private server backend for RotMG.
The goal of this project is to create a base for new private servers that solves all of the problems current private server sources have: poor server/client performance, awkward/slow workflow, bugs, low stability, difficult to maintain, lack of monitoring tools, and more.
## Getting started
- [.NET 10.0 SDK](https://dotnet.microsoft.com/es-es/download/dotnet/10.0)
- [MySQL](https://dev.mysql.com/downloads/installer/)
- [Docker](https://www.docker.com/) (optional, meant for deployment)
- Recommended IDEs: Visual Studio Community 2026 or IntellIJ Rider.

The server is designed to work out of the box with its C# client counterpart [NotTheLegend/Alloy Client](https://github.com/NotTheLegend/AlloyClient)
### Architecture
The project is built with C# and .NET chosen for its mature ecosystem, strong tooling, and cross-platform support.
The repository consists of 3 main services:
- DbServer. Custom write-back cache layer between Web/Game servers and the persistent storage.
- WebServer. HTTP server that handles out-of-game functionality like account management, API requests, etc.
- GameServer. TCP server that handles real-time gameplay logic, packet processing, world and entity simulation.
### Database Design
- [Entity-Relationship Diagram (draw.io)](./Docs/Entity-Relationship%20Diagram.drawio)
- [Relational Model v3 (PDF)](./Docs/Alloy%20DB%20Relational%20Model%20-%20v3.pdf)
### Contributing
For now, feel free to open issues or PRs.

If you wish to follow the development progress of this project, visit the project the board:
[Project Board](https://github.com/users/Zolmex/projects/7)
## Credits
- [Zolmex](https://github.com/Zolmex)
### Additional credits
- [nekoT](https://github.com/EtichBruh)
- [realm-server](https://github.com/dhojka7/realm-server) - Inspiration and code snippets referenced inside the source
- [NR-Core](https://github.com/cp-nilly/NR-CORE/) - TimedLock and other stuff in Utilities
### Thanks to
Programmers and content developers of Astrum. Without Astrum, this project wouldn't exist.
- [patpot](https://github.com/patpot) - Programmer
- [minuie](https://github.com/minuie) - Programmer
- [Shmitty](https://github.com/Shmitttty) - Artist and programmer
- [Pixyde](https://github.com/Pixyde) - Programmer
- [Nevrine](https://github.com/Nevrinee) - Content
- [Panny](https://github.com/ExtraPanny) - Content
- [Evil](https://github.com/itsEvil) - Programmer
- unwised - Artist
