using Common.Network;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Common.Database;

public abstract class DbModel
{
    public abstract string Key { get; }
    [NotMapped] public int Version { get; set; }

    private readonly Dictionary<string, PropertySerializer> _serializers = new();

    protected DbModel()
    {
        RegisterProperty("Version",
            (ref wtr) => wtr.Write(Version),
            (ref rdr) => Version = rdr.ReadInt32()
        );
    }
    
    public delegate void SpanWriterAction(ref SpanWriter writer);
    public delegate void SpanReaderAction(ref SpanReader reader);
    
    protected void RegisterProperty(string propName, SpanWriterAction writer, SpanReaderAction reader)
    {
        _serializers.Add(propName, new PropertySerializer(propName, writer, reader));
    }
    
    public void WriteProperties(ref SpanWriter wtr, params string[] properties)
    {
        if (properties == null || properties.Length == 0)
        {
            wtr.Write((byte)_serializers.Count);
            foreach (var kvp in _serializers)
            {
                wtr.WriteUTF(kvp.Key);
                kvp.Value.Writer(ref wtr);
            }

            return;
        }

        wtr.Write((byte)properties.Length);
        foreach (var prop in properties)
        {
            if (_serializers.TryGetValue(prop, out var serializer))
            {
                wtr.WriteUTF(prop);
                serializer.Writer(ref wtr);
            }
        }
    }
    
    public void ReadProperties(ref SpanReader rdr)
    {
        var count = rdr.ReadByte();
        for (var i = 0; i < count; i++)
        {
            var propName = rdr.ReadUTF();
            if (_serializers.TryGetValue(propName, out var serializer))
                serializer.Reader(ref rdr);
        }
    }

    public async Task<FlushAck> Flush<T, TValue>(AppConnection con, params Expression<Func<T, TValue>>[] expressions) where T : DbModel
    {
        var props = new string[expressions.Length];
        for (var i = 0; i < expressions.Length; i++)
        {
            var expression = expressions[i];
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;
            props[i] = property.Name;
        }
        
        return await con.SendAndReceiveAsync<FlushAck>(new FlushMessage()
        {
            Key = Key,
            Version = Version,
            Entity = this,
            Properties = props
        });
    }
    
    public async Task<FlushAck> FlushAll(AppConnection con)
    {
        var props = new string[_serializers.Count];
        var i = 0;
        foreach (var kvp in _serializers)
        {
            props[i] = kvp.Key;
            i++;
        }

        return await con.SendAndReceiveAsync<FlushAck>(new FlushMessage()
        {
            Key = Key,
            Version = Version,
            Entity = this,
            Properties = props
        });
    }

    public static T Read<T>(ref SpanReader rdr) where T : DbModel, new()
    {
        if (!rdr.ReadBoolean())
            return null;

        var ret = new T();
        ret.ReadProperties(ref rdr);
        return ret;
    }

    private record PropertySerializer(string Property, SpanWriterAction Writer, SpanReaderAction Reader);
}
