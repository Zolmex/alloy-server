#region

using Common.API.Helpers;
using Common.API.Requests;
using Common.API.Requests.SubscriptionRequests;
using Common.Enums;
using Common.Resources.Config;
using Common.Resources.Sprites;
using Common.Resources.Xml;
using Common.Utilities;
using Discord;
using Discord.WebSocket;
using DiscordBot;
using DiscordBot.Handlers;
using System.Net;

#endregion

public class Program
{
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static DiscordSocketClient _client;

    private static HttpListener _listener;

    public static async Task Main()
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;

        await _client.LoginAsync(TokenType.Bot, "tokenlol");
        await _client.StartAsync();

        DiscordBotOutput.Load(_client);
        _listener = new HttpListener();
        var config = DiscordBotConfig.Config;
        using (var timer = new EasyTimer(LogLevel.Info, "Starting server...", $"Listening on {config.Address + ":" + config.Port} ({EasyTimer.Time})"))
        {
            EnumUtils.Load(); // Initialize and prepare our static classes for later use
            RequestManager.Load();
            XmlLibrary.Load(config.XmlsDir);
            SpriteLibrary.Load(config.SpritesDir);

            _listener.Prefixes.Add($"http://{config.Address}:{config.Port}/"); // Start receiving HTTP requests
            _listener.Start();
            Receive();
        }

        List<IAPIRequest> subscriptionRequests = new() { new LootDropSubscriptionRequest() { CallbackUrl = $"http://{config.Address}:{config.Port}/droploot/", LootDropRarity = LootDropRarity.All }, new EventSpawnSubscriptionRequest() { CallbackUrl = $"http://{config.Address}:{config.Port}/eventspawn/", Event = SubscribableEvent.All }, new EventDeathSubscriptionRequest() { CallbackUrl = $"http://{config.Address}:{config.Port}/eventdeath/", Event = SubscribableEvent.All }, new PlayerDeathSubscriptionRequest() { CallbackUrl = $"http://{config.Address}:{config.Port}/playerdeath/" } };
        var subscriptionRequestIndex = 0;
        do
        {
            var msg = await APIHelper.SendRequestNoAuth(subscriptionRequests[subscriptionRequestIndex]);

            if (msg.IsSuccessStatusCode)
            {
                subscriptionRequestIndex++;
            }
            else
            {
                Console.WriteLine($"Could not subscribe to API... Retrying");
                await Task.Delay(500);
            }
        } while (subscriptionRequestIndex < subscriptionRequests.Count);

        await Task.Delay(-1);
    }

    private static void Receive()
    {
        _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
    }

    private static void ListenerCallback(IAsyncResult result)
    {
        if (_listener.IsListening)
        {
            var context = _listener.EndGetContext(result);
            var request = context.Request;
            var response = context.Response;

            Console.WriteLine($"{request.Url.LocalPath}");
            if (request.HasEntityBody)
            {
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var reader = new StreamReader(body, encoding);

                var requestJson = reader.ReadToEnd();
                reader.Close();
                body.Close();

                var requestHandler = RequestManager.GetHandlerFromUri(request.Url.LocalPath);
                if (requestHandler == null) return;
                if (requestHandler.Handle(requestJson))
                {
                    response.StatusCode = 200; // OK
                    response.StatusDescription = "OK";
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        writer.Write("Request processed successfully.");
                    }

                    response.Close();
                }
            }

            Receive();
        }
    }

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}