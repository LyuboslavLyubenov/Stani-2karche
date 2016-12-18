using UnityEngine;
using System;
using System.Linq;

public class AskPlayerQuestionRouter : ExtendedMonoBehaviour
{
    public const int MinTimeToAnswerInSeconds = 10;

    const float ChanceToGenerateCorrectAnswer = 0.8f;

    const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
    const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;

    public ServerNetworkManager NetworkManager;
    public LocalGameData LocalGameData;

    int timeToAnswerInSeconds;
    int senderConnectionId;
    int friendConnectionId;
    int elapsedTime;

    bool activated = false;

    public bool Activated
    {
        get
        {
            return activated;
        }
    }

    public EventHandler OnActivated = delegate
    {
    };

    public EventHandler OnSent = delegate
    {
    };

    public EventHandler<UnhandledExceptionEventArgs> OnError = delegate
    {
    };

    void Start()
    {
        base.CoroutineUtils.RepeatEverySeconds(1f, UpdateTimer);
    }

    void OnReceivedFriendResponse(int connectionId, string answer)
    {
        if (connectionId != friendConnectionId)
        {
            NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedSelectedAnswerOneTimeCommand(OnReceivedFriendResponse));
            return;
        }

        if (elapsedTime >= timeToAnswerInSeconds || !NetworkManager.IsConnected(senderConnectionId))
        {
            return;
        }

        SendMainPlayerAnswerResponse(connectionId, answer);
    }

    void SendMainPlayerAnswerResponse(int connectionId, string answer)
    {
        var sendFriendResponseCommand = NetworkCommandData.From<AskPlayerResponseCommand>();
        var friendUsername = NetworkManager.GetClientUsername(connectionId);
        sendFriendResponseCommand.AddOption("Username", friendUsername);
        sendFriendResponseCommand.AddOption("Answer", answer);

        NetworkManager.SendClientCommand(senderConnectionId, sendFriendResponseCommand);

        OnSent(this, EventArgs.Empty);

        Deactivate();
    }

    void UpdateTimer()
    {
        if (!Activated)
        {
            return;
        }

        elapsedTime++;

        if (elapsedTime >= timeToAnswerInSeconds)
        {
            Deactivate();

            var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");
            NetworkManager.SendClientCommand(senderConnectionId, answerTimeoutCommandData);
            NetworkManager.SendClientCommand(friendConnectionId, answerTimeoutCommandData);
        }
    }

    void SendComputerGeneratedAnswer()
    {
        var secondsToWait = UnityEngine.Random.Range(MinTimeInSecondsToSendGeneratedAnswer, MaxTimeInSecondsToSendGeneratedAnswer);

        CoroutineUtils.WaitForSeconds(secondsToWait, () =>
            {
                LocalGameData.GetCurrentQuestion((question) =>
                    {
                        var shouldSendCorrect = UnityEngine.Random.value <= ChanceToGenerateCorrectAnswer;
                        var correctAnswer = question.Answers[question.CorrectAnswerIndex];

                        if (shouldSendCorrect)
                        {
                            SendMainPlayerAnswerResponse(NetworkCommandData.CODE_Option_ClientConnectionId_AI, correctAnswer);    
                        }
                        else
                        {
                            var rndWrongAnswer = question.Answers.Where(a => a != correctAnswer).ToArray().GetRandomElement();
                            SendMainPlayerAnswerResponse(NetworkCommandData.CODE_Option_ClientConnectionId_AI, rndWrongAnswer);
                        }
                    }, (exception) =>
                    {
                        Debug.LogException(exception);
                        Deactivate();
                        OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });
               
            });
    }

    void SendQuestionToFriend()
    {
        LocalGameData.GetCurrentQuestion((question) =>
            {
                var sendQuestionToFriend = NetworkCommandData.From<LoadQuestionCommand>(); 
                var questionJSON = JsonUtility.ToJson(question.Serialize());
                sendQuestionToFriend.AddOption("TimeToAnswer", LocalGameData.SecondsForAnswerQuestion.ToString());
                sendQuestionToFriend.AddOption("QuestionJSON", questionJSON);

                NetworkManager.SendClientCommand(friendConnectionId, sendQuestionToFriend);
                NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedSelectedAnswerOneTimeCommand(OnReceivedFriendResponse));
            }, (exception) =>
            {
                Debug.LogException(exception);
                Deactivate();
                OnError(this, new UnhandledExceptionEventArgs(exception, true));
            });
    }

    void SendJokerSettings(int connectionId)
    {
        var settingsCommandData = NetworkCommandData.From<HelpFromFriendJokerSettingsCommand>();
        settingsCommandData.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());
        NetworkManager.SendClientCommand(connectionId, settingsCommandData);
    }

    public void Activate(int timeToAnswerInSeconds, int senderConnectionId, int friendConnectionId)
    {
        if (Activated)
        {
            throw new InvalidOperationException("Already active");
        }

        if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
        {
            throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
        }

        this.senderConnectionId = senderConnectionId;
        this.friendConnectionId = friendConnectionId;
        this.timeToAnswerInSeconds = LocalGameData.SecondsForAnswerQuestion;

        SendJokerSettings(senderConnectionId);

        if (friendConnectionId == NetworkCommandData.CODE_Option_ClientConnectionId_AI)
        {
            SendComputerGeneratedAnswer();
        }
        else
        {
            SendQuestionToFriend();    
        }

        OnActivated(this, EventArgs.Empty);
        activated = true;
    }

    public void Deactivate()
    {
        StopAllCoroutines();

        timeToAnswerInSeconds = 0;
        senderConnectionId = 0;
        friendConnectionId = 0;
        elapsedTime = 0;

        activated = false;
    }
}