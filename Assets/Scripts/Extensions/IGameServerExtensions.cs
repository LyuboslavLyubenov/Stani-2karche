namespace Assets.Scripts.Extensions
{

    using Assets.Scripts.Interfaces;

    public static class IGameServerExtensions
    {
        public static string GetGameTypeName(this IGameServer server)
        {
            return server.GetType()
                .Name.Replace("Server", "");
        }
    }
}
