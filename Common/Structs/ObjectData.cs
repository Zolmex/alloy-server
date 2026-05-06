using Common.Network;
using Common.Utilities;

namespace Common.Structs;

public struct ObjectData {
    public ushort ObjectType;
    public ObjectStatusData Status;
    public BitMask256 PrivateMask;

    public static ObjectData Read(ref SpanReader rdr) {
        return new ObjectData { ObjectType = rdr.ReadUInt16(), Status = ObjectStatusData.Read(ref rdr) };
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ObjectType);
        Status.Write(Status.Pos, ref wtr, ref PrivateMask, ref PrivateMask);
    }
}