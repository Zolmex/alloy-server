using Common.Network;

namespace Common.Structs;

public struct SlotObjectData {
    public int ObjectId;
    public byte SlotId;

    public static SlotObjectData Read(ref SpanReader rdr) {
        return new SlotObjectData { ObjectId = rdr.ReadInt32(), SlotId = rdr.ReadByte() };
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId);
        wtr.Write(SlotId);
    }
}