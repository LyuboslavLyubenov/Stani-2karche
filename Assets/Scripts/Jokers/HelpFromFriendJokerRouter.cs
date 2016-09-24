using UnityEngine;
using System;

public class HelpFromFriendJokerRouter : ExtendedMonoBehaviour
{
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

    public void Activate(int timeToAnswerInSeconds, int senderConnectionId, int friendConnectionId)
    {
        Deactivate();

        if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
        {
            throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
        }

        this.timeToAnswerInSeconds = timeToAnswerInSeconds;
        this.senderConnectionId = senderConnectionId;
        this.friendConnectionId = friendConnectionId;

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
            }, (exception) =>
            {
                Debug.LogException(exception);
                Deactivate();
            });

        activated = true;
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