namespace DiscordBot.Handlers
{
    public interface IRequestHandler
    {
        public bool Handle(string requestJson);
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class RequestAttribute : Attribute
    {
        private string _uri;
        public string Uri => _uri;

        public RequestAttribute(string uri)
        {
            _uri = uri;
        }
    }
}