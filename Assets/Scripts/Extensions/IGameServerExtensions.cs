namespace Extensions
{

    using Interfaces;

    public static class IGameServerExtensions
    {
        public static string GetGameTypeName(this IGameServer server)
        {
            return server.GetType()
                .Name.Replace("Server", "");
        }
    }
}
