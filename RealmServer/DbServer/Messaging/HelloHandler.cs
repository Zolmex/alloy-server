using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using DbServer.Service;

namespace DbServer.Messaging;

public class HelloHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Hello;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var hello = (HelloMessage)msg;
        con.TargetName = hello.AppName; // We received the name of the app at the other end of the connection, set it
        NetworkService.Listener.RegisterConnection(hello.AppName, con);
        
        Logger.Debug($"Hello received from app '{hello.AppName}'");
    }
}