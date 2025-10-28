#region

using Common.Database;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace GameServer.Game.PartySystem
{
    public class Party
    {
        private readonly Logger _logger;
        private static readonly Dictionary<int, Party> _partyCache = new();

        private DbParty _data;
        private readonly Dictionary<int, World> _memberWorldInvites = new(); // AccId, Invite
        private readonly Dictionary<Player, DateTime> _summonCooldowns = new();
        private readonly List<Player> _connectedPlayers = new();
        private DateTime _nextSummonTime;

        public int PartyId => _data.PartyId;
        public int MaxSlots => _data.MaxMembers;
        public int MembersCount => _data.Members.Count;
        public bool IsFull => MembersCount >= MaxSlots;
        public IEnumerable<int> MembersIds => _data.Members;

        private Party(DbParty data)
        {
            _data = data;
            _logger = new Logger($"Party[{data.PartyId}]");
        }

        public static Party GetFromId(int partyId)
        {
            if (_partyCache.TryGetValue(partyId, out var party))
                return party;

            var dbParty = DbClient.GetParty(partyId).Result;

            if (dbParty is not null)
            {
                party = new Party(dbParty);
                _partyCache[partyId] = party;
                return party;
            }

            Logger.Error($"DbParty Id {partyId} does not exist in database.", "Party");
            return null;
        }

        // Adds a player to the DbParty
        public void AddMember(Player player)
        {
            _data.Members.Add(player.AccountId);

            player.User.Account.PartyId = PartyId;
            player.PartyId = PartyId;

            DbClient.Save(_data, player.User.Account);

            _logger.Debug($"AccountId {player.AccountId} was added to party id {_data.PartyId}.");
        }

        // Removes an AccountId from the DbParty
        public void RemoveMember(int accountId)
        {
            if (!_data.Members.Remove(accountId))
                return;

            DbClient.Save(_data);

            _logger.Debug($"AccountId {accountId} was removed from party id {_data.PartyId}.");
        }

        public void MemberConnected(Player player)
        {
            if (player.PartyId != _data.PartyId)
                return;

            if (_connectedPlayers.Contains(player))
                return;

            _logger.Debug($"{player.Name} is now online.");
            _connectedPlayers.Add(player);

            BroadcastInfo($"Party member {player.Name} connected!");
        }

        public void MemberDisconnected(Player player)
        {
            if (player.PartyId != _data.PartyId)
                return;

            if (!_connectedPlayers.Remove(player))
                return;

            _logger.Debug($"{player.Name} is now offline.");
            BroadcastInfo($"Party member {player.Name} disconnected.");
        }

        public PartyInvite GenerateInvite(Player invitedPlayer)
        {
            var invite = new PartyInvite { Code = invitedPlayer.Rand.Next(1000, 10000), InvitedAccId = invitedPlayer.AccountId, PartyId = PartyId };
            _logger.Debug($"Created party invite with code {invite.Code}");
            return invite;
        }

        private bool CanSummon(Player player)
        {
            if (_summonCooldowns.TryGetValue(player, out var nextSummonTime))
                return DateTime.UtcNow >= nextSummonTime;

            return true;
        }

        public void SummonPlayer(Player player, Player summoner)
        {
            if (!CanSummon(summoner))
            {
                var cooldown = _summonCooldowns[summoner] - DateTime.UtcNow;
                summoner.SendError($"You can summon again in {cooldown:g}");
                return;
            }

            if (_memberWorldInvites.ContainsKey(player.AccountId))
            {
                summoner.SendInfo("This player has already been summoned. Wait for them to accept or deny their current invite first.");
                return;
            }

            _memberWorldInvites[player.AccountId] = summoner.World;
            player.SendInfo($"You have been summoned to '{summoner.World.DisplayName}' by {summoner.Name}. Type '/party join' to accept.");

            _summonCooldowns[summoner] = DateTime.UtcNow + TimeSpan.FromMinutes(1);

            summoner.OnLeftWorld += _ =>
            {
                if (_memberWorldInvites.Remove(player.AccountId))
                    player.SendInfo("Party summoner has left their world, invite expired.");
            };
        }

        public void SummonAll(Player summoner)
        {
            if (!CanSummon(summoner))
            {
                var cooldown = _summonCooldowns[summoner] - DateTime.UtcNow;
                summoner.SendError($"You can summon again in {cooldown:g}");
                return;
            }

            var otherPlayers = GetOnlinePlayers()
                .Where(p => p != summoner && p.World != summoner.World)
                .ToList();

            foreach (var otherPlayer in otherPlayers)
            {
                if (_memberWorldInvites.ContainsKey(otherPlayer.AccountId))
                {
                    summoner.SendInfo("This player has already been summoned. Wait for them to accept or deny their current invite first.");
                    continue;
                }

                _memberWorldInvites[otherPlayer.AccountId] = summoner.World;
                otherPlayer.SendInfo($"You have been summoned to '{summoner.World.DisplayName}' by {summoner.Name}. Type '/party join' to accept.");
            }

            _summonCooldowns[summoner] = DateTime.UtcNow + TimeSpan.FromMinutes(1);

            summoner.OnLeftWorld += _ =>
            {
                foreach (var otherPlayer in otherPlayers)
                {
                    if (_memberWorldInvites.Remove(otherPlayer.AccountId))
                        otherPlayer.SendInfo("Party summoner has left their world, invite expired.");
                }
            };
        }

        public void JoinSummon(Player player)
        {
            if (!_memberWorldInvites.Remove(player.AccountId, out var world))
            {
                player.SendError("You have not been summoned.");
                return;
            }

            player.User.ReconnectTo(world);
        }

        public void DenySummon(Player player)
        {
            if (_memberWorldInvites.Remove(player.AccountId))
            {
                _logger.Debug($"{player.Name} denied world summon.");
                player.SendInfo("You can be now be summoned again.");
            }
        }

        public void Disband()
        {
            // Make current players leave first
            var toKick = new List<Player>(_connectedPlayers);
            foreach (var player in toKick)
            {
                player.SendInfo("Your current party has been disbanded.");
                player.LeaveParty();
            }

            foreach (var accId in MembersIds)
            {
                var acc = DbClient.GetAccount(accId).Result;
                acc.PartyId = -1;
                DbClient.Save(acc);
            }

            _partyCache.Remove(PartyId, out _);
            DbClient.DeleteParty(PartyId);
        }

        public void BroadcastInfo(string message)
        {
            foreach (var activePlayer in _connectedPlayers)
                activePlayer.SendInfo(message);
        }

        public void BroadcastChat(string message, Player speaker)
        {
            foreach (var activePlayer in _connectedPlayers)
                activePlayer.SendParty(message, speaker);
        }


        public bool IsMember(Player player)
        {
            return _data.Members.Contains(player.AccountId);
        }

        public IEnumerable<Player> GetOnlinePlayers()
        {
            return _connectedPlayers;
        }
    }
}