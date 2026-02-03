using Common.Utilities;
using System.Threading.Tasks;

namespace Common.Network.Messaging;

public interface IMessageHandler
{
    AppMessageId MessageId { get; }
    
    Task Handle(IAppMessage msg, AppConnection con);
}