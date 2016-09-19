using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

public class AskAudienceJokerRouter : ExtendedMonoBehaviour
{
    public const int MinTimeToAnswerInSeconds = 10;

    public ServerNetworkManager NetworkManager;
    public LocalGameData LocalGameData;
   
    int timeToAnswerInSeconds;
    int senderConnectionId;
    int elapsedTime;

    MainPlayerData mainPlayerData;

    List<int> votedClientsConnectionId = new List<int>();
    Dictionary<string, int> answersVotes = new Dictionary<string, int>();

    public bool Activated
    {
        get;
        private set;
    }

    void OnReceivedVote(int connectionId, string answer)
    {
        if (elapsedTime >= timeToAnswerInSeconds)
        {
            NoMoreTimeToAnswer();
            SendMainPlayerVoteResult();
            return;
        }

        if (votedClientsConnectionId.Contains(connectionId) || senderConnectionId == connectionId)
        {
            BeginReceiveVote();
            return;
        }

        answersVotes[answer]++;
        votedClientsConnectionId.Add(connectionId);

        if (votedClientsConnectionId.Count >= NetworkManager.ConnectedClientsCount - 1)
        {
            SendMainPlayerVoteResult();
            return;
        }

        BeginReceiveVote();
    }

    void BeginReceiveVote()
    {
        NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ServerReceivedAnswerSelectedOneTimeCommand(OnReceivedVote));
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
            SendRemainingTimeToAnswerToAudience(remainingTimeInSeconds);
        }
        else
        {   
            Deactivate();
            NoMoreTimeToAnswer();
            SendMainPlayerVoteResult();
        }
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
        NetworkManager.SendClientCommand(senderConnectionId, answerTimeoutCommandData);
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

    void SendVoteResult(Question currentQuestion)
    {
        var voteResultCommandData = new NetworkCommandData("AskAudienceVoteResult");
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

    public void Deactivate()
    {
        StopAllCoroutines();
        Activated = false;

        timeToAnswerInSeconds = 0;
        senderConnectionId = 0;
        elapsedTime = 0;

        votedClientsConnectionId.Clear();
        answersVotes.Clear();
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

        this.timeToAnswerInSeconds = timeToAnswerInSeconds;
        this.senderConnectionId = senderConnectionId;
        this.mainPlayerData = mainPlayerData;

        elapsedTime = 0;

        LocalGameData.GetCurrentQuestion((question) =>
            {
                var sendQuestionToFriend = new NetworkCommandData("LoadQuestion");
                var questionJSON = JsonUtility.ToJson(question);
                sendQuestionToFriend.AddOption("QuestionJSON", questionJSON);
                NetworkManager.SendAllClientsCommand(sendQuestionToFriend, senderConnectionId);

                CoroutineUtils.RepeatEverySeconds(1f, UpdateTimer);

                for (int i = 0; i < question.Answers.Length; i++)
                {
                    var answer = question.Answers[i];
                    answersVotes.Add(answer, 0);
                }

                BeginReceiveVote();
            }, (exception) =>
            {
                Debug.LogException(exception);
                Deactivate();
            });
    }
}