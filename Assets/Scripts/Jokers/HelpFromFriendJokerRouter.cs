using UnityEngine;
using System;

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
            var remainingTimeInSeconds = timeToAnswerInSeconds - elapsedTime;
            var remainingTimeToAnswerCommand = new NetworkCommandData("RemainingTimeToAnswer");
            remainingTimeToAnswerCommand.AddOption("TimeInSeconds", remainingTimeInSeconds.ToString());
            NetworkManager.SendClientCommand(friendConnectionId, remainingTimeToAnswerCommand);
        }
        else
        {   
            Deactivate();

            var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");

            NetworkManager.SendClientCommand(senderConnectionId, answerTimeoutCommandData);
            NetworkManager.SendClientCommand(friendConnectionId, answerTimeoutCommandData);
        }
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

        LocalGameData.GetCurrentQuestion((question) =>
            {
                var sendQuestionToFriend = new NetworkCommandData("LoadQuestion");
                var questionJSON = JsonUtility.ToJson(question);
                sendQuestionToFriend.AddOption("QuestionJSON", questionJSON);

                NetworkManager.SendClientCommand(friendConnectionId, sendQuestionToFriend);
                NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedSelectedAnswerOneTimeCommand(OnReceivedFriendResponse));

                base.CoroutineUtils.RepeatEverySeconds(1f, UpdateTimer);

                OnActivated(this, EventArgs.Empty);

                activated = true;
            }, (exception) =>
            {
                OnError(this, new UnhandledExceptionEventArgs(exception, true));
                Debug.LogException(exception);
                Deactivate();
            });
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