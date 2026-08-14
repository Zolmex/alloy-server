using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Common.Utilities;
using StreamJsonRpc;

namespace Common.Messaging;

public class IpcServer {
    public const string PIPE_NAME = "alloy_rpc";
    
    private static readonly Logger _log = new Logger(typeof(IpcServer));
    
    public static readonly ConcurrentDictionary<Guid, IGameServerRpc> Clients = new();

    public static async Task StartAsync<THandler>(CancellationToken ct = default) where THandler : IAccountServerHandler, new() {
        _log.Info($"[RPC] Starting IpcServer at pipe '{PIPE_NAME}'...");
        
        while (!ct.IsCancellationRequested)
        {
            // Named Pipe streams are single-use per connection.
            // Create a new stream for each incoming client.
            var pipeServer = new NamedPipeServerStream(
                PIPE_NAME,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);

            await pipeServer.WaitForConnectionAsync(ct);

            // Handle connection in background so the loop can accept new clients
            _ = HandleClientConnectionAsync<THandler>(pipeServer, ct);
        }
    }
    
    private static async Task HandleClientConnectionAsync<THandler>(NamedPipeServerStream pipeStream, CancellationToken cancellationToken) where THandler : IAccountServerHandler, new() {
        await using (pipeStream)
        {
            // Bind for incoming calls from GameServer
            var handler = new THandler();
            var jsonRpc = new JsonRpc(pipeStream);
            jsonRpc.AddLocalRpcTarget<IAccountServerRpc>(handler, null);
            
            var gameServerProxy = jsonRpc.Attach<IGameServerRpc>();
            handler.Attach(gameServerProxy);

            jsonRpc.StartListening();

            // Completion waits until the client disconnects or the pipe breaks
            await jsonRpc.Completion;

            handler.Close();
            _log.Info($"[RPC] GameServer {handler.ServerId} has disconnected.");
        }
    }
}