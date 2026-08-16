using System;
using Common.Resources.Xml;
using Common.Structs;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record ChangeGround : BehaviorScript {
    private readonly string[] _changeTo;
    private readonly int _dist;
    private readonly string[] _groundToChange;

    public ChangeGround(string[] groundToChange, string[] changeTo, int dist) {
        _groundToChange = groundToChange;
        _changeTo = changeTo;
        _dist = dist;
    }

    public override void Start(ref EntityView host) {
        var w = host.World;
        var pos = new IntPoint((int)host.Stats.Pos.X - _dist / 2, (int)host.Stats.Pos.Y - _dist / 2);

        for (var x = 0; x < _dist; x++)
            for (var y = 0; y < _dist; y++) {
                var tile = w.Map[x + pos.X, y + pos.Y];

                var r = Random.Shared.Next(_changeTo.Length);
                var type = XmlLibrary.Id2Tile(_changeTo[r]).GroundType;
                
                if (_groundToChange != null) {
                    foreach (var groundId in _groundToChange)
                        if (tile.Desc.GroundId == groundId)
                            tile.GroundType = type;
                }
                else {
                    tile.GroundType = type;
                }

                w.PlayerSights.TileUpdate(tile.Pos);
            }
    }
}