using Common.Network;

namespace Common.Structs;

public struct ObjectStatusData {
    public int ObjectId;
    public WorldPosData Pos;
    public StatValue[] Stats;
    public StatData[] UpdatedStats;
    public bool Update;

    public static ObjectStatusData Read(ref SpanReader rdr) {
        var ret = new ObjectStatusData();
        ret.ObjectId = rdr.ReadInt32();
        ret.Pos = WorldPosData.Read(ref rdr);
        ret.UpdatedStats = new StatData[rdr.ReadByte()];
        for (var i = 0; i < ret.UpdatedStats.Length; i++)
            ret.UpdatedStats[i] = StatData.Read(ref rdr);

        return ret;
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId);
        wtr.Write(Pos);

        var statCount = 0;
        var pos = wtr.Position;
        wtr.Write((byte)0); // Placeholder

        for (var i = 0; i < (int)StatType.StatTypeCount; i++) {
            var value = Stats[i];
            if (value.HasValue) {
                statCount++;
                StatData.Write(ref wtr, (StatType)i, value);
            }
        }

        var endPos = wtr.Position;
        wtr.Position = pos; // Write in the placeholder the real amount of stat counts
        wtr.Write((byte)statCount);
        wtr.Position = endPos;

        Update = false;
    }

    public void SetStat(StatType type, StatValue value) {
        Update = true;
        if (type == StatType.None)
            return;

        var id = (int)type;
        Stats[id] = value;
    }

    public void SetPos(WorldPosData pos) {
        Pos = pos;
    }
}