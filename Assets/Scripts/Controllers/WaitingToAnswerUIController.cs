using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class WaitingToAnswerUIController : MonoBehaviour
{
    public GameObject RemainingSecondsObject;
    public ClientNetworkManager NetworkManager;

    Text remainingSecondsText;
    DisableAfterDelay disableAfterDelay = null;

    List<int> stopReceivingAnswerClientsIds = new List<int>();
    List<int> sendRemainingTimeClientsIds = new List<int>();

    void OnEnable()
    {
        if (RemainingSecondsObject == null)
        {
            throw new NullReferenceException("RemainingSecondsObjects is null on WaitingToAnswerUIController obj");
        }

        disableAfterDelay = GetComponent<DisableAfterDelay>();

        if (disableAfterDelay == null)
        {
            throw new Exception("WaitingToAnswerUIController obj must have DisableAfterDelay component");
        }

        remainingSecondsText = RemainingSecondsObject.GetComponent<Text>();

        if (remainingSecondsText == null)
        {
            throw new Exception("RemainingSecondsObject obj is null or doesnt have Text component");
        }

        UpdateTimer();

        disableAfterDelay.OnTimePass += OnTimePass;
        disableAfterDelay.OnTimeEnd += OnTimeEnd;
    }

    void OnDisable()
    {
        disableAfterDelay.OnTimePass -= OnTimePass;
        disableAfterDelay.OnTimeEnd -= OnTimeEnd;

        stopReceivingAnswerClientsIds.Clear();
        sendRemainingTimeClientsIds.Clear();
    }

    void UpdateTimer()
    {
        remainingSecondsText.text = disableAfterDelay.DelayInSeconds - disableAfterDelay.PassedSeconds + " секунди";
    }

    void OnTimePass(object sender, RemainingTimeEventArgs args)
    {
        UpdateTimer();

        for (int i = 0; i < sendRemainingTimeClientsIds.Count; i++)
        {
            var connectionId = sendRemainingTimeClientsIds[i];
            SendRemainingTimeToClient(connectionId, args.Seconds);
        }
    }

    void OnTimeEnd(object sender, EventArgs args)
    {
        for (int i = 0; i < stopReceivingAnswerClientsIds.Count; i++)
        {
            var connectionId = stopReceivingAnswerClientsIds[i];
            StopReceivingAnswer(connectionId);
        }
    }

    void StopReceivingAnswer(int clientConnectionId)
    {
        var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");
        answerTimeoutCommandData.AddOption("ClientConnectionId", clientConnectionId.ToString());
        NetworkManager.SendServerCommand(answerTimeoutCommandData);
    }

    void SendRemainingTimeToClient(int clientConnectionId, int remainingTimeInSeconds)
    {
        var commandData = new NetworkCommandData("RemainingTimeToAnswer");
        commandData.AddOption("TimeInSeconds", remainingTimeInSeconds.ToString());
        commandData.AddOption("ClientConnectionId", clientConnectionId.ToString());
        NetworkManager.SendServerCommand(commandData);
    }

    public void ActivateSendStopReceivingAnswerCommand(int clientConnectionId)
    {
        if (clientConnectionId <= 0)
        {
            throw new ArgumentOutOfRangeException("clientConnectionId");
        }

        stopReceivingAnswerClientsIds.Add(clientConnectionId);
    }

    public void ActivateSendRemainingTimeToClientCommand(int clientConnectionId)
    {
        if (clientConnectionId <= 0)
        {
            throw new ArgumentOutOfRangeException("clientConnectionId");
        }

        sendRemainingTimeClientsIds.Add(clientConnectionId);
    }
}
