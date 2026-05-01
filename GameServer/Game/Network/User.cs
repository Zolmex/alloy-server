#region

using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Common.Database.Models;
using Common.Utilities;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game.Network;

public enum ConnectionState {
    Disconnected, // Client is waiting to be used
    Connected, // A connection received is using this instance of NetClient
    Reconnecting, // Client is moving from a world instance to another
    Ready // Client is ready to handle in-game packets
}

public enum DisconnectReason {
    Unknown,
    UserDisconnect,
    NetworkError,
    Failure,
    Death,
    IllegalAction
}

public class User : IIdentifiable {
    private static readonly Logger _log = new(typeof(User));
    private static int _nextClientId;
    
    public int Id { get; set; }

    public readonly NetworkHandler Network;
    public readonly GameInfo GameInfo;

    public ConnectionState State;
    public Account Account;
    public ClientRandom Random;
    public ClientRandom ServerRandom;
    
    public User() {
        Id = Interlocked.Increment(ref _nextClientId);
        Network = new NetworkHandler(this);
        GameInfo = new GameInfo(this);
    }

    public void Reset() {
        Network.Reset();
        GameInfo.Reset();
    }

    public void Setup(string ip, Socket socket) {
        Network.Setup(ip, socket);
    }

    public void StartNetwork() {
        State = ConnectionState.Connected;
        Network.StartReceive();
    }
    
    public void SetGameInfo(Account acc, uint randomSeed, World world) {
        Account = acc;

        Random = new ClientRandom(randomSeed);
        ServerRandom = new ClientRandom((uint)new Random().Next(1, int.MaxValue));

        GameInfo.SetWorld(world);
    }
    
    public void Load(Character chr, World world) {
        State = ConnectionState.Ready;

        GameInfo.Load(chr, world);

        SendPacket(new CreateSuccess(
            GameInfo.Player.Id,
            chr.AccCharId));
        SendPacket(new AccountList(
            AccountList.Locked,
            Account.AccountLocks.Select(i => i.LockedId).ToArray()));
        SendPacket(new AccountList(
            AccountList.Ignored,
            Account.AccountIgnores.Select(i => i.IgnoredId).ToArray()));
    }
    
    public void Unload(bool reconnect, bool death = false) {
        if (reconnect && GameInfo.State != GameState.Playing) // We can only unload when we've loaded in the first place
            return;

        GameInfo.Unload(reconnect, death);
    }
    
    public void SendPacket(IOutgoingPacket packet) {
        Network.WritePacket(packet);
    }
    
    public void SendFailure(int errorId = Failure.DEFAULT, string message = Failure.DEFAULT_MESSAGE,
        bool disconnect = true) {
        SendPacket(new Failure(errorId, message));
        if (disconnect)
            Disconnect(message, DisconnectReason.Failure);
    }

    public void Disconnect(string message = null, DisconnectReason reason = DisconnectReason.Unknown) {
        if (State == ConnectionState.Disconnected)
            return;

        _log.Debug($"Disconnecting user {Id} ({message}) (Reason:{reason})");

        State = ConnectionState.Disconnected;

        Unload(false, reason == DisconnectReason.Death);

        RealmManager.UserDisconnected(this);
        SocketServer.DisconnectUser(this);
    }
}