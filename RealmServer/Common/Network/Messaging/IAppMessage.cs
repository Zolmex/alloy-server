using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Network.Messaging;

public interface IAppMessage
{
    AppMessageId MessageId { get; }

    void Write(NetworkWriter wtr);
    void Read();
    void Handle();

    #region Static members

    private static readonly Dictionary<AppMessageId, IAppMessage> _messages = new();

    static IAppMessage()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        for (var i = 0; i < types.Length; i++)
        {
            var type = types[i];
            if (type != typeof(IAppMessage) && type.IsSubclassOf(typeof(IAppMessage)))
            {
                var msg = (IAppMessage)Activator.CreateInstance(type);
                _messages.Add(msg.MessageId, msg);
            }
        }
    }

    static IAppMessage Require(AppMessageId id)
    {
        return _messages[id];
    }

    #endregion
}