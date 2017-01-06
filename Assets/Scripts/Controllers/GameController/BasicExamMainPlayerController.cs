namespace Assets.Scripts.Controllers.GameController
{
    using System;
    using System.Collections;

    using Assets.Scripts.Utils;

    using Scripts.Jokers.AskPlayerQuestion;
    using Network.Leaderboard;

    using Commands;
    using Commands.Client;
    using Commands.GameData;
    using Commands.Jokers.Add;
    using Commands.Server;
    using Jokers;
    using DialogSwitchers;
    using EventArgs;
    using Interfaces;
    using Scripts.Jokers;
    using Scripts.Jokers.AudienceAnswerPoll;
    using Localization;
    using Network;
    using Network.NetworkManagers;
    using Notifications;
    using Utils.Unity;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class BasicExamMainPlayerController : ExtendedMonoBehaviour, IDisposable
    {
        private const string ServerBinaryName = "stani2karcheserver";

        public GameObject LeaderboardUI;
        public GameObject LoadingUI;
        public GameObject EndGameUI;
        public GameObject CallAFriendUI;
        public GameObject FriendAnswerUI;
        public GameObject WaitingToAnswerUI;
        public GameObject AudienceAnswerUI;
        public GameObject UnableToConnectUI;
        public GameObject ChooseCategoryUI;
        public GameObject MarkChangedConfetti;
        
        public BasicExamPlayerTeacherDialogSwitcher DialogSwitcher;
        public BasicExamPlayerTutorialUIController TutorialUIController;
        public AvailableJokersUIController AvailableJokersUIController;
        public QuestionUIController QuestionUIController;
        public MarkPanelController MarkPanelController;
        public QuestionsRemainingUIController QuestionsRemainingUIController;
        public ClientChooseCategoryUIController ChooseCategoryUIController;
        public SecondsRemainingUIController SecondsRemainingUIController;
        public SelectRandomJokerUIController SelectRandomJokerUIController;

        private UnableToConnectUIController unableToConnectUIController;

        private RemoteGameDataIterator remoteGameDataIterator = null;
        private AudienceAnswerPollResultRetriever audienceAnswerPollResultRetriever = null;
        private AskPlayerQuestionResultRetriever askPlayerQuestionResultRetriever = null;

        private LeaderboardReceiver leaderboardReceiver = null;
        
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            PlayerPrefs.DeleteKey("LoadedGameData");

            var threadUtils = ThreadUtils.Instance;

            this.remoteGameDataIterator = new RemoteGameDataIterator(ClientNetworkManager.Instance);
            this.audienceAnswerPollResultRetriever = new AudienceAnswerPollResultRetriever(ClientNetworkManager.Instance, 5);
            this.askPlayerQuestionResultRetriever = new AskPlayerQuestionResultRetriever(ClientNetworkManager.Instance, 5);
            this.leaderboardReceiver = new LeaderboardReceiver(ClientNetworkManager.Instance, 5);

            this.unableToConnectUIController = this.UnableToConnectUI.GetComponent<UnableToConnectUIController>();

            this.InitializeCommands();
            this.AttachEventHandlers();
            this.StartServerIfPlayerIsHost();

            if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
            {
                PlayerPrefsEncryptionUtils.SetString("ServerLocalIP", "127.0.0.1");
                //wait until server is loaded. starting the server takes about ~7 seconds on i7 + SSD.
                this.CoroutineUtils.WaitForSeconds(9f, this.ConnectToServer);
            }
            else
            {
                this.ConnectToServer();
            }

            this.LoadingUI.SetActive(true);
        }

        private void OnFoundServerIP(string ip)
        {
            try
            {
                ClientNetworkManager.Instance.ConnectToHost(ip);
                this.unableToConnectUIController.ServerIP = ip;
            }
            catch
            {
                this.OnFoundServerIPError();
            }
        }

        private void OnFoundServerIPError()
        {
            this.CoroutineUtils.WaitForSeconds(1f, this.ConnectToServer);
        }

        private void ConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }

        private void InitializeCommands()
        {
            var networkManager = ClientNetworkManager.Instance;

            networkManager.CommandsManager.AddCommand(new BasicExamGameEndCommand(this.EndGameUI, this.LeaderboardUI, this.leaderboardReceiver));
            networkManager.CommandsManager.AddCommand(new AddHelpFromFriendJokerCommand(this.AvailableJokersUIController, networkManager, this.askPlayerQuestionResultRetriever, this.CallAFriendUI, this.FriendAnswerUI, this.WaitingToAnswerUI, this.LoadingUI));
            networkManager.CommandsManager.AddCommand(new AddAskAudienceJokerCommand(this.AvailableJokersUIController, networkManager, this.audienceAnswerPollResultRetriever, this.WaitingToAnswerUI, this.AudienceAnswerUI, this.LoadingUI));
            networkManager.CommandsManager.AddCommand(new AddDisableRandomAnswersJokerCommand(this.AvailableJokersUIController, networkManager, this.QuestionUIController));
            networkManager.CommandsManager.AddCommand(new AddRandomJokerCommand(this.SelectRandomJokerUIController, networkManager));
        }

        private void OnLoadedGameData(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.ChooseCategoryUIController.gameObject.SetActive(false);
            this.QuestionUIController.HideAllAnswers();
            this.SecondsRemainingUIController.Paused = false;
            this.remoteGameDataIterator.GetCurrentQuestion(this.QuestionUIController.LoadQuestion, Debug.LogException);

            PlayerPrefs.SetString("LoadedGameData", "true");
        }

        private void AttachEventHandlers()
        {
            var networkManager = ClientNetworkManager.Instance;

            networkManager.OnConnectedEvent += this.OnConnectedToServer;
            networkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;

            this.QuestionUIController.OnAnswerClick += this.OnAnswerClick;
            this.QuestionUIController.OnQuestionLoaded += this.OnQuestionLoaded;

            this.remoteGameDataIterator.OnMarkIncrease += this.OnMarkIncrease;
            this.remoteGameDataIterator.OnLoaded += this.OnLoadedGameData;

            this.ChooseCategoryUIController.OnLoadedCategories += (sender, args) => this.LoadingUI.SetActive(false);
            this.ChooseCategoryUIController.OnChoosedCategory += this.OnChoosedCategory;

            this.unableToConnectUIController.OnTryingAgainToConnectToServer += (s, a) => this.LoadingUI.SetActive(true);

            this.AvailableJokersUIController.OnAddedJoker += this.OnAddedJoker;
            this.AvailableJokersUIController.OnUsedJoker += this.OnUsedJoker;
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.MarkChangedConfetti.SetActive(true);
            this.MarkPanelController.SetMark(args.Mark.ToString());
        }

        private void OnChoosedCategory(object sender, ChoosedCategoryEventArgs args)
        {
            var selectedCategoryCommand = NetworkCommandData.From<SelectedCategoryCommand>();
            selectedCategoryCommand.AddOption("Category", args.Name);
            ClientNetworkManager.Instance.SendServerCommand(selectedCategoryCommand);
        }

        private void OnAddedJoker(object sender, JokerEventArgs args)
        {
            args.Joker.OnFinishedExecution += this.OnFinishedExecutionJoker;
        }

        private void OnFinishedExecutionJoker(object sender, EventArgs args)
        {
            this.SecondsRemainingUIController.Paused = false;
            var joker = (IJoker)sender;
            joker.OnFinishedExecution -= this.OnFinishedExecutionJoker;
        }

        private void OnUsedJoker(object sender, JokerEventArgs args)
        {
            var jokerTypeNameUpper = args.Joker.GetType().Name.ToUpperInvariant();

            if (jokerTypeNameUpper == typeof(DisableRandomAnswersJoker).Name.ToUpperInvariant())
            {
                return;
            }

            this.SecondsRemainingUIController.Paused = true;  
        }

        private void OnQuestionLoaded(object sender, SimpleQuestionEventArgs args)
        {
            this.QuestionsRemainingUIController.SetRemainingQuestions(this.remoteGameDataIterator.RemainingQuestionsToNextMark);
            this.SecondsRemainingUIController.SetSeconds(this.remoteGameDataIterator.SecondsForAnswerQuestion);
        }

        private void OnActivateSceneChanged(Scene oldScene, Scene newScene)
        {
            this.KillLocalServer();
            this.Dispose();
            SceneManager.activeSceneChanged -= this.OnActivateSceneChanged;
        }

        private void OnApplicationQuit()
        {
            this.KillLocalServer();
            this.Dispose();
        }

        private void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.ChooseCategoryUIController.gameObject.SetActive(false);
            this.LoadingUI.SetActive(false);
            this.SecondsRemainingUIController.Paused = true;
            this.UnableToConnectUI.SetActive(true);   
        }

        private void OnConnectedToServer(object sender, EventArgs args)
        {
            this.AvailableJokersUIController.ClearAll();

            this.LoadingUI.SetActive(false);
            this.UnableToConnectUI.SetActive(false);   
            this.ChooseCategoryUIController.gameObject.SetActive(false);

            var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
            ClientNetworkManager.Instance.SendServerCommand(commandData);

            if (PlayerPrefs.HasKey("LoadedGameData"))
            {
                var loadedGameData = NetworkCommandData.From<LoadedGameDataCommand>();
                loadedGameData.AddOption("LevelCategory", this.remoteGameDataIterator.LevelCategory);
                ClientNetworkManager.Instance.CommandsManager.Execute(loadedGameData);
                return;
            }

            this.ChooseCategoryUIController.gameObject.SetActive(true);
            this.StartLoadingCategories();
        }

        private void OnAnswerClick(object sender, AnswerEventArgs args)
        {
            this.StartCoroutine(this.OnAnswerClickCoroutine(args.Answer, args.IsCorrect));
        }

        private IEnumerator OnAnswerClickCoroutine(string answer, bool isCorrect)
        {
            var commandData = new NetworkCommandData("AnswerSelected");
            commandData.AddOption("Answer", answer);

            ClientNetworkManager.Instance.SendServerCommand(commandData);

            yield return null;

            if (isCorrect)
            {
                this.remoteGameDataIterator.GetCurrentQuestion(this.QuestionUIController.LoadQuestion, Debug.LogException);
            }
            else
            {
                this.AvailableJokersUIController.ClearAll();
            }
        }

        private void ShowNotification(Color color, string message)
        {
            NotificationsServiceController.Instance.AddNotification(color, message);
        }

        private void StartLoadingCategories()
        {
            var remoteCategoriesReader = new RemoteAvailableCategoriesReader(ClientNetworkManager.Instance, () =>
                {
                    var errorMsg = LanguagesManager.Instance.GetValue("Errors/CantLoadCategories");
                    Debug.LogError(errorMsg);
                    this.ShowNotification(Color.red, errorMsg);

                    this.ChooseCategoryUI.SetActive(false);
                    ClientNetworkManager.Instance.Disconnect();
                }, 10);
            
            this.ChooseCategoryUIController.gameObject.SetActive(true);
            this.ChooseCategoryUIController.Initialize(remoteCategoriesReader);
        }

        private void StartServerIfPlayerIsHost()
        {
            if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
            {
                var serverPath = string.Format("Server\\{0}.exe", ServerBinaryName);
                System.Diagnostics.Process.Start(serverPath);
                SceneManager.activeSceneChanged += this.OnActivateSceneChanged;
            }
        }
        
        private void KillLocalServer()
        {
            if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
            {
                var serverProcesses = System.Diagnostics.Process.GetProcessesByName(ServerBinaryName);

                for (int i = 0; i < serverProcesses.Length; i++)
                {
                    serverProcesses[i].Kill();
                }
            }
        }

        public void Dispose()
        {
            this.audienceAnswerPollResultRetriever.Dispose();
            this.askPlayerQuestionResultRetriever.Dispose();
            this.leaderboardReceiver.Dispose();

            this.remoteGameDataIterator = null;
            this.audienceAnswerPollResultRetriever = null;
            this.askPlayerQuestionResultRetriever = null;
            this.leaderboardReceiver = null;

            PlayerPrefs.DeleteKey("LoadedGameData");
            PlayerPrefsEncryptionUtils.DeleteKey("MainPlayerHost");
        }
    }
}