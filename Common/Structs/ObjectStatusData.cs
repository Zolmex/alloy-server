using Common.Network;
using Common.Utilities;

namespace Common.Structs;

public struct ObjectStatusData {
    public int ObjectId;
    public WorldPosData Pos;
    public StatValue[] Stats;

    public static ObjectStatusData Read(ref SpanReader rdr) {
        var ret = new ObjectStatusData();
        ret.ObjectId = rdr.ReadInt32();
        ret.Pos = WorldPosData.Read(ref rdr);
        ret.Stats = new StatValue[rdr.ReadByte()];
        for (var i = 0; i < ret.Stats.Length; i++)
            ret.Stats[i] = StatData.Read(ref rdr).Value;

        return ret;
    }

    public void Write(ref SpanWriter wtr, ref BitMask256 privateMask, ref BitMask256 statUpdates) {
        wtr.Write(ObjectId);
        wtr.Write(Pos);

        var statCount = 0;
        var pos = wtr.Position;
        wtr.Write((byte)0); // Placeholder

        for (var i = 0; i < (int)StatType.StatTypeCount; i++) {
            var value = Stats[i];
            if (value.HasValue && privateMask.IsSet(i) && statUpdates.IsSet(i)) {
                statCount++;
                StatData.Write(ref wtr, (StatType)i, value);
            }
        }

        var endPos = wtr.Position;
        wtr.Position = pos; // Write in the placeholder the real amount of stat counts
        wtr.Write((byte)statCount);
        wtr.Position = endPos;
    }
}