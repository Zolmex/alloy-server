using System;
using System.Threading.Tasks;
using Common.Game;
using Common.Structs;
using PolyType;
using StreamJsonRpc;

namespace Common.Messaging;

[JsonRpcContract]
[GenerateShape(IncludeMethods = MethodShapeFlags.PublicInstance)]
public partial interface IGameServerRpc {
    Task<bool> GlobalAnnouncement(string from, string message);
    Task<ServerInfo> GetGameServer();
    Task<GameInfoDto?> GetUserInfo(string name, int accountId);
}

[JsonRpcContract]
[GenerateShape(IncludeMethods = MethodShapeFlags.PublicInstance)]
public partial interface IAccountServerRpc {
    Task GameServerConnected(Guid gameServerId);
    Task<GameInfoDto?> GetUserInfo(string name, int accountId);
}

public interface IAccountServerHandler : IAccountServerRpc {
    Guid ServerId { get; set; }
    void Attach(IGameServerRpc proxy);
    void Close();
}