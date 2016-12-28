using UnityEngine;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    using Debug = UnityEngine.Debug;

    public class CreatedGameInfoSenderService : ExtendedMonoBehaviour
    {
        public const string GameInfoTag = "[CreatedGameInfo]";
        public const string SendGameInfoCommandTag = "[SendGameInfo]";

        public SimpleTcpClient TcpClient;
        public SimpleTcpServer TcpServer;
        public GameInfoFactory GameInfoFactory;

        void Start()
        {
            if (!this.TcpClient.Initialized)
            {
                this.TcpClient.Initialize();
            }

            if (!this.TcpServer.Initialized)
            {
                this.TcpServer.Initialize(7774);
            }

            this.TcpServer.OnReceivedMessage += this.OnReceivedMessage;

            Debug.Log("Created game info inizialized");
        }

        void OnReceivedMessage(object sender, MessageEventArgs args)
        {
            if (!args.Message.Contains(SendGameInfoCommandTag))
            {   
                return;
            }

            var gameInfo = this.GameInfoFactory.Get();
            var gameInfoJSON = JsonUtility.ToJson(gameInfo);
            var messageToSend = GameInfoTag + gameInfoJSON;

            this.TcpClient.ConnectTo(args.IPAddress, this.TcpServer.Port, () => this.TcpClient.Send(args.IPAddress, messageToSend));

            Debug.Log("Sending game info to " + args.IPAddress);
        }
    }

}

