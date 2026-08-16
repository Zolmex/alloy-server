using Common.Game;
using Common.Resources.Xml;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record OpenGate : BehaviorScript {
    private readonly int _area;

    private readonly string _target;
    private readonly bool _useArea;
    private readonly int _xMax;
    private readonly int _xMin;
    private readonly int _yMax;
    private readonly int _yMin;

    public OpenGate(int xMin, int xMax, int yMin, int yMax) {
        _xMin = xMin;
        _xMax = xMax;
        _yMin = yMin;
        _yMax = yMax;
    }

    public OpenGate(string target, int area = 10) {
        _target = target;
        _area = area;
        _useArea = true;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        if (_useArea)
            for (var x = (int)host.Stats.Pos.X - _area; x <= (int)host.Stats.Pos.X + _area; x++) {
                for (var y = (int)host.Stats.Pos.Y - _area; y <= (int)host.Stats.Pos.Y + _area; y++) {
                    var tile = host.World.Map[x, y];
                    if (tile.ObjectType == XmlLibrary.Id2Object(_target).ObjectType) {
                        tile.ObjectType = 0;
                        tile.SetObject(null);
                    }
                }
            }
        else
            for (var x = _xMax; x <= _xMax; x++) {
                for (var y = _yMin; y <= _yMax; y++) {
                    var tile = host.World.Map[x, y];
                    tile.ObjectType = 0;
                    tile.SetObject(null);
                }
            }

        return BehaviorTickState.BehaviorActive;
    }
}