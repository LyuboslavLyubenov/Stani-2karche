using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using CielaSpike;
using System.Linq;

public class DisableRandomAnswersJoker : IJoker
{
    const int SettingsReceiveTimeoutInSeconds = 5;

    ClientNetworkManager networkManager;
    IQuestionUIController questionUIController;
    IGameData gameData;
    Timer receiveSettingsTimeoutTimer;

    public Sprite Image
    {
        get;
        private set;
    }

    public EventHandler OnActivated
    {
        get;
        set;
    }

    public bool Activated
    {
        get;
        private set;
    }

    public DisableRandomAnswersJoker(ClientNetworkManager networkManager, IGameData gameData, IQuestionUIController questionUIController)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }
            
        if (questionUIController == null)
        {
            throw new ArgumentNullException("questionUIController");
        }
            
        this.networkManager = networkManager;
        this.questionUIController = questionUIController;
        this.gameData = gameData;

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/DisableRandomAnswers");
    }

    public void Activate()
    {
        if (questionUIController.CurrentlyLoadedQuestion == null)
        {
            throw new InvalidOperationException();
        }

        var selectedJokerCommand = new NetworkCommandData("SelectedDisableRandomAnswersJoker");
        networkManager.SendServerCommand(selectedJokerCommand);

        var receiveJokerSettings = new ReceivedDisableRandomAnswerJokerSettingsCommand(OnReceivedJokerSettings);
        networkManager.CommandsManager.AddCommand("DisableRandomAnswersJokerSettings", receiveJokerSettings);

        receiveSettingsTimeoutTimer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
    }

    void OnReceiveSettingsTimeout()
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            { 
                receiveSettingsTimeoutTimer.Dispose();
                networkManager.CommandsManager.RemoveCommand("DisableRandomAnswersJokerSettings");
            });
    }

    void OnReceivedJokerSettings(int answersToDisableCount)
    {
        ActivateJoker(answersToDisableCount);

        Activated = true;

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }
    }

    void ActivateJoker(int answersToDisableCount)
    {
        var currentQuestion = questionUIController.CurrentlyLoadedQuestion;

        if (answersToDisableCount >= currentQuestion.Answers.Length)
        {
            throw new ArgumentException("Answers to disable count must be less than answers count");
        }

        var allAnswers = currentQuestion.Answers.ToList();
        var correctAnswerIndex = currentQuestion.CorrectAnswerIndex;
        var correctAnswer = allAnswers[correctAnswerIndex];
        var wrongAnswersIndexes = allAnswers.Where(a => a != correctAnswer)
            .ToArray()
            .GetRandomElements(answersToDisableCount)
            .Select(a => allAnswers.FindIndex(answer => answer == a))
            .ToArray();
        
        for (int i = 0; i < wrongAnswersIndexes.Length; i++)
        {
            var disabledAnswerIndex = wrongAnswersIndexes[i];
            questionUIController.HideAnswer(disabledAnswerIndex);
        }
    }
}

