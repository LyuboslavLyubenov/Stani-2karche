namespace Assets.Scripts.Controllers.GameController
{

    using System;
    using System.Collections;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Client;
    using Assets.Scripts.Commands.Jokers.Add;
    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Controllers.Jokers;
    using Assets.Scripts.DialogSwitchers;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Network;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils.Unity;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Debug = UnityEngine.Debug;
    using EventArgs = System.EventArgs;

    public class BasicExamMainPlayerController : ExtendedMonoBehaviour
    {
        const string ServerBinaryName = "stani2karcheserver";

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

        public ClientNetworkManager NetworkManager;

        public BasicExamPlayerTeacherDialogSwitcher DialogSwitcher;
        public BasicExamPlayerTutorialUIController TutorialUIController;

        public NotificationsServiceController NotificationService;

        public AvailableJokersUIController AvailableJokersUIController;
        public QuestionUIController QuestionUIController;
        public MarkPanelController MarkPanelController;
        public QuestionsRemainingUIController QuestionsRemainingUIController;
        public ClientChooseCategoryUIController ChooseCategoryUIController;
        public SecondsRemainingUIController SecondsRemainingUIController;

        public SelectRandomJokerUIController SelectRandomJokerUIController;

        public RemoteGameDataIterator RemoteGameDataIterator;

        //IGameData gameData;

        UnableToConnectUIController unableToConnectUIController;

        void Start()
        {
            throw new NotImplementedException();

            PlayerPrefs.DeleteKey("LoadedGameData");

            //this.gameData = new RemoteGameData(this.NetworkManager);

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

        void OnFoundServerIP(string ip)
        {
            try
            {
                this.NetworkManager.ConnectToHost(ip);
                this.unableToConnectUIController.ServerIP = ip;
            }
            catch
            {
                this.OnFoundServerIPError();
            }
        }

        void OnFoundServerIPError()
        {
            this.CoroutineUtils.WaitForSeconds(1f, this.ConnectToServer);
        }

        void ConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }

        void InitializeCommands()
        {
            this.NetworkManager.CommandsManager.AddCommand(new BasicExamGameEndCommand(this.EndGameUI, this.LeaderboardUI));
            this.NetworkManager.CommandsManager.AddCommand(new AddHelpFromFriendJokerCommand(this.AvailableJokersUIController, this.NetworkManager, this.CallAFriendUI, this.FriendAnswerUI, this.WaitingToAnswerUI, this.LoadingUI));
            this.NetworkManager.CommandsManager.AddCommand(new AddAskAudienceJokerCommand(this.AvailableJokersUIController, this.NetworkManager, this.WaitingToAnswerUI, this.AudienceAnswerUI, this.LoadingUI, this.NotificationService));
            this.NetworkManager.CommandsManager.AddCommand(new AddDisableRandomAnswersJokerCommand(this.AvailableJokersUIController, this.NetworkManager, this.QuestionUIController));
            this.NetworkManager.CommandsManager.AddCommand(new AddRandomJokerCommand(this.SelectRandomJokerUIController, this.NetworkManager));
        }
        
        void OnLoadedGameData(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.ChooseCategoryUIController.gameObject.SetActive(false);
            this.QuestionUIController.HideAllAnswers();
            this.SecondsRemainingUIController.Paused = false;
            this.RemoteGameDataIterator.GetCurrentQuestion(this.QuestionUIController.LoadQuestion, Debug.LogException);

            PlayerPrefs.SetString("LoadedGameData", "true");
        }

        void AttachEventHandlers()
        {
            this.NetworkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.NetworkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;

            this.QuestionUIController.OnAnswerClick += this.OnAnswerClick;
            this.QuestionUIController.OnQuestionLoaded += this.OnQuestionLoaded;

            this.RemoteGameDataIterator.OnMarkIncrease += this.OnMarkIncrease;
            this.RemoteGameDataIterator.OnLoaded += this.OnLoadedGameData;

            this.ChooseCategoryUIController.OnLoadedCategories += (sender, args) => this.LoadingUI.SetActive(false);
            this.ChooseCategoryUIController.OnChoosedCategory += this.OnChoosedCategory;

            this.unableToConnectUIController.OnTryingAgainToConnectToServer += (s, a) => this.LoadingUI.SetActive(true);

            this.AvailableJokersUIController.OnAddedJoker += this.OnAddedJoker;
            this.AvailableJokersUIController.OnUsedJoker += this.OnUsedJoker;
        }

        void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.MarkChangedConfetti.SetActive(true);
            this.MarkPanelController.SetMark(args.Mark.ToString());
        }

        void OnChoosedCategory(object sender, ChoosedCategoryEventArgs args)
        {
            var selectedCategoryCommand = new NetworkCommandData("SelectedCategory");
            selectedCategoryCommand.AddOption("Category", args.Name);
            this.NetworkManager.SendServerCommand(selectedCategoryCommand);
        }

        void OnAddedJoker(object sender, JokerEventArgs args)
        {
            args.Joker.OnFinishedExecution += this.OnFinishedExecutionJoker;
        }

        void OnFinishedExecutionJoker(object sender, EventArgs args)
        {
            this.SecondsRemainingUIController.Paused = false;
            var joker = (IJoker)sender;
            joker.OnFinishedExecution -= this.OnFinishedExecutionJoker;
        }

        void OnUsedJoker(object sender, JokerEventArgs args)
        {
            var jokerTypeNameUpper = args.Joker.GetType().Name.ToUpperInvariant();

            if (jokerTypeNameUpper == typeof(DisableRandomAnswersJoker).Name.ToUpperInvariant())
            {
                return;
            }

            this.SecondsRemainingUIController.Paused = true;  
        }

        void OnQuestionLoaded(object sender, SimpleQuestionEventArgs args)
        {
            this.QuestionsRemainingUIController.SetRemainingQuestions(this.RemoteGameDataIterator.RemainingQuestionsToNextMark);
            this.SecondsRemainingUIController.SetSeconds(this.RemoteGameDataIterator.SecondsForAnswerQuestion);
        }

        void OnActivateSceneChanged(Scene oldScene, Scene newScene)
        {
            this.KillLocalServer();
            this.CleanUp();
            SceneManager.activeSceneChanged -= this.OnActivateSceneChanged;
        }

        void OnApplicationQuit()
        {
            this.KillLocalServer();
            this.CleanUp();
        }

        void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.ChooseCategoryUIController.gameObject.SetActive(false);
            this.LoadingUI.SetActive(false);
            this.SecondsRemainingUIController.Paused = true;
            this.UnableToConnectUI.SetActive(true);   
        }

        void OnConnectedToServer(object sender, EventArgs args)
        {
            this.AvailableJokersUIController.ClearAll();

            this.LoadingUI.SetActive(false);
            this.UnableToConnectUI.SetActive(false);   
            this.ChooseCategoryUIController.gameObject.SetActive(false);

            var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
            this.NetworkManager.SendServerCommand(commandData);

            if (PlayerPrefs.HasKey("LoadedGameData"))
            {
                var loadedGameData = NetworkCommandData.From<LoadedGameDataCommand>();
                loadedGameData.AddOption("LevelCategory", this.RemoteGameDataIterator.LevelCategory);
                this.NetworkManager.CommandsManager.Execute(loadedGameData);
                return;
            }

            this.ChooseCategoryUIController.gameObject.SetActive(true);
            this.StartLoadingCategories();
        }

        void OnAnswerClick(object sender, AnswerEventArgs args)
        {
            this.StartCoroutine(this.OnAnswerClickCoroutine(args.Answer, args.IsCorrect));
        }

        IEnumerator OnAnswerClickCoroutine(string answer, bool isCorrect)
        {
            var commandData = new NetworkCommandData("AnswerSelected");
            commandData.AddOption("Answer", answer);

            this.NetworkManager.SendServerCommand(commandData);

            yield return null;

            if (isCorrect)
            {
                this.RemoteGameDataIterator.GetCurrentQuestion(this.QuestionUIController.LoadQuestion, Debug.LogException);
            }
            else
            {
                this.AvailableJokersUIController.ClearAll();
            }
        }

        void ShowNotification(Color color, string message)
        {
            if (this.NotificationService != null)
            {
                this.NotificationService.AddNotification(color, message);
            }
        }

        void StartLoadingCategories()
        {
            var remoteCategoriesReader = new RemoteAvailableCategoriesReader(this.NetworkManager, () =>
                {
                    var errorMsg = LanguagesManager.Instance.GetValue("Errors/CantLoadCategories");
                    Debug.LogError(errorMsg);
                    this.ShowNotification(Color.red, errorMsg);

                    this.ChooseCategoryUI.SetActive(false);
                    this.NetworkManager.Disconnect();
                }, 10);
            
            this.ChooseCategoryUIController.gameObject.SetActive(true);
            this.ChooseCategoryUIController.Initialize(remoteCategoriesReader);
        }
        
        void StartServerIfPlayerIsHost()
        {
            if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
            {
                var serverPath = string.Format("Server\\{0}.exe", ServerBinaryName);
                System.Diagnostics.Process.Start(serverPath);
                SceneManager.activeSceneChanged += this.OnActivateSceneChanged;
            }
        }

        void CleanUp()
        {
            PlayerPrefs.DeleteKey("LoadedGameData");
            PlayerPrefsEncryptionUtils.DeleteKey("MainPlayerHost");
        }

        void KillLocalServer()
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
    }

}