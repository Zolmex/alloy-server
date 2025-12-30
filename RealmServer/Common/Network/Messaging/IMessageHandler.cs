using Common.Utilities;

namespace Common.Network.Messaging;

public interface IMessageHandler
{
    AppMessageId MessageId { get; }
    
    void Handle(IAppMessage msg, AppConnection con);
}