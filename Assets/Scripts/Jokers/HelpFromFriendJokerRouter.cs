using UnityEngine;
using System;
using System.Linq;

public class HelpFromFriendJokerRouter : ExtendedMonoBehaviour, IJokerRouter
{
    public EventHandler OnActivated
    {
        get;
        set;
    }

    public EventHandler OnFinished
    {
        get;
        set;
    }

    public EventHandler<UnhandledExceptionEventArgs> OnError
    {
        get;
        set;
    }

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

    void Awake()
    {
        OnActivated = delegate
        {
        };

        OnFinished = delegate
        {
        };

        OnError = delegate
        {
        };
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
        var sendFriendResponseCommand = new NetworkCommandData("HelpFromFriendJokerResponse");
        var friendUsername = NetworkManager.GetClientUsername(connectionId);
        sendFriendResponseCommand.AddOption("Username", friendUsername);
        sendFriendResponseCommand.AddOption("Answer", answer);

        NetworkManager.SendClientCommand(senderConnectionId, sendFriendResponseCommand);

        OnFinished(this, EventArgs.Empty);

        Deactivate();
    }

    void UpdateTimer()
    {
        if (!Activated)
        {
            return;
        }

        elapsedTime++;

        if (elapsedTime < timeToAnswerInSeconds)
        {
        }
        else
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
                var sendQuestionToFriend = new NetworkCommandData("LoadQuestion");
                var questionJSON = JsonUtility.ToJson(question);
                sendQuestionToFriend.AddOption("TimeToAnswer", LocalGameData.SecondsForAnswerQuestion.ToString());
                sendQuestionToFriend.AddOption("QuestionJSON", questionJSON);

                NetworkManager.SendClientCommand(friendConnectionId, sendQuestionToFriend);
                NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedSelectedAnswerOneTimeCommand(OnReceivedFriendResponse));

                base.CoroutineUtils.RepeatEverySeconds(1f, UpdateTimer);

                OnActivated(this, EventArgs.Empty);

                activated = true;
            }, (exception) =>
            {
                Debug.LogException(exception);
                Deactivate();
                OnError(this, new UnhandledExceptionEventArgs(exception, true));
            });
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

        var settingsCommandData = new NetworkCommandData("HelpFromFriendJokerSettings");
        settingsCommandData.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());
        NetworkManager.SendClientCommand(senderConnectionId, settingsCommandData);

        var remainingTimeCommand = new NetworkCommandData("RemainingTime");
        remainingTimeCommand.AddOption("TimeInSeconds", LocalGameData.SecondsForAnswerQuestion.ToString());
        NetworkManager.SendClientCommand(friendConnectionId, remainingTimeCommand);

        if (friendConnectionId == NetworkCommandData.CODE_Option_ClientConnectionId_AI)
        {
            SendComputerGeneratedAnswer();
        }
        else
        {
            SendQuestionToFriend();    
        }
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