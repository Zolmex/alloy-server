#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace GameServer.Game.Network.Messaging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketAttribute : Attribute
    {
        public PacketId ID { get; }

        public PacketAttribute(PacketId id)
        {
            ID = id;
        }
    }

    public static class PacketLib
    {
        public static Dictionary<PacketId, IIncomingPacket> LoadIncoming()
        {
            var ret = new Dictionary<PacketId, IIncomingPacket>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsInterface && type.GetInterfaces().Contains(typeof(IIncomingPacket)))
                {
                    var pktId = type.GetCustomAttribute<PacketAttribute>().ID;
                    ret.Add(pktId, (IIncomingPacket)Activator.CreateInstance(type));
                }
            }

            return ret;
        }
    }
}