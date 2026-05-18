using Common.Network;
using Common.Utilities.Collections;

namespace Common.Structs;

public struct ObjectDropData {
    public EntityId ObjectId;
    public bool Explode;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectId.Value);
        wtr.Write(Explode);
    }
}