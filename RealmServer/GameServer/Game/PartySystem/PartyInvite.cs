namespace GameServer.Game.PartySystem
{
    public struct PartyInvite
    {
        public int Code;
        public int PartyId;
        public int InvitedAccId;

        public bool IsValid(int code, int invitedAccId)
        {
            return code == Code && invitedAccId == InvitedAccId;
        }
    }
}