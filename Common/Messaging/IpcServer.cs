using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Common.Utilities;
using StreamJsonRpc;

namespace Common.Messaging;

public class IpcServer {
    public const string PIPE_NAME = "alloy_gameserver_rpc";
    
    private static readonly Logger _log = new Logger(typeof(IpcServer));

    public async Task StartAsync(CancellationToken ct) {
        _log.Info("[RPC] Starting IpcServer...");
        
        while (!ct.IsCancellationRequested)
        {
            // Named Pipe streams in .NET are single-use per connection.
            // Create a new stream for each incoming client.
            var pipeServer = new NamedPipeServerStream(
                PIPE_NAME,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);

            await pipeServer.WaitForConnectionAsync(ct);

            // Handle connection in background so the loop can accept new clients
            _ = HandleClientConnectionAsync(pipeServer, ct);
        }
    }
    
    private async Task HandleClientConnectionAsync(NamedPipeServerStream pipeStream, CancellationToken cancellationToken)
    {
        using (pipeStream)
        {
            // Attach the RPC target to the stream
            var handler = new GameServerRpcHandler();
            var jsonRpc = JsonRpc.Attach(pipeStream, handler);

            Console.WriteLine("[RPC] WebServer connected.");

            // Completion waits until the client disconnects or the pipe breaks
            await jsonRpc.Completion;

            Console.WriteLine("[RPC] WebServer disconnected.");
        }
    }
}