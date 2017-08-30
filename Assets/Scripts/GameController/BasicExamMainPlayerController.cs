using AskPlayerQuestionResultRetriever = Jokers.Retrievers.AskPlayerQuestionResultRetriever;
using AudienceAnswerPollResultRetriever = Jokers.Retrievers.AudienceAnswerPollResultRetriever;
using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;

namespace GameController
{

    using System;
    using System.Collections;

    using Assets.Scripts.Interfaces;

    using Commands;
    using Commands.Client;
    using Commands.GameData;
    using Commands.Jokers.Add;
    using Commands.Server;

    using Controllers;
    using Controllers.Jokers;

    using DialogSwitchers;

    using EventArgs;

    using Interfaces.GameData;
    using Interfaces.Network.Jokers;
    using Interfaces.Network.Leaderboard;

    using Localization;

    using Network;
    using Network.Leaderboard;
    using Network.NetworkManagers;

    using Notifications;

    using Scripts.Utils;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Utils;
    using Utils.Unity;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class BasicExamMainPlayerController : ExtendedMonoBehaviour, IDisposable
    {
        private const string ServerBinaryName = "server";

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

        private IGameDataIterator remoteGameDataIterator = null;
        private IAnswerPollResultRetriever audienceAnswerPollResultRetriever = null;
        private IAskClientQuestionResultRetriever askClientQuestionResultRetriever = null;

        private ILeaderboardReceiver leaderboardReceiver = null;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            PlayerPrefs.DeleteKey("LoadedGameData");

            var threadUtils = ThreadUtils.Instance;

            this.remoteGameDataIterator = new RemoteGameDataIterator(ClientNetworkManager.Instance);
            this.audienceAnswerPollResultRetriever = new AudienceAnswerPollResultRetriever(ClientNetworkManager.Instance, 10);
            this.askClientQuestionResultRetriever = new AskPlayerQuestionResultRetriever(ClientNetworkManager.Instance, 10);
            this.leaderboardReceiver = new LeaderboardReceiver(ClientNetworkManager.Instance, 10);

            this.unableToConnectUIController = this.UnableToConnectUI.GetComponent<UnableToConnectUIController>();

            this.InitializeCommands();
            this.AttachEventHandlers();

            if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
            {
                PlayerPrefsEncryptionUtils.DeleteKey("MainPlayerHost");

                //wait until server is loaded. starting the server takes about ~7 seconds on i7 + SSD.
                this.CoroutineUtils.WaitForSeconds(9f, () => this.ConnectToServer("127.0.0.1"));
                
                GameServerUtils.StartServer("BasicExam");
            }
            else
            {
                this.FindServerIpAndConnectToServer();
            }

            this.LoadingUI.SetActive(true);
        }

        private void OnFoundServerIP(string ip)
        {
            this.ConnectToServer(ip);
        }

        private void OnFoundServerIPError()
        {
            this.CoroutineUtils.WaitForSeconds(1f, this.FindServerIpAndConnectToServer);
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
        
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;

            var networkManager = ClientNetworkManager.Instance;

            networkManager.OnConnectedEvent -= this.OnConnectedToServer;
            networkManager.OnDisconnectedEvent -= this.OnDisconnectedFromServer;

            this.KillLocalServer();
            this.Dispose();
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

            if (this.SecondsRemainingUIController.Running)
            {
                this.SecondsRemainingUIController.StopTimer();
            }
            
            this.SecondsRemainingUIController.InvervalInSeconds = this.remoteGameDataIterator.SecondsForAnswerQuestion;
            this.SecondsRemainingUIController.StartTimer();
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

        private IEnumerator OnAnswerClickCoroutine(string answer, bool? isCorrect)
        {
            var commandData = new NetworkCommandData("AnswerSelected");
            commandData.AddOption("Answer", answer);

            ClientNetworkManager.Instance.SendServerCommand(commandData);

            yield return new WaitForSeconds(0.25f);

            if (isCorrect == null)
            {
                yield break;
            }

            if (isCorrect.Value)
            {
                this.remoteGameDataIterator.GetNextQuestion(this.QuestionUIController.LoadQuestion, Debug.LogException);
            }
            else
            {
                this.AvailableJokersUIController.ClearAll();
            }
        }

        private void OnLoadedCategories(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(false);
        }

        private void OnTryingAgainToConnectToServer(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(true);
        }

        private void ConnectToServer(string ip)
        {
            try
            {
                ClientNetworkManager.Instance.ConnectToHost(ip);
                this.unableToConnectUIController.ServerIP = ip;
            }
            catch
            {
                ClientNetworkManager.Instance.Disconnect();//just in case
                this.OnFoundServerIPError();
            }
        }
        
        private void FindServerIpAndConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }

        private void InitializeCommands()
        {
            var networkManager = ClientNetworkManager.Instance;

            networkManager.CommandsManager.AddCommand(new GameEndCommand(this.EndGameUI, this.LeaderboardUI, this.leaderboardReceiver));
            networkManager.CommandsManager.AddCommand(new AddHelpFromFriendJokerCommand(this.AvailableJokersUIController, networkManager, this.askClientQuestionResultRetriever, this.CallAFriendUI, this.FriendAnswerUI, this.WaitingToAnswerUI, this.LoadingUI));
            networkManager.CommandsManager.AddCommand(new AddAskAudienceJokerCommand(this.AvailableJokersUIController, this.audienceAnswerPollResultRetriever, this.WaitingToAnswerUI, this.AudienceAnswerUI, this.LoadingUI));
            networkManager.CommandsManager.AddCommand(new AddDisableRandomAnswersJokerCommand(this.AvailableJokersUIController, networkManager, this.QuestionUIController));
            networkManager.CommandsManager.AddCommand(new AddRandomJokerCommand(this.SelectRandomJokerUIController));
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

            this.ChooseCategoryUIController.OnLoadedCategories += this.OnLoadedCategories;
            this.ChooseCategoryUIController.OnChoosedCategory += this.OnChoosedCategory;

            this.unableToConnectUIController.OnTryingAgainToConnectToServer += this.OnTryingAgainToConnectToServer;

            this.AvailableJokersUIController.OnAddedJoker += this.OnAddedJoker;
            this.AvailableJokersUIController.OnUsedJoker += this.OnUsedJoker;

            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }

        private void ShowNotification(Color color, string message)
        {
            NotificationsController.Instance.AddNotification(color, message);
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

        private void KillLocalServer()
        {
            var serverProcesses = System.Diagnostics.Process.GetProcessesByName(ServerBinaryName);

            for (int i = 0; i < serverProcesses.Length; i++)
            {
                serverProcesses[i].Kill();
            }
        }

        public void Dispose()
        {
            this.audienceAnswerPollResultRetriever.Dispose();
            this.askClientQuestionResultRetriever.Dispose();
            this.leaderboardReceiver.Dispose();

            ClientNetworkManager.Instance.CommandsManager.RemoveAllCommands();
            ClientNetworkManager.Instance.Dispose();

            this.remoteGameDataIterator = null;
            this.audienceAnswerPollResultRetriever = null;
            this.askClientQuestionResultRetriever = null;
            this.leaderboardReceiver = null;

            Resources.UnloadUnusedAssets();
            GC.Collect();

            PlayerPrefs.DeleteKey("LoadedGameData");
            PlayerPrefsEncryptionUtils.DeleteKey("MainPlayerHost");
            PlayerPrefsEncryptionUtils.DeleteKey("ServerLocalIP");
            PlayerPrefsEncryptionUtils.DeleteKey("ServerExternalIP");
        }
    }
}