namespace GameServer.Game.Entities
{
    public class OneWayContainer : Container
    {
        public OneWayContainer(ushort objType, int ownerId)
            : base(objType, 8, ownerId, -1)
        { }
    }
}