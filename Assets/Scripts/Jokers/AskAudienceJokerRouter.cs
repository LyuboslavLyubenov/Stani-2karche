using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AskAudienceJokerRouter : ExtendedMonoBehaviour
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

    const float MinCorrectAnswerVoteProcentage = 0.45f;
    const float MaxCorrectAnswerVoteProcentage = 0.85f;

    const float MinTimeInSecondsToSendGeneratedAnswer = 1f;
    const float MaxTimeInSecondsToSendGeneratedAnswer = 4f;

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
                OnError(this, new UnhandledExceptionEventArgs(this, true));
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

        OnFinished(this, EventArgs.Empty);

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
        notificationCommand.AddOption("Color", "yellow");
        notificationCommand.AddOption("Message", "Voting is over!");

        for (int i = 0; i < clients.Count; i++)
        {
            var connectionId = clients[i];
            NetworkManager.SendClientCommand(connectionId, notificationCommand);
        }
    }

    void ResetAnswerVotes(ISimpleQuestion question)
    {
        answersVotes.Clear();

        for (int i = 0; i < question.Answers.Length; i++)
        {
            var answer = question.Answers[i];
            answersVotes.Add(answer, 0);
        }
    }

    void GenerateAudienceVotes(ISimpleQuestion question)
    { 
        var correctAnswer = question.Answers[question.CorrectAnswerIndex]; 
        var correctAnswerChance = (int)(UnityEngine.Random.Range(MinCorrectAnswerVoteProcentage, MaxCorrectAnswerVoteProcentage) * 100); 
        var wrongAnswersLeftOverChance = 100 - correctAnswerChance; 
        answersVotes.Add(correctAnswer, correctAnswerChance); 

        for (int i = 0; i < question.Answers.Length - 1; i++)
        { 
            if (i == question.CorrectAnswerIndex)
            { 
                continue; 
            } 

            var wrongAnswerChance = UnityEngine.Random.Range(0, wrongAnswersLeftOverChance); 
            answersVotes.Add(question.Answers[i], wrongAnswersLeftOverChance); 
            wrongAnswersLeftOverChance -= wrongAnswerChance; 
        }  

        answersVotes.Add(question.Answers.Last(), wrongAnswersLeftOverChance);
    }

    void SendGeneratedResultToMainPlayer()
    {
        var secondsToWait = UnityEngine.Random.Range(MinTimeInSecondsToSendGeneratedAnswer, MaxTimeInSecondsToSendGeneratedAnswer);
        CoroutineUtils.WaitForSeconds(secondsToWait, () =>
            {
                LocalGameData.GetCurrentQuestion((question) =>
                    {
                        GenerateAudienceVotes(question);
                        SendMainPlayerVoteResult();
                    }, (exception) =>
                    {
                        Debug.LogException(exception);
                        Deactivate();
                        OnError(this, new UnhandledExceptionEventArgs(exception, true));
                    });    
            });
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

    public void Activate(int senderConnectionId, MainPlayerData mainPlayerData)
    {
        if (Activated)
        {
            throw new InvalidOperationException("Already active");
        }

        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");
        }

        var minClients = AskAudienceJoker.MinClientsForOnlineVote_Release;

        this.timeToAnswerInSeconds = LocalGameData.SecondsForAnswerQuestion;
        this.senderConnectionId = senderConnectionId;

        if (NetworkManager.ConnectedClientsCount < minClients)
        {
            answersVotes.Clear();
            SendJokerSettings();
            SendGeneratedResultToMainPlayer();
            return;
        }

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

                OnActivated(this, EventArgs.Empty);
            }, (exception) =>
            {
                Debug.LogException(exception);
                Deactivate();
                OnError(this, new UnhandledExceptionEventArgs(exception, true));
            });
    }
}