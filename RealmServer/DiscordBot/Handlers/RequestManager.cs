#region

using System.Reflection;

#endregion

namespace DiscordBot.Handlers
{
    public class RequestManager
    {
        private static Dictionary<string, IRequestHandler> _requestHandlers = new();

        public static void Load()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && typeof(IRequestHandler).IsAssignableFrom(type))
                {
                    var request = (IRequestHandler)Activator.CreateInstance(type);
                    var requestAttribute = Attribute.GetCustomAttribute(type, typeof(RequestAttribute)) as RequestAttribute;
                    _requestHandlers.Add(string.Format("/{0}/", requestAttribute.Uri), request);
                }
            }
        }

        public static IRequestHandler GetHandlerFromUri(string uri)
        {
            if (_requestHandlers.ContainsKey(uri))
                return _requestHandlers[uri];
            return null;
        }
    }
}