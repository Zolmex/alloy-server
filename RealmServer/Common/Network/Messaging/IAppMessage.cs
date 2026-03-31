using Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Network.Messaging;

public interface IAppMessage
{
    private static readonly Logger _log = new Logger(typeof(IAppMessage));
    
    AppMessageId MessageId { get; }
    int Sequence { get; set; }
    bool IsAck => this is IAppMessageAck;

    void Write(ref SpanWriter wtr);
    void Read(ref SpanReader rdr);

    async Task HandleAsync(AppConnection con)
    {
        if (!_handlers.TryGetValue(MessageId, out var handler))
        {
            _log.Error($"No handler for '{MessageId}'");
            return;
        }

        await handler.HandleAsync(this, con);
    }

    #region Static members

    private static readonly Dictionary<AppMessageId, IAppMessage> _messages = new();
    private static readonly Dictionary<AppMessageId, IAppMessageAck> _messageAcks = new();
    private static readonly Dictionary<AppMessageId, IMessageHandler> _handlers = new();
    private static int _sequence;

    static IAppMessage()
    {
        var types = Assembly.GetExecutingAssembly()!.GetTypes(); // Common
        for (var i = 0; i < types.Length; i++)
        {
            var type = types[i];
            if (type.IsInterface)
                continue;
            
            if (typeof(IAppMessageAck).IsAssignableFrom(type))
            {
                var msg = (IAppMessageAck)Activator.CreateInstance(type);
                _messageAcks.Add(msg.MessageId, msg);
            }
            else if (typeof(IAppMessage).IsAssignableFrom(type))
            {
                var msg = (IAppMessage)Activator.CreateInstance(type);
                _messages.Add(msg.MessageId, msg);
            }
        }

        types = Assembly.GetEntryAssembly()!.GetTypes(); // App-specific domain
        for (var i = 0; i < types.Length; i++)
        {
            var type = types[i];
            if (!type.IsInterface && typeof(IMessageHandler).IsAssignableFrom(type))
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
    
    static IAppMessageAck RequireAck(AppMessageId id)
    {
        return _messageAcks[id];
    }

    static void SetSequence(IAppMessage msg)
    {
        msg.Sequence = Interlocked.Increment(ref _sequence);
    }

    #endregion
}

public interface IAppMessageAck : IAppMessage
{
    // Empty
}