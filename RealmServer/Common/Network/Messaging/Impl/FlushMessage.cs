using Common.Database;
using Common.Utilities;
using System;
using System.IO;

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

    public void Write(ref SpanWriter wtr)
    {
        wtr.WriteUTF(Key);
        wtr.Write(Version);
        wtr.Write((ushort)Properties.Length);
        foreach (var prop in Properties)
            wtr.WriteUTF(prop);

        wtr.Write((ushort)0); // Placeholder for properties length
        var begin = wtr.Position;
        Entity.WriteProperties(ref wtr, Properties);
        var length = wtr.Position - begin;
        wtr.Position = begin - 2;
        wtr.Write((ushort)length);
        wtr.Position += length;
    }

    public void Read(ref SpanReader rdr)
    {
        Key = rdr.ReadUTF();
        Version = rdr.ReadInt32();
        var len = rdr.ReadUInt16();
        Properties =  new string[len];
        for (var i = 0; i < len; i++)
            Properties[i] = rdr.ReadUTF();
        
        PropertiesBuffer = rdr.ReadBytes(rdr.ReadUInt16()).ToArray();
    }
}