using Common.Network;

namespace Common.Structs;

public struct ObjectDropData {
    public int ObjectId;
    public bool Explode;

    public static ObjectDropData Read(NetworkReader rdr) {
        return new ObjectDropData { ObjectId = rdr.ReadInt32(), Explode = rdr.ReadBoolean() };
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId);
        wtr.Write(Explode);
    }
}