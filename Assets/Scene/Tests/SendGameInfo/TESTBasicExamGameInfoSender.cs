public class TESTBasicExamGameInfoSender : ExtendedMonoBehaviour
{
    const int Port = 4444;

    public CreatedGameInfoSenderService Sender;
    public ServerNetworkManager ServerNetworkManager;
    public P2PSocket P2PSocket;

    void Start()
    {
        ServerNetworkManager.StartServer();
        P2PSocket.Initialize(Port);
    }
}
