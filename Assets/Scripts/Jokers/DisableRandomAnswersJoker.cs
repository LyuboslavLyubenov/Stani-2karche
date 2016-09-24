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
        gameData.GetCurrentQuestion((question) => ActivateJoker(question, answersToDisableCount), Debug.LogException);

        Activated = true;

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }
    }

    void ActivateJoker(ISimpleQuestion currentQuestion, int answersToDisableCount)
    {
        if (currentQuestion.Answers.Length >= answersToDisableCount)
        {
            throw new ArgumentException("Cannot disable all answers");
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

