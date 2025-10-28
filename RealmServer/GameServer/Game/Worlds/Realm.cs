#region

using Common.Resources.Config;
using Common.Utilities;
using GameServer.Game.Entities;
using System.Linq;

#endregion

namespace GameServer.Game.Worlds
{
    public class Realm : World
    {
        public bool Closed { get; set; }

        public Oryx Oryx { get; }

        public Realm()
            : base(REALM, -1)
        {
            DisplayName = GetRealmName();

            Oryx = new Oryx(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            Oryx.Initialize();
            RealmManager.OnRealmAdded(this);
        }

        public override void Tick(RealmTime time)
        {
            if (!Initialized)
                return;

            base.Tick(time);

            Oryx.Tick(time);
        }

        public override void RemoveEntity(Entity en)
        {
            base.RemoveEntity(en);

            Oryx.OnEnemyKilled(en);
        }

        public void CloseRealm()
        {
            Oryx.Close();
        }

        public Entity GetActiveEvent()
        {
            return Oryx.ActiveEvent;
        }

        public string GetRealmName()
        {
            string ret = null;
            while (ret == null || RealmManager.ActiveRealms.Values.Any(i => i.EqualsIgnoreCase(ret)))
                ret = RealmConfig.Config.Names.RandomElement();
            return ret;
        }
    }
}