using System;
using System.Threading.Tasks;
using Common.Structs;
using PolyType;
using StreamJsonRpc;

namespace Common.Messaging;

[JsonRpcContract]
[GenerateShape(IncludeMethods = MethodShapeFlags.PublicInstance)]
public partial interface IGameServerRpc {
    Task<bool> GlobalAnnouncement(string from, string message);
    Task<ServerInfo> GetGameServer();
}

[JsonRpcContract]
[GenerateShape(IncludeMethods = MethodShapeFlags.PublicInstance)]
public partial interface IWebServerRpc {
    Task GameServerConnected(Guid gameServerId);
}

public interface IWebServerHandler : IWebServerRpc {
    Guid ServerId { get; set; }
    void Attach(IGameServerRpc proxy);
    void Close();
}