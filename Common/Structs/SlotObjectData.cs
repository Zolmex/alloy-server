using Common.Network;
using Common.Utilities.Collections;

namespace Common.Structs;

public record struct SlotObjectData {
    public EntityId ObjectId;
    public byte SlotId;

    public static SlotObjectData Read(ref SpanReader rdr) {
        return new SlotObjectData { ObjectId = EntityId.Read(ref rdr), SlotId = rdr.ReadByte() };
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId.Value);
        wtr.Write(SlotId);
    }
}