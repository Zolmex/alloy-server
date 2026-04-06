#region

using GameServer.Game.Entities;
using GameServer.Utilities.Collections;
using System.Threading.Tasks;

#endregion

namespace GameServer.Game.Worlds;

public class MapChunk
{
    private readonly World _world;

    public LazyCollection<Entity> Entities; // Excluding players
    public LazyCollection<Player> Players;

    public MapChunk(World world, int cX, int cY)
    {
        _world = world;

        CX = cX;
        CY = cY;
        Entities = new LazyCollection<Entity>();
        Players = new LazyCollection<Player>();
    }

    public long TickCount { get; private set; }
    public int CX { get; }
    public int CY { get; }

    public void Update()
    {
        Entities.Update();
        Players.Update();
    }

    public void Tick(RealmTime time)
    {
        TickCount = time.TickCount;
        foreach (var en in Entities.Values)
        {
            if (en.Dead)
                return;

            _world.ActiveEntities.Add(en); // Here we put into a list the entities that belong to active chunks

            if (en is CharacterEntity chr)
                chr.Tick(time);
            else if (en is Container c)
                c.Tick(time);
            else if (en.Lifetime != -1)
                en.Tick(time);

            _world.InvokeEntityTick(en);
        }
    }

    public void Insert(Entity en)
    {
        if (en.IsPlayer)
            Players.Add(en as Player);
        else
            Entities.Add(en);
    }

    public void Remove(Entity en)
    {
        if (en.IsPlayer)
            Players.Remove(en as Player);
        else
            Entities.Remove(en);
    }

    public int DistSqr(MapChunk chunk)
    {
        var dx = CX - chunk.CX;
        var dy = CY - chunk.CY;
        return (dx * dx) + (dy * dy);
    }
}