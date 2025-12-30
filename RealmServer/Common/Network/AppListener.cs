using Common.Utilities;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Network;

public class AppListener
{
    private static readonly Logger _log = new(typeof(AppListener));
    
    private readonly Socket _socket;

    public readonly Dictionary<string, AppConnection> AppConnections = new();
    
    public AppListener(int port)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, port));
        _socket.Listen();
    }

    public async Task Start(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var skt = await _socket.AcceptAsync(ct);
            var ip = ((IPEndPoint)skt.RemoteEndPoint)!.Address.ToString();
            _log.Info($"Received App connection from {ip}");

            var con = new AppConnection();
            con.Setup(skt);
            con.Start(Assembly.GetEntryAssembly()!.GetName().Name);
            AppConnections.Add(ip, con);
        }
    }
}