using Common.Database;
using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct FlushMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.Flush;
    public int Sequence { get; set; }
    
    public string Key { get; set; }
    public int Version { get; set; }
    public DbModel Entity { get; set; }
    public string[] Properties { get; set; }
    
    public byte[] PropertiesBuffer { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Key);
        wtr.Write(Version);
        wtr.Write(Properties);

        wtr.Write((ushort)0); // Placeholder for properties length
        var begin = wtr.BaseStream.Position;
        Entity.WriteProperties(wtr, Properties);
        var length = wtr.BaseStream.Position - begin;
        wtr.BaseStream.Position = begin - 2;
        wtr.Write((ushort)length);
        wtr.BaseStream.Position += length;
    }

    public void Read(NetworkReader rdr)
    {
        Key = rdr.ReadUTF();
        Version = rdr.ReadInt32();
        var len = rdr.ReadUInt16();
        Properties =  new string[len];
        for (var i = 0; i < len; i++)
            Properties[i] = rdr.ReadUTF();
        
        PropertiesBuffer = rdr.ReadBytes(rdr.ReadUInt16());
    }
}