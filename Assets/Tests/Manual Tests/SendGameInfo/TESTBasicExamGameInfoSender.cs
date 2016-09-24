public class TESTBasicExamGameInfoSender : ExtendedMonoBehaviour
{
    public const int Port = 4444;

    public CreatedGameInfoSenderService senderService;

    void Start()
    {
        senderService.TcpClient.Initialize();
        senderService.TcpServer.Initialize(Port);
    }
}
