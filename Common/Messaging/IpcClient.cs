using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Common.Messaging;

public static class IpcClient {
    
    public static async Task<(JsonRpc Session, IAccountServerRpc ServerProxy)> ConnectAsync(IGameServerRpc localHandler, CancellationToken cancellationToken = default)
    {
        var pipeClient = new NamedPipeClientStream(
            ".", // Localhost
            IpcServer.PIPE_NAME,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        await pipeClient.ConnectAsync(cancellationToken);

        // Listener for incoming calls from IpcServer
        var jsonRpc = JsonRpc.Attach(pipeClient, localHandler);
        
        // Proxy for outgoing calls to IpcServer
        var proxy = jsonRpc.Attach<IAccountServerRpc>();

        return (jsonRpc, proxy);
    }
}