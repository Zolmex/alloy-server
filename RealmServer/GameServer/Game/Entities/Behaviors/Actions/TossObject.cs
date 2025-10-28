#region

using Common;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class TossObjectInfo
    {
        public int CooldownLeft;
    }

    public record TossObject : BehaviorScript
    {
        private readonly float _range;
        private readonly float _angle;
        private readonly int _cooldownMS;
        private readonly int _cooldownOffsetMS;
        private readonly bool _tossInvis;
        private readonly float _probability;
        private readonly string[] _children;
        private readonly float _minRange;
        private readonly float _maxRange;
        private readonly float _minAngle;
        private readonly float _maxAngle;
        private readonly float _densityRange;
        private readonly int _maxDensity;
        private readonly string _group;
        private readonly TileRegion _region;
        private readonly double _regionRange;
        private readonly bool _targeted;
        private List<IntPoint> _reproduceRegions;

        public TossObject(string child, float range = 5, float angle = 0,
            int cooldownMS = 1000, int cooldownOffsetMS = 0,
            bool tossInvis = false, float probability = 1, string group = null,
            float minAngle = 0, float maxAngle = 0,
            float minRange = 0, float maxRange = 0,
            float densityRange = 0, int maxDensity = 1,
            TileRegion region = TileRegion.None, float regionRange = 10,
            bool targeted = false)
        {
            if (group == null)
                _children = new string[] { child };
            else
                _children = XmlLibrary.ObjectDescs.Values
                    .Where(x => x.Group == group)
                    .Select(x => x.ObjectId).ToArray();

            _range = range;
            _angle = angle.Deg2Rad();
            _cooldownMS = cooldownMS;
            _cooldownOffsetMS = cooldownOffsetMS;
            _tossInvis = tossInvis;
            _probability = probability;
            _minRange = minRange;
            _maxRange = maxRange;
            _minAngle = minAngle.Deg2Rad();
            _maxAngle = maxAngle.Deg2Rad();
            _densityRange = densityRange;
            _maxDensity = maxDensity;
            _group = group;
            _region = region;
            _regionRange = regionRange;
            _targeted = targeted;
        }

        public override void Start(Character host)
        {
            var tossObjectInfo = host.ResolveResource<TossObjectInfo>(this);
            tossObjectInfo.CooldownLeft = _cooldownOffsetMS;

            if (_region == TileRegion.None)
                return;

            var map = host.World.Map;
            var w = map.Width;
            var h = map.Height;

            _reproduceRegions = new List<IntPoint>();
            for (var y = 0; y < h; y++)
                for (var x = 0; x < w; x++)
                {
                    if (map[x, y].Region != _region)
                        continue;

                    _reproduceRegions.Add(new IntPoint(x, y));
                }
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var tossObjectInfo = host.ResolveResource<TossObjectInfo>(this);
            if (tossObjectInfo.CooldownLeft <= 0)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                    return BehaviorTickState.BehaviorFailed;

                if (Random.Shared.NextDouble() > _probability)
                {
                    tossObjectInfo.CooldownLeft = _cooldownMS;
                    return BehaviorTickState.BehaviorDeactivate;
                }

                Entity player = _targeted ? host.GetNearestPlayer(_range) : null;
                if (_densityRange != 0 && _maxDensity != 0)
                {
                    var cnt = 0;
                    if (_children.Length > 1)
                        cnt = host.GetOtherEnemiesByName(_group, _densityRange).Count();
                    else
                    {
                        cnt = host.GetOtherEnemiesByName(_children[0], _densityRange).Count();
                    }

                    if (cnt >= _maxDensity)
                    {
                        tossObjectInfo.CooldownLeft = _cooldownMS;
                        return BehaviorTickState.BehaviorDeactivate;
                    }
                }

                var r = _range;
                if (_minRange != 0 && _maxRange != 0)
                    r = (float)(_minRange + (Random.Shared.NextDouble() * (_maxRange - _minRange)));

                var a = _angle;
                if (_angle == 0 && _minAngle != 0 && _maxAngle != 0)
                    a = (float)(_minAngle + (Random.Shared.NextDouble() * (_maxAngle - _minAngle)));

                WorldPosData target;
                if (player == null)
                    target = new WorldPosData() { X = host.Position.X + (float)(r * Math.Cos(a)), Y = host.Position.Y + (float)(r * Math.Sin(a)) };
                else
                    target = new WorldPosData() { X = player.Position.X, Y = player.Position.Y };

                if (_reproduceRegions != null && _reproduceRegions.Count > 0)
                {
                    var sx = (int)host.Position.X;
                    var sy = (int)host.Position.Y;
                    var regions = _reproduceRegions
                        .Where(p => Math.Abs(sx - p.X) <= _regionRange &&
                                    Math.Abs(sy - p.Y) <= _regionRange).ToList();
                    var tile = regions[Random.Shared.Next(regions.Count)];
                    target = new WorldPosData() { X = tile.X, Y = tile.Y };
                }

                if (!_tossInvis)
                    host.World.BroadcastAll(p =>
                        ShowEffect.Write(p.User.Network,
                            (byte)ShowEffectIndex.Throw,
                            host.Id,
                            0xFFBF00,
                            0,
                            target,
                            default
                        ));

                var world = host.World;
                if (!world.IsPassable(target.X, target.Y, true))
                    return BehaviorTickState.BehaviorFailed;

                var objType = XmlLibrary.Id2Object(_children[Random.Shared.Next(_children.Length)]).ObjectType;
                var entity = Entity.Resolve(objType);

                if (host.Spawned)
                {
                    entity.Spawned = true;
                }

                entity.Move(target.X, target.Y);
                entity.EnterWorld(world);
                tossObjectInfo.CooldownLeft = _cooldownMS;
            }
            else
                tossObjectInfo.CooldownLeft -= time.ElapsedMsDelta;

            return BehaviorTickState.BehaviorActive;
        }
    }
}