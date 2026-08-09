using System;

namespace Common.Structs;

public readonly record struct ServerInfo(Guid Guid, ServerType Type, long UptimeMs, int PlayerCount);