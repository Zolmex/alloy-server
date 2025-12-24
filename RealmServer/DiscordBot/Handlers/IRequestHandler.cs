namespace DiscordBot.Handlers;

public interface IRequestHandler
{
    bool Handle(string requestJson);
}

[AttributeUsage(AttributeTargets.Class)]
public class RequestAttribute : Attribute
{
    public RequestAttribute(string uri)
    {
        Uri = uri;
    }

    public string Uri { get; }
}