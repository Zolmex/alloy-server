using Common.Network;
using Common.Network.Messaging.Impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        Prepare();
    }
    
    protected abstract void Prepare();

    protected void RegisterProperty(string propName, Action<NetworkWriter> writer, Action<NetworkReader> reader)
    {
        _serializers.Add(propName, new PropertySerializer(propName, writer, reader));
    }
    
    public void WriteProperties(NetworkWriter wtr, params string[] properties)
    {
        if (properties == null || properties.Length == 0)
        {
            wtr.Write((byte)_serializers.Count);
            foreach (var kvp in _serializers)
            {
                wtr.Write(kvp.Key);
                kvp.Value.Writer(wtr);
            }

            return;
        }

        wtr.Write((byte)properties.Length);
        foreach (var prop in properties)
        {
            if (_serializers.TryGetValue(prop, out var serializer))
            {
                wtr.Write(prop);
                serializer.Writer(wtr);
            }
        }
    }
    
    public void ReadProperties(NetworkReader rdr)
    {
        var count = rdr.ReadByte();
        for (var i = 0; i < count; i++)
        {
            var propName = rdr.ReadUTF();
            if (_serializers.TryGetValue(propName, out var serializer))
                serializer.Reader(rdr);
        }
    }

    public async Task Flush<T>(AppConnection con, params Expression<Func<T, object>>[] expressions) where T : DbModel
    {
        var props = new string[expressions.Length];
        for (var i = 0; i < expressions.Length; i++)
        {
            var expression = expressions[i];
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;
            props[i] = property.Name;
        }
        
        await con.SendAndReceiveAsync(new FlushMessage()
        {
            Key = Key,
            Version = Version,
            Entity = this,
            Properties = props
        });
    }

    public static T Read<T>(NetworkReader rdr) where T : DbModel, new()
    {
        if (!rdr.ReadBoolean())
            return null;

        var ret = new T();
        ret.ReadProperties(rdr);
        return ret;
    }
    
    private record PropertySerializer(string Property, Action<NetworkWriter> Writer, Action<NetworkReader> Reader);
}
