#region

using Common.Resources.Config;
using Common.Utilities;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

#endregion

namespace GameServer.Game.Network
{
    // TCP socket server
    public static class SocketServer
    {
        private static readonly Logger _log = new(typeof(SocketServer));
        private static ConcurrentFactory<User> _userFactory;
        private static Dictionary<string, int> _ips;
        private static Socket _socket;

        // Start accepting connections
        public static void Start(int port, int maxConnections)
        {
            _ips = new Dictionary<string, int>(); // Used to keep track of how many clients per IP address
            _userFactory = new ConcurrentFactory<User>(maxConnections); // Used to prevent memory leaks

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(3000); // Backlog is the max number of pending connections

            StartAccept();
        }

        private static void StartAccept()
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += ProcessAccept;

            try
            {
                // Socket async methods return false when they complete synchronously, meaning that the Completed
                // event won't be invoked, so we have to manually call the callback ourselves
                if (!_socket.AcceptAsync(args))
                    ProcessAccept(null, args);
            }
            catch
            { }
        }

        private static void ProcessAccept(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success) // If error, recycle and continue
            {
                args.Dispose();
                StartAccept();
                return;
            }

            var skt = args.AcceptSocket;
            var ip = ((IPEndPoint)skt.RemoteEndPoint).Address.ToString();

            var maxClientsReached = false;
            var ipConnected = _ips.TryGetValue(ip, out var clientCount);
            if (ipConnected && clientCount >= GameServerConfig.Config.MaxClientsPerIP)
                maxClientsReached = true;

            if (ip == null || maxClientsReached)
            {
                args.Dispose();
                StartAccept();
                return;
            }

            if (ipConnected)
                _ips[ip] += 1;
            else
                _ips.TryAdd(ip, 1);

            var user = _userFactory.Pop(); // Give the connection a NetClient instance to communicate with
            if (user == null)
            {
                args.Dispose();
                StartAccept();
                return;
            }

            _log.Debug($"Received client @ {ip}");

            user.Setup(ip, skt);

            RealmManager.ConnectUser(user);

            // Recycle the SAEA object
            args.Dispose();
            StartAccept();
        }

        public static void DisconnectUser(User user)
        {
            // Terminate connection with the Socket
            try
            {
                user.Network.Socket.Shutdown(SocketShutdown.Both);
                user.Network.Socket.Close();

                _ips.Remove(user.Network.IP);
            }
            catch
            { }

            // Recycle client instance
            user.Reset();

            _userFactory.Push(user);
        }
    }
}