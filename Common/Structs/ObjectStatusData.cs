using Common.Network;
using Common.Utilities;

namespace Common.Structs;

public struct ObjectStatusData {
    public int ObjectId;
    public WorldPosData Pos;
    public StatValue[] Stats;
    public StatData[] StatUpdates;
    public BitMask256 PrivacyMask;
    public int StatCount;

    public static ObjectStatusData Read(ref SpanReader rdr) {
        var ret = new ObjectStatusData();
        ret.ObjectId = rdr.ReadInt32();
        ret.Pos = WorldPosData.Read(ref rdr);
        ret.Stats = new StatValue[rdr.ReadByte()];
        for (var i = 0; i < ret.Stats.Length; i++)
            ret.Stats[i] = StatData.Read(ref rdr).Value;
        ret.StatCount = ret.Stats.Length;

        return ret;
    }

    public void WriteHeader(ref SpanWriter wtr) {
        wtr.Write(ObjectId);
        wtr.Write(Pos);
    }

    public void WriteForUpdate(ref SpanWriter wtr) {
        WriteHeader(ref wtr);
        var pos = wtr.Position;
        wtr.Write((byte)0);

        var count = 0;
        for (var i = 0; i < StatCount; i++) {
            var value = Stats[i];
            if (value.HasValue && PrivacyMask.IsSet(i)) {
                StatData.Write(ref wtr, (StatType)i, value);
                count++;
            }
        }

        OverwriteCount(ref wtr, pos, count);
    }

    public void WriteForNewTick(ref SpanWriter wtr) {
        WriteHeader(ref wtr);
        var pos = wtr.Position;
        wtr.Write((byte)0);

        var count = 0;
        for (var i = 0; i < StatCount; i++) {
            var stat = StatUpdates[i];
            if (stat.Value.HasValue && PrivacyMask.IsSet((int)stat.Type)) {
                stat.Write(ref wtr);
                count++;
            }
        }

        OverwriteCount(ref wtr, pos, count);
    }

    private static void OverwriteCount(ref SpanWriter wtr, int pos, int count) {
        var endPos = wtr.Position;
        wtr.Position = pos;
        wtr.Write((byte)count);
        wtr.Position = endPos;
    }
}