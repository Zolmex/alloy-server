using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;

namespace GameServer.Game.Entities;

public abstract class Ability
{
    protected Item _item;
    protected readonly Player _player;

    protected Ability(Player player)
    {
        _player = player;
        _item = player.Inventory[1];
    }
    
    public abstract void Use(Item item, WorldPosData usePos, int clientTime);
    public abstract bool Validate(Item item, Entity en);
}