using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Network.Messaging;

public interface IAppMessage
{
    AppMessageId MessageId { get; }

    void Write(NetworkWriter wtr);
    void Read(NetworkReader rdr);

    void Handle(AppConnection con)
    {
        _handlers[MessageId].Handle(this, con);
    }

    #region Static members

    private static readonly Dictionary<AppMessageId, IAppMessage> _messages = new();
    private static readonly Dictionary<AppMessageId, IMessageHandler> _handlers = new();

    static IAppMessage()
    {
        var types = Assembly.GetExecutingAssembly()!.GetTypes(); // Common
        for (var i = 0; i < types.Length; i++)
        {
            var type = types[i];
            if (type != typeof(IAppMessage) && typeof(IAppMessage).IsAssignableFrom(type))
            {
                var msg = (IAppMessage)Activator.CreateInstance(type);
                _messages.Add(msg.MessageId, msg);
            }
        }

        types = Assembly.GetEntryAssembly()!.GetTypes(); // App-specific domain
        for (var i = 0; i < types.Length; i++)
        {
            var type = types[i];
            if (!type.IsInterface && type.GetInterface("IMessageHandler") != null)
            {
                var handler = (IMessageHandler)Activator.CreateInstance(type);
                _handlers.Add(handler.MessageId, handler);
            }
        }
    }

    static IAppMessage Require(AppMessageId id)
    {
        return _messages[id];
    }

    #endregion
}