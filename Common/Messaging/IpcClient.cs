using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Common.Messaging;

public class IpcClient {
    
    public static async Task<IGameServerRpc> ConnectAsync(CancellationToken cancellationToken = default)
    {
        var pipeClient = new NamedPipeClientStream(
            ".", // Localhost
            IpcServer.PIPE_NAME,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        await pipeClient.ConnectAsync(cancellationToken);

        var rpc = JsonRpc.Attach<IGameServerRpc>(pipeClient);

        return rpc;
    }
}