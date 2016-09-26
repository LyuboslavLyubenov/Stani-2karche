using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AskAudienceJokerRouter : ExtendedMonoBehaviour
{
    public const int MinTimeToAnswerInSeconds = 10;

    public ServerNetworkManager NetworkManager;
    public LocalGameData LocalGameData;
   
    int timeToAnswerInSeconds;
    int senderConnectionId;
    int elapsedTime;

    List<int> clientsThatMustVote = new List<int>();
    List<int> votedClientsConnectionId = new List<int>();
    Dictionary<string, int> answersVotes = new Dictionary<string, int>();


    public bool Activated
    {
        get;
        private set;
    }

    void UpdateTimer()
    {
        if (!Activated)
        {
            return;
        }

        elapsedTime++;

        if (AreFinishedVoting())
        {
            TellClientsThatJokerIsDeactivated();
            SendMainPlayerVoteResult();
            return;
        }

        var remainingTimeInSeconds = timeToAnswerInSeconds - elapsedTime;
        SendRemainingTimeToAnswerToAudience(remainingTimeInSeconds);
    }

    void SendRemainingTimeToAnswerToAudience(int remainingTimeInSeconds)
    {
        var remainingTimeToAnswerCommand = new NetworkCommandData("RemainingTimeToAnswer");
        remainingTimeToAnswerCommand.AddOption("TimeInSeconds", remainingTimeInSeconds.ToString());
        NetworkManager.SendAllClientsCommand(remainingTimeToAnswerCommand, senderConnectionId);
    }

    void NoMoreTimeToAnswer()
    {
        var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");
        NetworkManager.SendAllClientsCommand(answerTimeoutCommandData, senderConnectionId);
    }

    void SendMainPlayerVoteResult()
    {
        LocalGameData.GetCurrentQuestion(SendVoteResult, 
            (exception) =>
            {
                //TODO:
                Debug.LogException(exception);
                Deactivate();
            });
    }

    void SendVoteResult(ISimpleQuestion currentQuestion)
    {
        var voteResultCommandData = new NetworkCommandData("AskAudienceJokerVoteResult");
        var answersVotesPairs = answersVotes.ToArray();

        for (int i = 0; i < answersVotesPairs.Length; i++)
        {
            var answer = answersVotesPairs[i].Key;
            var answerVoteCount = answersVotesPairs[i].Value;
            voteResultCommandData.AddOption(answer, answerVoteCount.ToString());
        }

        NetworkManager.SendClientCommand(senderConnectionId, voteResultCommandData);
        Deactivate();
    }

    void BeginReceiveVote()
    {
        NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedSelectedAnswerOneTimeCommand(OnReceivedVote));
    }

    void OnReceivedVote(int connectionId, string answer)
    {
        if (!Activated)
        {
            return;
        }

        if (!clientsThatMustVote.Contains(connectionId))
        {
            BeginReceiveVote();
            return;
        }

        answersVotes[answer]++;
        votedClientsConnectionId.Add(connectionId);

        if (AreFinishedVoting())
        {
            NoMoreTimeToAnswer();
            SendMainPlayerVoteResult();
            return;
        }

        BeginReceiveVote();
    }

    bool AreFinishedVoting()
    {
        if (elapsedTime >= timeToAnswerInSeconds)
        {
            return true;
        }

        for (int i = 0; i < clientsThatMustVote.Count; i++)
        {
            var connectionId = clientsThatMustVote[i];
            
            if (!votedClientsConnectionId.Contains(connectionId))
            {
                return false;
            }
        }

        return true;
    }

    void SendJokerSettings()
    {
        var setAskAudienceJokerSettingsCommand = new NetworkCommandData("AskAudienceJokerSettings");
        setAskAudienceJokerSettingsCommand.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());

        NetworkManager.SendClientCommand(senderConnectionId, setAskAudienceJokerSettingsCommand);
    }

    void SendQuestionToAudience(ISimpleQuestion question)
    {
        var sendQuestionCommand = new NetworkCommandData("LoadQuestion");
        var questionJSON = JsonUtility.ToJson(question.Serialize());
        sendQuestionCommand.AddOption("QuestionJSON", questionJSON);
        NetworkManager.SendAllClientsCommand(sendQuestionCommand, senderConnectionId);
    }

    void TellClientsThatJokerIsDeactivated()
    {
        var clients = clientsThatMustVote.ToList();
        clients.Add(senderConnectionId);

        var notificationCommand = new NetworkCommandData("ShowNotification");
        notificationCommand.AddOption("Color", "red");
        notificationCommand.AddOption("Message", "Voting is over!");

        for (int i = 0; i < clients.Count; i++)
        {
            var connectionId = clients[i];
            NetworkManager.SendClientCommand(connectionId, notificationCommand);
        }
    }

    void ResetAnswerVotes(ISimpleQuestion question)
    {
        for (int i = 0; i < question.Answers.Length; i++)
        {
            var answer = question.Answers[i];
            answersVotes.Add(answer, 0);
        }
    }

    public void Deactivate()
    {
        TellClientsThatJokerIsDeactivated();

        StopAllCoroutines();

        clientsThatMustVote.Clear();
        votedClientsConnectionId.Clear();
        answersVotes.Clear();

        timeToAnswerInSeconds = 0;
        senderConnectionId = 0;
        elapsedTime = -1;

        Activated = false;
    }

    public void Activate(int timeToAnswerInSeconds, int senderConnectionId, MainPlayerData mainPlayerData)
    {
        if (timeToAnswerInSeconds < MinTimeToAnswerInSeconds)
        {
            throw new ArgumentOutOfRangeException("timeToAnswerInSeconds", "Time must be minimum " + MinTimeToAnswerInSeconds + " seconds");
        }

        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");
        }

        var minClients = AskAudienceJoker.MinClientsForOnlineVote_Development;

        #if DEVELOPMENT_BUILD
        minClients = AskAudienceJoker.MinClientsForOnlineVote_Development;
        #endif

        if (NetworkManager.ConnectedClientsCount < minClients)
        {
            var cannotActivateNotificationCommand = new NetworkCommandData("ShowNotification");
            cannotActivateNotificationCommand.AddOption("Color", "Red");
            cannotActivateNotificationCommand.AddOption("Message", "Cannot activate Ask Audience Joker because there arent enough players in audience");

            NetworkManager.SendClientCommand(senderConnectionId, cannotActivateNotificationCommand);
            return;
        }

        this.timeToAnswerInSeconds = timeToAnswerInSeconds;
        this.senderConnectionId = senderConnectionId;

        elapsedTime = 1;

        var audienceConnectionIds = NetworkManager.ConnectedClientsConnectionId.Where(connectionId => connectionId != senderConnectionId);
        clientsThatMustVote.AddRange(audienceConnectionIds);

        LocalGameData.GetCurrentQuestion((question) =>
            {
                ResetAnswerVotes(question);
                SendJokerSettings();
                SendQuestionToAudience(question);
                BeginReceiveVote();
                Activated = true;

                CoroutineUtils.RepeatEverySeconds(1f, UpdateTimer);
            }, (exception) =>
            {
                Debug.LogException(exception);
                Deactivate();
            });
    }
}