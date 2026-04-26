using Common.Network;

namespace Common.Structs;

public struct ObjectData {
    public ushort ObjectType;
    public ObjectStatusData Status;

    public static ObjectData Read(ref SpanReader rdr) {
        return new ObjectData { ObjectType = rdr.ReadUInt16(), Status = ObjectStatusData.Read(ref rdr) };
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectType);
        Status.Write(ref wtr);
    }
}