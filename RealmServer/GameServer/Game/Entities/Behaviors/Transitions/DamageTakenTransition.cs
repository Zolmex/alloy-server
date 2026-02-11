namespace GameServer.Game.Entities.Behaviors.Transitions;

public class DamageTakenRecord
{
    public int DamageTaken;

    public void OnEntityDamaged(CharacterEntity en, CharacterEntity from, int dmgTaken)
    {
        DamageTaken += dmgTaken;
    }
}

public class DamageTakenTransition : BehaviorTransition
{
    private readonly int _damage;

    public DamageTakenTransition(int damage, string targetState)
        : base()
    {
        RegisterTargetStates(targetState);
        _damage = damage;
    }

    public override void Start(CharacterEntity host)
    {
        var dmgTakenInfo = host.ResolveResource<DamageTakenRecord>(this);
        host.OnDamagedBy += dmgTakenInfo.OnEntityDamaged;
    }

    public override string Tick(CharacterEntity host, RealmTime time)
    {
        var dmgTakenInfo = host.ResolveResource<DamageTakenRecord>(this);
        if (dmgTakenInfo.DamageTaken >= _damage)
        {
            return GetTargetState();
        }

        return null;
    }

    public override void End(CharacterEntity host, RealmTime time)
    {
        var dmgTakenInfo = host.ResolveResource<DamageTakenRecord>(this);
        host.OnDamagedBy -= dmgTakenInfo.OnEntityDamaged;
    }
}