#region

using Common.Database.Models;
using Common.Utilities;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

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
    public readonly Session Session;

    public volatile ConnectionState State;
    public ClientRandom Random;
    public ClientRandom ServerRandom;
    
    public User() {
        Id = Interlocked.Increment(ref _nextClientId);
        Network = new NetworkHandler(this);
        Session = new Session(this);
    }

    public void Reset() {
        Network.Reset();
        Session.Reset();
    }

    public void Setup(string ip, Socket socket) {
        Network.Setup(ip, socket);
    }

    public void StartNetwork() {
        State = ConnectionState.Connected;
        Network.StartReceive();
    }
    
    public void SetGameInfo(Account acc, uint randomSeed, World world) {
        Random = new ClientRandom(randomSeed);
        ServerRandom = new ClientRandom((uint)new Random().Next(1, int.MaxValue));

        Session.SetWorld(acc, world);
    }
    
    public void Load(Character chr, World world) {
        GameLogic.Enqueue(() => {
            State = ConnectionState.Ready;
            
            Session.Load(chr, world);

            SendPacket(new CreateSuccess(
                Session.PlayerId,
                chr.CharId));
            SendPacket(new AccountList(
                AccountList.Locked,
                Session.Account.LockedAccounts.ToArray()));
            SendPacket(new AccountList(
                AccountList.Ignored,
                Session.Account.IgnoredAccounts.ToArray()));
        });
    }
    
    public void Unload(bool reconnect) {
        if (reconnect && Session.State != GameState.Playing) // We can only unload when we've loaded in the first place
            return;

        Session.Unload();
    }
    
    public void SendPacket<T>(in T packet) where T : IOutgoingPacket, allows ref struct {
        Network.WritePacket(in packet);
    }
    
    public void SendFailure(int errorId = Failure.DEFAULT, string message = Failure.DEFAULT_MESSAGE,
        bool disconnect = true) {
        SendPacket(new Failure(errorId, message));
        if (disconnect)
            Disconnect(message, DisconnectReason.Failure);
    }
    
    public void ReconnectTo(World world) {
        if (world == null || world.Deleted)
            return;

        State = ConnectionState.Reconnecting;

        Unload(true); // Begin reconnect process, player leaves world and set gamestate to idle

        SendPacket(new Reconnect(world.Id));
    }

    public void Disconnect(string message = null, DisconnectReason reason = DisconnectReason.Unknown) {
        if (State == ConnectionState.Disconnected)
            return;

        _log.Debug($"Disconnecting user {Id} ({message}) (Reason:{reason})");

        State = ConnectionState.Disconnected;

        GameLogic.Enqueue(FinishDisconnect);
    }

    private void FinishDisconnect()
    {
        Unload(false);

        RealmManager.UserDisconnected(this);
        SocketServer.DisconnectUser(this);
    }
}