using System.Threading.Tasks;

namespace Common.Network.Messaging;

public interface IMessageHandler {
    AppMessageId MessageId { get; }

    Task HandleAsync(IAppMessage msg, AppConnection con);
}