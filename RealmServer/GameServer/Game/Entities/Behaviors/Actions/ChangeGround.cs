#region

using Common;
using Common.Resources.Xml;
using System;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record ChangeGround : BehaviorScript
    {
        private readonly string[] _groundToChange;
        private readonly string[] _changeTo;
        private readonly int _dist;

        public ChangeGround(string[] groundToChange, string[] changeTo, int dist)
        {
            _groundToChange = groundToChange;
            _changeTo = changeTo;
            _dist = dist;
        }

        public override void Start(Character host)
        {
            var w = host.World;
            var pos = new IntPoint((int)host.Position.X - (_dist / 2), (int)host.Position.Y - (_dist / 2));

            for (var x = 0; x < _dist; x++)
                for (var y = 0; y < _dist; y++)
                {
                    var tile = w.Map[x + pos.X, y + pos.Y];

                    var r = Random.Shared.Next(_changeTo.Length);
                    if (_groundToChange != null)
                    {
                        foreach (var groundId in _groundToChange)
                        {
                            if (tile.TileDesc.GroundId == groundId)
                            {
                                tile.GroundType = XmlLibrary.Id2Tile(_changeTo[r]).GroundType;
                            }
                        }
                    }
                    else
                    {
                        tile.GroundType = XmlLibrary.Id2Tile(_changeTo[r]).GroundType;
                    }

                    w.BroadcastAll(p => p.TileUpdate(tile));
                }
        }
    }
}