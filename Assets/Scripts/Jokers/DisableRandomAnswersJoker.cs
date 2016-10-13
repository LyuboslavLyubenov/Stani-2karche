using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;

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

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/FifthyFifthyChance");
    }

    public void Activate()
    {
        if (questionUIController.CurrentlyLoadedQuestion == null)
        {
            throw new InvalidOperationException();
        }

        var selectedJokerCommand = new NetworkCommandData("SelectedFifthyFifthyJoker");
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

        var correctAnswerIndex = currentQuestion.CorrectAnswerIndex;
        var disabledAnswersIndex = new List<int>();

        for (int i = 0; i < answersToDisableCount; i++)
        {
            int n;

            while (true)
            {
                //make sure we dont disable correct answer and we dont disable answer 2 times
                n = UnityEngine.Random.Range(0, answersToDisableCount);

                if (n != correctAnswerIndex && !disabledAnswersIndex.Contains(n))
                {
                    break;
                }
            }

            disabledAnswersIndex.Add(n);
        }

        for (int i = 0; i < disabledAnswersIndex.Count; i++)
        {
            var disabledIndex = disabledAnswersIndex[i];
            questionUIController.HideAnswer(disabledIndex);
        }
    }
}

