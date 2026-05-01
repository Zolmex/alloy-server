#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace GameServer.Game.Network.Messaging;

[AttributeUsage(AttributeTargets.Class)]
public class PacketAttribute : Attribute {
    public PacketAttribute(PacketId id) {
        ID = id;
    }

    public PacketId ID { get; }
}

public static class PacketLib {
    public static Dictionary<PacketId, Type> LoadIncoming() {
        var ret = new Dictionary<PacketId, Type>();
        var types = Assembly.GetExecutingAssembly().GetTypes();
        for (var i = 0; i < types.Length; i++) {
            var type = types[i];
            if (!type.IsInterface && type.GetInterfaces().Contains(typeof(IIncomingPacket))) {
                var pktId = type.GetCustomAttribute<PacketAttribute>().ID;
                ret.Add(pktId, type);
            }
        }

        return ret;
    }
}