#region

using Common.Database;
using GameServer.Game.PartySystem;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        private readonly Dictionary<int, PartyInvite> _partyInvites = new(); // code, invite

        public Party Party { get; private set; }

        public bool IsInParty => PartyId != -1 || Party is not null;

        private void SetParty()
        {
            if (!IsInParty)
                return;

            Party = Party.GetFromId(PartyId);
            Party.MemberConnected(this);
        }

        public void UnsetParty()
        {
            if (Party is null)
                return;

            Party.MemberDisconnected(this);
            Party = null;
        }

        public void CreateParty()
        {
            if (IsInParty)
            {
                SendError("You are already in a party.");
                return;
            }

            if (!DbClient.TryCreateParty(User.Account, out var dbParty))
            {
                SendError("Failed to create party.");
                return;
            }

            PartyId = dbParty.PartyId;
            SetParty();

            SendInfo($"Created Party!");
        }

        public void LeaveParty()
        {
            if (!IsInParty)
                return;

            if (Party is null)
                return;

            Party.RemoveMember(AccountId);
            UnsetParty();

            User.Account.PartyId = -1;
            PartyId = -1;
            DbClient.Save(User.Account);

            SendInfo("You have left your current party.");
        }

        /// <summary>
        /// Invites this player to another player's party
        /// </summary>
        public void InviteToParty(Player inviter)
        {
            if (!inviter.IsInParty)
            {
                inviter.SendError("You are not in a party!");
                return;
            }

            if (inviter == this)
            {
                SendError("You cannot invite yourself. Get some friends!");
                return;
            }

            if (IsInParty)
            {
                inviter.SendError($"{Name} is already in a party.");
                return;
            }

            if (inviter.Party.IsFull)
            {
                inviter.SendError("Your party is currently full.");
                return;
            }

            var invite = inviter.Party.GenerateInvite(this);
            _partyInvites[invite.Code] = invite;
            SendInfo(
                $"You have been invited to a party by {inviter.Name}! type '/party accept {invite.Code}' to accept the invitation.");
        }

        public void AcceptPartyInvite(int code)
        {
            if (IsInParty)
            {
                SendError("Leave your current party first.");
                return;
            }

            if (!_partyInvites.TryGetValue(code, out var invite))
            {
                SendError("You have not been invited to a party.");
                return;
            }

            var party = Party.GetFromId(invite.PartyId);
            if (party is null)
            {
                SendError("Failed to join party.");
                _partyInvites.Remove(code);
                return;
            }

            if (party.IsFull)
            {
                SendError("The party you tried to join is now full.");
                _partyInvites.Remove(code);
                return;
            }

            party.AddMember(this);
            SetParty();

            _partyInvites.Remove(code);
            party.BroadcastInfo($"{Name} has joined the party!");
        }
    }
}