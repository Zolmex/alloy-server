#region

using Common.Database;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Net.Sockets;
using System.Threading;

#endregion

namespace GameServer.Game;

public enum ConnectionState
{
    Disconnected, // Client is waiting to be used
    Connected, // A connection received is using this instance of NetClient
    Reconnecting, // Client is moving from a world instance to another
    Ready // Client is ready to handle in-game packets
}

public enum DisconnectReason
{
    Unknown,
    UserDisconnect,
    NetworkError,
    Failure,
    Death,
    IllegalAction
}

public class User : IIdentifiable
{
    private static readonly Logger _log = new(typeof(User));
    private static int _nextClientId;

    public int Id { get; set; }
    public NetworkHandler Network { get; }
    public GameInfo GameInfo { get; }

    public ConnectionState State { get; private set; }
    public DbAccount Account { get; private set; }
    public ClientRandom Random { get; private set; }
    public ClientRandom ServerRandom { get; private set; }

    private readonly object _disconnectLock = new();

    public User()
    {
        Id = Interlocked.Increment(ref _nextClientId);
        Network = new NetworkHandler(this);
        GameInfo = new GameInfo(this);
    }

    public void Setup(string ip, Socket socket)
    {
        Network.Setup(ip, socket);
    }

    public void StartNetwork()
    {
        State = ConnectionState.Connected;
        Network.StartReceive();
    }

    public void SetGameInfo(DbAccount acc, uint randomSeed, World world)
    {
        Account = acc;
        RealmManager.UserAccIds.TryAdd(this, acc.AccountId);

        Random = new ClientRandom(randomSeed);
        ServerRandom = new ClientRandom((uint)new Random().Next(1, int.MaxValue));

        GameInfo.SetWorld(world);
    }

    public void Load(DbChar chr, World world)
    {
        State = ConnectionState.Ready;

        GameInfo.Load(chr, world);

        SendPacket(new CreateSuccess(
            GameInfo.Player.Id,
            chr.CharId));
        SendPacket(new AccountList(
            AccountList.Locked,
            Account.LockedIds ?? Array.Empty<int>()));
        SendPacket(new AccountList(
            AccountList.Ignored,
            Account.IgnoredIds ?? Array.Empty<int>()));
    }

    public void Unload(bool reconnect, bool death = false)
    {
        if (reconnect && GameInfo.State != GameState.Playing) // We can only unload when we've loaded in the first place
            return;

        GameInfo.Unload(reconnect, death);
    }

    public void Reset()
    {
        Network.Reset();
        GameInfo.Reset();

        Random = null;
    }

    public void SendFailure(int errorId = Failure.DEFAULT, string message = Failure.DEFAULT_MESSAGE, bool disconnect = true)
    {
        SendPacket(new Failure(errorId, message));
        if (disconnect)
            RealmManager.AddTimedAction(1000, () => Disconnect(message, DisconnectReason.Failure));
    }

    public void Disconnect(string message = null, DisconnectReason reason = DisconnectReason.Unknown)
    {
        if (State == ConnectionState.Disconnected)
            return;

        _log.Debug($"Disconnecting user {Id} ({message}) (Reason:{reason})");

        State = ConnectionState.Disconnected;

        Unload(false, reason == DisconnectReason.Death);

        RealmManager.DisconnectUser(this);
        SocketServer.DisconnectUser(this);
    }

    public void ReconnectTo(World world)
    {
        if (world == null || !world.Active || world.Deleted)
            return;

        if (!world.Initialized)
        {
            world.OnInitialize(() => ReconnectTo(world));
            return;
        }

        State = ConnectionState.Reconnecting;

        Unload(true); // Begin reconnect process, player leaves world and set gamestate to idle

        Reconnect.Write(Network,
            world.Id
        );
    }

    public void ReconnectTo(int worldId)
    {
        if (!RealmManager.Worlds.TryGetValue(worldId, out var world))
            return;

        ReconnectTo(world);
    }
    public void SendPacket<T>(T packet)
        where T : IOutgoingPacket
    {
        var state = Network.SendState;
        var wtr = state.Writer;
        using (TimedLock.Lock(state))
        {
            var begin = state.PacketBegin();

            packet.Write(wtr);

            state.PacketEnd(begin, T.PacketId);
        }
    }
}