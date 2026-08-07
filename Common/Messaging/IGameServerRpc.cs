using System.Threading.Tasks;
using PolyType;
using StreamJsonRpc;

namespace Common.Messaging;

[JsonRpcContract]
[GenerateShape(IncludeMethods = MethodShapeFlags.PublicInstance)]
public partial interface IGameServerRpc {
    Task<bool> GlobalAnnouncement(string from, string message);
    Task<GameServerStatus> GetGameServerStatus();
}