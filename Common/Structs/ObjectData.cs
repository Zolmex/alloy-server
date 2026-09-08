using Common.Network;
using Common.Utilities;

namespace Common.Structs;

public struct ObjectData {
    public ushort ObjectType;
    public ObjectStatusData Status;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectType);
        Status.WriteForUpdate(ref wtr);
    }
}