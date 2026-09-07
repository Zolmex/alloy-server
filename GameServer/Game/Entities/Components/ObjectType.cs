namespace GameServer.Game.Entities.Components;

public readonly struct ObjectType 
{
    public readonly ushort Value;

    public ObjectType(ushort value) 
    {
        Value = value;
    }

    public static implicit operator ushort(ObjectType objType) => objType.Value;
}